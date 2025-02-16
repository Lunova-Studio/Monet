using Bless.Monet.Utilities;

namespace Bless.Monet.Media;

public readonly struct ViewingConditions {
    public static readonly ViewingConditions Default =
        DefaultWithBackgroundLstar(50.0);

    internal double C { get; }
    internal double Z { get; }
    internal double N { get; }
    internal double Aw { get; }
    internal double Nc { get; }
    internal double Fl { get; }
    internal double Ncb { get; }
    internal double Nbb { get; }
    internal double FlRoot { get; }
    internal double[] RgbD { get; }

    internal ViewingConditions(double n, double aw, double nbb, double ncb, double c,
        double nc, double[] rgbD, double fl, double flRoot, double z) {
        //厚礼蟹这沟槽的谷歌
        N = n;
        Aw = aw;
        Nbb = nbb;
        Ncb = ncb;
        C = c;
        Nc = nc;
        RgbD = rgbD;
        Fl = fl;
        FlRoot = flRoot;
        Z = z;
    }

    public static ViewingConditions DefaultWithBackgroundLstar(double lstar) {
        var whitePoint = ColorUtil.WHITE_POINT_D65;
        double backgroundY = (200.0 / Math.PI) * (ColorUtil.YFromLstar(50.0) / 100.0);
        double surround = 2.0;
        bool isFluorescent = false;

        return Create(
            whitePoint,
            backgroundY,
            lstar,
            surround,
            isFluorescent);
    }

    /// <summary>
    /// Create ViewingConditions from a simple, physically relevant, set of parameters.
    /// </summary>
    /// <param name="whitePoint">White point, measured in the XYZ color space. default = D65, or sunny day afternoon</param>
    /// <param name="adaptingLuminance"></param>
    /// <param name="backgroundLstar"></param>
    /// <param name="surround"></param>
    /// <param name="discountingIlluminant"></param>
    /// <returns></returns>
    public static ViewingConditions Create(double[] whitePoint, double adaptingLuminance,
        double backgroundLstar, double surround, bool discountingIlluminant) {
        // A background of pure black is non-physical and leads to infinities that represent the idea
        // that any color viewed in pure black can't be seen.
        backgroundLstar = Math.Max(0.1, backgroundLstar);

        // Transform white point XYZ to 'cone'/'rgb' responses
        double[,] matrix = Cam16.XYZ_TO_CAM16RGB;
        double[] xyz = whitePoint;

        double rW = 0, gW = 0, bW = 0;
        for (int i = 0; i < 3; i++) {
            double sum = 0;
            for (int j = 0; j < 3; j++) {
                sum += whitePoint[j] * Cam16.XYZ_TO_CAM16RGB[i, j];
            }

            if (i == 0) rW = sum;
            else if (i == 1) gW = sum;
            else if (i == 2) bW = sum;
        }

        double f = 0.8 + (surround / 10.0);
        double c = f >= 0.9
            ? MathUtil.Lerp(0.59, 0.69, (f - 0.9) * 10.0)
            : MathUtil.Lerp(0.525, 0.59, (f - 0.8) * 10.0);

        double d = discountingIlluminant
            ? 1.0
            : f * (1.0 - (1.0 / 3.6) * Math.Exp(-(adaptingLuminance - 42.0) / 92.0));

        d = Math.Clamp(d, 0.0, 1.0);
        double nc = f;

        double[] rgbD = [
            d * (100.0 / rW) + 1.0 - d,
            d * (100.0 / gW) + 1.0 - d,
            d * (100.0 / bW) + 1.0 - d
        ];

        double k = 1.0 / (5.0 * adaptingLuminance + 1.0);
        double k4 = Math.Pow(k, 4);
        double k4F = 1.0 - k4;
        double fl = k4 * adaptingLuminance + 0.1 * k4F * k4F * Math.Pow(5.0 * adaptingLuminance, 1.0 / 3.0);

        double n = (ColorUtil.YFromLstar(backgroundLstar) / whitePoint[1]);
        double z = 1.48 + Math.Sqrt(n);

        double nbb = 0.725 / Math.Pow(n, 0.2);
        double ncb = nbb;

        double[] rgbAFactors = [
            Math.Pow(fl * rgbD[0] * rW / 100.0, 0.42),
            Math.Pow(fl * rgbD[1] * gW / 100.0, 0.42),
            Math.Pow(fl * rgbD[2] * bW / 100.0, 0.42)
        ];

        double[] rgbA = [
            (400.0 * rgbAFactors[0]) / (rgbAFactors[0] + 27.13),
            (400.0 * rgbAFactors[1]) / (rgbAFactors[1] + 27.13),
            (400.0 * rgbAFactors[2]) / (rgbAFactors[2] + 27.13)
        ];

        double aw = (2.0 * rgbA[0] + rgbA[1] + 0.05 * rgbA[2]) * nbb;
        return new ViewingConditions(n, aw, nbb, ncb, c, nc, rgbD, fl,
            Math.Pow(fl, 0.25), z);
    }
}