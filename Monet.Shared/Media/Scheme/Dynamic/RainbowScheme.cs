using Monet.Shared.Enums;
using Monet.Shared.Media.ColorSpace;
using Monet.Shared.Media.Palettes;
using Monet.Shared.Utilities;

namespace Monet.Shared.Media.Scheme.Dynamic;

public sealed class RainbowScheme(Hct hct, bool isDark, double contrastLevel) : DynamicScheme(
    hct,
    isDark,
    Variant.Rainbow,
    contrastLevel,
    TonalPalette.CreateFromHueChroma(hct.H, 48.0),
    TonalPalette.CreateFromHueChroma(hct.H, 16.0),
    TonalPalette.CreateFromHueChroma(MathUtil.SanitizeDegrees(hct.H + 60.0), 24.0),
    TonalPalette.CreateFromHueChroma(hct.H, 0.0),
    TonalPalette.CreateFromHueChroma(hct.H, 0.0));