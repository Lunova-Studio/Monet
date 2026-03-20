using Monet.Shared.Utilities;
using System.Runtime.CompilerServices;

namespace Monet.Shared.Media.ColorSpace;

public struct Cam16 {
    internal static readonly double[,] XYZ_TO_CAM16RGB = {
        { 0.401288, 0.650173, -0.051461 },
        { -0.250268, 1.204414, 0.045854 },
        { -0.002079, 0.048952, 0.953127 }
    };

    private static readonly double[] CAM16RGB_TO_XYZ_FLAT = [
        1.8620678, -1.0112547, 0.14918678,
        0.38752654, 0.62144744, -0.00897398,
       -0.01584150, -0.03412294, 1.0499644
    ];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Mul3(double r, double g, double b, double[] m, out double x, out double y, out double z) {
        x = r * m[0] + g * m[1] + b * m[2];
        y = r * m[3] + g * m[4] + b * m[5];
        z = r * m[6] + g * m[7] + b * m[8];
    }

    public double H { get; set; }
    public double C { get; set; }
    public double J { get; set; }
    public double Q { get; set; }
    public double M { get; set; }
    public double S { get; set; }

    private double JStar { get; set; }
    private double AStar { get; set; }
    private double BStar { get; set; }

    public Cam16(double h, double c, double j, double q, double m, double s, double jstar, double astar, double bstar) {
        H = h;
        C = c;
        J = j;
        Q = q;
        M = m;
        S = s;
        JStar = jstar;
        AStar = astar;
        BStar = bstar;
    }

    public readonly double Distance(Cam16 other) {
        double dJ = JStar - other.JStar;
        double dA = AStar - other.AStar;
        double dB = BStar - other.BStar;

        double dE = Math.Sqrt(dJ * dJ + dA * dA + dB * dB);
        return 1.41 * Math.Pow(dE, 0.63);
    }

    public readonly double[] XyzInViewingConditions(ViewingConditions vc, double[] returnArray) {
        double alpha = (C == 0.0 || J == 0.0) ? 0.0 : C / Math.Sqrt(J / 100.0);

        double t = Math.Pow(alpha / Math.Pow(1.64 - Math.Pow(0.29, vc.N), 0.73), 1.0 / 0.9);
        double hRad = MathUtil.Radians(H);

        double eHue = 0.25 * (Math.Cos(hRad + 2.0) + 3.8);
        double ac = vc.Aw * Math.Pow(J / 100.0, 1.0 / vc.C / vc.Z);
        double p1 = eHue * (50000.0 / 13.0) * vc.Nc * vc.Ncb;
        double p2 = ac / vc.Nbb;

        double hSin = Math.Sin(hRad);
        double hCos = Math.Cos(hRad);

        double gamma = 23.0 * (p2 + 0.305) * t /
                       (23.0 * p1 + 11.0 * t * hCos + 108.0 * t * hSin);

        double a = gamma * hCos;
        double b = gamma * hSin;

        double rA = (460.0 * p2 + 451.0 * a + 288.0 * b) / 1403.0;
        double gA = (460.0 * p2 - 891.0 * a - 261.0 * b) / 1403.0;
        double bA = (460.0 * p2 - 220.0 * a - 6300.0 * b) / 1403.0;

        // Chromatic adaptation (optimized)
        double rC = Math.Sign(rA) * (100.0 / vc.Fl) *
                    Math.Pow(Math.Max(0, 27.13 * Math.Abs(rA) / (400.0 - Math.Abs(rA))), 1.0 / 0.42);

        double gC = Math.Sign(gA) * (100.0 / vc.Fl) *
                    Math.Pow(Math.Max(0, 27.13 * Math.Abs(gA) / (400.0 - Math.Abs(gA))), 1.0 / 0.42);

        double bC = Math.Sign(bA) * (100.0 / vc.Fl) *
                    Math.Pow(Math.Max(0, 27.13 * Math.Abs(bA) / (400.0 - Math.Abs(bA))), 1.0 / 0.42);

        double rF = rC / vc.RgbD[0];
        double gF = gC / vc.RgbD[1];
        double bF = bC / vc.RgbD[2];

        // 高性能路径：CAM16RGB → XYZ
        Mul3(rF, gF, bF, CAM16RGB_TO_XYZ_FLAT, out double x, out double y, out double z);

        if (returnArray != null) {
            returnArray[0] = x;
            returnArray[1] = y;
            returnArray[2] = z;
            return returnArray;
        }

        return [x, y, z];
    }

    public static Cam16 Parse(uint argb)
        => ParseByViewingConditions(argb, ViewingConditions.Default);

    internal static Cam16 ParseByViewingConditions(uint argb, ViewingConditions vc) {
        uint r = (argb >> 16) & 255;
        uint g = (argb >> 8) & 255;
        uint b = argb & 255;

        double rL = ColorUtil.Linearized(r);
        double gL = ColorUtil.Linearized(g);
        double bL = ColorUtil.Linearized(b);

        // sRGB → XYZ（直接内联）
        double x = 0.41233895 * rL + 0.35762064 * gL + 0.18051042 * bL;
        double y = 0.2126 * rL + 0.7152 * gL + 0.0722 * bL;
        double z = 0.01932141 * rL + 0.11916382 * gL + 0.95034478 * bL;

        return ParseByXyzByViewingConditions(x, y, z, vc);
    }

    internal static Cam16 ParseByXyzByViewingConditions(double x, double y, double z, ViewingConditions vc) {
        // XYZ → CAM16RGB（保持 double[,]）
        var mat = XYZ_TO_CAM16RGB;

        double rT = x * mat[0, 0] + y * mat[0, 1] + z * mat[0, 2];
        double gT = x * mat[1, 0] + y * mat[1, 1] + z * mat[1, 2];
        double bT = x * mat[2, 0] + y * mat[2, 1] + z * mat[2, 2];

        // Discount illuminant
        double rD = vc.RgbD[0] * rT;
        double gD = vc.RgbD[1] * gT;
        double bD = vc.RgbD[2] * bT;

        // Chromatic adaptation
        double rAF = Math.Pow(vc.Fl * Math.Abs(rD) / 100.0, 0.42);
        double gAF = Math.Pow(vc.Fl * Math.Abs(gD) / 100.0, 0.42);
        double bAF = Math.Pow(vc.Fl * Math.Abs(bD) / 100.0, 0.42);

        double rA = Math.Sign(rD) * 400.0 * rAF / (rAF + 27.13);
        double gA = Math.Sign(gD) * 400.0 * gAF / (gAF + 27.13);
        double bA = Math.Sign(bD) * 400.0 * bAF / (bAF + 27.13);

        // redness-greenness
        double a = (11.0 * rA - 12.0 * gA + bA) / 11.0;

        // yellowness-blueness
        double bb = (rA + gA - 2.0 * bA) / 9.0;

        // auxiliary components
        double u = (20.0 * rA + 20.0 * gA + 21.0 * bA) / 20.0;
        double p2 = (40.0 * rA + 20.0 * gA + bA) / 20.0;

        // hue
        double hue = MathUtil.Degrees(Math.Atan2(bb, a));
        if (hue < 0) hue += 360.0;
        else if (hue >= 360.0) hue -= 360.0;

        double hueRad = MathUtil.Radians(hue);

        // achromatic response
        double ac = p2 * vc.Nbb;

        // lightness
        double j = 100.0 * Math.Pow(ac / vc.Aw, vc.C * vc.Z);

        // brightness
        double q = 4.0 / vc.C * Math.Sqrt(j / 100.0) * (vc.Aw + 4.0) * vc.FlRoot;

        // chroma / colorfulness / saturation
        double huePrime = hue < 20.14 ? hue + 360.0 : hue;
        double eHue = 0.25 * (Math.Cos(MathUtil.Radians(huePrime) + 2.0) + 3.8);
        double p1 = 50000.0 / 13.0 * eHue * vc.Nc * vc.Ncb;

        double t = p1 * MathUtil.Hypot(a, bb) / (u + 0.305);
        double alpha = Math.Pow(1.64 - Math.Pow(0.29, vc.N), 0.73) * Math.Pow(t, 0.9);

        double c = alpha * Math.Sqrt(j / 100.0);
        double m = c * vc.FlRoot;
        double s = 50.0 * Math.Sqrt((alpha * vc.C) / (vc.Aw + 4.0));

        // CAM16-UCS
        double jStar = (1.0 + 100.0 * 0.007) * j / (1.0 + 0.007 * j);
        double mStar = Math.Log(1.0 + 0.0228 * m) / 0.0228;
        double aStar = mStar * Math.Cos(hueRad);
        double bStar = mStar * Math.Sin(hueRad);

        return new Cam16(hue, c, j, q, m, s, jStar, aStar, bStar);
    }
}