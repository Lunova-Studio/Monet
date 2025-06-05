namespace Monet.Shared.Interfaces;

public interface IColorValueScheme {
    uint PrimaryColorValue { get; }
    uint OnPrimaryColorValue { get; }
    uint PrimaryContainerColorValue { get; }
    uint OnPrimaryContainerColorValue { get; }
    uint InversePrimaryColorValue { get; }

    uint SecondaryColorValue { get; }
    uint OnSecondaryColorValue { get; }
    uint SecondaryContainerColorValue { get; }
    uint OnSecondaryContainerColorValue { get; }

    uint TertiaryColorValue { get; }
    uint OnTertiaryColorValue { get; }
    uint TertiaryContainerColorValue { get; }
    uint OnTertiaryContainerColorValue { get; }

    uint ErrorColorValue { get; }
    uint OnErrorColorValue { get; }
    uint ErrorContainerColorValue { get; }
    uint OnErrorContainerColorValue { get; }

    uint SurfaceColorValue { get; }
    uint OnSurfaceColorValue { get; }
    uint SurfaceBrightColorValue { get; }
    uint SurfaceVariantColorValue { get; }
    uint SurfaceContainerColorValue { get; }
    uint SurfaceContainerLowColorValue { get; }
    uint SurfaceContainerHighColorValue { get; }
    uint OnSurfaceVariantColorValue { get; }
    uint InverseSurfaceColorValue { get; }
    uint InverseOnSurfaceColorValue { get; }

    uint OutlineColorValue { get; }
    uint OutlineVariantColorValue { get; }

    uint BackgroundColorValue { get; }
    uint OnBackgroundColorValue { get; }

    uint ScrimColorValue { get; }
    uint ShadowColorValue { get; }
}
