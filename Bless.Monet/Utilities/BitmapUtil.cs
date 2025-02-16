using Avalonia.Media.Imaging;
using Avalonia.Platform;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Quantization;
using Color = Avalonia.Media.Color;

namespace Bless.Monet.Utilities;

public static class BitmapUtil {
    /// <summary>
    /// 图像量化
    /// </summary>
    /// <returns>图像中提取出来的可能的对比度较高的 <see cref="Color"/></returns>
    public static IEnumerable<Color> QuantizeAndGetPrimaryColors(Image<Rgba32> bitmap, int maxColorCount = 5) {
        // 克隆和量化图像
        var image = bitmap.Clone(ctx => ctx.Resize(128, 0).Quantize(new OctreeQuantizer(new QuantizerOptions { MaxColors = maxColorCount + 3 })));
        var dictionary = new Dictionary<Rgba32, int>();

        // 统计每种颜色出现的频率
        for (int i = 0; i < image.Width; i++) {
            for (int j = 0; j < image.Height; j++) {
                var key = image[i, j];
                if (key.A != byte.MaxValue || key.R != byte.MaxValue || key.G != byte.MaxValue || key.B != byte.MaxValue) {
                    if (!dictionary.TryAdd(key, 1))
                        dictionary[key]++;
                }
            }
        }

        // 筛选并返回前 maxColorCount 种最常见的颜色
        return dictionary.OrderByDescending(c => c.Value).Take(maxColorCount)
            .Select(c => new Color(c.Key.A, c.Key.R, c.Key.G, c.Key.B));
    }
}