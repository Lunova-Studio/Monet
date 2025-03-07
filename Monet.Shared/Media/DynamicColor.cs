using Monet.Shared.Enums;
using Monet.Shared.Media.ColorSpace;
using Monet.Shared.Media.Palettes;
using Monet.Shared.Media.Scheme.Dynamic;
using Monet.Shared.Utilities;

namespace Monet.Shared.Media;

public sealed class DynamicColor {
    public readonly string Name;
    public readonly Func<DynamicScheme, TonalPalette> Palette;
    public readonly Func<DynamicScheme, double> Tone;
    public readonly bool IsBackground;
    public readonly Func<DynamicScheme, DynamicColor> Background;
    public readonly Func<DynamicScheme, DynamicColor> SecondBackground;
    public readonly ContrastCurve ContrastCurve;
    public readonly Func<DynamicScheme, ToneDeltaPair> ToneDeltaPair;
    public readonly Func<DynamicScheme, double> Opacity;
    private readonly Dictionary<DynamicScheme, Hct> _hctCache = [];

    public DynamicColor(string name,
        Func<DynamicScheme, TonalPalette> palette,
        Func<DynamicScheme, double> tone,
        bool isBackground,
        Func<DynamicScheme, DynamicColor> background = null!,
        Func<DynamicScheme, DynamicColor> secondBackground = null!,
        ContrastCurve contrastCurve = null!,
        Func<DynamicScheme, ToneDeltaPair> toneDeltaPair = null!,
        Func<DynamicScheme, double> opacity = null!) {
        Name = name;
        Palette = palette;
        Tone = tone;
        IsBackground = isBackground;
        Background = background;
        SecondBackground = secondBackground;
        ContrastCurve = contrastCurve;
        ToneDeltaPair = toneDeltaPair;
        Opacity = opacity;
    }

    public uint GetArgb(DynamicScheme scheme) {
        uint argb = GetHct(scheme).ToUInt32();
        if (Opacity == null)
            return argb;

        double percentage = Opacity(scheme);
        var alpha = MathUtil.ClampUint(0, 255, (int)Math.Round(percentage * 255));

        return (argb & 0x00ffffff) | (alpha << 24);
    }

    public Hct GetHct(DynamicScheme scheme) {
        if (_hctCache.TryGetValue(scheme, out var cachedAnswer))
            return cachedAnswer;

        double tone = GetTone(scheme);
        var answer = Palette(scheme).GetHct(tone);

        if (_hctCache.Count > 4)
            _hctCache.Clear();

        _hctCache[scheme] = answer;
        return answer;
    }

    public double GetTone(DynamicScheme scheme) {
        bool decreasingContrast = scheme.ContrastLevel < 0;

        if (ToneDeltaPair != null) {
            var toneDeltaPair = ToneDeltaPair(scheme);
            var roleA = toneDeltaPair.RoleA;
            var roleB = toneDeltaPair.RoleB;
            double delta = toneDeltaPair.Delta;
            var polarity = toneDeltaPair.Polarity;
            bool stayTogether = toneDeltaPair.StayTogether;
            var bg = Background(scheme);
            double bgTone = bg.GetTone(scheme);

            bool aIsNearer = (polarity == TonePolarity.Nearer || (polarity == TonePolarity.Lighter && !scheme.IsDark) ||
                              (polarity == TonePolarity.Darker && scheme.IsDark));
            var nearer = aIsNearer ? roleA : roleB;
            var farther = aIsNearer ? roleB : roleA;
            bool amNearer = Name.Equals(nearer.Name);
            double expansionDir = scheme.IsDark ? 1 : -1;

            double nContrast = nearer.ContrastCurve.GetContrastRatios(scheme.ContrastLevel);
            double fContrast = farther.ContrastCurve.GetContrastRatios(scheme.ContrastLevel);

            double nInitialTone = nearer.Tone(scheme);
            double nTone = Contrast.RatioOfTones(bgTone, nInitialTone) >= nContrast
                ? nInitialTone
                : ForegroundTone(bgTone, nContrast);

            double fInitialTone = farther.Tone(scheme);
            double fTone = Contrast.RatioOfTones(bgTone, fInitialTone) >= fContrast
                ? fInitialTone
                : ForegroundTone(bgTone, fContrast);

            if (decreasingContrast) {
                nTone = ForegroundTone(bgTone, nContrast);
                fTone = ForegroundTone(bgTone, fContrast);
            }

            if ((fTone - nTone) * expansionDir < delta) {
                //fTone = Math.Clamp(0, 100, nTone + delta * expansionDir);
                fTone = Math.Clamp(nTone + delta * expansionDir, 0, 100);

                if ((fTone - nTone) * expansionDir < delta)
                    //nTone = Math.Clamp(0, 100, fTone - delta * expansionDir);
                    nTone = Math.Clamp(fTone - delta * expansionDir, 0, 100);
            }

            // Avoids the 50-59 awkward zone.
            if (50 <= nTone && nTone < 60) {
                // If `nearer` is in the awkward zone, move it away, together with
                // `farther`.
                if (expansionDir > 0) {
                    nTone = 60;
                    fTone = Math.Max(fTone, nTone + delta * expansionDir);
                } else {
                    nTone = 49;
                    fTone = Math.Min(fTone, nTone + delta * expansionDir);
                }
            } else if (50 <= fTone && fTone < 60) {
                if (stayTogether) {
                    // Fixes both, to avoid two colors on opposite sides of the "awkward
                    // zone".
                    if (expansionDir > 0) {
                        nTone = 60;
                        fTone = Math.Max(fTone, nTone + delta * expansionDir);
                    } else {
                        nTone = 49;
                        fTone = Math.Min(fTone, nTone + delta * expansionDir);
                    }
                } else {
                    // Not required to stay together; fixes just one.
                    if (expansionDir > 0) {
                        fTone = 60;
                    } else {
                        fTone = 49;
                    }
                }
            }

            return amNearer ? nTone : fTone;
        } else {
            double answer = Tone(scheme);

            if (Background == null)
                return answer;

            double bgTone = Background(scheme).GetTone(scheme);
            double desiredRatio = ContrastCurve.GetContrastRatios(scheme.ContrastLevel);

            if (Contrast.RatioOfTones(bgTone, answer) < desiredRatio) {
                answer = ForegroundTone(bgTone, desiredRatio);
            }

            if (decreasingContrast) {
                answer = ForegroundTone(bgTone, desiredRatio);
            }

            if (IsBackground && 50 <= answer && answer < 60) {
                if (Contrast.RatioOfTones(49, bgTone) >= desiredRatio)
                    answer = 49;
                else
                    answer = 60;
            }

            if (SecondBackground != null) {
                double bgTone1 = Background(scheme).GetTone(scheme);
                double bgTone2 = SecondBackground(scheme).GetTone(scheme);
                double upper = Math.Max(bgTone1, bgTone2);
                double lower = Math.Min(bgTone1, bgTone2);

                if (Contrast.RatioOfTones(upper, answer) >= desiredRatio && Contrast.RatioOfTones(lower, answer) >= desiredRatio)
                    return answer;

                double lightOption = Contrast.Lighter(upper, desiredRatio);
                double darkOption = Contrast.Darker(lower, desiredRatio);

                var availables = new List<double>();
                if (lightOption != -1)
                    availables.Add(lightOption);
                if (darkOption != -1)
                    availables.Add(darkOption);

                bool prefersLight = TonePrefersLightForeground(bgTone1) || TonePrefersLightForeground(bgTone2);

                if (prefersLight)
                    return lightOption == -1 ? 100 : lightOption;

                if (availables.Count == 1)
                    return availables[0];

                return darkOption == -1 ? 0 : darkOption;
            }

            return answer;
        }
    }

    public static DynamicColor CreateFromPalette(string name,
        Func<DynamicScheme, TonalPalette> palette,
        Func<DynamicScheme, double> tone) {
        return new DynamicColor(name, palette, tone, false);
    }

    public static double ForegroundTone(double bgTone, double ratio) {
        double lighterTone = Contrast.LighterUnsafe(bgTone, ratio);
        double darkerTone = Contrast.DarkerUnsafe(bgTone, ratio);
        double lighterRatio = Contrast.RatioOfTones(lighterTone, bgTone);
        double darkerRatio = Contrast.RatioOfTones(darkerTone, bgTone);

        bool preferLighter = TonePrefersLightForeground(bgTone);

        if (preferLighter) {
            bool negligibleDifference = Math.Abs(lighterRatio - darkerRatio) < 0.1 && lighterRatio < ratio && darkerRatio < ratio;

            if (lighterRatio >= ratio || lighterRatio >= darkerRatio || negligibleDifference)
                return lighterTone;
            else
                return darkerTone;
        } else {
            return darkerRatio >= ratio || darkerRatio >= lighterRatio ? darkerTone : lighterTone;
        }
    }

    public static double EnableLightForeground(double tone) {
        if (TonePrefersLightForeground(tone) && !ToneAllowsLightForeground(tone))
            return 49.0;

        return tone;
    }

    public static bool TonePrefersLightForeground(double tone) {
        return Math.Round(tone) < 60;
    }

    public static bool ToneAllowsLightForeground(double tone) {
        return Math.Round(tone) <= 49;
    }
}

public sealed class ToneDeltaPair {
    public DynamicColor RoleA { get; }
    public DynamicColor RoleB { get; }

    public double Delta { get; }
    public bool StayTogether { get; }
    public TonePolarity Polarity { get; }

    public ToneDeltaPair(DynamicColor roleA, DynamicColor roleB, double delta, TonePolarity polarity, bool stayTogether) {
        RoleA = roleA;
        RoleB = roleB;
        Delta = delta;
        Polarity = polarity;
        StayTogether = stayTogether;
    }
}