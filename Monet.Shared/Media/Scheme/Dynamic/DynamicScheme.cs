using Monet.Shared.Enums;
using Monet.Shared.Interfaces;
using Monet.Shared.Media.ColorSpace;
using Monet.Shared.Media.Palettes;
using Monet.Shared.Utilities;
using System.Collections.Frozen;

namespace Monet.Shared.Media.Scheme.Dynamic;

/// <summary>
/// Provides important settings for creating colors dynamically, and 6 color palettes. Requires:
/// 1. A color 
/// 2. A theme. (Variant) 
/// 3. Whether or not its dark mode. 
/// 4. Contrast level. (-1 to 1, currently contrast ratio 3.0 and 7.0)
/// </summary>
public class DynamicScheme : IColorValueScheme {
    private readonly MaterialDynamicColor _mdc = new();

    public bool IsDark { get; init; }
    public Variant Variant { get; init; }
    public Hct SourceColorHct { get; init; }
    public double ContrastLevel { get; init; }
    public uint SourceColorArgb { get; init; }

    public TonalPalette ErrorPalette { get; init; }
    public TonalPalette PrimaryPalette { get; init; }
    public TonalPalette NeutralPalette { get; init; }
    public TonalPalette TertiaryPalette { get; init; }
    public TonalPalette SecondaryPalette { get; init; }
    public TonalPalette NeutralVariantPalette { get; init; }


    public uint NeutralPaletteKeyColorValue => Resolve(_mdc.NeutralPaletteKeyColor);
    public uint PrimaryPaletteKeyColorValue => Resolve(_mdc.PrimaryPaletteKeyColor);
    public uint TertiaryPaletteKeyColorValue => Resolve(_mdc.TertiaryPaletteKeyColor);
    public uint SecondaryPaletteKeyColorValue => Resolve(_mdc.SecondaryPaletteKeyColor);
    public uint NeutralVariantPaletteKeyColorValue => Resolve(_mdc.NeutralVariantPaletteKeyColor);

    public uint SurfaceColorValue => Resolve(_mdc.Surface);
    public uint OnSurfaceColorValue => Resolve(_mdc.OnSurface);
    public uint BackgroundColorValue => Resolve(_mdc.Background);
    public uint OnBackgroundColorValue => Resolve(_mdc.OnBackground);
    public uint SurfaceColorDimColorValue => Resolve(_mdc.SurfaceDim);
    public uint SurfaceBrightColorValue => Resolve(_mdc.SurfaceBright);
    public uint SurfaceVariantColorValue => Resolve(_mdc.SurfaceVariant);
    public uint InverseSurfaceColorValue => Resolve(_mdc.InverseSurface);
    public uint SurfaceContainerColorValue => Resolve(_mdc.SurfaceContainer);
    public uint OnSurfaceVariantColorValue => Resolve(_mdc.OnSurfaceVariant);
    public uint InverseOnSurfaceColorValue => Resolve(_mdc.InverseOnSurface);
    public uint SurfaceContainerLowColorValue => Resolve(_mdc.SurfaceContainerLow);
    public uint SurfaceContainerHighColorValue => Resolve(_mdc.SurfaceContainerHigh);
    public uint SurfaceContainerHighestColorValue => Resolve(_mdc.SurfaceContainerHighest);

    public uint ScrimColorValue => Resolve(_mdc.Scrim);
    public uint ShadowColorValue => Resolve(_mdc.Shadow);
    public uint OutlineColorValue => Resolve(_mdc.Outline);
    public uint OutlineVariantColorValue => Resolve(_mdc.OutlineVariant);

    public uint PrimaryColorValue => Resolve(_mdc.Primary);
    public uint OnPrimaryColorValue => Resolve(_mdc.OnPrimary);
    public uint InversePrimaryColorValue => Resolve(_mdc.InversePrimary);
    public uint PrimaryContainerColorValue => Resolve(_mdc.PrimaryContainer);
    public uint OnPrimaryContainerColorValue => Resolve(_mdc.OnPrimaryContainer);

    public uint SecondaryColorValue => Resolve(_mdc.Secondary);
    public uint OnSecondaryColorValue => Resolve(_mdc.OnSecondary);
    public uint SecondaryContainerColorValue => Resolve(_mdc.SecondaryContainer);
    public uint OnSecondaryContainerColorValue => Resolve(_mdc.OnSecondaryContainer);

    public uint TertiaryColorValue => Resolve(_mdc.Tertiary);
    public uint OnTertiaryColorValue => Resolve(_mdc.OnTertiary);
    public uint TertiaryContainerColorValue => Resolve(_mdc.TertiaryContainer);
    public uint OnTertiaryContainerColorValue => Resolve(_mdc.OnTertiaryContainer);

    public uint ErrorColorValue => Resolve(_mdc.Error);
    public uint OnErrorColorValue => Resolve(_mdc.OnError);
    public uint ErrorContainerColorValue => Resolve(_mdc.ErrorContainer);
    public uint OnErrorContainerColorValue => Resolve(_mdc.OnErrorContainer);

    public IDictionary<string, uint> Resources => new Dictionary<string, uint> {
        ["PrimaryPaletteKeyColor"] = PrimaryPaletteKeyColorValue,
        ["SecondaryPaletteKeyColor"] = SecondaryPaletteKeyColorValue,
        ["TertiaryPaletteKeyColor"] = TertiaryPaletteKeyColorValue,
        ["NeutralPaletteKeyColor"] = NeutralPaletteKeyColorValue,
        ["NeutralVariantPaletteKeyColor"] = NeutralVariantPaletteKeyColorValue,

        ["BackgroundColor"] = BackgroundColorValue,
        ["OnBackgroundColor"] = OnBackgroundColorValue,
        ["SurfaceColor"] = SurfaceColorValue,
        ["OnSurfaceColor"] = OnSurfaceColorValue,
        ["SurfaceColorDimColor"] = SurfaceColorDimColorValue,
        ["SurfaceBrightColor"] = SurfaceBrightColorValue,
        ["SurfaceContainerLowColor"] = SurfaceContainerLowColorValue,
        ["SurfaceContainerColor"] = SurfaceContainerColorValue,
        ["SurfaceContainerHighColor"] = SurfaceContainerHighColorValue,
        ["SurfaceContainerHighestColor"] = SurfaceContainerHighestColorValue,
        ["SurfaceVariantColor"] = SurfaceVariantColorValue,
        ["OnSurfaceVariantColor"] = OnSurfaceVariantColorValue,
        ["InverseSurfaceColor"] = InverseSurfaceColorValue,
        ["InverseOnSurfaceColor"] = InverseOnSurfaceColorValue,

        ["OutlineColor"] = OutlineColorValue,
        ["OutlineVariantColor"] = OutlineVariantColorValue,
        ["ShadowColor"] = ShadowColorValue,
        ["ScrimColor"] = ScrimColorValue,

        ["PrimaryColor"] = PrimaryColorValue,
        ["OnPrimaryColor"] = OnPrimaryColorValue,
        ["PrimaryContainerColor"] = PrimaryContainerColorValue,
        ["OnPrimaryContainerColor"] = OnPrimaryContainerColorValue,
        ["InversePrimaryColor"] = InversePrimaryColorValue,

        ["SecondaryColor"] = SecondaryColorValue,
        ["OnSecondaryColor"] = OnSecondaryColorValue,
        ["SecondaryContainerColor"] = SecondaryContainerColorValue,
        ["OnSecondaryContainerColor"] = OnSecondaryContainerColorValue,

        ["TertiaryColor"] = TertiaryColorValue,
        ["OnTertiaryColor"] = OnTertiaryColorValue,
        ["TertiaryContainerColor"] = TertiaryContainerColorValue,
        ["OnTertiaryContainerColor"] = OnTertiaryContainerColorValue,

        ["ErrorColor"] = ErrorColorValue,
        ["OnErrorColor"] = OnErrorColorValue,
        ["ErrorContainerColor"] = ErrorContainerColorValue,
        ["OnErrorContainerColor"] = OnErrorContainerColorValue,
    }.ToFrozenDictionary();

    public DynamicScheme(
        Hct sourceColorHct,
        bool isDark,
        Variant variant,
        double contrastLevel,
        TonalPalette primaryPalette,
        TonalPalette secondaryPalette,
        TonalPalette tertiaryPalette,
        TonalPalette neutralPalette,
        TonalPalette neutralVariantPalette) {
        SourceColorHct = sourceColorHct;
        SourceColorArgb = sourceColorHct;
        IsDark = isDark;
        Variant = variant;
        ContrastLevel = contrastLevel;

        PrimaryPalette = primaryPalette;
        NeutralPalette = neutralPalette;
        TertiaryPalette = tertiaryPalette;
        SecondaryPalette = secondaryPalette;
        NeutralVariantPalette = neutralVariantPalette;

        // Google's fixed error palette
        ErrorPalette = TonalPalette.CreateFromHueChroma(25.0, 84.0);
    }

    public Hct GetHct(DynamicColor dynamicColor) => dynamicColor.GetHct(this);
    public uint GetArgb(DynamicColor dynamicColor) => dynamicColor.GetArgb(this);
    private uint Resolve(DynamicColor dc) => dc.GetArgb(this);

    /// <summary>
    /// Given a set of hues and set of hue rotations, locate which hues the source color's hue is between,
    /// apply the rotation at the same index as the first hue in the range,
    /// and return the rotated hue.
    /// </summary>
    public static double GetRotatedHue(Hct hct, double[] hues, double[] rotations) {
        double h = hct.H;

        if (rotations.Length == 1)
            return MathUtil.SanitizeDegrees(h + rotations[0]);

        for (int i = 0; i < hues.Length - 1; i++) {
            if (hues[i] < h && h < hues[i + 1])
                return MathUtil.SanitizeDegrees(h + rotations[i]);
        }

        return h;
    }
}
