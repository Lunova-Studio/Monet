namespace Bless.Monet.Interface;

/// <summary>
///  An interface to allow use of different color spaces by quantizers.
/// </summary>
public interface IPointProvider {
    /// <summary>
    /// The four components in the color space of an sRGB color.
    /// </summary>
    /// <param name="argb"></param>
    /// <returns></returns>
    double[] FromUInt32(uint argb);

    /// <summary>
    /// The ARGB (i.e. hex code) representation of this color.
    /// </summary>
    uint ToUInt32(double[] point);

    /// <summary>
    /// Squared distance between two colors. Distance is defined by scientific color spaces and
    /// referred to as delta E.
    /// </summary>
    double Distance(double[] a, double[] b);
}