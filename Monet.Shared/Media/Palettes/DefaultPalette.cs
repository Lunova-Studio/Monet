using Monet.Shared.Media.ColorSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monet.Shared.Media.Palettes;

/// <summary>
/// An intermediate concept between the key color for a UI theme, and a full color scheme. 
/// 5 sets of tones are generated, all except one use the same hue as the key color, and all vary in chroma.
/// </summary>
public sealed class DefaultPalette {
    internal DefaultPalette(uint argb, bool isContent) {
        Hct hct = argb;

        if (isContent) {

        }
    }

    public static DefaultPalette CreateFromArgb(uint argb) {

    }
}
