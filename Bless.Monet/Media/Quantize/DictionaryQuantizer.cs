using Bless.Monet.Interface;

namespace Bless.Monet.Media.Quantize;

/// <summary>
/// Creates a dictionary with keys of colors, and values of count of the color
/// </summary>
public sealed class DictionaryQuantizer : IQuantizer {
    public Dictionary<uint, uint> ColorToCount { get; private set; } = [];

    public QuantizerResult Quantize(uint[] pixels, int colorCount) {
        var pixelByCount = new Dictionary<uint, uint>();

        foreach (uint pixel in pixels)
            if (pixelByCount.TryGetValue(pixel, out uint currentPixelCount))
                pixelByCount[pixel] = currentPixelCount + 1;
            else
                pixelByCount[pixel] = 1;

        ColorToCount = pixelByCount;
        return new QuantizerResult(pixelByCount);
    }
}