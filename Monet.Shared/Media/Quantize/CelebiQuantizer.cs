/*
* Copyright 2021 Google LLC
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

namespace Monet.Shared.Media.Quantize;

/// <summary>
/// An image quantizer that improves on the quality of a standard K-Means algorithm by setting
/// the K-Means initial state to the output of a Wu quantizer, instead of random centroids.
/// Improves on speed by several optimizations, as implemented in Wsmeans, or Weighted Square
/// Means, K-Means with those optimizations.
///
/// This algorithm was designed by M. Emre Celebi, and was found in their 2011 paper,
/// "Improving the Performance of K-Means for Color Quantization".
/// </summary>
public static class CelebiQuantizer {

    /// <summary>
    /// Reduce the number of colors needed to represent the input, minimizing the difference
    /// between the original image and the recolored image.
    ///
    /// <param name="pixels">Colors in ARGB format.</param>
    /// <param name="maxColors">The number of colors to divide the image into. A lower number may be returned.</param>
    /// <returns>
    /// Dictionary with keys of colors in ARGB format, and values of number of pixels in the
    /// original image that correspond to the color in the quantized image.
    /// </returns>
    /// </summary>
    public static Dictionary<uint, int> Quantize(uint[] pixels, int maxColors = 5) {
        var wu = new WuQuantizer();
        var wuResult = wu.Quantize(pixels, maxColors + 3);

        var wuClusters = new uint[wuResult.ColorToCount.Count];
        int index = 0;

        foreach (var argb in wuResult.ColorToCount.Keys) {
            wuClusters[index++] = argb;
        }

        return WsmeansQuantizer.Quantize(pixels, wuClusters, maxColors);
    }
}