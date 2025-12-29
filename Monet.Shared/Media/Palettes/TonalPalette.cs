using Monet.Shared.Media.ColorSpace;

namespace Monet.Shared.Media.Palettes;

/// <summary>
/// A convenience class for retrieving colors that are constant in hue and chroma, but vary in tone.
/// </summary>
public sealed class TonalPalette {
    private readonly Dictionary<int, uint> _cache;

    public double H { get; }
    public double C { get; }
    public Hct KeyColor { get; }

    internal TonalPalette(double h, double c, Hct keyColor) {
        H = h;
        C = c;
        KeyColor = keyColor;

        _cache = new Dictionary<int, uint>(32);
    }

    /// <summary>
    /// Create tones using the HCT hue and chroma from a color.
    /// </summary>
    /// <param name="argb">ARGB representation of a color</param>
    /// <returns>Tones matching that color's hue and chroma.</returns>
    public static TonalPalette CreateFromArgb(uint argb) {
        return CreateFromHct(new Hct(argb));
    }

    /// <summary>
    /// Create tones using a HCT color.
    /// </summary>
    /// <param name="hct">HCT representation of a color.</param>
    /// <returns>Tones matching that color's hue and chroma.</returns>
    public static TonalPalette CreateFromHct(Hct hct) {
        return new TonalPalette(hct.H, hct.C, hct);
    }

    /// <summary>
    /// Create tones from a defined HCT hue and chroma.
    /// </summary>
    /// <param name="h">HCT hue</param>
    /// <param name="c">HCT chroma</param>
    /// <returns>Tones matching hue and chroma.</returns>
    public static TonalPalette CreateFromHueChroma(double h, double c) {
        var keyColor = new KeyColor(h, c).Create();
        return new TonalPalette(h, c, keyColor);
    }

    /// <summary>
    /// Create an ARGB color with HCT hue and chroma of this Tones instance, and the provided HCT tone.
    /// </summary>
    /// <param name="tone">HCT tone, measured from 0 to 100.</param>
    /// <returns>ARGB representation of a color with that tone.</returns>
    public uint CreateFromTone(int tone) {
        if (_cache.TryGetValue(tone, out uint color))
            return color;

        color = Hct.Parse(H, C, tone).ToUInt32();
        _cache[tone] = color;
        return color;
    }

    /// <summary>
    /// Given a tone, use hue and chroma of palette to create a color, and return it as HCT
    /// </summary>
    /// <returns></returns>
    public Hct GetHct(double tone) {
        return Hct.Parse(H, C, tone);
    }
}

/// <summary>
/// Key color is a color that represents the hue and chroma of a tonal palette.
/// </summary>
internal readonly struct KeyColor {
    private readonly Dictionary<int, double> _chromaCache;

    private const double MAX_CHROMA = 200.0;

    public double H { get; }
    public double RequestedC { get; }

    public KeyColor(double h, double requestedC) {
        H = h;
        RequestedC = requestedC;
        _chromaCache = new Dictionary<int, double>(64);
    }

    /// <summary>
    /// Creates a key color from a [hue] and a [chroma].
    /// The key color is the first tone, starting from T50, matching the given hue and chroma.
    /// </summary>
    /// <returns>Key color [Hct]</returns>
    public readonly Hct Create() {
        int pivotTone = 50;
        int toneStepSize = 1;
        double epsilon = 0.01;

        // Binary search to find the tone that can provide a chroma that is closest
        // to the requested chroma.
        int lowerTone = 0;
        int upperTone = 100;

        while (lowerTone < upperTone) {
            int midTone = (lowerTone + upperTone) / 2;

            // 优化：避免重复计算 MaxChroma
            double cMid = MaxChroma(midTone);
            double cNext = MaxChroma(midTone + toneStepSize);

            bool isAscending = cMid < cNext;
            bool sufficientC = cMid >= RequestedC - epsilon;

            if (sufficientC) {
                if (Math.Abs(lowerTone - pivotTone) < Math.Abs(upperTone - pivotTone)) {
                    upperTone = midTone;
                } else {
                    if (lowerTone == midTone)
                        return Hct.Parse(H, RequestedC, lowerTone);

                    lowerTone = midTone;
                }
            } else {
                if (isAscending)
                    lowerTone = midTone + toneStepSize;
                else
                    upperTone = midTone;
            }
        }

        return Hct.Parse(H, RequestedC, lowerTone);
    }

    private readonly double MaxChroma(int tone) {
        if (_chromaCache.TryGetValue(tone, out var value))
            return value;

        value = Hct.Parse(H, MAX_CHROMA, tone).C;
        _chromaCache[tone] = value;
        return value;
    }
}