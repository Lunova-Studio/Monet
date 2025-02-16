namespace Bless.Monet.Utilities;

public static class MathUtil {
    internal const double RADIANS_TO_DEGREES = 57.29577951308232;
    internal const double DEGREES_TO_RADIANS = 0.017453292519943295;

    public static int SignNumber(double num) {
        if (num < 0)
            return -1;
        else if (num is 0)
            return 0;
        else
            return 1;
    }

    public static uint ClampUint(int min, int max, int input) {
        if (input < min)
            return Convert.ToUInt32(min);
        else if ((input > max))
            return Convert.ToUInt32(max);

        return Convert.ToUInt32(input);
    }

    public static double Hypot(double x, double y) {
        return Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
    }

    public static double Radians(double angdeg) {
        return angdeg * DEGREES_TO_RADIANS;
    }

    public static double Degrees(double angrad) {
        return angrad * RADIANS_TO_DEGREES;
    }

    public static double SanitizeDegrees(double degrees) {
        degrees %= 360.0;
        if (degrees < 0)
            degrees += 360.0;

        return degrees;
    }

    /// <summary>
    /// Multiplies a 1x3 row vector with a 3x3 matrix
    /// </summary>
    /// <returns></returns>
    public static double[] MatrixMultiply(double[] row, double[,] matrix) {
        double a = row[0] * matrix[0, 0] + row[1] * matrix[0, 1] + row[2] * matrix[0, 2];
        double b = row[0] * matrix[1, 0] + row[1] * matrix[1, 1] + row[2] * matrix[1, 2];
        double c = row[0] * matrix[2, 0] + row[1] * matrix[2, 1] + row[2] * matrix[2, 2];
        return [a, b, c];
    }

    /// <summary>
    /// The linear interpolation function.
    /// </summary>
    /// <returns>start if amount = 0 and stop if amount = 1</returns>
    public static double Lerp(double start, double stop, double amount) {
        return (1.0 - amount) * start + amount * stop;
    }
}