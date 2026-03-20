namespace Monet.Shared.Utilities;

public static class MathUtil {
    internal const double RADIANS_TO_DEGREES = 180.0 / Math.PI;
    internal const double DEGREES_TO_RADIANS = Math.PI / 180.0;

    public static int SignNumber(double num)
        => Math.Sign(num);

    public static uint ClampUint(int min, int max, int input) {
        if (min > max)
            throw new ArgumentException("min cannot be greater than max.");

        return (uint)Math.Clamp(input, min, max);
    }

    public static double Hypot(double x, double y)
        => Math.Sqrt(x * x + y * y);

    public static double Radians(double angdeg)
        => angdeg * DEGREES_TO_RADIANS;

    public static double Degrees(double angrad)
        => angrad * RADIANS_TO_DEGREES;

    public static double SanitizeDegrees(double degrees) {
        degrees %= 360.0;
        return degrees < 0 ? degrees + 360.0 : degrees;
    }

    /// <summary>
    /// Multiplies a 1x3 row vector with a 3x3 matrix.
    /// </summary>
    public static void MatrixMultiply(
        ReadOnlySpan<double> row,
        Matrix3x3 matrix,
        Span<double> dest) {
        if (row.Length != 3)
            throw new ArgumentException("Row must have length 3.");
        if (dest.Length != 3)
            throw new ArgumentException("Dest must have length 3.");

        dest[0] = row[0] * matrix[0, 0] + row[1] * matrix[0, 1] + row[2] * matrix[0, 2];
        dest[1] = row[0] * matrix[1, 0] + row[1] * matrix[1, 1] + row[2] * matrix[1, 2];
        dest[2] = row[0] * matrix[2, 0] + row[1] * matrix[2, 1] + row[2] * matrix[2, 2];
    }

    public static double Lerp(double start, double stop, double amount)
        => start + (stop - start) * amount;
}