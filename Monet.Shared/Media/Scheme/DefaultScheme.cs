using Monet.Shared.Interfaces;
using Monet.Shared.Media.Palettes;

namespace Monet.Shared.Media.Scheme;

/// <summary>
/// Represents a Material color scheme, a mapping of color roles to colors.
/// </summary>
public sealed class DefaultScheme : IColorValueScheme {
    #region Colors

    public required uint PrimaryColorValue { get; set; }
    public required uint OnPrimaryColorValue { get; set; }
    public required uint PrimaryContainerColorValue { get; set; }
    public required uint OnPrimaryContainerColorValue { get; set; }
    public required uint InversePrimaryColorValue { get; set; }

    public required uint SecondaryColorValue { get; set; }
    public required uint OnSecondaryColorValue { get; set; }
    public required uint SecondaryContainerColorValue { get; set; }
    public required uint OnSecondaryContainerColorValue { get; set; }

    public required uint TertiaryColorValue { get; set; }
    public required uint OnTertiaryColorValue { get; set; }
    public required uint TertiaryContainerColorValue { get; set; }
    public required uint OnTertiaryContainerColorValue { get; set; }

    public required uint ErrorColorValue { get; set; }
    public required uint OnErrorColorValue { get; set; }
    public required uint ErrorContainerColorValue { get; set; }
    public required uint OnErrorContainerColorValue { get; set; }

    public required uint SurfaceColorValue { get; set; }
    public required uint OnSurfaceColorValue { get; set; }
    public required uint SurfaceVariantColorValue { get; set; }
    public required uint OnSurfaceVariantColorValue { get; set; }
    public required uint InverseSurfaceColorValue { get; set; }
    public required uint InverseOnSurfaceColorValue { get; set; }

    public required uint OutlineColorValue { get; set; }
    public required uint OutlineVariantColorValue { get; set; }

    public required uint BackgroundColorValue { get; set; }
    public required uint OnBackgroundColorValue { get; set; }

    public required uint ScrimColorValue { get; set; }
    public required uint ShadowColorValue { get; set; }

    #endregion

    /// <summary>
    /// Creates a light theme Scheme from a source color in ARGB, i.e. a hex code.
    /// </summary>
    public static DefaultScheme CreateLightScheme(uint argb) {
        return CreateFromPalette(DefaultPalette.CreateFromArgb(argb), false);
    }

    /// <summary>
    /// Creates a dark theme Scheme from a source color in ARGB, i.e. a hex code.
    /// </summary>
    public static DefaultScheme CreateDarkScheme(uint argb) {
        return CreateFromPalette(DefaultPalette.CreateFromArgb(argb), true);
    }

    /// <summary>
    /// Creates a light theme content-based Scheme from a source color in ARGB, i.e. a hex code.
    /// </summary>
    public static DefaultScheme CreateLightContentScheme(uint argb) {
        return CreateFromPalette(DefaultPalette.CreateForContent(argb), false);
    }

    /// <summary>
    /// Creates a dark theme content-based Scheme from a source color in ARGB, i.e. a hex code.
    /// </summary>
    public static DefaultScheme CreateDarkContentScheme(uint argb) {
        return CreateFromPalette(DefaultPalette.CreateForContent(argb), true);
    }

    private static DefaultScheme CreateFromPalette(DefaultPalette palette, bool isDark) {
        return isDark 
            ? CreateDarkScheme(palette) 
            : CreateLightScheme(palette);

        static DefaultScheme CreateDarkScheme(DefaultPalette palette) {
            return new DefaultScheme {
                PrimaryColorValue = palette.A1.CreateFromTone(80),
                OnPrimaryColorValue = palette.A1.CreateFromTone(20),
                PrimaryContainerColorValue = palette.A1.CreateFromTone(30),
                OnPrimaryContainerColorValue = palette.A1.CreateFromTone(90),

                SecondaryColorValue = palette.A2.CreateFromTone(80),
                OnSecondaryColorValue = palette.A2.CreateFromTone(20),
                SecondaryContainerColorValue = palette.A2.CreateFromTone(30),
                OnSecondaryContainerColorValue = palette.A2.CreateFromTone(90),

                TertiaryColorValue = palette.A3.CreateFromTone(80),
                OnTertiaryColorValue = palette.A3.CreateFromTone(20),
                TertiaryContainerColorValue = palette.A3.CreateFromTone(30),
                OnTertiaryContainerColorValue = palette.A3.CreateFromTone(90),

                ErrorColorValue = palette.Error.CreateFromTone(80),
                OnErrorColorValue = palette.Error.CreateFromTone(20),
                ErrorContainerColorValue = palette.Error.CreateFromTone(30),
                OnErrorContainerColorValue = palette.Error.CreateFromTone(80),

                SurfaceColorValue = palette.N1.CreateFromTone(10),
                OnSurfaceColorValue = palette.N1.CreateFromTone(90),
                SurfaceVariantColorValue = palette.N2.CreateFromTone(30),
                OnSurfaceVariantColorValue = palette.N2.CreateFromTone(80),

                OutlineColorValue = palette.N2.CreateFromTone(60),
                OutlineVariantColorValue = palette.N2.CreateFromTone(30),

                BackgroundColorValue = palette.N1.CreateFromTone(10),
                OnBackgroundColorValue = palette.N1.CreateFromTone(90),

                ScrimColorValue = palette.N1.CreateFromTone(0),
                ShadowColorValue = palette.N1.CreateFromTone(0),

                InversePrimaryColorValue = palette.A1.CreateFromTone(40),
                InverseSurfaceColorValue = palette.N1.CreateFromTone(90),
                InverseOnSurfaceColorValue = palette.N1.CreateFromTone(20),
            };
        }

        static DefaultScheme CreateLightScheme(DefaultPalette palette) {
            return new DefaultScheme {
                PrimaryColorValue = palette.A1.CreateFromTone(40),
                OnPrimaryColorValue = palette.A1.CreateFromTone(100),
                PrimaryContainerColorValue = palette.A1.CreateFromTone(90),
                OnPrimaryContainerColorValue = palette.A1.CreateFromTone(10),

                SecondaryColorValue = palette.A2.CreateFromTone(40),
                OnSecondaryColorValue = palette.A2.CreateFromTone(100),
                SecondaryContainerColorValue = palette.A2.CreateFromTone(90),
                OnSecondaryContainerColorValue = palette.A2.CreateFromTone(10),

                TertiaryColorValue = palette.A3.CreateFromTone(40),
                OnTertiaryColorValue = palette.A3.CreateFromTone(100),
                TertiaryContainerColorValue = palette.A3.CreateFromTone(90),
                OnTertiaryContainerColorValue = palette.A3.CreateFromTone(10),

                ErrorColorValue = palette.Error.CreateFromTone(40),
                OnErrorColorValue = palette.Error.CreateFromTone(100),
                ErrorContainerColorValue = palette.Error.CreateFromTone(90),
                OnErrorContainerColorValue = palette.Error.CreateFromTone(10),

                SurfaceColorValue = palette.N1.CreateFromTone(99),
                OnSurfaceColorValue = palette.N1.CreateFromTone(10),
                SurfaceVariantColorValue = palette.N2.CreateFromTone(90),
                OnSurfaceVariantColorValue = palette.N2.CreateFromTone(30),

                OutlineColorValue = palette.N2.CreateFromTone(50),
                OutlineVariantColorValue = palette.N2.CreateFromTone(80),

                BackgroundColorValue = palette.N1.CreateFromTone(99),
                OnBackgroundColorValue = palette.N1.CreateFromTone(10),

                ScrimColorValue = palette.N1.CreateFromTone(0),
                ShadowColorValue = palette.N1.CreateFromTone(0),

                InverseSurfaceColorValue = palette.N1.CreateFromTone(20),
                InversePrimaryColorValue = palette.A1.CreateFromTone(80),
                InverseOnSurfaceColorValue = palette.N1.CreateFromTone(95),
            };
        }
    }
}
