using Monet.Shared.Utilities;
using Monet.Shared.Utilities.Calculator;

namespace Monet.Shared.Media.ColorSpace;

/// <summary>
/// 谷歌的 HCT 色彩空间标准
/// </summary>
public partial struct Hct {
    private uint argb;

    /// <summary>
    /// Hue（色相）
    /// </summary>
    public double H { get; set; } = -1;

    /// <summary>
    /// Chroma（色度）
    /// </summary>
    public double C { get; set; } = -1;

    /// <summary>
    /// Tone（亮度）
    /// </summary>
    public double T { get; set; } = -1;

    public Hct(uint argb) {
        SetInternalState(argb);
    }

    public Hct SetHue(double h) {
        SetInternalState(HctCalculator.SolveToUint(h, C, T));
        return this;
    }

    public Hct SetChroma(double c) {
        SetInternalState(HctCalculator.SolveToUint(H, c, T));
        return this;
    }

    public Hct SetTone(double t) {
        SetInternalState(HctCalculator.SolveToUint(H, C, t));
        return this;
    }

    public readonly uint ToUInt32() {
        return argb;
    }

    public static Hct Parse(double h, double c, double t) {
        var argb = HctCalculator.SolveToUint(h, c, t);
        return new(argb);
    }

    public readonly Hct Parse(ViewingConditions vc) {
        // 1. Use CAM16 to find XYZ coordinates of color in specified VC.
        var cam16 = Cam16.Parse(argb);
        double[] viewedInVc = cam16.XyzInViewingConditions(vc, default!);

        //2. Create CAM16 of those XYZ coordinates in default VC.
        var recastInVc = Cam16.ParseByXyzByViewingConditions(viewedInVc[0],
            viewedInVc[1], viewedInVc[2], ViewingConditions.Default);

        // 3. Create HCT from:
        // - CAM16 using default VC with XYZ coordinates in specified VC.
        // - L* converted from Y in XYZ coordinates in specified VC.
        return Parse(recastInVc.H, recastInVc.C, ColorUtil.LstarFromY(viewedInVc[1]));
    }

    private void SetInternalState(uint argb) {
        this.argb = argb;
        var cam = Cam16.Parse(argb);

        H = cam.H;
        C = cam.C;
        T = ColorUtil.LStarFromArgb(argb);
    }

    internal readonly bool IsDefault() {
        return H is -1 || C is -1 || T is -1;
    }

    public static implicit operator Hct(uint argb) {
        return new(argb);
    }
}