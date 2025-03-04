using Monet.Shared.Utilities;

namespace Monet.Shared.Media.ColorSpace;

/// <summary>
/// CAM16, a color appearance model. Colors are not just defined by their hex code, but rather, a hex
/// code and viewing conditions.
/// </summary>
public struct Cam16 {
    internal static readonly double[,] XYZ_TO_CAM16RGB = {
        { 0.401288, 0.650173, -0.051461 },
        { -0.250268, 1.204414, 0.045854 },
        { -0.002079, 0.048952, 0.953127 }
    };

    internal static readonly double[,] CAM16RGB_TO_XYZ = {
        {1.8620678, -1.0112547, 0.14918678},
        {0.38752654, 0.62144744, -0.00897398},
        {-0.01584150, -0.03412294, 1.0499644}
    };

    /// <summary>
    ///  Avoid allocations during conversion by pre-allocating an array.
    /// </summary>
    private readonly double[] TempArray = [0.0, 0.0, 0.0];

    /// <summary>
    /// Hue
    /// </summary>
    public double H { get; set; }

    /// <summary>
    /// Chroma
    /// </summary>
    public double C { get; set; }

    public double J { get; set; }
    public double Q { get; set; }
    public double M { get; set; }
    public double S { get; set; }

    //Coordinates in UCS space. Used to determine color distance, like delta E equations in L*a*b*.
    private double JStar { get; set; }
    private double AStar { get; set; }
    private double BStar { get; set; }

    public Cam16(double h, double c, double j, double q,
        double m, double s, double jstar, double astar, double bstar) {
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

    public double Distance(Cam16 @cam16) {
        var dJ = JStar - @cam16.JStar;
        var dA = AStar - @cam16.AStar;
        var dB = BStar - @cam16.BStar;
        var dEPrime = Math.Sqrt(dJ * dJ + dA * dA + dB * dB);
        return 1.41 * Math.Pow(dEPrime, 0.63);
    }

    public double[] XyzInViewingConditions(ViewingConditions viewingConditions, double[] returnArray) {
        double alpha = (C == 0.0 || J == 0.0) ? 0.0 : C / Math.Sqrt(J / 100.0);

        double t = Math.Pow(alpha / Math.Pow(1.64 - Math.Pow(0.29, viewingConditions.N), 0.73), 1.0 / 0.9);
        double hRad = MathUtil.Radians(H);

        double eHue = 0.25 * (Math.Cos(hRad + 2.0) + 3.8);
        double ac = viewingConditions.Aw * Math.Pow(J / 100.0, 1.0 / viewingConditions.C / viewingConditions.Z);
        double p1 = eHue * (50000.0 / 13.0) * viewingConditions.Nc * viewingConditions.Ncb;
        double p2 = (ac / viewingConditions.Nbb);

        double hSin = Math.Sin(hRad);
        double hCos = Math.Cos(hRad);

        double gamma = 23.0 * (p2 + 0.305) * t / (23.0 * p1 + 11.0 * t * hCos + 108.0 * t * hSin);
        double a = gamma * hCos;
        double b = gamma * hSin;
        double rA = (460.0 * p2 + 451.0 * a + 288.0 * b) / 1403.0;
        double gA = (460.0 * p2 - 891.0 * a - 261.0 * b) / 1403.0;
        double bA = (460.0 * p2 - 220.0 * a - 6300.0 * b) / 1403.0;

        double rCBase = Math.Max(0, (27.13 * Math.Abs(rA)) / (400.0 - Math.Abs(rA)));
        double rC = Math.Sign(rA) * (100.0 / viewingConditions.Fl) * Math.Pow(rCBase, 1.0 / 0.42);
        double gCBase = Math.Max(0, (27.13 * Math.Abs(gA)) / (400.0 - Math.Abs(gA)));
        double gC = Math.Sign(gA) * (100.0 / viewingConditions.Fl) * Math.Pow(gCBase, 1.0 / 0.42);
        double bCBase = Math.Max(0, (27.13 * Math.Abs(bA)) / (400.0 - Math.Abs(bA)));
        double bC = Math.Sign(bA) * (100.0 / viewingConditions.Fl) * Math.Pow(bCBase, 1.0 / 0.42);
        double rF = rC / viewingConditions.RgbD[0];
        double gF = gC / viewingConditions.RgbD[1];
        double bF = bC / viewingConditions.RgbD[2];

        double[,] matrix = CAM16RGB_TO_XYZ;
        double x = (rF * matrix[0, 0]) + (gF * matrix[0, 1]) + (bF * matrix[0, 2]);
        double y = (rF * matrix[1, 0]) + (gF * matrix[1, 1]) + (bF * matrix[1, 2]);
        double z = (rF * matrix[2, 0]) + (gF * matrix[2, 1]) + (bF * matrix[2, 2]);

        if (returnArray != null) {
            returnArray[0] = x;
            returnArray[1] = y;
            returnArray[2] = z;
            return returnArray;
        } else
            return [x, y, z];
    }

    public static Cam16 Parse(uint argb) {
        return ParseByViewingConditions(argb, ViewingConditions.Default);
    }

    /// <summary>
    /// Create a CAM16 color from a color in defined viewing conditions.
    /// </summary>
    /// <param name="argb"></param>
    /// <param name="conditions"></param>
    /// <returns></returns>
    internal static Cam16 ParseByViewingConditions(uint argb, ViewingConditions conditions) {
        // Transform ARGB int to XYZ
        uint red = (argb & 0x00ff0000) >> 16;
        uint green = (argb & 0x0000ff00) >> 8;
        uint blue = (argb & 0x000000ff);

        double redL = ColorUtil.Linearized(red);
        double greenL = ColorUtil.Linearized(green);
        double blueL = ColorUtil.Linearized(blue);

        double x = 0.41233895 * redL + 0.35762064 * greenL + 0.18051042 * blueL;
        double y = 0.2126 * redL + 0.7152 * greenL + 0.0722 * blueL;
        double z = 0.01932141 * redL + 0.11916382 * greenL + 0.95034478 * blueL;

        return ParseByXyzByViewingConditions(x, y, z, conditions);
    }

    internal static Cam16 ParseByXyzByViewingConditions(double x, double y, double z, ViewingConditions conditions) {
        // Transform XYZ to 'cone'/'rgb' responses
        var matrix = XYZ_TO_CAM16RGB;

        double rT = (x * matrix[0, 0]) + (y * matrix[0, 1]) + (z * matrix[0, 2]);
        double gT = (x * matrix[1, 0]) + (y * matrix[1, 1]) + (z * matrix[1, 2]);
        double bT = (x * matrix[2, 0]) + (y * matrix[2, 1]) + (z * matrix[2, 2]);

        // Discount illuminant
        double rD = conditions.RgbD[0] * rT;
        double gD = conditions.RgbD[1] * gT;
        double bD = conditions.RgbD[2] * bT;

        // Chromatic adaptation
        double rAF = Math.Pow(conditions.Fl * Math.Abs(rD) / 100.0, 0.42);
        double gAF = Math.Pow(conditions.Fl * Math.Abs(gD) / 100.0, 0.42);
        double bAF = Math.Pow(conditions.Fl * Math.Abs(bD) / 100.0, 0.42);

        double rA = Math.Sign(rD) * 400.0 * rAF / (rAF + 27.13);
        double gA = Math.Sign(gD) * 400.0 * gAF / (gAF + 27.13);
        double bA = Math.Sign(bD) * 400.0 * bAF / (bAF + 27.13);

        // redness-greenness
        double a = (11.0 * rA + -12.0 * gA + bA) / 11.0;

        // yellowness-blueness
        double b = (rA + gA - 2.0 * bA) / 9.0;

        // auxiliary components
        double u = (20.0 * rA + 20.0 * gA + 21.0 * bA) / 20.0;
        double p2 = (40.0 * rA + 20.0 * gA + bA) / 20.0;

        //hue
        double atan2 = Math.Atan2(b, a);
        double atanDegrees = MathUtil.Degrees(atan2);
        double hue = atanDegrees < 0
            ? atanDegrees + 360.0
            : atanDegrees >= 360 ? atanDegrees - 360.0 : atanDegrees;

        double hueRadians = MathUtil.Radians(hue);

        // achromatic response to color
        double ac = p2 * conditions.Nbb;

        // CAM16 lightness and brightness
        double j = 100.0 * Math.Pow(ac / conditions.Aw,
            conditions.C * conditions.Z);

        double q = 4.0 / conditions.C * Math.Sqrt(j / 100.0) * (conditions.Aw + 4.0)
            * conditions.FlRoot;

        // CAM16 chroma, colorfulness, and saturation.
        double huePrime = (hue < 20.14) ? hue + 360 : hue;
        double eHue = 0.25 * (Math.Cos(MathUtil.Radians(huePrime) + 2.0) + 3.8);
        double p1 = 50000.0 / 13.0 * eHue * conditions.Nc * conditions.Ncb;
        double t = p1 * MathUtil.Hypot(a, b) / (u + 0.305);
        double alpha = Math.Pow(1.64 - Math.Pow(0.29, conditions.N), 0.73) * Math.Pow(t, 0.9);

        // CAM16 chroma, colorfulness, saturation
        double c = alpha * Math.Sqrt(j / 100.0);
        double m = c * conditions.FlRoot;
        double s = 50.0 * Math.Sqrt((alpha * conditions.C) / (conditions.Aw + 4.0));

        // CAM16-UCS components
        double jstar = (1.0 + 100.0 * 0.007) * j / (1.0 + 0.007 * j);
        double mstar = 1.0 / 0.0228 * Math.Log(1.0 + 0.0228 * m);
        double astar = mstar * Math.Cos(hueRadians);
        double bstar = mstar * Math.Sin(hueRadians);

        return new Cam16(hue, c, j, q, m, s, jstar, astar, bstar);
    }
}