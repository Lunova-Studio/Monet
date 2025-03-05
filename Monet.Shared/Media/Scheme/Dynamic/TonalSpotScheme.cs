using Monet.Shared.Enums;
using Monet.Shared.Media.ColorSpace;
using Monet.Shared.Media.Palettes;
using Monet.Shared.Utilities;

namespace Monet.Shared.Media.Scheme.Dynamic;

/// <summary>
/// A calm theme, sedated colors that aren't particularly chromatic.
/// </summary>
public sealed class TonalSpotScheme(Hct hct, bool isDark, double contrastLevel) : DynamicScheme(
    hct,
    isDark, 
    Variant.Tonal_Spot, 
    contrastLevel,
    TonalPalette.CreateFromHueChroma(hct.H, 36.0),
    TonalPalette.CreateFromHueChroma(hct.H, 16.0),
    TonalPalette.CreateFromHueChroma(MathUtil.SanitizeDegrees(hct.H + 60.0), 24.0),
    TonalPalette.CreateFromHueChroma(hct.H, 6.0),
    TonalPalette.CreateFromHueChroma(hct.H, 8.0));