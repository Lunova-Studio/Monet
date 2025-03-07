using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Styling;
using Monet.Avalonia;
using Monet.Avalonia.Extensions;
using Monet.Shared.Enums;
using Monet.Shared.Media.Scheme.Dynamic;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Linq;
using Color = Avalonia.Media.Color;

namespace Monet.Sample.Avalonia;

public partial class MainWindow : Window {
    private double level = 0.57;
    private MonetColors _monet = Application.Current.Styles[1] as MonetColors;
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
        _monet.IsColorMatch = true;
        _monet.IsDarkMode = change.IsChecked ?? true;
        Change(_monet.IsDarkMode);
    }

    private void SchemeComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e) {
        Change(change.IsChecked ?? true);
    }

    void Change(bool isDark) {
        this.RequestedThemeVariant = isDark ? ThemeVariant.Dark : ThemeVariant.Light;

        switch (schemeComboBox.SelectedIndex) {
            case 0:
                _monet.BuildScheme(Variant.Rainbow, defaultColor);
                break;
            case 1:
                _monet.BuildScheme(Variant.Content, defaultColor);
                break;
            case 2:
                _monet.BuildScheme(Variant.Fruit_Salad, defaultColor);
                break;
            case 3:
                _monet.BuildScheme(Variant.Vibrant, defaultColor);
                break;
            case 4:
                _monet.BuildScheme(Variant.Neutral, defaultColor);
                break;
            case 5:
                _monet.BuildScheme(Variant.Fidelity, defaultColor);
                break;
            case 6:
                _monet.BuildScheme(Variant.Expressive, defaultColor);
                break;
            case 7:
                _monet.BuildScheme(Variant.Monochrome, defaultColor);
                break;
            case 8:
                _monet.BuildScheme(Variant.Tonal_Spot, defaultColor);
                break;
            default:
                break;
        }
    }
}