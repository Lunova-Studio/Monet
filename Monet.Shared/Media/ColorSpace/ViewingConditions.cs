using Monet.Shared.Utilities;

namespace Monet.Shared.Media.ColorSpace;

public readonly struct ViewingConditions {
    public static readonly ViewingConditions Default =
        DefaultWithBackgroundLstar(50.0);

    public required double C { get; init; }
    public required double Z { get; init; }
    public required double N { get; init; }
    public required double Aw { get; init; }
    public required double Nc { get; init; }
    public required double Fl { get; init; }
    public required double Ncb { get; init; }
    public required double Nbb { get; init; }
    public required double FlRoot { get; init; }
    public required double[] RgbD { get; init; }

    public static ViewingConditions DefaultWithBackgroundLstar(double lstar) {
        var whitePoint = ColorUtil.WHITE_POINT_D65;
        double backgroundY = (200.0 / Math.PI) * (ColorUtil.YFromLstar(50.0) / 100.0);
        double surround = 2.0;
        bool discount = false;

        return Create(
            whitePoint,
            backgroundY,
            lstar,
            surround,
            discount);
    }

    public static ViewingConditions Create(
        double[] whitePoint,
        double adaptingLuminance,
        double backgroundLstar,
        double surround,
        bool discountingIlluminant) {
        // 避免纯黑背景导致的非物理结果
        backgroundLstar = Math.Max(0.1, backgroundLstar);

        // XYZ -> CAM16RGB（这里必须用 XYZ_TO_CAM16RGB）
        double rW =
            whitePoint[0] * Cam16.XYZ_TO_CAM16RGB[0, 0] +
            whitePoint[1] * Cam16.XYZ_TO_CAM16RGB[0, 1] +
            whitePoint[2] * Cam16.XYZ_TO_CAM16RGB[0, 2];

        double gW =
            whitePoint[0] * Cam16.XYZ_TO_CAM16RGB[1, 0] +
            whitePoint[1] * Cam16.XYZ_TO_CAM16RGB[1, 1] +
            whitePoint[2] * Cam16.XYZ_TO_CAM16RGB[1, 2];

        double bW =
            whitePoint[0] * Cam16.XYZ_TO_CAM16RGB[2, 0] +
            whitePoint[1] * Cam16.XYZ_TO_CAM16RGB[2, 1] +
            whitePoint[2] * Cam16.XYZ_TO_CAM16RGB[2, 2];

        double f = 0.8 + surround / 10.0;

        double c = f >= 0.9
            ? MathUtil.Lerp(0.59, 0.69, (f - 0.9) * 10.0)
            : MathUtil.Lerp(0.525, 0.59, (f - 0.8) * 10.0);

        double d = discountingIlluminant
            ? 1.0
            : f * (1.0 - (1.0 / 3.6) * Math.Exp(-(adaptingLuminance - 42.0) / 92.0));

        d = Math.Clamp(d, 0.0, 1.0);
        double nc = f;

        double invRw = 100.0 / rW;
        double invGw = 100.0 / gW;
        double invBw = 100.0 / bW;

        double[] rgbD = [
            d * invRw + 1.0 - d,
            d * invGw + 1.0 - d,
            d * invBw + 1.0 - d
        ];

        double k = 1.0 / (5.0 * adaptingLuminance + 1.0);
        double k4 = k * k * k * k;
        double k4F = 1.0 - k4;

        double fl = k4 * adaptingLuminance + 0.1 * k4F * k4F * Math.Pow(5.0 * adaptingLuminance, 1.0 / 3.0);
        double n = ColorUtil.YFromLstar(backgroundLstar) / whitePoint[1];
        double z = 1.48 + Math.Sqrt(n);

        double nbb = 0.725 / Math.Pow(n, 0.2);
        double ncb = nbb;

        double flRw = fl * rgbD[0] * rW / 100.0;
        double flGw = fl * rgbD[1] * gW / 100.0;
        double flBw = fl * rgbD[2] * bW / 100.0;

        double rAF = Math.Pow(flRw, 0.42);
        double gAF = Math.Pow(flGw, 0.42);
        double bAF = Math.Pow(flBw, 0.42);

        double rA = 400.0 * rAF / (rAF + 27.13);
        double gA = 400.0 * gAF / (gAF + 27.13);
        double bA = 400.0 * bAF / (bAF + 27.13);

        double aw = (2.0 * rA + gA + 0.05 * bA) * nbb;

        return new ViewingConditions {
            C = c,
            Z = z,
            N = n,
            Nc = nc,
            Fl = fl,
            Aw = aw,
            Nbb = nbb,
            Ncb = ncb,
            RgbD = rgbD,
            FlRoot = Math.Pow(fl, 0.25)
        };
    }
}