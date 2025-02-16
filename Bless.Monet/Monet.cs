using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;
using Bless.Monet.Media;
using Bless.Monet.Utilities;
using SixLabors.ImageSharp.PixelFormats;
using Color = Avalonia.Media.Color;
using Image = SixLabors.ImageSharp.Image;

namespace Bless.Monet;

public sealed class Monet : Styles, IResourceProvider {
    private ResourceDictionary _resources = null!;

    public Color? PrimaryColor { get; set; }

    public bool IsAutoSetColor { get; set; }

    public string? ImageSource { get; set; }

    public Monet() {
        Init();
    }

    private void Init() {
        if (string.IsNullOrWhiteSpace(ImageSource)) {
            RefreshDynamicColors(PrimaryColor ?? Application.Current!.PlatformSettings.GetColorValues().AccentColor1);
            return;
        }

        RefreshDynamicColorsFromBitmap(ImageSource);
    }

    public void RefreshDynamicColorsFromBitmap(string path) {
        ImageSource = path;
        PrimaryColor = BitmapUtil.QuantizeAndGetPrimaryColors(Image.Load<Rgba32>(path))?
            .ToList()?
            .FirstOrDefault();

        if (PrimaryColor is null)
            return;

        RefreshDynamicColors(PrimaryColor.Value);
    }

    public void RefreshDynamicColors(Color primaryColours) {
        if (_resources is not null)
            Resources.MergedDictionaries.Remove(_resources);

        _resources = [];
        int tone = 0;

        PrimaryColor = primaryColours;
        for (int i = 0; i < 11; i++) {
            //PrimaryColor A1
            HctColor primaryHct = primaryColours;

            primaryHct.SetChroma(36);
            _resources.Add($"PrimaryColor{tone}", (Color)primaryHct.SetTone(tone));

            //SecondaryColor A2
            HctColor secondaryHct = primaryColours;
            secondaryHct.SetChroma(16);

            _resources.Add($"SecondaryColor{tone}", (Color)secondaryHct.SetTone(tone));

            //TertiaryColor A3
            HctColor tertiaryHct = primaryColours;
            tertiaryHct.SetHue(tertiaryHct.H + 60).SetChroma(24);

            _resources.Add($"TertiaryColor{tone}", (Color)tertiaryHct.SetTone(tone));

            //NectralColor N1
            HctColor nectralHct = primaryColours;
            nectralHct.SetChroma(4);

            _resources.Add($"NectralColor{tone}", (Color)nectralHct.SetTone(tone));

            //NectralVariantColor N2
            HctColor nectralVariantHct = primaryColours;
            nectralVariantHct.SetChroma(8);

            _resources.Add($"NectralVariantColor{tone}", (Color)nectralVariantHct.SetTone(tone));

            tone += 10;
        }

        _resources.Add($"PrimaryColor95",
            (Color)((HctColor)primaryColours).SetChroma(36).SetTone(95));

        _resources.Add($"SecondaryColor95",
            (Color)((HctColor)primaryColours).SetChroma(16).SetTone(95));

        HctColor c = primaryColours;
        _resources.Add($"TertiaryColor95",
           (Color)c.SetChroma(24).SetHue(c.H + 60).SetTone(95));

        _resources.Add($"NectralColor95",
            (Color)((HctColor)primaryColours).SetChroma(4).SetTone(95));

        _resources.Add($"NectralVariantColor95",
            (Color)((HctColor)primaryColours).SetChroma(8).SetTone(95));

        Resources.MergedDictionaries.Add(_resources);
    }
}