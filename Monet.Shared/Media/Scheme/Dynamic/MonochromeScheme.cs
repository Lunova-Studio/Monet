using Monet.Shared.Enums;
using Monet.Shared.Media.ColorSpace;
using Monet.Shared.Media.Palettes;
using Monet.Shared.Utilities;

namespace Monet.Shared.Media.Scheme.Dynamic;

/// <summary>
/// A monochrome theme, colors are purely black / white / gray
/// </summary>
public sealed class MonochromeScheme(Hct hct, bool isDark, double contrastLevel) : DynamicScheme(
    hct,
    isDark,
    Variant.Monochrome,
    contrastLevel,
    TonalPalette.CreateFromHueChroma(hct.H, 0.0),
    TonalPalette.CreateFromHueChroma(hct.H, 0.0),
    TonalPalette.CreateFromHueChroma(hct.H, 0.0),
    TonalPalette.CreateFromHueChroma(hct.H, 0.0),
    TonalPalette.CreateFromHueChroma(hct.H, 0.0));