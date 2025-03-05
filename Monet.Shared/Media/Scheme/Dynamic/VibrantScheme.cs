using Monet.Shared.Enums;
using Monet.Shared.Media.ColorSpace;
using Monet.Shared.Media.Palettes;

namespace Monet.Shared.Media.Scheme.Dynamic;

public sealed class VibrantScheme(Hct hct, bool isDark, double contrastLevel) : DynamicScheme(
    hct,
    isDark,
    Variant.Vibrant,
    contrastLevel,
    TonalPalette.CreateFromHueChroma(hct.H, 200.0),
    TonalPalette.CreateFromHueChroma(GetRotatedHue(hct, _hues, _secondaryRotations), 24.0),
    TonalPalette.CreateFromHueChroma(GetRotatedHue(hct, _hues, _tertiaryRotations), 32.0),
    TonalPalette.CreateFromHueChroma(hct.H, 10.0),
    TonalPalette.CreateFromHueChroma(hct.H, 12.0)) {
    private static readonly double[] _hues = [0, 41, 61, 101, 131, 181, 251, 301, 360];
    private static readonly double[] _secondaryRotations = [18, 15, 10, 12, 15, 18, 15, 12, 12];
    private static readonly double[] _tertiaryRotations = [35, 30, 20, 25, 30, 35, 30, 25, 25];
}
