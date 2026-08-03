using Avalonia.Media;
using Monet.Shared.Enums;
using Monet.Shared.Interfaces;
using Monet.Shared.Media.Scheme.Dynamic;

namespace Monet.Avalonia;

public static class MonetSchemeFactory {
    public static IColorValueScheme Create(Variant variant, Color seedColor, bool isDark, double level) {
        var color = seedColor.ToUInt32();

        return variant switch {
            Variant.Rainbow => new RainbowScheme(color, isDark, level),
            Variant.Content => new ContentScheme(color, isDark, level),
            Variant.FruitSalad => new FruitSaladScheme(color, isDark, level),
            Variant.Vibrant => new VibrantScheme(color, isDark, level),
            Variant.TonalSpot => new TonalSpotScheme(color, isDark, level),
            Variant.Monochrome => new MonochromeScheme(color, isDark, level),
            Variant.Expressive => new ExpressiveScheme(color, isDark, level),
            Variant.Fidelity => new FidelitySceme(color, isDark, level),
            Variant.Neutral => new NeutralScheme(color, isDark, level),
            _ => throw new ArgumentOutOfRangeException(nameof(variant), variant, null)
        };
    }
}