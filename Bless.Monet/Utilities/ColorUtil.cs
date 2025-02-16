namespace Bless.Monet.Utilities;

public static class ColorUtil {
    internal readonly static double[] WHITE_POINT_D65 =
        [95.047, 100.0, 108.883];

    public static readonly double[,] SRGB_TO_XYZ = new double[,] {
        {0.41233895, 0.35762064, 0.18051042},
        {0.2126, 0.7152, 0.0722},
        {0.01932141, 0.11916382, 0.95034478}
    };

    public static readonly double[][] XYZ_TO_SRGB = [
        [3.2413774792388685, -1.5376652402851851, -0.49885366846268053],
        [-0.9691452513005321, 1.8758853451067872, 0.04156585616912061],
        [0.05562093689691305, -0.20395524564742123, 1.0571799111220335]
    ];


    public static double LStarFromArgb(uint argb) {
        double y = XYZFromArgb(argb)[1];
        return 116.0 * LabF(y / 100.0) - 16.0;
    }

    public static uint ArgbFromLstar(double lstar) {
        double y = YFromLstar(lstar);
        uint component = DeLinearized(y);

        return Convert.ToUInt32(RgbToArgb(component, component, component));
    }

    public static double YFromLstar(double lstar) {
        return 100.0 * LabInvf((lstar + 16.0) / 116.0);
    }

    public static double LstarFromY(double y) {
        return LabF(y / 100.0) * 116.0 - 16.0;
    }

    private static double LabInvf(double ft) {
        double e = 216.0 / 24389.0;
        double kappa = 24389.0 / 27.0;
        double ft3 = ft * ft * ft;

        if (ft3 > e)
            return ft3;
        else
            return (116 * ft - 16) / kappa;
    }

    private static double LabF(double t) {
        double e = 216.0 / 24389.0;
        double kappa = 24389.0 / 27.0;

        return t > e
            ? Math.Pow(t, 1.0 / 3.0)
            : (kappa * t + 16) / 116;
    }

    public static uint LinrgbToArgb(double[] linrgb) {
        var r = DeLinearized(linrgb[0]);
        var g = DeLinearized(linrgb[1]);
        var b = DeLinearized(linrgb[2]);

        return Convert.ToUInt32(RgbToArgb(r, g, b));
    }

    public static uint RgbToArgb(uint red, uint green, uint blue) {
        return (255u << 24) | ((red & 255) << 16) | ((green & 255) << 8) | blue & 255;
    }

    public static uint DeLinearized(double rgbComponent) {
        double normalized = rgbComponent / 100.0;
        double delinearized = 0.0;
        if (normalized <= 0.0031308)
            delinearized = normalized * 12.92;
        else
            delinearized = 1.055 * Math.Pow(normalized, 1.0 / 2.4) - 0.055;

        return MathUtil.ClampUint(0, 255, (int)Math.Round(delinearized * 255.0));
    }

    /// <summary>
    /// Converts a color from ARGB representation to L*a*b* representation.
    /// </summary>
    /// <param name="argb">the ARGB representation of a color</param>
    /// <returns>a Lab object representing the color</returns>
    public static double[] LabFromArgb(uint argb) {
        double linearR = Linearized(ToRedByArgb(argb));
        double linearG = Linearized(ToGreenByArgb(argb));
        double linearB = Linearized(ToBuleByArgb(argb));

        var matrix = SRGB_TO_XYZ;
        double x = matrix[0, 0] * linearR + matrix[0, 1] * linearG + matrix[0, 2] * linearB;
        double y = matrix[1, 0] * linearR + matrix[1, 1] * linearG + matrix[1, 2] * linearB;
        double z = matrix[2, 0] * linearR + matrix[2, 1] * linearG + matrix[2, 2] * linearB;

        double[] whitePoint = WHITE_POINT_D65;
        double xNormalized = x / whitePoint[0];
        double yNormalized = y / whitePoint[1];
        double zNormalized = z / whitePoint[2];
        double fx = LabF(xNormalized);
        double fy = LabF(yNormalized);
        double fz = LabF(zNormalized);
        double l = 116.0 * fy - 16;
        double a = 500.0 * (fx - fy);
        double b = 200.0 * (fy - fz);
        return new double[] { l, a, b };
    }

    /// <summary>
    /// Converts a color represented in Lab color space into an ARGB uint.
    /// </summary>
    /// <param name="l"></param>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static uint ArgbFromLab(double l, double a, double b) {
        double[] whitePoint = WHITE_POINT_D65;
        double fy = (l + 16.0) / 116.0;
        double fx = a / 500.0 + fy;
        double fz = fy - b / 200.0;

        double xNormalized = LabInvf(fx);
        double yNormalized = LabInvf(fy);
        double zNormalized = LabInvf(fz);

        double x = xNormalized * whitePoint[0];
        double y = yNormalized * whitePoint[1];
        double z = zNormalized * whitePoint[2];

        return ArgbFromXyz(x, y, z);
    }

    /// <summary>
    /// Converts a color from ARGB to XYZ.
    /// </summary>
    public static uint ArgbFromXyz(double x, double y, double z) {
        double[][] matrix = XYZ_TO_SRGB;
        double linearR = matrix[0][0] * x + matrix[0][1] * y + matrix[0][2] * z;
        double linearG = matrix[1][0] * x + matrix[1][1] * y + matrix[1][2] * z;
        double linearB = matrix[2][0] * x + matrix[2][1] * y + matrix[2][2] * z;

        uint r = DeLinearized(linearR);
        uint g = DeLinearized(linearG);
        uint b = DeLinearized(linearB);

        return RgbToArgb(r, g, b);
    }

    /// <summary>
    ///  Linearizes an RGB component.
    /// </summary>
    /// <param name="rgbComponent"></param>
    /// <returns>0.0 <= output <= 100.0, color channel converted to linear RGB space</returns>
    public static double Linearized(uint rgbComponent) {
        double normalized = rgbComponent / 255.0;

        if (normalized <= 0.040449936)
            return normalized / 12.92 * 100.0;
        else
            return Math.Pow((normalized + 0.055) / 1.055, 2.4) * 100.0;
    }

    /// <summary>
    /// Converts a color from XYZ to ARGB
    /// </summary>
    /// <param name="argb"></param>
    /// <returns></returns>
    public static double[] XYZFromArgb(uint argb) {
        double r = Linearized(ToRedByArgb(argb));
        double g = Linearized(ToGreenByArgb(argb));
        double b = Linearized(ToBuleByArgb(argb));
        return MathUtil.MatrixMultiply([r, g, b], SRGB_TO_XYZ);
    }

    /// <summary>
    /// Returns the red component of a color in ARGB format.
    /// </summary>
    /// <param name="argb"></param>
    /// <returns></returns>
    public static uint ToRedByArgb(uint argb) {
        return (argb >> 16) & 255;
    }

    /// <summary>
    ///  Returns the green component of a color in ARGB format
    /// </summary>
    /// <param name="argb"></param>
    /// <returns></returns>
    public static uint ToGreenByArgb(uint argb) {
        return (argb >> 8) & 255;
    }

    /// <summary>
    /// Returns the blue component of a color in ARGB format.
    /// </summary>
    /// <param name="argb"></param>
    /// <returns></returns>
    public static uint ToBuleByArgb(uint argb) {
        return argb & 255;
    }
}