using Monet.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Monet.Shared;
public static class Contrast {
    public const double RATIO_MIN = 1.0;
    public const double RATIO_MAX = 21.0;
    public const double RATIO_30 = 3.0;
    public const double RATIO_45 = 4.5;
    public const double RATIO_70 = 7.0;

    private const double CONTRAST_RATIO_EPSILON = 0.04;
    private const double LUMINANCE_GAMUT_MAP_TOLERANCE = 0.4;

    public static double RatioOfYs(double y1, double y2) {
        var lighter = Math.Max(y1, y2);
        var darker = (lighter == y2) ? y1 : y2;

        return (lighter + 5.0) / (darker + 5.0);
    }

    public static double RatioOfTones(double t1, double t2) {
        return RatioOfYs(ColorUtil.YFromLstar(t1), ColorUtil.YFromLstar(t2));
    }

    public static double Lighter(double t, double ratio) {
        if (t is < 0.0 or > 100.0)
            return -1.0;

        // Invert the contrast ratio equation to determine lighter Y given a ratio and darker Y.
        var darkY = ColorUtil.YFromLstar(t);
        var lightY = ratio * (darkY + 5.0) - 5.0;

        if (lightY is < 0.0 or > 100.0)
            return -1.0;

        var realContrast = RatioOfYs(lightY, darkY);
        var delta = Math.Abs(realContrast - ratio);

        if (realContrast < ratio && delta > CONTRAST_RATIO_EPSILON)
            return -1.0;

        var result = ColorUtil.LstarFromY(lightY) + LUMINANCE_GAMUT_MAP_TOLERANCE;
        if (result is < 0.0 or > 100.0)
            return -1.0;

        return result;
    }

    public static double LighterUnsafe(double t, double ratio) {
        var lighterSafe = Lighter(t, ratio);
        return lighterSafe < 0.0 ? 100.0 : lighterSafe;
    }

    public static double Darker(double t, double ratio) {
        if (t is < 0.0 or > 100.0)
            return -1.0;

        // Invert the contrast ratio equation to determine lighter Y given a ratio and darker Y.
        var lightY = ColorUtil.YFromLstar(t);
        var darkY = ((lightY + 5.0) / ratio) - 5.0;

        if(darkY is < 0.0 or > 100.0)
            return -1.0;

        var realContrast = RatioOfYs(lightY, darkY);
        var delta = Math.Abs(realContrast - ratio);

        if (realContrast < ratio && delta > CONTRAST_RATIO_EPSILON)
            return -1.0;

        var result = ColorUtil.LstarFromY(darkY) - LUMINANCE_GAMUT_MAP_TOLERANCE;
        if(result is < 0.0 || result > 100.0)
            return -1.0;

        return result;
    }

    public static double DarkerUnsafe(double t, double ratio) {
        var darkerSafe = Darker(t, ratio);
        return Math.Max(0.0, darkerSafe);
    }
}
