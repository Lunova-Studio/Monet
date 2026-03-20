using Monet.Shared.Media.ColorSpace;
using Monet.Shared.Utilities;

namespace Monet.Shared.Media;

/// <summary>
/// Design utilities using color temperature theory.
/// </summary>
public sealed class TemperatureManager {
    private readonly Hct _hct;

    private Hct _precomputedComplement;
    private List<Hct> _precomputedHctsByHue;
    private List<Hct> _precomputedHctsByTemp;
    private Dictionary<Hct, double> _precomputedTempsByHct;

    private Hct Warmest => GetHctsByTemp().Last();
    private Hct Coldest => GetHctsByTemp().First();

    public TemperatureManager(Hct hct) {
        _hct = hct;
    }

    /// <summary>
    /// A color that complements the input color aesthetically.
    /// </summary>
    public Hct GetComplement() {
        if (!_precomputedComplement.IsDefault())
            return _precomputedComplement;

        var temps = GetTempsByHct();
        var hctsByTemp = GetHctsByTemp();

        var coldest = hctsByTemp.First();
        var warmest = hctsByTemp.Last();

        double coldestTemp = temps[coldest];
        double warmestTemp = temps[warmest];
        double range = warmestTemp - coldestTemp;

        double coldestH = coldest.H;
        double warmestH = warmest.H;

        bool startHueIsColdestToWarmest = GetIsBetween(_hct.H, coldestH, warmestH);
        double startH = startHueIsColdestToWarmest ? warmestH : coldestH;
        double endH = startHueIsColdestToWarmest ? coldestH : warmestH;
        double directionOfRotation = 1.0;
        double smallestError = 1000.0;

        var hctsByHue = GetHctsByHue();

        Hct answer = hctsByHue[Convert.ToInt32(Math.Round(_hct.H))];
        double complementRelativeTemp = GetRelativeTemperature(_hct);

        // Find the color in the other section, closest to the inverse percentile
        // of the input color. This is the complement.
        for (double i = 0.0; i <= 360.0; i += 1.0) {
            var h = MathUtil.SanitizeDegrees(startH + directionOfRotation * i);
            if (!GetIsBetween(h, startH, endH))
                continue;

            var possibleAnswer = hctsByHue[Convert.ToInt32(Math.Round(h))];

            // 这里直接用相同 range/coldestTemp 计算相对温度，避免重复调用 GetRelativeTemperature
            var relativeTemp = range == 0.0
                ? 0.5
                : (temps[possibleAnswer] - coldestTemp) / range;

            var error = Math.Abs(complementRelativeTemp - relativeTemp);

            if (error < smallestError) {
                smallestError = error;
                answer = possibleAnswer;
            }
        }

        _precomputedComplement = answer;
        return _precomputedComplement;
    }

    /// <summary>
    /// A set of colors with differing hues, equidistant in temperature.
    /// </summary>
    /// <param name="count">The number of colors to return, includes the input color.</param>
    /// <param name="divisons">The number of divisions on the color wheel.</param>
    public List<Hct> GetAnalogousColors(int count, int divisions) {
        var hctsByHue = GetHctsByHue();
        var temps = GetTempsByHct();

        int startH = Convert.ToInt32(Math.Round(_hct.H));
        Hct startHct = hctsByHue[startH];

        double lastTemp = GetRelativeTemperature(startHct);

        var allColors = new List<Hct> {
            startHct
        };

        double absoluteTotalTempDelta = 0.0;
        for (int i = 0; i < 360; i++) {
            int h = Convert.ToInt32(MathUtil.SanitizeDegrees(startH + i));
            Hct hct = hctsByHue[h];
            double temp = GetRelativeTemperature(hct);
            double tempDelta = Math.Abs(temp - lastTemp);

            lastTemp = temp;
            absoluteTotalTempDelta += tempDelta;
        }

        int hAddend = 1;
        double tempStep = absoluteTotalTempDelta / divisions;
        double totalTempDelta = 0.0;
        lastTemp = GetRelativeTemperature(startHct);

        while (allColors.Count < divisions) {
            int h = Convert.ToInt32(MathUtil.SanitizeDegrees(startH + hAddend));
            Hct hct = hctsByHue[h];
            double temp = GetRelativeTemperature(hct);
            double tempDelta = Math.Abs(temp - lastTemp);
            totalTempDelta += tempDelta;

            double desiredTotalTempDeltaForIndex = allColors.Count * tempStep;
            bool indexSatisfied = totalTempDelta >= desiredTotalTempDeltaForIndex;
            int indexAddend = 1;

            // Keep adding this hue to the answers until its temperature is
            // insufficient. This ensures consistent behavior when there aren't
            // `divisions` discrete steps between 0 and 360 in hue with `tempStep`
            // delta in temperature between them.
            //
            // For example, white and black have no analogues: there are no other
            // colors at T100/T0. Therefore, they should just be added to the array
            // as answers.
            while (indexSatisfied && allColors.Count < divisions) {
                allColors.Add(hct);
                desiredTotalTempDeltaForIndex = (allColors.Count + indexAddend) * tempStep;
                indexSatisfied = totalTempDelta >= desiredTotalTempDeltaForIndex;
                indexAddend++;
            }

            lastTemp = temp;
            hAddend++;

            if (hAddend > 360) {
                // 原代码这里是 while (allColors.Count > divisions) allColors.Add(hct);
                // 会死循环，这里按注释语义修正为填满不足的情况
                while (allColors.Count < divisions)
                    allColors.Add(hct);

                break;
            }
        }

        var answers = new List<Hct> {
            _hct
        };

        int ccwCount = (int)Math.Floor(((double)count - 1.0) / 2.0);
        for (int i = 1; i < (ccwCount + 1); i++) {
            int index = 0 - i;
            while (index < 0)
                index += allColors.Count + index;

            if (index >= allColors.Count)
                index %= allColors.Count;

            answers.Insert(0, allColors[index]);
        }

        int cwCount = count - ccwCount - 1;
        for (int i = 1; i < (cwCount + 1); i++) {
            int index = i;
            while (index < 0) {
                index = allColors.Count + index;
            }
            if (index >= allColors.Count) {
                index %= allColors.Count;
            }
            answers.Add(allColors[index]);
        }

        return answers;
    }

    /// <summary>
    /// HCTs for all colors with the same chroma/tone as the input.
    /// </summary>
    private List<Hct> GetHctsByTemp() {
        if (_precomputedHctsByTemp != null)
            return _precomputedHctsByTemp;

        var temps = GetTempsByHct();
        var hcts = temps.Keys.ToList();
        hcts.Sort((a, b) => temps[a].CompareTo(temps[b]));
        _precomputedHctsByTemp = hcts;
        return _precomputedHctsByTemp;
    }

    /// <summary>
    /// HCTs for all colors with the same chroma/tone as the input.
    /// </summary>
    private List<Hct> GetHctsByHue() {
        if (_precomputedHctsByHue != null)
            return _precomputedHctsByHue;

        var hcts = new List<Hct>(361);
        for (int i = 0; i <= 360; i++) {
            var colorAtH = Hct.Parse(i, _hct.C, _hct.T);
            hcts.Add(colorAtH);
        }

        _precomputedHctsByHue = hcts;
        return _precomputedHctsByHue;
    }

    /// <summary>
    /// Keys of HCTs in getHctsByTemp, values of raw temperature.
    /// </summary>
    private Dictionary<Hct, double> GetTempsByHct() {
        if (_precomputedTempsByHct != null)
            return _precomputedTempsByHct;

        var hcts = GetHctsByHue();
        hcts.Add(_hct);

        var temperaturesByHct = new Dictionary<Hct, double>(hcts.Count);
        foreach (var hct in hcts) {
            if (!temperaturesByHct.ContainsKey(hct))
                temperaturesByHct[hct] = GetRawTemperature(hct);
        }

        _precomputedTempsByHct = temperaturesByHct;
        return _precomputedTempsByHct;
    }

    /// <summary>
    /// Temperature relative to all colors with the same chroma and tone.
    /// </summary>
    public double GetRelativeTemperature(Hct hct) {
        var temps = GetTempsByHct();
        var warmest = Warmest;
        var coldest = Coldest;

        double range = temps[warmest] - temps[coldest];
        double differenceFromColdest = temps[hct] - temps[coldest];

        return range is 0.0 ? 0.5 : differenceFromColdest / range;
    }

    public static double GetRawTemperature(Hct hct) {
        double[] lab = ColorUtil.LabFromArgb(hct.ToUInt32());

        double h = MathUtil.SanitizeDegrees(MathUtil.Degrees(Math.Atan2(lab[2], lab[1])));
        double c = MathUtil.Hypot(lab[1], lab[2]);

        return -0.5 + 0.02 * Math.Pow(c, 1.07) * Math.Cos(MathUtil.Radians(MathUtil.SanitizeDegrees(h - 50.0)));
    }

    private static bool GetIsBetween(double angle, double a, double b) {
        if (a < b)
            return a <= angle && angle <= b;

        return a <= angle || angle <= b;
    }
}
