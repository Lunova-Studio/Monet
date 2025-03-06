using Monet.Shared.Enums;
using Monet.Shared.Media.ColorSpace;
using Monet.Shared.Media.Palettes;
using Monet.Shared.Utilities;

namespace Monet.Shared.Media.Scheme.Dynamic;

public sealed class FidelitySceme(Hct hct, bool isDark, double contrastLevel) : DynamicScheme(
    hct,
    isDark,
    Variant.Fidelity,
    contrastLevel,
    TonalPalette.CreateFromHueChroma(hct.H, hct.C),
    TonalPalette.CreateFromHueChroma(hct.H, Math.Max(hct.C - 32.0, hct.C * 0.5)),
    TonalPalette.CreateFromHct(ColorUtil.Fix(new TemperatureManager(hct).GetComplement())),
    TonalPalette.CreateFromHueChroma(hct.H, hct.C / 8.0),
    TonalPalette.CreateFromHueChroma(hct.H, (hct.C / 8.0) + 4.0));