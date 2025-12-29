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
                Variant.FruitSalad => new FruitSaladScheme(color, isDark, level),
                Variant.Vibrant => new VibrantScheme(color, isDark, level),
                Variant.TonalSpot => new TonalSpotScheme(color, isDark, level),
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

        _resource = [];
        foreach (var item in scheme.Resources) {
            var colorR = Color.FromUInt32(item.Value);

            _resource.Add(item.Key, colorR);
            _resource.Add(item.Key.Replace("Color", "Brush"), new SolidColorBrush(colorR));
        }

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