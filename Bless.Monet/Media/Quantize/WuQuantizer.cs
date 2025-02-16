using Bless.Monet.Enums;
using Bless.Monet.Interface;
using Bless.Monet.Utilities;

namespace Bless.Monet.Media.Quantize;

/// <summary>
/// An image quantizer that divides the image's pixels into clusters by recursively cutting an RGB
/// cube, based on the weight of pixels in each area of the cube.
/// </summary>
/// <remarks>
/// The algorithm was described by Xiaolin Wu in Graphic Gems II, published in 1991.
/// </remarks>
public sealed class WuQuantizer : IQuantizer {
    private const int INDEX_BITS = 5;
    private const int INDEX_COUNT = 33;
    private const int TOTAL_SIZE = 35937;
    
    private Box[] _cubes = default!;
    private long[] _weights = default!;
    private long[] _momentsR = default!;
    private long[] _momentsG = default!;
    private long[] _momentsB = default!;
    private double[] _moments = default!;

    public QuantizerResult Quantize(uint[] pixels, int maxColors) {
        var result = new DictionaryQuantizer().Quantize(pixels, maxColors);
        ConstructHistogram(result.ColorToCount);
        CreateMoments();

        var createBoxesResult = CreateBoxes(maxColors);
        var colors = CreateResult(createBoxesResult.ResultCount);

        var resultDct = new Dictionary<uint, uint>();
        foreach (var color in colors)
            resultDct.Add(color, 0);

        return new QuantizerResult(resultDct);
    }

    private bool Cut(Box one, Box two) {
        var wholeR = Volume(one, _momentsR);
        var wholeG = Volume(one, _momentsG);
        var wholeB = Volume(one, _momentsB);
        var wholeW = Volume(one, _weights);

        var maxRResult = Maximize(one, Direction.Red, one.R0 + 1, one.R1, wholeR, wholeG, wholeB, wholeW);
        var maxGResult = Maximize(one, Direction.Green, one.G0 + 1, one.G1, wholeR, wholeG, wholeB, wholeW);
        var maxBResult = Maximize(one, Direction.Blue, one.B0 + 1, one.B1, wholeR, wholeG, wholeB, wholeW);

        Direction cutDirection;

        double maxR = maxRResult.Maximum;
        double maxG = maxGResult.Maximum;
        double maxB = maxBResult.Maximum;

        if (maxR >= maxG && maxR >= maxB) {
            if (maxRResult.CutLocation < 0) {
                return false;
            }
            cutDirection = Direction.Red;
        } else if (maxG >= maxR && maxG >= maxB)
            cutDirection = Direction.Green;
        else
            cutDirection = Direction.Blue;

        two.R1 = one.R1;
        two.G1 = one.G1;
        two.B1 = one.B1;

        switch (cutDirection) {
            case Direction.Red:
                one.R1 = maxRResult.CutLocation;
                two.R0 = one.R1;
                two.G0 = one.G0;
                two.B0 = one.B0;
                break;
            case Direction.Green:
                one.G1 = maxGResult.CutLocation;
                two.R0 = one.R0;
                two.G0 = one.G1;
                two.B0 = one.B0;
                break;
            case Direction.Blue:
                one.B1 = maxBResult.CutLocation;
                two.R0 = one.R0;
                two.G0 = one.G0;
                two.B0 = one.B1;
                break;
        }

        one.Vol = (one.R1 - one.R0) * (one.G1 - one.G0) * (one.B1 - one.B0);
        two.Vol = (two.R1 - two.R0) * (two.G1 - two.G0) * (two.B1 - two.B0);
        return true;
    }

    private List<uint> CreateResult(int colorCount) {
        List<uint> colors = [];

        for (int i = 0; i < colorCount; ++i) {
            Box cube = _cubes[i];
            var weight = Volume(cube, _weights);

            if (weight > 0) {
                var r = Volume(cube, _momentsR) / weight;
                var g = Volume(cube, _momentsG) / weight;
                var b = Volume(cube, _momentsB) / weight;

                long color = (255 << 24) | ((r & 0x0ff) << 16) | ((g & 0x0ff) << 8) | (b & 0x0ff);
                colors.Add(Convert.ToUInt32(color));
            }
        }
        return colors;
    }

    private MaximizeResult Maximize(Box cube, Direction direction, long first, long last, long wholeR, long wholeG, long wholeB, long wholeW) {
        var bottomR = Bottom(cube, direction, _momentsR);
        var bottomG = Bottom(cube, direction, _momentsG);
        var bottomB = Bottom(cube, direction, _momentsB);
        var bottomW = Bottom(cube, direction, _weights);

        double max = 0.0;
        long cut = -1;

        long halfR = 0;
        long halfG = 0;
        long halfB = 0;
        long halfW = 0;
        for (long i = first; i < last; i++) {
            halfR = bottomR + Top(cube, direction, i, _momentsR);
            halfG = bottomG + Top(cube, direction, i, _momentsG);
            halfB = bottomB + Top(cube, direction, i, _momentsB);
            halfW = bottomW + Top(cube, direction, i, _weights);
            if (halfW == 0) {
                continue;
            }

            double tempNumerator = halfR * halfR + halfG * halfG + halfB * halfB;
            double tempDenominator = halfW;
            double temp = tempNumerator / tempDenominator;

            halfR = wholeR - halfR;
            halfG = wholeG - halfG;
            halfB = wholeB - halfB;
            halfW = wholeW - halfW;
            if (halfW == 0) {
                continue;
            }

            tempNumerator = halfR * halfR + halfG * halfG + halfB * halfB;
            tempDenominator = halfW;
            temp += (tempNumerator / tempDenominator);

            if (temp > max) {
                max = temp;
                cut = i;
            }
        }
        return new MaximizeResult(cut, max);
    }

    private static long Volume(Box cube, long[] moment) {
        return (moment[GetIndex(cube.R1, cube.G1, cube.B1)]
            - moment[GetIndex(cube.R1, cube.G1, cube.B0)]
            - moment[GetIndex(cube.R1, cube.G0, cube.B1)]
            + moment[GetIndex(cube.R1, cube.G0, cube.B0)]
            - moment[GetIndex(cube.R0, cube.G1, cube.B1)]
            + moment[GetIndex(cube.R0, cube.G1, cube.B0)]
            + moment[GetIndex(cube.R0, cube.G0, cube.B1)]
            - moment[GetIndex(cube.R0, cube.G0, cube.B0)]);
    }

    private double Variance(Box cube) {
        var dr = Volume(cube, _momentsR);
        var dg = Volume(cube, _momentsG);
        var db = Volume(cube, _momentsB);
        double xx =
            _moments[GetIndex(cube.R1, cube.G1, cube.B1)]
                - _moments[GetIndex(cube.R1, cube.G1, cube.B0)]
                - _moments[GetIndex(cube.R1, cube.G0, cube.B1)]
                + _moments[GetIndex(cube.R1, cube.G0, cube.B0)]
                - _moments[GetIndex(cube.R0, cube.G1, cube.B1)]
                + _moments[GetIndex(cube.R0, cube.G1, cube.B0)]
                + _moments[GetIndex(cube.R0, cube.G0, cube.B1)]
                - _moments[GetIndex(cube.R0, cube.G0, cube.B0)];

        var hypotenuse = dr * dr + dg * dg + db * db;
        var volume = Volume(cube, _weights);
        return xx - hypotenuse / ((double)volume);
    }

    private CreateBoxesResult CreateBoxes(int maxColorCount) {
        _cubes = new Box[maxColorCount];
        for (int i = 0; i < maxColorCount; i++) {
            _cubes[i] = new Box();
        }

        var volumeVariance = new double[maxColorCount];
        Box firstBox = _cubes[0];
        firstBox.R1 = INDEX_COUNT - 1;
        firstBox.G1 = INDEX_COUNT - 1;
        firstBox.B1 = INDEX_COUNT - 1;

        int generatedColorCount = maxColorCount;
        int next = 0;
        for (int i = 1; i < maxColorCount; i++) {
            if (Cut(_cubes[next], _cubes[i])) {
                volumeVariance[next] = (_cubes[next].Vol > 1) ? Variance(_cubes[next]) : 0.0;
                volumeVariance[i] = (_cubes[i].Vol > 1) ? Variance(_cubes[i]) : 0.0;
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

    private void CreateMoments() {
        for (int r = 1; r < INDEX_COUNT; ++r) {
            var area = new long[INDEX_COUNT];
            var areaR = new long[INDEX_COUNT];
            var areaG = new long[INDEX_COUNT];
            var areaB = new long[INDEX_COUNT];
            double[] area2 = new double[INDEX_COUNT];

            for (int g = 1; g < INDEX_COUNT; ++g) {
                long line = 0;
                long lineR = 0;
                long lineG = 0;
                long lineB = 0;
                double line2 = 0.0;
                for (int b = 1; b < INDEX_COUNT; ++b) {
                    var index = GetIndex(r, g, b);
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

                    var previousIndex = GetIndex(r - 1, g, b);
                    _weights[index] = _weights[previousIndex] + area[b];
                    _momentsR[index] = _momentsR[previousIndex] + areaR[b];
                    _momentsG[index] = _momentsG[previousIndex] + areaG[b];
                    _momentsB[index] = _momentsB[previousIndex] + areaB[b];
                    _moments[index] = _moments[previousIndex] + area2[b];
                }
            }
        }
    }

    private void ConstructHistogram(Dictionary<uint, uint> pixels) {
        _weights = new long[TOTAL_SIZE];
        _momentsR = new long[TOTAL_SIZE];
        _momentsG = new long[TOTAL_SIZE];
        _momentsB = new long[TOTAL_SIZE];
        _moments = new double[TOTAL_SIZE];

        foreach (var pair in pixels) {
            var pixel = pair.Key;
            var count = pair.Value;
            var red = ColorUtil.ToRedByArgb(pixel);
            var green = ColorUtil.ToGreenByArgb(pixel);
            var blue = ColorUtil.ToBuleByArgb(pixel);
            var bitsToRemove = 8 - INDEX_BITS;
            var iR = (red >> bitsToRemove) + 1;
            var iG = (green >> bitsToRemove) + 1;
            var iB = (blue >> bitsToRemove) + 1;
            var index = GetIndex(iR, iG, iB);

            _weights[index] += count;
            _momentsR[index] += (red * count);
            _momentsG[index] += (green * count);
            _momentsB[index] += (blue * count);
            _moments[index] += (count * ((red * red) + (green * green) + (blue * blue)));
        }

    }

    private static long GetIndex(long r, long g, long b) {
        return (r << (INDEX_BITS * 2)) + (r << (INDEX_BITS + 1)) + r + (g << INDEX_BITS) + g + b;
    }

    private static long Bottom(Box cube, Direction direction, long[] moment) {
        return direction switch {
            Direction.Red => (-moment[GetIndex(cube.R0, cube.G1, cube.B1)]
                                    + moment[GetIndex(cube.R0, cube.G1, cube.B0)]
                                    + moment[GetIndex(cube.R0, cube.G0, cube.B1)]
                                    - moment[GetIndex(cube.R0, cube.G0, cube.B0)]),
            Direction.Green => (-moment[GetIndex(cube.R1, cube.G0, cube.B1)]
                                    + moment[GetIndex(cube.R1, cube.G0, cube.B0)]
                                    + moment[GetIndex(cube.R0, cube.G0, cube.B1)]
                                    - moment[GetIndex(cube.R0, cube.G0, cube.B0)]),
            Direction.Blue => (-moment[GetIndex(cube.R1, cube.G1, cube.B0)]
                                    + moment[GetIndex(cube.R1, cube.G0, cube.B0)]
                                    + moment[GetIndex(cube.R0, cube.G1, cube.B0)]
                                    - moment[GetIndex(cube.R0, cube.G0, cube.B0)]),
            _ => throw new InvalidOperationException("unexpected direction " + direction),
        };
    }

    private static long Top(Box cube, Direction direction, long position, long[] moment) {
        return direction switch {
            Direction.Red => (moment[GetIndex(position, cube.G1, cube.B1)]
                                - moment[GetIndex(position, cube.G1, cube.B0)]
                                - moment[GetIndex(position, cube.G0, cube.B1)]
                                + moment[GetIndex(position, cube.G0, cube.B0)]),
            Direction.Green => (moment[GetIndex(cube.R1, position, cube.B1)]
                                - moment[GetIndex(cube.R1, position, cube.B0)]
                                - moment[GetIndex(cube.R0, position, cube.B1)]
                                + moment[GetIndex(cube.R0, position, cube.B0)]),
            Direction.Blue => (moment[GetIndex(cube.R1, cube.G1, position)]
                                - moment[GetIndex(cube.R1, cube.G0, position)]
                                - moment[GetIndex(cube.R0, cube.G1, position)]
                                + moment[GetIndex(cube.R0, cube.G0, position)]),
            _ => throw new InvalidOperationException("unexpected direction " + direction),
        };
    }
}

public record struct MaximizeResult {
    public double Maximum { get; set; }
    public long CutLocation { get; set; }

    public MaximizeResult(long cut, double max) {
        CutLocation = cut;
        Maximum = max;
    }
}

public record struct CreateBoxesResult {
    public int ResultCount { get; set; }
    public int RequestedCount { get; set; }

    public CreateBoxesResult(int requestedCount, int resultCount) {
        RequestedCount = requestedCount;
        ResultCount = resultCount;
    }
}

public record struct Box {
    public Box() { }

    public long R0 { get; set; } = 0;
    public long R1 { get; set; } = 0;
    public long G0 { get; set; } = 0;
    public long G1 { get; set; } = 0;
    public long B0 { get; set; } = 0;
    public long B1 { get; set; } = 0;
    public long Vol { get; set; } = 0;
}