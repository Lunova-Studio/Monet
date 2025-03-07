using Monet.Shared.Enums;
using Monet.Shared.Interfaces;
using Monet.Shared.Media.ColorSpace;
using Monet.Shared.Media.Palettes;
using Monet.Shared.Utilities;

namespace Monet.Shared.Media.Scheme.Dynamic;

/// <summary>
/// Provides important settings for creating colors dynamically, and 6 color palettes. Requires:
/// 1. A color 
/// 2. A theme. (Variant) 
/// 3. Whether or not its dark mode. 
/// 4. Contrast level. (-1 to 1, currently contrast ratio 3.0 and 7.0)
/// </summary>
public class DynamicScheme : IColorValueScheme {
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

    public uint PrimaryPaletteKeyColorValue => 
        GetArgb(new MaterialDynamicColor().PrimaryPaletteKeyColor);

    public uint SecondaryPaletteKeyColorValue =>
        GetArgb(new MaterialDynamicColor().SecondaryPaletteKeyColor);

    public uint TertiaryPaletteKeyColorValue =>
        GetArgb(new MaterialDynamicColor().TertiaryPaletteKeyColor);

    public uint NeutralPaletteKeyColorValue =>
        GetArgb(new MaterialDynamicColor().NeutralPaletteKeyColor);

    public uint NeutralVariantPaletteKeyColorValue =>
        GetArgb(new MaterialDynamicColor().NeutralVariantPaletteKeyColor);

    public uint BackgroundColorValue =>
        GetArgb(new MaterialDynamicColor().Background);

    public uint OnBackgroundColorValue =>
        GetArgb(new MaterialDynamicColor().OnBackground);

    public uint SurfaceColorValue =>
        GetArgb(new MaterialDynamicColor().Surface);

    public uint OnSurfaceColorValue =>
        GetArgb(new MaterialDynamicColor().OnSurface);

    public uint SurfaceColorDimColorValue =>
        GetArgb(new MaterialDynamicColor().SurfaceDim);
    
    public uint SurfaceBrightColorValue =>
        GetArgb(new MaterialDynamicColor().SurfaceDim);

    public uint SurfaceContainerLowestColorValue =>
        GetArgb(new MaterialDynamicColor().SurfaceDim);

    public uint SurfaceContainerColorValue =>
        GetArgb(new MaterialDynamicColor().SurfaceDim);

    public uint SurfaceContainerHighColorValue =>
        GetArgb(new MaterialDynamicColor().SurfaceDim);

    public uint SurfaceContainerHighestColorValue =>
        GetArgb(new MaterialDynamicColor().SurfaceDim);

    public uint SurfaceVariantColorValue =>
        GetArgb(new MaterialDynamicColor().SurfaceDim);

    public uint OnSurfaceVariantColorValue =>
        GetArgb(new MaterialDynamicColor().SurfaceDim);

    public uint InverseSurfaceColorValue =>
        GetArgb(new MaterialDynamicColor().SurfaceDim);

    public uint InverseOnSurfaceColorValue =>
        GetArgb(new MaterialDynamicColor().SurfaceDim);

    public uint OutlineColorValue =>
        GetArgb(new MaterialDynamicColor().SurfaceDim);

    public uint OutlineVariantColorValue =>
        GetArgb(new MaterialDynamicColor().SurfaceDim);

    public uint ShadowColorValue =>
        GetArgb(new MaterialDynamicColor().SurfaceDim);

    public uint ScrimColorValue =>
        GetArgb(new MaterialDynamicColor().SurfaceDim);

    public uint PrimaryColorValue =>
        GetArgb(new MaterialDynamicColor().Primary);

    public uint OnPrimaryColorValue =>
        GetArgb(new MaterialDynamicColor().OnPrimary);

    public uint PrimaryContainerColorValue =>
        GetArgb(new MaterialDynamicColor().PrimaryContainer);

    public uint OnPrimaryContainerColorValue =>
        GetArgb(new MaterialDynamicColor().OnPrimaryContainer);

    public uint InversePrimaryColorValue =>
        GetArgb(new MaterialDynamicColor().InversePrimary);

    public uint SecondaryColorValue =>
        GetArgb(new MaterialDynamicColor().Secondary);

    public uint OnSecondaryColorValue =>
        GetArgb(new MaterialDynamicColor().OnSecondary);

    public uint SecondaryContainerColorValue =>
        GetArgb(new MaterialDynamicColor().SecondaryContainer);

    public uint OnSecondaryContainerColorValue =>
        GetArgb(new MaterialDynamicColor().OnSecondaryContainer);

    public uint TertiaryColorValue =>
        GetArgb(new MaterialDynamicColor().Tertiary);

    public uint OnTertiaryColorValue =>
        GetArgb(new MaterialDynamicColor().OnTertiary);

    public uint TertiaryContainerColorValue =>
        GetArgb(new MaterialDynamicColor().TertiaryContainer);

    public uint OnTertiaryContainerColorValue =>
        GetArgb(new MaterialDynamicColor().OnTertiaryContainer);

    public uint ErrorColorValue => 
        GetArgb(new MaterialDynamicColor().Error);

    public uint OnErrorColorValue =>
        GetArgb(new MaterialDynamicColor().OnError);

    public uint ErrorContainerColorValue =>
        GetArgb(new MaterialDynamicColor().ErrorContainer);

    public uint OnErrorContainerColorValue =>
        GetArgb(new MaterialDynamicColor().OnErrorContainer);

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
        ErrorPalette = TonalPalette.CreateFromHueChroma(25.0, 84.0);
    }

    public Hct GetHct(DynamicColor dynamicColor) {
        return dynamicColor.GetHct(this);
    }

    public uint GetArgb(DynamicColor dynamicColor) {
        return dynamicColor.GetArgb(this);
    }

    /// <summary>
    /// Given a set of hues and set of hue rotations, locate which hues the source color's hue is between,
    /// apply the rotation at the same index as the first hue in the range,
    /// and return the rotated hue.
    /// </summary>
    /// <param name="hct">The color whose hue should be rotated.</param>
    /// <param name="hues">A set of hues.</param>
    /// <param name="rotations">A set of hue rotations.</param>
    /// <returns>Color's hue with a rotation applied.</returns>
    public static double GetRotatedHue(Hct hct, double[] hues, double[] rotations) {
        var h = hct.H;
        if (rotations.Length is 1)
            return MathUtil.SanitizeDegrees(h + rotations[0]);

        var size = hues.Length;
        for (var i = 0; i < (size - 2); i++) {
            var h2 = hues[i];
            var nextH = hues[i + 1];
            if(h2 < h && h < nextH)
                return MathUtil.SanitizeDegrees(h + rotations[i]);
        }

        // If this statement executes, something is wrong, there should have been a rotation
        // found using the arrays.
        return h;
    }
}