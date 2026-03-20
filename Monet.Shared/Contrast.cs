using Monet.Shared.Utilities;

namespace Monet.Shared;

public static class Contrast {
    public const double RATIO_30 = 3.0;
    public const double RATIO_45 = 4.5;
    public const double RATIO_70 = 7.0;
    public const double RATIO_MIN = 1.0;
    public const double RATIO_MAX = 21.0;

    private const double CONTRAST_RATIO_EPSILON = 0.04;
    private const double LUMINANCE_GAMUT_MAP_TOLERANCE = 0.4;

    public static double RatioOfYs(double y1, double y2) {
        double lighter = y1 > y2 ? y1 : y2;
        double darker = y1 > y2 ? y2 : y1;

        return (lighter + 5.0) / (darker + 5.0);
    }

    public static double RatioOfTones(double t1, double t2)
        => RatioOfYs(ColorUtil.YFromLstar(t1), ColorUtil.YFromLstar(t2));

    public static double Lighter(double t, double ratio) {
        if (t is < 0.0 or > 100.0)
            return -1.0;

        double darkY = ColorUtil.YFromLstar(t);
        double lightY = ratio * (darkY + 5.0) - 5.0;

        if (lightY is < 0.0 or > 100.0)
            return -1.0;

        double realContrast = RatioOfYs(lightY, darkY);
        double delta = Math.Abs(realContrast - ratio);

        if (realContrast < ratio && delta > CONTRAST_RATIO_EPSILON)
            return -1.0;

        double result = ColorUtil.LstarFromY(lightY) + LUMINANCE_GAMUT_MAP_TOLERANCE;
        return result is >= 0.0 and <= 100.0 ? result : -1.0;
    }

    public static double LighterUnsafe(double t, double ratio) {
        double lighterSafe = Lighter(t, ratio);
        return lighterSafe < 0.0 ? 100.0 : lighterSafe;
    }

    public static double Darker(double t, double ratio) {
        if (t is < 0.0 or > 100.0)
            return -1.0;

        double lightY = ColorUtil.YFromLstar(t);
        double darkY = (lightY + 5.0) / ratio - 5.0;

        if (darkY is < 0.0 or > 100.0)
            return -1.0;

        double realContrast = RatioOfYs(lightY, darkY);
        double delta = Math.Abs(realContrast - ratio);

        if (realContrast < ratio && delta > CONTRAST_RATIO_EPSILON)
            return -1.0;

        double result = ColorUtil.LstarFromY(darkY) - LUMINANCE_GAMUT_MAP_TOLERANCE;
        return result is >= 0.0 and <= 100.0 ? result : -1.0;
    }

    public static double DarkerUnsafe(double t, double ratio) {
        double darkerSafe = Darker(t, ratio);
        return darkerSafe < 0.0 ? 0.0 : darkerSafe;
    }
}