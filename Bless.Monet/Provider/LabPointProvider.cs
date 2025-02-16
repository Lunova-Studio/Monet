using Bless.Monet.Interface;
using Bless.Monet.Utilities;

namespace Bless.Monet.Provider;

/// <summary>
///  Provides conversions needed for K-Means quantization. Converting input to points, and converting
///  the final state of the K-Means algorithm to colors.
/// </summary>
public sealed class LabPointProvider : IPointProvider {
    /// <summary>
    /// Convert a color represented in ARGB to a 3-element array of L*a*b* coordinates of the color.
    /// </summary>
    public double[] FromUInt32(uint argb) {
        var lab = ColorUtil.LabFromArgb(argb);
        return [lab[0], lab[1], lab[2]];
    }

    /// <summary>
    ///  Convert a 3-element array to a color represented in ARGB.
    /// </summary>
    public uint ToUInt32(double[] lab) {
        return ColorUtil.ArgbFromLab(lab[0], lab[1], lab[2]);
    }

    /// <summary>
    /// Standard CIE 1976 delta E formula also takes the square root, unneeded here. This method is
    /// used by quantization algorithms to compare distance, and the relative ordering is the same,
    /// with or without a square root.
    /// <para>
    /// This relatively minor optimization is helpful because this method is called at least once
    /// for each pixel in an image.
    /// </para>
    /// </summary>
    public double Distance(double[] one, double[] two) {
        double dL = (one[0] - two[0]);
        double dA = (one[1] - two[1]);
        double dB = (one[2] - two[2]);
        return (dL * dL + dA * dA + dB * dB);
    }
}