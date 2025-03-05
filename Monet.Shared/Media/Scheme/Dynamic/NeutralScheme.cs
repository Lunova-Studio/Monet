using Monet.Shared.Enums;
using Monet.Shared.Media.ColorSpace;
using Monet.Shared.Media.Palettes;

namespace Monet.Shared.Media.Scheme.Dynamic;

public sealed class NeutralScheme(Hct hct, bool isDark, double contrastLevel) : DynamicScheme(
    hct,
    isDark,
    Variant.Neutral,
    contrastLevel,
    TonalPalette.CreateFromHueChroma(hct.H, 12.0),
    TonalPalette.CreateFromHueChroma(hct.H, 8.0),
    TonalPalette.CreateFromHueChroma(hct.H, 16.0),
    TonalPalette.CreateFromHueChroma(hct.H, 2.0),
    TonalPalette.CreateFromHueChroma(hct.H, 2.0));