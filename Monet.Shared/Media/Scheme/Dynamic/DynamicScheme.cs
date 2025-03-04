using Monet.Shared.Enums;
using Monet.Shared.Media.ColorSpace;
using Monet.Shared.Media.Palettes;

namespace Monet.Shared.Media.Scheme.Dynamic;

/// <summary>
/// Provides important settings for creating colors dynamically, and 6 color palettes. Requires:
/// 1. A color 
/// 2. A theme. (Variant) 
/// 3. Whether or not its dark mode. 
/// 4. Contrast level. (-1 to 1, currently contrast ratio 3.0 and 7.0)
/// </summary>
public class DynamicScheme {
    public Hct SourceColorHct { get; init; }
    public uint SourceColorArgb { get; init; }
    public bool IsDark { get; init; }
    public Variant Variant { get; init; }
    public double ContrastLevel { get; init; }

    public TonalPalette ErrorPalette { get; init; }
    public TonalPalette PrimaryPalette { get; init; }
    public TonalPalette NeutralPalette { get; init; }
    public TonalPalette TertiaryPalette { get; init; }
    public TonalPalette SecondaryPalette { get; init; }
    public TonalPalette NeutralVariantPalette { get; init; }

    public DynamicScheme(
        Hct sourceColorHct,
        bool isDark, 
        Variant variant, 
        double contrastLevel,
        TonalPalette primaryPalette,
        TonalPalette neutralPalette,
        TonalPalette tertiaryPalette,
        TonalPalette secondaryPalette,
        TonalPalette neutralVariantPalette) {
        SourceColorHct = sourceColorHct;
        SourceColorArgb = sourceColorHct.ToUInt32();
        IsDark = isDark;
        Variant = variant;
        ContrastLevel = contrastLevel;

        PrimaryPalette = primaryPalette;
        NeutralPalette = neutralPalette;
        TertiaryPalette = tertiaryPalette;
        SecondaryPalette = secondaryPalette;
        NeutralVariantPalette = neutralVariantPalette;
        ErrorPalette = TonalPalette.CreateFromHueChroma(25.0, 84.0);
    }

    //public uint PrimaryPaletteKeyColor => GetArgb(new MaterialDynamicColors().primaryPaletteKeyColor());

    public Hct GetHct(DynamicColor dynamicColor) {
        return dynamicColor.GetHct(this);
    }

    public uint GetArgb(DynamicColor dynamicColor) {
        return dynamicColor.GetArgb(this);
    }
}