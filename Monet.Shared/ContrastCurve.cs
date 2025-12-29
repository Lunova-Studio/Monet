using Monet.Shared.Utilities;

namespace Monet.Shared;

public sealed class ContrastCurve {
    private readonly double _low;    // level -1.0
    private readonly double _normal; // level  0.0
    private readonly double _medium; // level  0.5
    private readonly double _high;   // level  1.0

    public ContrastCurve(double low, double normal, double medium, double high) {
        _low = low;
        _normal = normal;
        _medium = medium;
        _high = high;
    }

    public double GetContrastRatios(double level) {
        if (level <= -1.0)
            return _low;

        if (level < 0.0)
            return MathUtil.Lerp(_low, _normal, level + 1.0);

        if (level < 0.5)
            return MathUtil.Lerp(_normal, _medium, level * 2.0);

        if (level < 1.0)
            return MathUtil.Lerp(_medium, _high, (level - 0.5) * 2.0);

        return _high;
    }
}