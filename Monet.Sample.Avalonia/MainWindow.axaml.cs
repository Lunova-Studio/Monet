using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Styling;
using Monet.Shared.Enums;
using Monet.Shared.Media.Scheme.Dynamic;
using Monet.Shared.Utilities;
using System;
using System.Diagnostics;

namespace Monet.Sample.Avalonia;
public partial class MainWindow : Window {
    public MainWindow() {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e) {
        base.OnLoaded(e);

        change.IsCheckedChanged += Change_IsCheckedChanged;
        schemeComboBox.SelectionChanged += SchemeComboBox_SelectionChanged;

        schemeComboBox.SelectedIndex = 0;
    }

    private void Change_IsCheckedChanged(object? sender, RoutedEventArgs e) {
        Change(change.IsChecked ?? true);
    }

    private void SchemeComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e) {
        Change(change.IsChecked ?? true);
    }

    ResourceDictionary resources;
    void BuildScheme(Color color, Variant variant, bool isDark) {
        DynamicScheme scheme = variant switch {
            Variant.Rainbow => new RainbowScheme(color.ToUInt32(), isDark, 1),
            Variant.Content => new ContentScheme(color.ToUInt32(), isDark, 1),
            Variant.Fruit_Salad => new FruitSaladScheme(color.ToUInt32(), isDark, 1),
            Variant.Vibrant => new VibrantScheme(color.ToUInt32(), isDark, 1),
            Variant.Tonal_Spot => new TonalSpotScheme(color.ToUInt32(), isDark, 1),
            Variant.Monochrome => new MonochromeScheme(color.ToUInt32(), isDark, 1),
            Variant.Expressive => new ExpressiveScheme(color.ToUInt32(), isDark, 1),
            Variant.Fidelity => new FidelitySceme(color.ToUInt32(), isDark, 1),
            Variant.Neutral => new NeutralScheme(color.ToUInt32(), isDark, 1),
            _ => throw new Exception()
        };

        ThemeScheme.Text = variant.ToString();

        if(resources != null) {
            Resources.MergedDictionaries.Remove(resources);
        }

        resources = new() {
            { "PrimaryBrush", new SolidColorBrush(Color.FromUInt32(scheme.PrimaryColorValue)) },
            { "OnPrimaryBrush", new SolidColorBrush(Color.FromUInt32(scheme.OnPrimaryColorValue)) },
            { "PrimaryContainerBrush", new SolidColorBrush(Color.FromUInt32(scheme.PrimaryContainerColorValue)) },
            { "OnPrimaryContainerBrush", new SolidColorBrush(Color.FromUInt32(scheme.OnPrimaryContainerColorValue)) },
            { "SecondaryBrush", new SolidColorBrush(Color.FromUInt32(scheme.SecondaryColorValue)) },
            { "OnSecondaryBrush", new SolidColorBrush(Color.FromUInt32(scheme.OnSecondaryColorValue)) },
            { "SecondaryContainerBrush", new SolidColorBrush(Color.FromUInt32(scheme.SecondaryContainerColorValue)) },
            { "OnSecondaryContainerBrush", new SolidColorBrush(Color.FromUInt32(scheme.OnSecondaryContainerColorValue)) },
            { "TertiaryBrush", new SolidColorBrush(Color.FromUInt32(scheme.TertiaryColorValue)) },
            { "OnTertiaryBrush", new SolidColorBrush(Color.FromUInt32(scheme.OnTertiaryColorValue)) },
            { "TertiaryContainerBrush", new SolidColorBrush(Color.FromUInt32(scheme.TertiaryContainerColorValue)) },
            { "OnTertiaryContainerBrush", new SolidColorBrush(Color.FromUInt32(scheme.OnTertiaryContainerColorValue)) },
            { "BackgroundBrush", new SolidColorBrush(Color.FromUInt32(scheme.BackgroundColorValue)) },
            { "OnBackgroundBrush", new SolidColorBrush(Color.FromUInt32(scheme.OnBackgroundColorValue)) }
        };

        Resources.MergedDictionaries.Add(resources);
    }

    void Change(bool isDark) {
        this.RequestedThemeVariant = isDark ? ThemeVariant.Dark : ThemeVariant.Light;

        switch (schemeComboBox.SelectedIndex) {
            case 0:
                BuildScheme(Color.FromUInt32(ColorUtil.GOOGLE_BLUE), Variant.Rainbow, isDark);
                break;
            case 1:
                BuildScheme(Color.FromUInt32(ColorUtil.GOOGLE_BLUE), Variant.Content, isDark);
                break;
            case 2:
                BuildScheme(Color.FromUInt32(ColorUtil.GOOGLE_BLUE), Variant.Fruit_Salad, isDark);
                break;
            case 3:
                BuildScheme(Color.FromUInt32(ColorUtil.GOOGLE_BLUE), Variant.Vibrant, isDark);
                break;
            case 4:
                BuildScheme(Color.FromUInt32(ColorUtil.GOOGLE_BLUE), Variant.Neutral, isDark);
                break;
            case 5:
                BuildScheme(Color.FromUInt32(ColorUtil.GOOGLE_BLUE), Variant.Fidelity, isDark);
                break;
            case 6:
                BuildScheme(Color.FromUInt32(ColorUtil.GOOGLE_BLUE), Variant.Expressive, isDark);
                break;
            case 7:
                BuildScheme(Color.FromUInt32(ColorUtil.GOOGLE_BLUE), Variant.Monochrome, isDark);
                break;
            case 8:
                BuildScheme(Color.FromUInt32(ColorUtil.GOOGLE_BLUE), Variant.Tonal_Spot, isDark);
                break;
            default:
                break;
        }
    }
}