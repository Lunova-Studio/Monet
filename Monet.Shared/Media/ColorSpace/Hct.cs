using Monet.Shared.Utilities;
using Monet.Shared.Utilities.Calculator;

namespace Monet.Shared.Media.ColorSpace;

public partial struct Hct {
    private uint argb;

    public double H { get; set; } = -1;
    public double C { get; set; } = -1;
    public double T { get; set; } = -1;

    public Hct(uint argb) {
        this = default;
        SetInternalState(argb);
    }

    //public Hct SetHue(double h) {
    //    SetInternalState(HctCalculator.SolveToUint(h, C, T));
    //    return this;
    //}

    //public Hct SetChroma(double c) {
    //    SetInternalState(HctCalculator.SolveToUint(H, c, T));
    //    return this;
    //}

    //public Hct SetTone(double t) {
    //    SetInternalState(HctCalculator.SolveToUint(H, C, t));
    //    return this;
    //}

    public readonly Hct Parse(ViewingConditions vc) {
        // 1. CAM16 in default VC
        var cam16 = Cam16.Parse(argb);

        // 2. XYZ in target VC (zero allocation)
        double[] xyz = cam16.XyzInViewingConditions(vc, null);

        // 3. Recast XYZ back to CAM16 in default VC
        var recast = Cam16.ParseByXyzByViewingConditions(
            xyz[0], xyz[1], xyz[2], ViewingConditions.Default);

        // 4. Build HCT
        return Parse(recast.H, recast.C, ColorUtil.LstarFromY(xyz[1]));
    }

    public readonly uint ToUInt32() => argb;
    public static Hct Parse(double h, double c, double t) => new(HctCalculator.SolveToUint(h, c, t));
    internal readonly bool IsDefault() => H < 0 || C < 0 || T < 0;

    private void SetInternalState(uint argb) {
        this.argb = argb;
        var cam = Cam16.Parse(argb);

        H = cam.H;
        C = cam.C;
        T = ColorUtil.LStarFromArgb(argb);
    }

    public static implicit operator uint(Hct hct) => hct.argb;
    public static implicit operator Hct(uint argb) => new(argb);
}