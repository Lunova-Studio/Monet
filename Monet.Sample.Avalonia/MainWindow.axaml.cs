using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Monet.Shared;
using Monet.Shared.Media.Scheme;
using Monet.Shared.Media.Scheme.Dynamic;
using Monet.Shared.Utilities;

namespace Monet.Sample.Avalonia;
public partial class MainWindow : Window {
    public MainWindow() {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e) {
        base.OnLoaded(e);

        var scheme = new FruitSaladScheme(ColorUtil.GOOGLE_BLUE, true, 5);

        Resources.Add("PrimaryBrush", new SolidColorBrush(Color.FromUInt32(scheme.PrimaryColorValue)));
        Resources.Add("OnPrimaryBrush", new SolidColorBrush(Color.FromUInt32(scheme.OnPrimaryColorValue)));
        Resources.Add("PrimaryContainerBrush", new SolidColorBrush(Color.FromUInt32(scheme.PrimaryContainerColorValue)));
        Resources.Add("OnPrimaryContainerBrush", new SolidColorBrush(Color.FromUInt32(scheme.OnPrimaryContainerColorValue)));

        Resources.Add("SecondaryBrush", new SolidColorBrush(Color.FromUInt32(scheme.SecondaryColorValue)));
        Resources.Add("OnSecondaryBrush", new SolidColorBrush(Color.FromUInt32(scheme.OnSecondaryColorValue)));
        Resources.Add("SecondaryContainerBrush", new SolidColorBrush(Color.FromUInt32(scheme.SecondaryContainerColorValue)));
        Resources.Add("OnSecondaryContainerBrush", new SolidColorBrush(Color.FromUInt32(scheme.OnSecondaryContainerColorValue)));


        Resources.Add("TertiaryBrush", new SolidColorBrush(Color.FromUInt32(scheme.TertiaryColorValue)));
        Resources.Add("OnTertiaryBrush", new SolidColorBrush(Color.FromUInt32(scheme.OnTertiaryColorValue)));
        Resources.Add("TertiaryContainerBrush", new SolidColorBrush(Color.FromUInt32(scheme.TertiaryContainerColorValue)));
        Resources.Add("OnTertiaryContainerBrush", new SolidColorBrush(Color.FromUInt32(scheme.OnTertiaryContainerColorValue)));

        Resources.Add("BackgroundBrush", new SolidColorBrush(Color.FromUInt32(scheme.BackgroundColorValue)));
        Resources.Add("OnBackgroundBrush", new SolidColorBrush(Color.FromUInt32(scheme.OnBackgroundColorValue)));
    }
}