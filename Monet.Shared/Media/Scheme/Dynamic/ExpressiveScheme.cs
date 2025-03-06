using Monet.Shared.Enums;
using Monet.Shared.Media.ColorSpace;
using Monet.Shared.Media.Palettes;
using Monet.Shared.Utilities;

namespace Monet.Shared.Media.Scheme.Dynamic;

public sealed class ExpressiveScheme(Hct hct, bool isDark, double contrastLevel) : DynamicScheme(
    hct,
    isDark,
    Variant.Expressive,
    contrastLevel,
    TonalPalette.CreateFromHueChroma(MathUtil.SanitizeDegrees(hct.H + 240.0), 40.0),
    TonalPalette.CreateFromHueChroma(GetRotatedHue(hct,_hues, _secondaryRotations), 24.0),
    TonalPalette.CreateFromHueChroma(GetRotatedHue(hct, _hues, _tertiaryRotations), 32.0),
    TonalPalette.CreateFromHueChroma(MathUtil.SanitizeDegrees(hct.H + 15.0), 8.0),
    TonalPalette.CreateFromHueChroma(MathUtil.SanitizeDegrees(hct.H + 15.0), 12.0)) {
    private static readonly double[] _hues = [0, 21, 51, 121, 151, 191, 271, 321, 360];
    private static readonly double[] _secondaryRotations = [45, 95, 45, 20, 45, 90, 45, 45, 45];
    private static readonly double[] _tertiaryRotations = [120, 120, 20, 45, 20, 15, 20, 120, 120];
}