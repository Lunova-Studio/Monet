using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Styling;
using Monet.Avalonias.Extensions;
using Monet.Shared.Enums;
using Monet.Shared.Media.Scheme.Dynamic;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Linq;
using Color = Avalonia.Media.Color;

namespace Monet.Sample.Avalonia;

public partial class MainWindow : Window {
    private double level = 0.57;
    private ResourceDictionary resources = null!;
    private Color defaultColor = Application.Current!.PlatformSettings!.GetColorValues().AccentColor1;

    public MainWindow() {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e) {
        base.OnLoaded(e);

        IButton.Click += IButton_Click;
        change.IsCheckedChanged += Change_IsCheckedChanged;
        numericUpDown.ValueChanged += NumericUpDown_ValueChanged;
        schemeComboBox.SelectionChanged += SchemeComboBox_SelectionChanged;

        schemeComboBox.SelectedIndex = 0;
    }

    private async void IButton_Click(object? sender, RoutedEventArgs e) {
        var res = await this.StorageProvider.OpenFilePickerAsync(new() { Title = "Image" });
        if (res is null || res.Count is 0)
            return;

        var result = SixLabors.ImageSharp.Image
            .Load<Rgba32>(res[0].Path.LocalPath)
            .QuantizeAndGetPrimaryColors()
            .First();

        defaultColor = result;
        Text_Test.Text = res[0].Path.LocalPath;

        Change(change.IsChecked ?? true);
    }

    private void NumericUpDown_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e) {
        level = Convert.ToDouble(e.NewValue);
        Change(change.IsChecked ?? true);
    }

    private void Change_IsCheckedChanged(object? sender, RoutedEventArgs e) {
        Change(change.IsChecked ?? true);
    }

    private void SchemeComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e) {
        Change(change.IsChecked ?? true);
    }

    void Change(bool isDark) {
        this.RequestedThemeVariant = isDark ? ThemeVariant.Dark : ThemeVariant.Light;

        switch (schemeComboBox.SelectedIndex) {
            case 0:
                BuildScheme(defaultColor, Variant.Rainbow, isDark);
                break;
            case 1:
                BuildScheme(defaultColor, Variant.Content, isDark);
                break;
            case 2:
                BuildScheme(defaultColor, Variant.Fruit_Salad, isDark);
                break;
            case 3:
                BuildScheme(defaultColor, Variant.Vibrant, isDark);
                break;
            case 4:
                BuildScheme(defaultColor, Variant.Neutral, isDark);
                break;
            case 5:
                BuildScheme(defaultColor, Variant.Fidelity, isDark);
                break;
            case 6:
                BuildScheme(defaultColor, Variant.Expressive, isDark);
                break;
            case 7:
                BuildScheme(defaultColor, Variant.Monochrome, isDark);
                break;
            case 8:
                BuildScheme(defaultColor, Variant.Tonal_Spot, isDark);
                break;
            default:
                break;
        }
    }

    void BuildScheme(Color color, Variant variant, bool isDark) {
        DynamicScheme scheme = variant switch {
            Variant.Rainbow => new RainbowScheme(color.ToUInt32(), isDark, level),
            Variant.Content => new ContentScheme(color.ToUInt32(), isDark, level),
            Variant.Fruit_Salad => new FruitSaladScheme(color.ToUInt32(), isDark, level),
            Variant.Vibrant => new VibrantScheme(color.ToUInt32(), isDark, level),
            Variant.Tonal_Spot => new TonalSpotScheme(color.ToUInt32(), isDark, level),
            Variant.Monochrome => new MonochromeScheme(color.ToUInt32(), isDark, level),
            Variant.Expressive => new ExpressiveScheme(color.ToUInt32(), isDark, level),
            Variant.Fidelity => new FidelitySceme(color.ToUInt32(), isDark, level),
            Variant.Neutral => new NeutralScheme(color.ToUInt32(), isDark, level),
            _ => throw new Exception()
        };

        ThemeScheme.Text = variant.ToString();

        if (resources != null) {
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
}