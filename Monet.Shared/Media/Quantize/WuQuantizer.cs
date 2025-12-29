using Monet.Shared.Enums;
using Monet.Shared.Interfaces;
using Monet.Shared.Utilities;

namespace Monet.Shared.Media.Quantize;

public sealed partial class WuQuantizer : IQuantizer {
    private int[] _weights;
    private int[] _momentsR;
    private int[] _momentsG;
    private int[] _momentsB;
    private double[] _moments;

    private Box[] _cubes;

    private const int INDEX_BITS = 5;
    private const int INDEX_COUNT = 33;
    private const int TOTAL_SIZE = 35937;

    public QuantizedColorMap Quantize(uint[] pixels, int colorCount) {
        var mapResult = new QuantizerMap().Quantize(pixels, colorCount);

        ConstructHistogram(mapResult.ColorToCount);
        CreateMoments();

        var createBoxesResult = CreateBoxes(colorCount);
        var colors = CreateResult(createBoxesResult.ActualCount);

        var resultMap = new Dictionary<uint, int>();
        foreach (var color in colors)
            resultMap[color] = 0;

        return new QuantizedColorMap(resultMap);
    }

    private static int GetIndex(int r, int g, int b) {
        return (r << (INDEX_BITS * 2))
             + (r << (INDEX_BITS + 1))
             + r
             + (g << INDEX_BITS)
             + g
             + b;
    }

    private void ConstructHistogram(Dictionary<uint, int> pixels) {
        _weights = new int[TOTAL_SIZE];
        _momentsR = new int[TOTAL_SIZE];
        _momentsG = new int[TOTAL_SIZE];
        _momentsB = new int[TOTAL_SIZE];
        _moments = new double[TOTAL_SIZE];

        foreach (var pair in pixels) {
            uint pixel = pair.Key;
            int count = pair.Value;

            int red = (int)ColorUtil.ToRedByArgb(pixel);
            int green = (int)ColorUtil.ToGreenByArgb(pixel);
            int blue = (int)ColorUtil.ToBuleByArgb(pixel);

            int bitsToRemove = 8 - INDEX_BITS;

            int iR = (red >> bitsToRemove) + 1;
            int iG = (green >> bitsToRemove) + 1;
            int iB = (blue >> bitsToRemove) + 1;

            int index = GetIndex(iR, iG, iB);

            _weights[index] += count;
            _momentsR[index] += red * count;
            _momentsG[index] += green * count;
            _momentsB[index] += blue * count;
            _moments[index] += count * (red * red + green * green + blue * blue);
        }
    }

    private void CreateMoments() {
        for (int r = 1; r < INDEX_COUNT; ++r) {
            int[] area = new int[INDEX_COUNT];
            int[] areaR = new int[INDEX_COUNT];
            int[] areaG = new int[INDEX_COUNT];
            int[] areaB = new int[INDEX_COUNT];
            double[] area2 = new double[INDEX_COUNT];

            for (int g = 1; g < INDEX_COUNT; ++g) {
                int line = 0;
                int lineR = 0;
                int lineG = 0;
                int lineB = 0;
                double line2 = 0.0;

                for (int b = 1; b < INDEX_COUNT; ++b) {
                    int index = GetIndex(r, g, b);

                    line += _weights[index];
                    lineR += _momentsR[index];
                    lineG += _momentsG[index];
                    lineB += _momentsB[index];
                    line2 += _moments[index];

                    area[b] += line;
                    areaR[b] += lineR;
                    areaG[b] += lineG;
                    areaB[b] += lineB;
                    area2[b] += line2;

                    int previousIndex = GetIndex(r - 1, g, b);

                    _weights[index] = _weights[previousIndex] + area[b];
                    _momentsR[index] = _momentsR[previousIndex] + areaR[b];
                    _momentsG[index] = _momentsG[previousIndex] + areaG[b];
                    _momentsB[index] = _momentsB[previousIndex] + areaB[b];
                    _moments[index] = _moments[previousIndex] + area2[b];
                }
            }
        }
    }

    private CreateBoxesResult CreateBoxes(int maxColorCount) {
        _cubes = new Box[maxColorCount];
        for (int i = 0; i < maxColorCount; i++)
            _cubes[i] = new Box();

        double[] volumeVariance = new double[maxColorCount];

        Box firstBox = _cubes[0];
        firstBox.RedEnd = INDEX_COUNT - 1;
        firstBox.GreenEnd = INDEX_COUNT - 1;
        firstBox.BlueEnd = INDEX_COUNT - 1;

        int generatedColorCount = maxColorCount;
        int next = 0;

        for (int i = 1; i < maxColorCount; i++) {
            if (Cut(_cubes[next], _cubes[i])) {
                volumeVariance[next] = (_cubes[next].Volume > 1) ? Variance(_cubes[next]) : 0.0;
                volumeVariance[i] = (_cubes[i].Volume > 1) ? Variance(_cubes[i]) : 0.0;
            } else {
                volumeVariance[next] = 0.0;
                i--;
            }

            next = 0;
            double temp = volumeVariance[0];

            for (int j = 1; j <= i; j++) {
                if (volumeVariance[j] > temp) {
                    temp = volumeVariance[j];
                    next = j;
                }
            }

            if (temp <= 0.0) {
                generatedColorCount = i + 1;
                break;
            }
        }

        return new CreateBoxesResult(maxColorCount, generatedColorCount);
    }

    private List<uint> CreateResult(int colorCount) {
        var colors = new List<uint>();

        for (int i = 0; i < colorCount; ++i) {
            Box cube = _cubes[i];
            int weight = Volume(cube, _weights);

            if (weight > 0) {
                int r = Volume(cube, _momentsR) / weight;
                int g = Volume(cube, _momentsG) / weight;
                int b = Volume(cube, _momentsB) / weight;

                uint color =
                    (uint)((255 << 24) |
                           ((r & 0xFF) << 16) |
                           ((g & 0xFF) << 8) |
                           (b & 0xFF));

                colors.Add(color);
            }
        }

        return colors;
    }

    private double Variance(Box cube) {
        int dr = Volume(cube, _momentsR);
        int dg = Volume(cube, _momentsG);
        int db = Volume(cube, _momentsB);

        double xx =
            _moments[GetIndex(cube.RedEnd, cube.GreenEnd, cube.BlueEnd)]
            - _moments[GetIndex(cube.RedEnd, cube.GreenEnd, cube.BlueStart)]
            - _moments[GetIndex(cube.RedEnd, cube.GreenStart, cube.BlueEnd)]
            + _moments[GetIndex(cube.RedEnd, cube.GreenStart, cube.BlueStart)]
            - _moments[GetIndex(cube.RedStart, cube.GreenEnd, cube.BlueEnd)]
            + _moments[GetIndex(cube.RedStart, cube.GreenEnd, cube.BlueStart)]
            + _moments[GetIndex(cube.RedStart, cube.GreenStart, cube.BlueEnd)]
            - _moments[GetIndex(cube.RedStart, cube.GreenStart, cube.BlueStart)];

        int hypotenuse = dr * dr + dg * dg + db * db;
        int vol = Volume(cube, _weights);

        return xx - hypotenuse / (double)vol;
    }

    private bool Cut(Box one, Box two) {
        int wholeR = Volume(one, _momentsR);
        int wholeG = Volume(one, _momentsG);
        int wholeB = Volume(one, _momentsB);
        int wholeW = Volume(one, _weights);

        var maxR = Maximize(one, Direction.Red, one.RedStart + 1, one.RedEnd, wholeR, wholeG, wholeB, wholeW);
        var maxG = Maximize(one, Direction.Green, one.GreenStart + 1, one.GreenEnd, wholeR, wholeG, wholeB, wholeW);
        var maxB = Maximize(one, Direction.Blue, one.BlueStart + 1, one.BlueEnd, wholeR, wholeG, wholeB, wholeW);

        Direction cutDirection;
        double rScore = maxR.MaximumScore;
        double gScore = maxG.MaximumScore;
        double bScore = maxB.MaximumScore;

        if (rScore >= gScore && rScore >= bScore) {
            if (maxR.CutPosition < 0)
                return false;

            cutDirection = Direction.Red;
        } else if (gScore >= rScore && gScore >= bScore) {
            cutDirection = Direction.Green;
        } else {
            cutDirection = Direction.Blue;
        }

        two.RedEnd = one.RedEnd;
        two.GreenEnd = one.GreenEnd;
        two.BlueEnd = one.BlueEnd;

        switch (cutDirection) {
            case Direction.Red:
                one.RedEnd = maxR.CutPosition;
                two.RedStart = one.RedEnd;
                two.GreenStart = one.GreenStart;
                two.BlueStart = one.BlueStart;
                break;

            case Direction.Green:
                one.GreenEnd = maxG.CutPosition;
                two.RedStart = one.RedStart;
                two.GreenStart = one.GreenEnd;
                two.BlueStart = one.BlueStart;
                break;

            case Direction.Blue:
                one.BlueEnd = maxB.CutPosition;
                two.RedStart = one.RedStart;
                two.GreenStart = one.GreenStart;
                two.BlueStart = one.BlueEnd;
                break;
        }

        one.Volume =
            (one.RedEnd - one.RedStart) *
            (one.GreenEnd - one.GreenStart) *
            (one.BlueEnd - one.BlueStart);

        two.Volume =
            (two.RedEnd - two.RedStart) *
            (two.GreenEnd - two.GreenStart) *
            (two.BlueEnd - two.BlueStart);

        return true;
    }

    private MaximizeResult Maximize(
        Box cube,
        Direction direction,
        int first,
        int last,
        int wholeR,
        int wholeG,
        int wholeB,
        int wholeW) {
        int bottomR = Bottom(cube, direction, _momentsR);
        int bottomG = Bottom(cube, direction, _momentsG);
        int bottomB = Bottom(cube, direction, _momentsB);
        int bottomW = Bottom(cube, direction, _weights);

        double max = 0.0;
        int cut = -1;

        for (int i = first; i < last; i++) {
            int halfR = bottomR + Top(cube, direction, i, _momentsR);
            int halfG = bottomG + Top(cube, direction, i, _momentsG);
            int halfB = bottomB + Top(cube, direction, i, _momentsB);
            int halfW = bottomW + Top(cube, direction, i, _weights);

            if (halfW == 0)
                continue;

            double temp = (halfR * halfR + halfG * halfG + halfB * halfB) / (double)halfW;

            halfR = wholeR - halfR;
            halfG = wholeG - halfG;
            halfB = wholeB - halfB;
            halfW = wholeW - halfW;

            if (halfW == 0)
                continue;

            temp += (halfR * halfR + halfG * halfG + halfB * halfB) / (double)halfW;

            if (temp > max) {
                max = temp;
                cut = i;
            }
        }

        return new MaximizeResult(cut, max);
    }

    private static int Volume(Box cube, int[] moment) {
        return
            moment[GetIndex(cube.RedEnd, cube.GreenEnd, cube.BlueEnd)]
            - moment[GetIndex(cube.RedEnd, cube.GreenEnd, cube.BlueStart)]
            - moment[GetIndex(cube.RedEnd, cube.GreenStart, cube.BlueEnd)]
            + moment[GetIndex(cube.RedEnd, cube.GreenStart, cube.BlueStart)]
            - moment[GetIndex(cube.RedStart, cube.GreenEnd, cube.BlueEnd)]
            + moment[GetIndex(cube.RedStart, cube.GreenEnd, cube.BlueStart)]
            + moment[GetIndex(cube.RedStart, cube.GreenStart, cube.BlueEnd)]
            - moment[GetIndex(cube.RedStart, cube.GreenStart, cube.BlueStart)];
    }

    private static int Bottom(Box cube, Direction direction, int[] moment) {
        return direction switch {
            Direction.Red =>
                -moment[GetIndex(cube.RedStart, cube.GreenEnd, cube.BlueEnd)]
                + moment[GetIndex(cube.RedStart, cube.GreenEnd, cube.BlueStart)]
                + moment[GetIndex(cube.RedStart, cube.GreenStart, cube.BlueEnd)]
                - moment[GetIndex(cube.RedStart, cube.GreenStart, cube.BlueStart)],

            Direction.Green =>
                -moment[GetIndex(cube.RedEnd, cube.GreenStart, cube.BlueEnd)]
                + moment[GetIndex(cube.RedEnd, cube.GreenStart, cube.BlueStart)]
                + moment[GetIndex(cube.RedStart, cube.GreenStart, cube.BlueEnd)]
                - moment[GetIndex(cube.RedStart, cube.GreenStart, cube.BlueStart)],

            Direction.Blue =>
                -moment[GetIndex(cube.RedEnd, cube.GreenEnd, cube.BlueStart)]
                + moment[GetIndex(cube.RedEnd, cube.GreenStart, cube.BlueStart)]
                + moment[GetIndex(cube.RedStart, cube.GreenEnd, cube.BlueStart)]
                - moment[GetIndex(cube.RedStart, cube.GreenStart, cube.BlueStart)],

            _ => throw new ArgumentException("Unexpected direction " + direction),
        };
    }

    private static int Top(Box cube, Direction direction, int position, int[] moment) {
        return direction switch {
            Direction.Red =>
                moment[GetIndex(position, cube.GreenEnd, cube.BlueEnd)]
                - moment[GetIndex(position, cube.GreenEnd, cube.BlueStart)]
                - moment[GetIndex(position, cube.GreenStart, cube.BlueEnd)]
                + moment[GetIndex(position, cube.GreenStart, cube.BlueStart)],

            Direction.Green =>
                moment[GetIndex(cube.RedEnd, position, cube.BlueEnd)]
                - moment[GetIndex(cube.RedEnd, position, cube.BlueStart)]
                - moment[GetIndex(cube.RedStart, position, cube.BlueEnd)]
                + moment[GetIndex(cube.RedStart, position, cube.BlueStart)],

            Direction.Blue =>
                moment[GetIndex(cube.RedEnd, cube.GreenEnd, position)]
                - moment[GetIndex(cube.RedEnd, cube.GreenStart, position)]
                - moment[GetIndex(cube.RedStart, cube.GreenEnd, position)]
                + moment[GetIndex(cube.RedStart, cube.GreenStart, position)],

            _ => throw new ArgumentException("Unexpected direction " + direction),
        };
    }
}

internal class Box {
    public int RedStart = 0;
    public int RedEnd = 0;

    public int GreenStart = 0;
    public int GreenEnd = 0;

    public int BlueStart = 0;
    public int BlueEnd = 0;

    public int Volume = 0;
}

internal class MaximizeResult {
    public int CutPosition { get; }
    public double MaximumScore { get; }

    public MaximizeResult(int cutPosition, double maximumScore) {
        CutPosition = cutPosition;
        MaximumScore = maximumScore;
    }
}

internal class CreateBoxesResult {
    public int RequestedCount { get; }
    public int ActualCount { get; }

    public CreateBoxesResult(int requestedCount, int actualCount) {
        RequestedCount = requestedCount;
        ActualCount = actualCount;
    }
}