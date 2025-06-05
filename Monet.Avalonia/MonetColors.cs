using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Styling;
using Monet.Shared.Enums;
using Monet.Shared.Interfaces;
using Monet.Shared.Media.Scheme;
using Monet.Shared.Media.Scheme.Dynamic;
using Monet.Shared.Utilities;

namespace Monet.Avalonia;

//W.I.P
public sealed class MonetColors : Styles, IMonet {
    private uint _color;
    private double _level;
    private bool _isColorMatch = true;
    private ResourceDictionary _resource = null!;

    private static IPlatformSettings PlatformSettings => Application.Current!.PlatformSettings!;

    public Variant CurrentVariant { get; private set; }
    public IColorValueScheme? DesignTokens { get; private set; }
    public bool IsDarkMode { get; set; } = PlatformSettings.GetColorValues().ThemeVariant is PlatformThemeVariant.Dark;

    public bool IsColorMatch {
        get => _isColorMatch;
        set {
            if (_isColorMatch == value)
                return;

            _isColorMatch = value;
            BuildScheme(CurrentVariant, _color, _level);
        }
    }

    public event EventHandler<ColorSchemeChangedEventArgs>? ColorSchemeChanged;

    public void Initialize() => BuildScheme(Variant.Default, ColorUtil.GOOGLE_BLUE, 0);

    public void BuildScheme(Variant variant, Color color, double level = 0.0) => BuildScheme(variant, color.ToUInt32(), level);

    private void BuildScheme(Variant variant, uint color, double level = 0.0) {
        bool isDark = IsDarkMode;
        _color = color;
        _level = level;
        CurrentVariant = variant;

        IColorValueScheme? scheme;
        if (IsColorMatch)
            scheme = variant switch {
                Variant.Rainbow => new RainbowScheme(color, isDark, level),
                Variant.Content => new ContentScheme(color, isDark, level),
                Variant.Fruit_Salad => new FruitSaladScheme(color, isDark, level),
                Variant.Vibrant => new VibrantScheme(color, isDark, level),
                Variant.Tonal_Spot => new TonalSpotScheme(color, isDark, level),
                Variant.Monochrome => new MonochromeScheme(color, isDark, level),
                Variant.Expressive => new ExpressiveScheme(color, isDark, level),
                Variant.Fidelity => new FidelitySceme(color, isDark, level),
                Variant.Neutral => new NeutralScheme(color, isDark, level),
                _ => isDark
                    ? DefaultScheme.CreateDarkScheme(color)
                    : DefaultScheme.CreateLightScheme(color)
            };
        else
            scheme = isDark
                ? DefaultScheme.CreateDarkScheme(color)
                : DefaultScheme.CreateLightScheme(color);

        if (_resource != null)
            Application.Current!.Resources.MergedDictionaries.Remove(_resource);

        #region 添加颜色资源

        var scrimColor = Color.FromUInt32(scheme.ScrimColorValue);
        var shadowColor = Color.FromUInt32(scheme.ShadowColorValue);
        var backgroundColor = Color.FromUInt32(scheme.BackgroundColorValue);
        var onBackgroundColor = Color.FromUInt32(scheme.OnBackgroundColorValue);
        var outlineColor = Color.FromUInt32(scheme.OutlineColorValue);
        var outlineVariantColor = Color.FromUInt32(scheme.OutlineVariantColorValue);
        var primaryColor = Color.FromUInt32(scheme.PrimaryColorValue);
        var onPrimaryColor = Color.FromUInt32(scheme.OnPrimaryColorValue);
        var primaryContainerColor = Color.FromUInt32(scheme.PrimaryContainerColorValue);
        var onPrimaryContainerColor = Color.FromUInt32(scheme.OnPrimaryContainerColorValue);
        var inversePrimaryColor = Color.FromUInt32(scheme.InversePrimaryColorValue);
        var secondaryColor = Color.FromUInt32(scheme.SecondaryColorValue);
        var onSecondaryColor = Color.FromUInt32(scheme.OnSecondaryColorValue);
        var secondaryContainerColor = Color.FromUInt32(scheme.SecondaryContainerColorValue);
        var onSecondaryContainerColor = Color.FromUInt32(scheme.OnSecondaryContainerColorValue);
        var tertiaryColor = Color.FromUInt32(scheme.TertiaryColorValue);
        var onTertiaryColor = Color.FromUInt32(scheme.OnTertiaryColorValue);
        var tertiaryContainerColor = Color.FromUInt32(scheme.TertiaryContainerColorValue);
        var onTertiaryContainerColor = Color.FromUInt32(scheme.OnTertiaryContainerColorValue);
        var surfaceColor = Color.FromUInt32(scheme.SurfaceColorValue);
        var onSurfaceColor = Color.FromUInt32(scheme.OnSurfaceColorValue);
        var surfaceBrightColor = Color.FromUInt32(scheme.SurfaceBrightColorValue);
        var surfaceVariantColor = Color.FromUInt32(scheme.SurfaceVariantColorValue);
        var surfaceContainerColor = Color.FromUInt32(scheme.SurfaceContainerColorValue);
        var surfaceContainerLowColor = Color.FromUInt32(scheme.SurfaceContainerLowColorValue);
        var surfaceContainerHighColor = Color.FromUInt32(scheme.SurfaceContainerHighColorValue);
        var onSurfaceVariantColor = Color.FromUInt32(scheme.OnSurfaceVariantColorValue);
        var inverseSurfaceColor = Color.FromUInt32(scheme.InverseSurfaceColorValue);
        var inverseOnSurfaceColor = Color.FromUInt32(scheme.InverseOnSurfaceColorValue);
        var errorColor = Color.FromUInt32(scheme.ErrorColorValue);
        var onErrorColor = Color.FromUInt32(scheme.OnErrorColorValue);
        var errorContainerColor = Color.FromUInt32(scheme.ErrorContainerColorValue);
        var onErrorContainerColor = Color.FromUInt32(scheme.OnErrorContainerColorValue);

        _resource = new() {
            { "ScrimColor", scrimColor },
            { "ShadowColor", shadowColor },
            { "BackgroundColor", backgroundColor },
            { "OnBackgroundColor", onBackgroundColor },
            { "OutlineColor", outlineColor },
            { "OutlineVariantColor", outlineVariantColor },
            { "PrimaryColor", primaryColor },
            { "OnPrimaryColor", onPrimaryColor },
            { "PrimaryContainerColor", primaryContainerColor },
            { "OnPrimaryContainerColor", onPrimaryContainerColor },
            { "InversePrimaryColor", inversePrimaryColor },
            { "SecondaryColor", secondaryColor },
            { "OnSecondaryColor", onSecondaryColor },
            { "SecondaryContainerColor", secondaryContainerColor },
            { "OnSecondaryContainerColor", onSecondaryContainerColor },
            { "TertiaryColor", tertiaryColor },
            { "OnTertiaryColor", onTertiaryColor },
            { "TertiaryContainerColor", tertiaryContainerColor },
            { "OnTertiaryContainerColor", onTertiaryContainerColor },
            { "SurfaceColor", surfaceColor },
            { "OnSurfaceColor", onSurfaceColor },
            { "SurfaceBrightColor", surfaceBrightColor },
            { "SurfaceVariantColor", surfaceVariantColor },
            { "SurfaceContainerColor", surfaceContainerColor },
            { "SurfaceContainerLowColor", surfaceContainerLowColor },
            { "SurfaceContainerHighColor", surfaceContainerHighColor },
            { "OnSurfaceVariantColor", onSurfaceVariantColor },
            { "InverseSurfaceColor", inverseSurfaceColor },
            { "InverseOnSurfaceColor", inverseOnSurfaceColor },
            { "ErrorColor", errorColor },
            { "OnErrorColor", onErrorColor },
            { "ErrorContainerColor", errorContainerColor },
            { "OnErrorContainerColor", onErrorContainerColor },
            { "ScrimBrush", new SolidColorBrush(scrimColor) },
            { "ShadowBrush", new SolidColorBrush(shadowColor) },
            { "BackgroundBrush", new SolidColorBrush(backgroundColor) },
            { "OnBackgroundBrush", new SolidColorBrush(onBackgroundColor) },
            { "OutlineBrush", new SolidColorBrush(outlineColor) },
            { "OutlineVariantBrush", new SolidColorBrush(outlineVariantColor) },
            { "PrimaryBrush", new SolidColorBrush(primaryColor) },
            { "OnPrimaryBrush", new SolidColorBrush(onPrimaryColor) },
            { "PrimaryContainerBrush", new SolidColorBrush(primaryContainerColor) },
            { "OnPrimaryContainerBrush", new SolidColorBrush(onPrimaryContainerColor) },
            { "InversePrimaryBrush", new SolidColorBrush(inversePrimaryColor) },
            { "SecondaryBrush", new SolidColorBrush(secondaryColor) },
            { "OnSecondaryBrush", new SolidColorBrush(onSecondaryColor) },
            { "SecondaryContainerBrush", new SolidColorBrush(secondaryContainerColor) },
            { "OnSecondaryContainerBrush", new SolidColorBrush(onSecondaryContainerColor) },
            { "TertiaryBrush", new SolidColorBrush(tertiaryColor) },
            { "OnTertiaryBrush", new SolidColorBrush(onTertiaryColor) },
            { "TertiaryContainerBrush", new SolidColorBrush(tertiaryContainerColor) },
            { "OnTertiaryContainerBrush", new SolidColorBrush(onTertiaryContainerColor) },
            { "SurfaceBrush", new SolidColorBrush(surfaceColor) },
            { "SurfaceBrightBrush", new SolidColorBrush(surfaceBrightColor) },
            { "OnSurfaceBrush", new SolidColorBrush(onSurfaceColor) },
            { "SurfaceContainerBrush", new SolidColorBrush(surfaceContainerColor) },
            { "SurfaceContainerLowBrush", new SolidColorBrush(surfaceContainerLowColor) },
            { "SurfaceContainerHighBrush", new SolidColorBrush(surfaceContainerHighColor) },
            { "SurfaceVariantBrush", new SolidColorBrush(surfaceVariantColor) },
            { "OnSurfaceVariantBrush", new SolidColorBrush(onSurfaceVariantColor) },
            { "InverseSurfaceBrush", new SolidColorBrush(inverseSurfaceColor) },
            { "InverseOnSurfaceBrush", new SolidColorBrush(inverseOnSurfaceColor) },
            { "ErrorBrush", new SolidColorBrush(errorColor) },
            { "OnErrorBrush", new SolidColorBrush(onErrorColor) },
            { "ErrorContainerBrush", new SolidColorBrush(errorContainerColor) },
            { "OnErrorContainerBrush", new SolidColorBrush(onErrorContainerColor) },
        };

        #endregion

        Application.Current!.Resources.MergedDictionaries.Add(_resource);
        ColorSchemeChanged?.Invoke(this, new(DesignTokens = scheme, Application.Current?.ActualThemeVariant!));
    }
}

public class ColorSchemeChangedEventArgs : EventArgs {
    public ThemeVariant Theme { get; }
    public IColorValueScheme Scheme { get; }

    public ColorSchemeChangedEventArgs(IColorValueScheme scheme, ThemeVariant theme) {
        Scheme = scheme;
        Theme = theme;
    }
}