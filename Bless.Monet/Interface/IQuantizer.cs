namespace Bless.Monet.Interface;

/// <summary>
/// 统一量化器接口
/// </summary>
public interface IQuantizer {
    /// <summary>
    /// 量化图像
    /// </summary>
    QuantizerResult Quantize(uint[] pixels, int maxColors);
}

public readonly struct QuantizerResult {
    public readonly Dictionary<uint, uint> ColorToCount;

    internal QuantizerResult(Dictionary<uint, uint> colorToCount) {
        ColorToCount = colorToCount;
    }
}