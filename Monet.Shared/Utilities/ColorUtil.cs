using Monet.Shared.Media.ColorSpace;
using System.Runtime.CompilerServices;

namespace Monet.Shared.Utilities;

public static class ColorUtil {
    public const uint GOOGLE_BLUE = 0xFF1b6ef3;
    private const double GAMMA = 2.4;
    private const double INV_3 = 1.0 / 3.0;
    private const double INV_2_4 = 1.0 / 2.4;
    private const double LAB_E = 216.0 / 24389.0;
    private const double LAB_KAPPA = 24389.0 / 27.0;

    internal static readonly double[] WHITE_POINT_D65 = [95.047, 100.0, 108.883];

    private static readonly double[] SRGB_TO_XYZ_FLAT = [
        0.41233895, 0.35762064, 0.18051042,
        0.2126, 0.7152, 0.0722,
        0.01932141, 0.11916382, 0.95034478
    ];

    private static readonly double[] XYZ_TO_SRGB_FLAT = [
        3.2413774792388685, -1.5376652402851851, -0.49885366846268053,
       -0.9691452513005321, 1.8758853451067872, 0.04156585616912061,
        0.05562093689691305, -0.20395524564742123, 1.0571799111220335
    ];

    public static uint ToBuleByArgb(uint argb) => argb & 255u;

    public static uint ToRedByArgb(uint argb) => (argb >> 16) & 255u;

    public static uint ToGreenByArgb(uint argb) => (argb >> 8) & 255u;

    public static double LstarFromY(double y) => 116.0 * LabF(y / 100.0) - 16.0;

    public static double YFromLstar(double lstar) => 100.0 * LabInvf((lstar + 16.0) / 116.0);

    public static Hct Fix(Hct hct) => GetIsDisliked(hct) ? Hct.Parse(hct.H, hct.C, 70.0) : hct;

    public static uint RgbToArgb(uint r, uint g, uint b) => (255u << 24) | ((r & 255u) << 16) | ((g & 255u) << 8) | (b & 255u);

    private static double LabF(double t) => t > LAB_E ? Math.Pow(t, INV_3) : (LAB_KAPPA * t + 16.0) / 116.0;

    public static double LStarFromArgb(uint argb) {
        double r = Linearized(ToRedByArgb(argb));
        double b = Linearized(ToBuleByArgb(argb));
        double g = Linearized(ToGreenByArgb(argb));
        double y = 0.2126 * r + 0.7152 * g + 0.0722 * b;

        return 116.0 * LabF(y / 100.0) - 16.0;
    }

    public static uint ArgbFromLstar(double lstar) {
        double y = YFromLstar(lstar);
        uint c = DeLinearized(y);

        return RgbToArgb(c, c, c);
    }

    public static uint LinrgbToArgb(double[] linrgb) {
        uint r = DeLinearized(linrgb[0]);
        uint g = DeLinearized(linrgb[1]);
        uint b = DeLinearized(linrgb[2]);

        return RgbToArgb(r, g, b);
    }

    public static uint DeLinearized(double rgbComponent) {
        double x = rgbComponent / 100.0;
        double v = x <= 0.0031308
            ? x * 12.92
            : 1.055 * Math.Pow(x, INV_2_4) - 0.055;

        return MathUtil.ClampUint(0, 255, (int)Math.Round(v * 255.0));
    }

    public static double[] LabFromArgb(uint argb) {
        double r = Linearized(ToRedByArgb(argb));
        double g = Linearized(ToGreenByArgb(argb));
        double b = Linearized(ToBuleByArgb(argb));

        Mul3(r, g, b, SRGB_TO_XYZ_FLAT, out double x, out double y, out double z);

        double fx = LabF(x / WHITE_POINT_D65[0]);
        double fy = LabF(y / WHITE_POINT_D65[1]);
        double fz = LabF(z / WHITE_POINT_D65[2]);

        return [
            116.0 * fy - 16.0,
            500.0 * (fx - fy),
            200.0 * (fy - fz)
        ];
    }

    public static uint ArgbFromLab(double l, double a, double b) {
        double fy = (l + 16.0) / 116.0;
        double fx = fy + a / 500.0;
        double fz = fy - b / 200.0;

        double x = LabInvf(fx) * WHITE_POINT_D65[0];
        double y = LabInvf(fy) * WHITE_POINT_D65[1];
        double z = LabInvf(fz) * WHITE_POINT_D65[2];

        return ArgbFromXyz(x, y, z);
    }

    public static uint ArgbFromXyz(double x, double y, double z) {
        Mul3(x, y, z, XYZ_TO_SRGB_FLAT, out double lr, out double lg, out double lb);

        return RgbToArgb(DeLinearized(lr), DeLinearized(lg), DeLinearized(lb));
    }

    public static double Linearized(uint c) {
        double x = c / 255.0;

        return x <= 0.040449936
            ? (x / 12.92) * 100.0
            : Math.Pow((x + 0.055) / 1.055, GAMMA) * 100.0;
    }

    public static double[] XYZFromArgb(uint argb) {
        double r = Linearized(ToRedByArgb(argb));
        double g = Linearized(ToGreenByArgb(argb));
        double b = Linearized(ToBuleByArgb(argb));
        Mul3(r, g, b, SRGB_TO_XYZ_FLAT, out double x, out double y, out double z);

        return [x, y, z];
    }

    public static bool GetIsDisliked(Hct hct) {
        double h = Math.Round(hct.H);
        double c = Math.Round(hct.C);
        double t = Math.Round(hct.T);

        return h >= 90.0 && h <= 111.0 && c > 16.0 && t < 65.0;
    }

    private static double LabInvf(double ft) {
        double ft3 = ft * ft * ft;

        return ft3 > LAB_E ? ft3 : (116.0 * ft - 16.0) / LAB_KAPPA;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Mul3(double r, double g, double b, double[] m, out double x, out double y, out double z) {
        x = r * m[0] + g * m[1] + b * m[2];
        y = r * m[3] + g * m[4] + b * m[5];
        z = r * m[6] + g * m[7] + b * m[8];
    }
}