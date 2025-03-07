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
    private bool _isColorMatch;
    private Variant _variant;
    private ResourceDictionary _resource = null!;

    private static IPlatformSettings PlatformSettings => Application.Current!.PlatformSettings!;

    public bool IsDarkMode { get; set; } = PlatformSettings.GetColorValues().ThemeVariant is PlatformThemeVariant.Dark;

    public bool IsColorMatch {
        get => _isColorMatch;
        set {
            if (_isColorMatch == value)
                return;

            _isColorMatch = value;
            BuildScheme(_variant, _color, _level);
        }
    }

    public void Initialize() => BuildScheme(Variant.Default, ColorUtil.GOOGLE_BLUE, 0);

    public void BuildScheme(Variant variant, Color color, double level = 0.0) => BuildScheme(variant, color.ToUInt32(), level);

    private void BuildScheme(Variant variant, uint color, double level = 0.0) {
        bool isDark = IsDarkMode;
        IColorValueScheme scheme = null!;

        _color = color;
        _level = level;
        _variant = variant;

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

        _resource = new() {
            { "ScrimBrush", new SolidColorBrush(Color.FromUInt32(scheme.ScrimColorValue)) },
            { "ShadowBrush", new SolidColorBrush(Color.FromUInt32(scheme.ShadowColorValue)) },
            { "BackgroundBrush", new SolidColorBrush(Color.FromUInt32(scheme.BackgroundColorValue)) },
            { "OnBackgroundBrush", new SolidColorBrush(Color.FromUInt32(scheme.OnBackgroundColorValue)) },
            { "OutlineBrush", new SolidColorBrush(Color.FromUInt32(scheme.OutlineColorValue)) },
            { "OutlineVariantBrush", new SolidColorBrush(Color.FromUInt32(scheme.OutlineVariantColorValue)) },
            { "PrimaryBrush", new SolidColorBrush(Color.FromUInt32(scheme.PrimaryColorValue)) },
            { "OnPrimaryBrush", new SolidColorBrush(Color.FromUInt32(scheme.OnPrimaryColorValue)) },
            { "PrimaryContainerBrush", new SolidColorBrush(Color.FromUInt32(scheme.PrimaryContainerColorValue)) },
            { "OnPrimaryContainerBrush", new SolidColorBrush(Color.FromUInt32(scheme.OnPrimaryContainerColorValue)) },
            { "InversePrimaryBrush", new SolidColorBrush(Color.FromUInt32(scheme.InversePrimaryColorValue)) },
            { "SecondaryBrush", new SolidColorBrush(Color.FromUInt32(scheme.SecondaryColorValue)) },
            { "OnSecondaryBrush", new SolidColorBrush(Color.FromUInt32(scheme.OnSecondaryColorValue)) },
            { "SecondaryContainerBrush", new SolidColorBrush(Color.FromUInt32(scheme.SecondaryContainerColorValue)) },
            { "OnSecondaryContainerBrush", new SolidColorBrush(Color.FromUInt32(scheme.OnSecondaryContainerColorValue)) },
            { "TertiaryBrush", new SolidColorBrush(Color.FromUInt32(scheme.TertiaryColorValue)) },
            { "OnTertiaryBrush", new SolidColorBrush(Color.FromUInt32(scheme.OnTertiaryColorValue)) },
            { "TertiaryContainerBrush", new SolidColorBrush(Color.FromUInt32(scheme.TertiaryContainerColorValue)) },
            { "OnTertiaryContainerBrush", new SolidColorBrush(Color.FromUInt32(scheme.OnTertiaryContainerColorValue)) },
            { "SurfaceBrush", new SolidColorBrush(Color.FromUInt32(scheme.SurfaceColorValue)) },
            { "OnSurfaceBrush", new SolidColorBrush(Color.FromUInt32(scheme.OnSurfaceColorValue)) },
            { "SurfaceVariantBrush", new SolidColorBrush(Color.FromUInt32(scheme.SurfaceVariantColorValue)) },
            { "OnSurfaceVariantBrush", new SolidColorBrush(Color.FromUInt32(scheme.OnSurfaceVariantColorValue)) },
            { "InverseSurfaceBrush", new SolidColorBrush(Color.FromUInt32(scheme.InverseSurfaceColorValue)) },
            { "InverseOnSurfaceBrush", new SolidColorBrush(Color.FromUInt32(scheme.InverseOnSurfaceColorValue)) },
            { "ErrorBrush", new SolidColorBrush(Color.FromUInt32(scheme.ErrorColorValue)) },
            { "OnErrorBrush", new SolidColorBrush(Color.FromUInt32(scheme.OnErrorColorValue)) },
            { "ErrorContainerBrush", new SolidColorBrush(Color.FromUInt32(scheme.ErrorContainerColorValue)) },
            { "OnErrorContainerBrush", new SolidColorBrush(Color.FromUInt32(scheme.OnErrorContainerColorValue)) },
        };

        Application.Current!.Resources.MergedDictionaries.Add(_resource);
    }
}