using Bless.Monet.Interface;

namespace Bless.Monet.Media.Quantize;

/// <summary>
/// An image quantizer that improves on the quality of a standard K-Means algorithm by setting the
/// K-Means initial state to the output of a Wu quantizer, instead of random centroids. Improves on
/// speed by several optimizations, as implemented in Wsmeans, or Weighted Square Means, K-Means with
/// those optimizations.
/// </summary>
/// 此类不会实现 IQuantizer 接口
public static class CelebiQuantizer {
    /// <summary>
    /// Reduce the number of colors needed to represented the input, minimizing the difference between
    /// the original image and the recolored image.
    /// </summary>
    /// <param name="pixels">pixels Colors in ARGB format.</param>
    /// <param name="maxColors">maxColors The number of colors to divide the image into. A lower number of colors may be returned.</param>
    /// <returns>Dictionary with keys of colors in ARGB format, and values of number of pixels in the original image that correspond to the color in the quantized image.</returns>
    public static Dictionary<uint, uint> Quantize(uint[] pixels, int maxColors) {
        WuQuantizer wuQuantizer = new();
        var wuResult = wuQuantizer.Quantize(pixels, maxColors);

        HashSet<uint> wuClustersAsObjects = new(wuResult.ColorToCount.Keys);
        int index = 0;
        uint[] wuClusters = new uint[wuClustersAsObjects.Count];

        foreach (var argb in wuClustersAsObjects)
            wuClusters[index++] = argb;

        return WsmeansQuantizer.Quantize(pixels, wuClusters, maxColors);
    }
}