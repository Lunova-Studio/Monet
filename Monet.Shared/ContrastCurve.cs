using Monet.Shared.Utilities;

namespace Monet.Shared;

public sealed class ContrastCurve {
    private double _low; //level -1.0
    private double _normal; //level 0.0
    private double _medium; //level 0.5
    private double _high; //level 1.0

    public ContrastCurve(double low, double normal, double medium, double high) {
        _low = low;
        _high = high;
        _normal = normal;
        _medium = medium;
    }

    public double GetContrastRatios(double level) {
        if (level <= -1.0)
            return _low;
        else if (level < 0.0)
            return MathUtil.Lerp(_low, _normal, (level - -1) / 1);
        else if (level < 0.5)
            return MathUtil.Lerp(_normal, _medium, (level - 0) / 0.5);
        else if (level < 1.0)
            return MathUtil.Lerp(_medium, _high, (level - 0.5) / 0.5);
        else
            return _high;
    }
}