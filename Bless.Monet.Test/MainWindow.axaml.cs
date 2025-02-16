using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using Avalonia.Threading;
using Bless.Monet.Media.Quantize;
using Bless.Monet.Utilities;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Diagnostics;
using System.IO;
using Color = Avalonia.Media.Color;
using Image = SixLabors.ImageSharp.Image;

namespace Bless.Monet.Test;
public partial class MainWindow : Window {
    private Monet _monet;

    public MainWindow() {
        InitializeComponent();
        _monet = (Application.Current.Styles[0] as Monet)!;
        _monet.RefreshDynamicColors(Colors.Red);
        ActualThemeVariantChanged += (_, arg) => {
            Theme.Text = ActualThemeVariant == ThemeVariant.Dark ? "Dark Mode" : "Light Mode";
            _monet.RefreshDynamicColors(Colors.Red);
        };
    }

    private async void IButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {
        //var fi = new FileInfo(Test_Text.Text! ?? "");
        //if (!fi.Exists) {
        //    return;
        //}

        //Dispatcher.UIThread.Post(() => {
        //    _monet.RefreshDynamicColorsFromBitmap(fi.FullName);
        //}, DispatcherPriority.ApplicationIdle);

        //RB(fi.FullName);
    }

    private void RB(string s) {
        Application.Current!.TryFindResource("PrimaryColor50", out var PrimaryColor50);
        Application.Current!.TryFindResource("PrimaryColor60", out var PrimaryColor60);
        Application.Current!.TryFindResource("PrimaryColor70", out var PrimaryColor70);
        Application.Current!.TryFindResource("PrimaryColor80", out var PrimaryColor80);

        Background = new ImageBrush(new Bitmap(s)) { Stretch = Stretch.UniformToFill };
    }
}