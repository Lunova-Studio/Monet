using Monet.Shared.Enums;
using Monet.Shared.Media.ColorSpace;
using Monet.Shared.Media.Palettes;
using Monet.Shared.Utilities;

namespace Monet.Shared.Media.Scheme.Dynamic;

public sealed class FruitSaladScheme(Hct hct, bool isDark, double contrastLevel) : DynamicScheme(
    hct,
    isDark,
    Variant.FruitSalad,
    contrastLevel,
    TonalPalette.CreateFromHueChroma(MathUtil.SanitizeDegrees(hct.H - 50.0), 48.0),
    TonalPalette.CreateFromHueChroma(MathUtil.SanitizeDegrees(hct.H - 50.0), 16.0),
    TonalPalette.CreateFromHueChroma(hct.H, 36.0),
    TonalPalette.CreateFromHueChroma(hct.H, 10.0),
    TonalPalette.CreateFromHueChroma(hct.H, 16.0));
