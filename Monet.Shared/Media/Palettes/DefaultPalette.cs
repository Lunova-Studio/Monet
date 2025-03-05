using Monet.Shared.Media.ColorSpace;

namespace Monet.Shared.Media.Palettes;

/// <summary>
/// An intermediate concept between the key color for a UI theme, and a full color scheme. 
/// 5 sets of tones are generated, all except one use the same hue as the key color, and all vary in chroma.
/// </summary>
public sealed class DefaultPalette {
    public TonalPalette A1 { get; set; }
    public TonalPalette A2 { get; set; }
    public TonalPalette A3 { get; set; }
    public TonalPalette N1 { get; set; }
    public TonalPalette N2 { get; set; }
    public TonalPalette Error { get; set; }

    internal DefaultPalette(uint argb, bool isContent) {
        Hct hct = argb;

        Error = TonalPalette.CreateFromHueChroma(25, 84);
        if (isContent) {
            A1 = TonalPalette.CreateFromHueChroma(hct.H, hct.C);
            A2 = TonalPalette.CreateFromHueChroma(hct.H, hct.C / 3d);
            A3 = TonalPalette.CreateFromHueChroma(hct.H + 60d, hct.C / 2d);
            N1 = TonalPalette.CreateFromHueChroma(hct.H, Math.Min(hct.C / 12d, 4d));
            N2 = TonalPalette.CreateFromHueChroma(hct.H, Math.Min(hct.C / 6d, 8d));
            return;
        }

        A1 = TonalPalette.CreateFromHueChroma(hct.H, Math.Max(48d, hct.C));
        A2 = TonalPalette.CreateFromHueChroma(hct.H, 16d);
        A3 = TonalPalette.CreateFromHueChroma(hct.H + 60, 24d);
        N1 = TonalPalette.CreateFromHueChroma(hct.H, 4d);
        N2 = TonalPalette.CreateFromHueChroma(hct.H, 8d);
    }

    /// <summary>
    /// Create key tones from a color.
    /// </summary>
    /// <param name="argb">ARGB representation of a color</param>
    public static DefaultPalette CreateFromArgb(uint argb) {
        return new DefaultPalette(argb, false);
    }

    /// <summary>
    /// Create content key tones from a color.
    /// </summary>
    /// <param name="argb">ARGB representation of a color</param>
    /// 
    public static DefaultPalette CreateForContent(uint argb) {
        return new DefaultPalette(argb, true);
    }
}
