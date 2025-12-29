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

using Monet.Shared.Interfaces;

namespace Monet.Shared.Media.Quantize;

/// <summary>
/// Creates a dictionary with keys of colors, and values of count of the color.
/// </summary>
public sealed class QuantizerMap : IQuantizer {
    public Dictionary<uint, int>? ColorToCount { get; private set; }

    public QuantizedColorMap Quantize(uint[] pixels, int maxColors) {
        var pixelByCount = new Dictionary<uint, int>();

        foreach (var pixel in pixels)
            if (pixelByCount.TryGetValue(pixel, out int count))
                pixelByCount[pixel] = count + 1;
            else
                pixelByCount[pixel] = 1;

        ColorToCount = pixelByCount;
        return new(pixelByCount);
    }
}