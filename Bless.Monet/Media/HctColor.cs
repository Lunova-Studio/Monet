using Avalonia.Media;
using Bless.Monet.Calculator;
using Bless.Monet.Utilities;

namespace Bless.Monet.Media;

///// <summary>
///// 谷歌的 HCT 色彩空间标准
///// </summary>
//public struct HctColor {
//    private uint _argb;
//    private double _h;
//    private double _c;
//    private double _t;

//    /// <summary>
//    /// Hue（色相）
//    /// </summary>
//    public double H {
//        readonly get => _h;
//        set {
//            SetInternalState(HctCalculator.SolveToUint(value, C, T));
//        }
//    }

//    /// <summary>
//    /// Chroma（色度）
//    /// </summary>
//    public double C {
//        readonly get => _c;
//        set {
//            SetInternalState(HctCalculator.SolveToUint(H, value, T));
//        }
//    }

//    /// <summary>
//    /// Tone（亮度）
//    /// </summary>
//    public double T {
//        readonly get => _t;
//        set {
//            SetInternalState(HctCalculator.SolveToUint(H, C, value));
//        }
//    }

//    public HctColor(uint argb) {
//        SetInternalState(argb);
//    }

//    public readonly uint ToUInt32() {
//        return _argb;
//    }

//    public readonly Color ToColor() {
//        return Color.FromUInt32(_argb);
//    }

//    public static HctColor Parse(double h, double c, double t) {
//        var argb = HctCalculator.SolveToUint(h, c, t);
//        return new(argb);
//    }

//    public readonly HctColor Parse(ViewingConditions vc) {
//        // 1. Use CAM16 to find XYZ coordinates of color in specified VC.
//        var cam16 = Cam16.Parse(_argb);
//        double[] viewedInVc = cam16.XyzInViewingConditions(vc, default!);

//        //2. Create CAM16 of those XYZ coordinates in default VC.
//        var recastInVc = Cam16.ParseByXyzByViewingConditions(viewedInVc[0],
//            viewedInVc[1], viewedInVc[2], ViewingConditions.Default);

//        // 3. Create HCT from:
//        // - CAM16 using default VC with XYZ coordinates in specified VC.
//        // - L* converted from Y in XYZ coordinates in specified VC.
//        return Parse(recastInVc.H, recastInVc.C, ColorUtil.LstarFromY(viewedInVc[1]));
//    }

//    private void SetInternalState(uint argb) {
//        this._argb = argb;
//        var cam = Cam16.Parse(argb);

//        _h = cam.H;
//        _c = cam.C;
//        _t = ColorUtil.LStarFromArgb(argb);
//    }

//    public static implicit operator HctColor(uint argb) {
//        return new(argb);
//    }

//    public static implicit operator HctColor(Color color) {
//        return new(color.ToUInt32());
//    }
//}

/// <summary>
/// 谷歌的 HCT 色彩空间标准
/// </summary>
public struct HctColor {
    private uint argb;

    /// <summary>
    /// Hue（色相）
    /// </summary>
    public double H { get; set; }

    /// <summary>
    /// Chroma（色度）
    /// </summary>
    public double C { get; set; }

    /// <summary>
    /// Tone（亮度）
    /// </summary>
    public double T { get; set; }

    public HctColor(uint argb) {
        SetInternalState(argb);
    }

    public HctColor SetHue(double h) {
        SetInternalState(HctCalculator.SolveToUint(h, C, T));
        return this;
    }

    public HctColor SetChroma(double c) {
        SetInternalState(HctCalculator.SolveToUint(H, c, T));
        return this;
    }

    public HctColor SetTone(double t) {
        SetInternalState(HctCalculator.SolveToUint(H, C, t));
        return this;
    }

    public readonly uint ToUInt32() {
        return argb;
    }

    public static HctColor Parse(double h, double c, double t) {
        var argb = HctCalculator.SolveToUint(h, c, t);
        return new(argb);
    }

    public readonly HctColor Parse(ViewingConditions vc) {
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

    public static implicit operator Color(HctColor hct) {
        return Color.FromUInt32(hct.ToUInt32());
    }

    public static implicit operator HctColor(Color c) {
        return new HctColor(c.ToUInt32());
    }
}