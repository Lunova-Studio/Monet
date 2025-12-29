namespace Monet.Shared;

/// <summary>
/// A 3x3 matrix backed by a contiguous Span<double>.
/// Must be exactly 9 elements.
/// </summary>
public readonly ref struct Matrix3x3 {
    public readonly Span<double> M;

    public Matrix3x3(Span<double> data) {
        if (data.Length != 9)
            throw new ArgumentException("Matrix3x3 requires exactly 9 elements.");
        M = data;
    }

    public double this[int r, int c]
        => M[r * 3 + c];

    public Span<double> Row(int r)
        => M.Slice(r * 3, 3);
}