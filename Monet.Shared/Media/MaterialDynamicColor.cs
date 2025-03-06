using Monet.Shared.Enums;
using Monet.Shared.Media.ColorSpace;
using Monet.Shared.Media.Scheme.Dynamic;
using Monet.Shared.Utilities;
using System.Diagnostics;

namespace Monet.Shared.Media;

/// <summary>
/// Named colors, otherwise known as tokens, or roles, in the Material Design system
/// </summary>
public sealed class MaterialDynamicColor {
    private readonly bool _isExtendedFidelity;

    #region DynamicColors

    public DynamicColor PrimaryPaletteKeyColor =>
        DynamicColor.CreateFromPalette(nameof(PrimaryPaletteKeyColor),
            x => x.PrimaryPalette,
                x1 => x1.PrimaryPalette.KeyColor.T);

    public DynamicColor SecondaryPaletteKeyColor =>
        DynamicColor.CreateFromPalette(nameof(SecondaryPaletteKeyColor),
            x => x.SecondaryPalette,
                x1 => x1.SecondaryPalette.KeyColor.T);

    public DynamicColor TertiaryPaletteKeyColor =>
        DynamicColor.CreateFromPalette(nameof(TertiaryPaletteKeyColor),
            x => x.TertiaryPalette,
                x1 => x1.TertiaryPalette.KeyColor.T);

    public DynamicColor NeutralPaletteKeyColor =>
        DynamicColor.CreateFromPalette(nameof(NeutralPaletteKeyColor),
            x => x.NeutralPalette,
                x1 => x1.NeutralPalette.KeyColor.T);

    public DynamicColor NeutralVariantPaletteKeyColor =>
        DynamicColor.CreateFromPalette(nameof(NeutralVariantPaletteKeyColor),
            x => x.NeutralVariantPalette,
                x1 => x1.NeutralVariantPalette.KeyColor.T);

    public DynamicColor Background =>
        new(nameof(Background),
            x => x.NeutralPalette,
                x1 => x1.IsDark ? 6.0 : 98.0,
                true);

    public DynamicColor OnBackground =>
        new(nameof(OnBackground),
            x => x.NeutralPalette,
                x1 => x1.IsDark ? 90.0 : 10.0,
                false,
                x2 => Background, 
                contrastCurve: new(3.0, 3.0, 4.5, 7.0));

    public DynamicColor Surface =>
        new(nameof(Surface),
            x => x.NeutralPalette,
                x1 => x1.IsDark ? 6.0 : 98.0,
                true);

    public DynamicColor SurfaceDim =>
        new(nameof(SurfaceDim),
            x => x.NeutralPalette,
                x1 => x1.IsDark ? 6.0 : new ContrastCurve(87.0, 87.0, 80.0, 75.0).GetContrastRatios(x1.ContrastLevel),
                true);

    public DynamicColor SurfaceBright =>
        new(nameof(SurfaceBright),
            x => x.NeutralPalette,
                x1 => x1.IsDark ? new ContrastCurve(24.0, 24.0, 29.0, 34.0).GetContrastRatios(x1.ContrastLevel) : 98.0,
                true);

    public DynamicColor SurfaceContainerLowest =>
        new(nameof(SurfaceContainerLowest),
            x => x.NeutralPalette,
                x1 => x1.IsDark ? new ContrastCurve(4.0, 4.0, 2.0, 0.0).GetContrastRatios(x1.ContrastLevel) : 100.0,
                true);

    public DynamicColor SurfaceContainerLow =>
        new(nameof(SurfaceContainerLow),
            x => x.NeutralPalette,
            x1 => x1.IsDark 
                ? new ContrastCurve(10.0, 10.0, 11.0, 12.0).GetContrastRatios(x1.ContrastLevel)
                : new ContrastCurve(96.0, 96.0, 96.0, 95.0).GetContrastRatios(x1.ContrastLevel),
            true);

    public DynamicColor SurfaceContainer =>
        new(nameof(SurfaceContainer),
            x => x.NeutralPalette,
            x1 => x1.IsDark 
                ? new ContrastCurve(12.0, 12.0, 16.0, 20.0).GetContrastRatios(x1.ContrastLevel)
                : new ContrastCurve(94.0, 94.0, 92.0, 90.0).GetContrastRatios(x1.ContrastLevel),
            true);

    public DynamicColor SurfaceContainerHigh =>
    new(nameof(SurfaceContainerHigh),
        x => x.NeutralPalette,
        x1 => x1.IsDark ? new ContrastCurve(17.0, 17.0, 21.0, 25.0).GetContrastRatios(x1.ContrastLevel)
                        : new ContrastCurve(92.0, 92.0, 88.0, 85.0).GetContrastRatios(x1.ContrastLevel),
        true);

    public DynamicColor SurfaceContainerHighest =>
        new(nameof(SurfaceContainerHighest),
            x => x.NeutralPalette,
            x1 => x1.IsDark 
                ? new ContrastCurve(22.0, 22.0, 26.0, 30.0).GetContrastRatios(x1.ContrastLevel)
                : new ContrastCurve(90.0, 90.0, 84.0, 80.0).GetContrastRatios(x1.ContrastLevel),
            true);

    public DynamicColor OnSurface =>
        new(nameof(OnSurface),
            x => x.NeutralPalette,
            x1 => x1.IsDark ? 90.0 : 10.0,
            false,
            GetHighestSurface,
            contrastCurve: new ContrastCurve(4.5, 7.0, 11.0, 21.0));

    public DynamicColor SurfaceVariant =>
        new(nameof(SurfaceVariant),
            x => x.NeutralVariantPalette,
            x1 => x1.IsDark ? 30.0 : 90.0,
            true);

    public DynamicColor OnSurfaceVariant =>
        new(nameof(OnSurfaceVariant),
            x => x.NeutralVariantPalette,
            x1 => x1.IsDark ? 80.0 : 30.0,
            false,
            GetHighestSurface,
            contrastCurve: new ContrastCurve(3.0, 4.5, 7.0, 11.0));

    public DynamicColor InverseSurface =>
        new(nameof(InverseSurface),
            x => x.NeutralPalette,
            x1 => x1.IsDark ? 90.0 : 20.0,
            false);

    public DynamicColor InverseOnSurface =>
        new(nameof(InverseOnSurface),
            x => x.NeutralPalette,
            x1 => x1.IsDark ? 20.0 : 95.0,
            false,
            x2 => InverseSurface,
            contrastCurve: new ContrastCurve(4.5, 7.0, 11.0, 21.0));

    public DynamicColor Outline =>
        new(nameof(Outline),
            x => x.NeutralVariantPalette,
            x1 => x1.IsDark ? 60.0 : 50.0,
            false,
            GetHighestSurface,
            contrastCurve: new ContrastCurve(1.5, 3.0, 4.5, 7.0));

    public DynamicColor OutlineVariant =>
        new(nameof(OutlineVariant),
            x => x.NeutralVariantPalette,
            x1 => x1.IsDark ? 30.0 : 80.0,
            false,
            GetHighestSurface,
            contrastCurve: new ContrastCurve(1.0, 1.0, 3.0, 4.5));

    public DynamicColor Shadow =>
        new(nameof(Shadow),
            x => x.NeutralPalette,
            x1 => 0.0,
            false);

    public DynamicColor Scrim =>
        new(nameof(Scrim),
            x => x.NeutralPalette,
            x1 => 0.0,
            false);

    public DynamicColor SurfaceTint =>
        new(nameof(SurfaceTint),
            x => x.PrimaryPalette,
            x1 => x1.IsDark ? 80.0 : 40.0,
            true);

    public DynamicColor Primary =>
        new(nameof(Primary),
            x => x.PrimaryPalette,
            x1 => {
                if (GetIsMonochrome(x1))
                    return x1.IsDark ? 100.0 : 0.0;

                return x1.IsDark ? 80.0 : 40.0;
            },
            true,
            x2 => GetHighestSurface(x2),
            contrastCurve: new ContrastCurve(3.0, 4.5, 7.0, 7.0),
            toneDeltaPair: x3 => new ToneDeltaPair(PrimaryContainer, Primary, 10.0, TonePolarity.Nearer, false));

    public DynamicColor OnPrimary =>
        new(nameof(OnPrimary),
            x => x.PrimaryPalette,
            x1 => {
                if (GetIsMonochrome(x1))
                    return x1.IsDark ? 10.0 : 90.0;

                return x1.IsDark ? 20.0 : 100.0;
            },
            false,
            x2 => Primary,
            contrastCurve: new ContrastCurve(4.5, 7.0, 11.0, 21.0));

    public DynamicColor PrimaryContainer =>
        new(nameof(PrimaryContainer),
            x => x.PrimaryPalette,
            x1 => {
                if (GetIsFidelity(x1)) {
                    return x1.SourceColorHct.T;
                }
                if (GetIsMonochrome(x1))
                    return x1.IsDark ? 85.0 : 25.0;

                return x1.IsDark ? 30.0 : 90.0;
            },
            true,
            GetHighestSurface,
            contrastCurve: new ContrastCurve(1.0, 1.0, 3.0, 4.5),
            toneDeltaPair: x3 => new ToneDeltaPair(PrimaryContainer, Primary, 10.0, TonePolarity.Nearer, false));

    public DynamicColor OnPrimaryContainer =>
        new(nameof(OnPrimaryContainer),
            x => x.PrimaryPalette,
            x1 => {
                if (GetIsFidelity(x1))
                    return DynamicColor.ForegroundTone(PrimaryContainer.Tone(x1), 4.5);

                if (GetIsMonochrome(x1))
                    return x1.IsDark ? 0.0 : 100.0;

                return x1.IsDark ? 90.0 : 30.0;
            },
            false,
            x2 => PrimaryContainer,
            contrastCurve: new ContrastCurve(3.0, 4.5, 7.0, 11.0));

    public DynamicColor InversePrimary =>
        new(nameof(InversePrimary),
            x => x.PrimaryPalette,
            x1 => x1.IsDark ? 40.0 : 80.0,
            false,
            x2 => InverseSurface,
            contrastCurve: new ContrastCurve(3.0, 4.5, 7.0, 7.0));

    public DynamicColor Secondary =>
        new(nameof(Secondary),
            x => x.SecondaryPalette,
            x1 => x1.IsDark ? 80.0 : 40.0,
            true,
            GetHighestSurface,
            contrastCurve: new ContrastCurve(3.0, 4.5, 7.0, 7.0),
            toneDeltaPair: x3 => new ToneDeltaPair(SecondaryContainer, Secondary, 10.0, TonePolarity.Nearer, false));

    public DynamicColor OnSecondary =>
        new(nameof(OnSecondary),
            x => x.SecondaryPalette,
            x1 => {
                if (GetIsMonochrome(x1))
                    return x1.IsDark ? 10.0 : 100.0;
                else 
                    return x1.IsDark ? 20.0 : 100.0;
            },
            false,
            x2 => Secondary,
            contrastCurve: new ContrastCurve(4.5, 7.0, 11.0, 21.0));

    public DynamicColor SecondaryContainer =>
        new(nameof(SecondaryContainer),
            x => x.SecondaryPalette,
            x1 => {
                double initialTone = x1.IsDark ? 30.0 : 90.0;
                if (GetIsMonochrome(x1))
                    return x1.IsDark ? 30.0 : 85.0;

                if (!GetIsFidelity(x1))
                    return initialTone;

                return FindDesiredChromaByTone(
                    x1.SecondaryPalette.H, x1.SecondaryPalette.C, initialTone, !x1.IsDark);
            },
            true,
            GetHighestSurface,
            contrastCurve: new ContrastCurve(1.0, 1.0, 3.0, 4.5),
            toneDeltaPair: x3 => new ToneDeltaPair(SecondaryContainer, Secondary, 10.0, TonePolarity.Nearer, false));

    public DynamicColor OnSecondaryContainer =>
        new(nameof(OnSecondaryContainer),
            x => x.SecondaryPalette,
            x1 => {
                if (GetIsMonochrome(x1))
                    return x1.IsDark ? 90.0 : 10.0;
                if (!GetIsFidelity(x1))
                    return x1.IsDark ? 90.0 : 30.0;

                return DynamicColor.ForegroundTone(SecondaryContainer.Tone(x1), 4.5);
            },
            false,
            x2 => SecondaryContainer,
            contrastCurve: new ContrastCurve(3.0, 4.5, 7.0, 11.0));

    public DynamicColor Tertiary =>
        new(nameof(Tertiary),
            x => x.TertiaryPalette,
            x1 => {
                if (GetIsMonochrome(x1)) 
                    return x1.IsDark ? 90.0 : 25.0;
                
                return x1.IsDark ? 80.0 : 40.0;
            },
            true,
            GetHighestSurface,
            contrastCurve: new ContrastCurve(3.0, 4.5, 7.0, 7.0),
            toneDeltaPair: x3 => new ToneDeltaPair(TertiaryContainer, Tertiary, 10.0, TonePolarity.Nearer, false));

    public DynamicColor OnTertiary =>
        new(nameof(OnTertiary),
            x => x.TertiaryPalette,
            x1 => {
                if (GetIsMonochrome(x1))
                    return x1.IsDark ? 10.0 : 90.0;

                return x1.IsDark ? 20.0 : 100.0;
            },
            false,
            x2 => Tertiary,
            contrastCurve: new ContrastCurve(4.5, 7.0, 11.0, 21.0));

    public DynamicColor TertiaryContainer =>
        new(nameof(TertiaryContainer),
            x => x.TertiaryPalette,
            x1 => {
                if (GetIsMonochrome(x1))
                    return x1.IsDark ? 60.0 : 49.0;

                if (!GetIsFidelity(x1))
                    return x1.IsDark ? 30.0 : 90.0;

                Hct proposedHct = x1.TertiaryPalette.GetHct(x1.SourceColorHct.T);
                return ColorUtil.Fix(proposedHct).T;
            },
            true,
            GetHighestSurface,
            contrastCurve: new ContrastCurve(1.0, 1.0, 3.0, 4.5),
            toneDeltaPair: x3 => new ToneDeltaPair(TertiaryContainer, Tertiary, 10.0, TonePolarity.Nearer, false));

    public DynamicColor OnTertiaryContainer =>
        new(nameof(OnTertiaryContainer),
            x => x.TertiaryPalette,
            x1 => {
                if (GetIsMonochrome(x1))
                    return x1.IsDark ? 0.0 : 100.0;

                if (!GetIsFidelity(x1))
                    return x1.IsDark ? 90.0 : 30.0;

                return DynamicColor.ForegroundTone(TertiaryContainer.Tone(x1), 4.5);
            },
            false,
            x2 => TertiaryContainer,
            contrastCurve: new ContrastCurve(3.0, 4.5, 7.0, 11.0));

    public DynamicColor Error =>
        new(nameof(Error),
            x => x.ErrorPalette,
            x1 => x1.IsDark ? 80.0 : 40.0,
            true,
            GetHighestSurface,
            contrastCurve: new ContrastCurve(3.0, 4.5, 7.0, 7.0),
            toneDeltaPair: x3 => new ToneDeltaPair(ErrorContainer, Error, 10.0, TonePolarity.Nearer, false));

    public DynamicColor OnError =>
        new(nameof(OnError),
            x => x.ErrorPalette,
            x1 => x1.IsDark ? 20.0 : 100.0,
            false,
            x2 => Error,
            contrastCurve: new ContrastCurve(4.5, 7.0, 11.0, 21.0));

    public DynamicColor ErrorContainer =>
        new(nameof(ErrorContainer),
            x => x.ErrorPalette,
            x1 => x1.IsDark ? 30.0 : 90.0,
            true,
            GetHighestSurface,
            contrastCurve: new ContrastCurve(1.0, 1.0, 3.0, 4.5),
            toneDeltaPair: x3 => new ToneDeltaPair(ErrorContainer, Error, 10.0, TonePolarity.Nearer, false));

    public DynamicColor OnErrorContainer =>
        new(nameof(OnErrorContainer),
            x => x.ErrorPalette,
            x1 => {
                if (GetIsMonochrome(x1))
                    return x1.IsDark ? 90.0 : 10.0;

                return x1.IsDark ? 90.0 : 30.0;
            },
            false,
            x2 => ErrorContainer,
            contrastCurve: new ContrastCurve(3.0, 4.5, 7.0, 11.0));

    public DynamicColor PrimaryFixed =>
    new(nameof(PrimaryFixed),
        x => x.PrimaryPalette,
        x1 => GetIsMonochrome(x1) ? 40.0 : 90.0,
        true,
        GetHighestSurface,
        contrastCurve: new ContrastCurve(1.0, 1.0, 3.0, 4.5),
        toneDeltaPair: x3 => new ToneDeltaPair(PrimaryFixed, PrimaryFixedDim, 10.0, TonePolarity.Lighter, true));

    public DynamicColor PrimaryFixedDim =>
        new(nameof(PrimaryFixedDim),
            x => x.PrimaryPalette,
            x1 => GetIsMonochrome(x1) ? 30.0 : 80.0,
            true,
            GetHighestSurface,
            contrastCurve: new ContrastCurve(1.0, 1.0, 3.0, 4.5),
            toneDeltaPair: x3 => new ToneDeltaPair(PrimaryFixed, PrimaryFixedDim, 10.0, TonePolarity.Lighter, true));

    public DynamicColor OnPrimaryFixed =>
        new(nameof(OnPrimaryFixed),
            x => x.PrimaryPalette,
            x1 => GetIsMonochrome(x1) ? 100.0 : 10.0,
            false,
            x2 => PrimaryFixedDim,
            x3 => PrimaryFixed,
            contrastCurve: new ContrastCurve(4.5, 7.0, 11.0, 21.0));

    public DynamicColor OnPrimaryFixedVariant =>
        new(nameof(OnPrimaryFixedVariant),
            x => x.PrimaryPalette,
            x1 => GetIsMonochrome(x1) ? 90.0 : 30.0,
            false,
            x2 => PrimaryFixedDim,
            x3 => PrimaryFixed,
            contrastCurve: new ContrastCurve(3.0, 4.5, 7.0, 11.0));

    public DynamicColor SecondaryFixed =>
        new(nameof(SecondaryFixed),
            x => x.SecondaryPalette,
            x1 => GetIsMonochrome(x1) ? 80.0 : 90.0,
            true,
            GetHighestSurface,
            contrastCurve: new ContrastCurve(1.0, 1.0, 3.0, 4.5),
            toneDeltaPair: x3 => new ToneDeltaPair(SecondaryFixed, SecondaryFixedDim, 10.0, TonePolarity.Lighter, true));

    public DynamicColor SecondaryFixedDim =>
        new(nameof(SecondaryFixedDim),
            x => x.SecondaryPalette,
            x1 => GetIsMonochrome(x1) ? 70.0 : 80.0,
            true,
            GetHighestSurface,
            contrastCurve: new ContrastCurve(1.0, 1.0, 3.0, 4.5),
            toneDeltaPair: x3 => new ToneDeltaPair(SecondaryFixed, SecondaryFixedDim, 10.0, TonePolarity.Lighter, true));

    public DynamicColor OnSecondaryFixed =>
        new(nameof(OnSecondaryFixed),
            x => x.SecondaryPalette,
            x1 => 10.0,
            false,
            x2 => SecondaryFixedDim,
            x3 => SecondaryFixed,
            contrastCurve: new ContrastCurve(4.5, 7.0, 11.0, 21.0));

    public DynamicColor OnSecondaryFixedVariant =>
        new(nameof(OnSecondaryFixedVariant),
            x => x.SecondaryPalette,
            x1 => GetIsMonochrome(x1) ? 25.0 : 30.0,
            false,
            x2 => SecondaryFixedDim,
            x3 => SecondaryFixed,
            contrastCurve: new ContrastCurve(3.0, 4.5, 7.0, 11.0));

    public DynamicColor TertiaryFixed =>
        new(nameof(TertiaryFixed),
            x => x.TertiaryPalette,
            x1 => GetIsMonochrome(x1) ? 40.0 : 90.0,
            true,
            GetHighestSurface,
            contrastCurve: new ContrastCurve(1.0, 1.0, 3.0, 4.5),
            toneDeltaPair: x3 => new ToneDeltaPair(TertiaryFixed, TertiaryFixedDim, 10.0, TonePolarity.Lighter, true));

    public DynamicColor TertiaryFixedDim =>
        new(nameof(TertiaryFixedDim),
            x => x.TertiaryPalette,
            x1 => GetIsMonochrome(x1) ? 30.0 : 80.0,
            true,
            GetHighestSurface,
            contrastCurve: new ContrastCurve(1.0, 1.0, 3.0, 4.5),
            toneDeltaPair: x3 => new ToneDeltaPair(TertiaryFixed, TertiaryFixedDim, 10.0, TonePolarity.Lighter, true));

    public DynamicColor OnTertiaryFixed =>
        new(nameof(OnTertiaryFixed),
            x => x.TertiaryPalette,
            x1 => GetIsMonochrome(x1) ? 100.0 : 10.0,
            false,
            x2 => TertiaryFixedDim,
            x3 => TertiaryFixed,
            contrastCurve: new ContrastCurve(4.5, 7.0, 11.0, 21.0));

    public DynamicColor OnTertiaryFixedVariant =>
        new(nameof(OnTertiaryFixedVariant),
            x => x.TertiaryPalette,
            x1 => GetIsMonochrome(x1) ? 90.0 : 30.0,
            false,
            x2 => TertiaryFixedDim,
            x3 => TertiaryFixed,
            contrastCurve: new ContrastCurve(3.0, 4.5, 7.0, 11.0));

    #endregion

    public MaterialDynamicColor(bool isExtendedFidelity = false) {
        _isExtendedFidelity = isExtendedFidelity;
    }

    public DynamicColor GetHighestSurface(DynamicScheme dynamicScheme) {
        return dynamicScheme.IsDark
            ? SurfaceBright
            : SurfaceDim;
    }

    private bool GetIsFidelity(DynamicScheme scheme) {
        if (_isExtendedFidelity &&
            scheme.Variant is not Variant.Monochrome and Variant.Neutral)
            return true;

        return scheme.Variant is Variant.Fidelity or Variant.Content;
    }

    private static bool GetIsMonochrome(DynamicScheme scheme) {
        return scheme.Variant is Enums.Variant.Monochrome;
    }

    private static double FindDesiredChromaByTone(double h, double c, double t, bool byDecreasingTone) {
        var answer = t;
        var closestToChroma = Hct.Parse(h, c, t);

        if (closestToChroma.C < c) {
            var chromaPeak = closestToChroma.C;

            while (closestToChroma.C < c) {
                answer += byDecreasingTone ? -1.0 : 1.0;
                var potentialSolution = Hct.Parse(h, c, answer);
                if (chromaPeak > potentialSolution.C)
                    break;

                if (Math.Abs(potentialSolution.C - c) < 0.4)
                    break;

                var potentialDelta = Math.Abs(potentialSolution.C - c);
                var currentDelta = Math.Abs(closestToChroma.C - c);

                if (potentialDelta < currentDelta)
                    closestToChroma = potentialSolution;

                chromaPeak = Math.Max(chromaPeak, potentialSolution.C);
            }
        }

        return answer;
    }
}