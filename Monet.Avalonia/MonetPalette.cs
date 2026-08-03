using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;
using Monet.Shared.Enums;
using Monet.Shared.Interfaces;
using Monet.Shared.Utilities;

namespace Monet.Avalonia;

public sealed class MonetPalette : Styles {
    private readonly ResourceDictionary _lightResources = new();
    private readonly ResourceDictionary _darkResources = new();

    public Variant Variant { get; set; } = Variant.TonalSpot;
    
    public IColorValueScheme LightScheme { get; private set; } = null!;
    public IColorValueScheme DarkScheme { get; private set; } = null!;
    
    public MonetPalette() {
        Resources.ThemeDictionaries.TryAdd(ThemeVariant.Dark, _darkResources);
        Resources.ThemeDictionaries.TryAdd(ThemeVariant.Light, _lightResources);

        Build(Variant, Color.FromUInt32(ColorUtil.GOOGLE_BLUE));
    }
    
    public void Build(Variant variant, Color seedColor, double level = 0) {
        Variant = variant;
        LightScheme =
            MonetSchemeFactory.Create(variant, seedColor, false, level);
        
        DarkScheme =
            MonetSchemeFactory.Create(variant, seedColor, true, level);
        Apply(_darkResources, DarkScheme);
        Apply(_lightResources, LightScheme);
    }

    private static void Apply(ResourceDictionary dictionary, IColorValueScheme scheme) {
        dictionary.Clear();
        
        foreach(var item in scheme.Resources) {
            var color = Color.FromUInt32(item.Value);
            
            dictionary[item.Key] = color;
            if(item.Key.EndsWith("Color"))
                dictionary[item.Key.Replace("Color", "Brush")] =
                    new SolidColorBrush(color);
        }
    }
}