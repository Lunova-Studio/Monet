using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Monet.Shared.Media.ColorSpace;
using Monet.Shared.Media.Quantize;

namespace Monet.Avalonia.Extensions;

/// <summary>
/// 量化扩展类
/// </summary>
public static class QuantizeExtension {
    /// <summary>
    /// 从图像中提取主要颜色（Primary Colors）
    /// </summary>
    /// <param name="bitmap">输入图像（Rgba32 格式）。</param>
    /// <param name="maxColorCount">希望提取的主色数量。</param>
    /// <returns>按出现频率排序的主色列表。</returns>
    public static IEnumerable<Color> ExtractPrimaryColors(this Bitmap bitmap, int maxColorCount = 5) {
        var pixels = bitmap.ToPixelArray();
        var primaryColors = CelebiQuantizer.Quantize(pixels, maxColorCount)
            .OrderByDescending(x => x.Value)
            .Select(x => Color.FromUInt32(x.Key));

        return primaryColors;
    }

    internal static uint[] ToPixelArray(this Bitmap bitmap) {
        int width = bitmap.PixelSize.Width;
        int height = bitmap.PixelSize.Height;
        int stride = width * 4;

        byte[] buffer = new byte[stride * height];

        unsafe {
            fixed (byte* ptr = buffer) {
                var rect = new PixelRect(0, 0, width, height);
                bitmap.CopyPixels(rect, (IntPtr)ptr, buffer.Length, stride);
            }
        }

        uint[] pixels = new uint[width * height];
        for (int i = 0; i < pixels.Length; i++) {
            int offset = i * 4;

            byte b = buffer[offset + 0];
            byte g = buffer[offset + 1];
            byte r = buffer[offset + 2];
            byte a = buffer[offset + 3];

            // BGRA -> ARGB (uint)
            pixels[i] = ((uint)a << 24) | ((uint)r << 16) | ((uint)g << 8) | b;
        }

        return pixels;
    }
}