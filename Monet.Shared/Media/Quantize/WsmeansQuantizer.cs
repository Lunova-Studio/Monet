using Monet.Shared.Interfaces;
using Monet.Shared.Media.Provider;

namespace Monet.Shared.Media.Quantize;

public static class WsmeansQuantizer {
    private sealed class Distance : IComparable<Distance> {
        public int Index;
        public double Value;

        public Distance() {
            Index = -1;
            Value = -1.0;
        }

        public int CompareTo(Distance other) {
            if (other is null) return 1;
            return Value.CompareTo(other.Value);
        }
    }

    private const int MaxIterations = 10;
    private const double MinMovementDistance = 3.0;

    /// <summary>
    /// Reduce the number of colors needed to represent the input, minimizing the difference between
    /// the original image and the recolored image.
    ///
    /// <param name="inputPixels">Colors in ARGB format.</param>
    /// <param name="startingClusters">
    /// Defines the initial state of the quantizer. Passing an empty array is fine, the implementation
    /// will create its own initial state that leads to reproducible results for the same inputs.
    /// Passing an array that is the result of Wu quantization leads to higher quality results.
    /// </param>
    /// <param name="maxColors">The number of colors to divide the image into. A lower number of colors may be returned.</param>
    /// <returns>
    /// Dictionary with keys of colors in ARGB format, values of how many of the input pixels belong
    /// to the color.
    /// </returns>
    /// </summary>
    public static Dictionary<uint, int> Quantize(
        uint[] inputPixels,
        uint[] startingClusters,
        int maxColors) {
        // Uses a seeded random number generator to ensure consistent results.
        var random = new Random(0x42688);

        var pixels = new uint[inputPixels.Length];
        var points = new double[inputPixels.Length][];
        var pointProvider = new PointProviderLab();
        var pixelToCount = new Dictionary<uint, int>();

        int pointCount = 0;
        for (int i = 0; i < inputPixels.Length; i++) {
            uint inputPixel = inputPixels[i];

            if (!pixelToCount.TryGetValue(inputPixel, out int pixelCount)) {
                points[pointCount] = pointProvider.FromUInt(inputPixel);
                pixels[pointCount] = inputPixel;
                pointCount++;

                pixelToCount[inputPixel] = 1;
            } else {
                pixelToCount[inputPixel] = pixelCount + 1;
            }
        }

        var counts = new int[pointCount];
        for (int i = 0; i < pointCount; i++) {
            uint pixel = pixels[i];
            counts[i] = pixelToCount[pixel];
        }

        int clusterCount = Math.Min(maxColors, pointCount);
        if (startingClusters.Length != 0) {
            clusterCount = Math.Min(clusterCount, startingClusters.Length);
        }

        var clusters = new double[clusterCount][];
        int clustersCreated = 0;
        for (int i = 0; i < startingClusters.Length && i < clusterCount; i++) {
            clusters[i] = pointProvider.FromUInt(startingClusters[i]);
            clustersCreated++;
        }

        int additionalClustersNeeded = clusterCount - clustersCreated;
        if (additionalClustersNeeded > 0) {
            // Original Java code leaves this empty: no extra initialization beyond startingClusters.
            for (int i = 0; i < additionalClustersNeeded; i++) { }
        }

        var clusterIndices = new int[pointCount];
        for (int i = 0; i < pointCount; i++) {
            clusterIndices[i] = random.Next(clusterCount);
        }

        var indexMatrix = new int[clusterCount][];
        for (int i = 0; i < clusterCount; i++) {
            indexMatrix[i] = new int[clusterCount];
        }

        var distanceToIndexMatrix = new Distance[clusterCount][];
        for (int i = 0; i < clusterCount; i++) {
            distanceToIndexMatrix[i] = new Distance[clusterCount];
            for (int j = 0; j < clusterCount; j++) {
                distanceToIndexMatrix[i][j] = new Distance();
            }
        }

        var pixelCountSums = new int[clusterCount];

        for (int iteration = 0; iteration < MaxIterations; iteration++) {
            // Precompute inter-cluster distances and sort neighbors for triangle inequality pruning.
            for (int i = 0; i < clusterCount; i++) {
                for (int j = i + 1; j < clusterCount; j++) {
                    double distance = pointProvider.Distance(clusters[i], clusters[j]);

                    distanceToIndexMatrix[j][i].Value = distance;
                    distanceToIndexMatrix[j][i].Index = i;

                    distanceToIndexMatrix[i][j].Value = distance;
                    distanceToIndexMatrix[i][j].Index = j;
                }

                Array.Sort(distanceToIndexMatrix[i]);

                for (int j = 0; j < clusterCount; j++) {
                    indexMatrix[i][j] = distanceToIndexMatrix[i][j].Index;
                }
            }

            int pointsMoved = 0;

            for (int i = 0; i < pointCount; i++) {
                double[] point = points[i];
                int previousClusterIndex = clusterIndices[i];
                double[] previousCluster = clusters[previousClusterIndex];
                double previousDistance = pointProvider.Distance(point, previousCluster);

                double minimumDistance = previousDistance;
                int newClusterIndex = -1;

                for (int j = 0; j < clusterCount; j++) {
                    if (distanceToIndexMatrix[previousClusterIndex][j].Value >= 4.0 * previousDistance) {
                        continue;
                    }

                    double distance = pointProvider.Distance(point, clusters[j]);
                    if (distance < minimumDistance) {
                        minimumDistance = distance;
                        newClusterIndex = j;
                    }
                }

                if (newClusterIndex != -1) {
                    double distanceChange =
                        Math.Abs(Math.Sqrt(minimumDistance) - Math.Sqrt(previousDistance));

                    if (distanceChange > MinMovementDistance) {
                        pointsMoved++;
                        clusterIndices[i] = newClusterIndex;
                    }
                }
            }

            if (pointsMoved == 0 && iteration != 0) {
                break;
            }

            var componentASums = new double[clusterCount];
            var componentBSums = new double[clusterCount];
            var componentCSums = new double[clusterCount];
            Array.Fill(pixelCountSums, 0);

            for (int i = 0; i < pointCount; i++) {
                int clusterIndex = clusterIndices[i];
                double[] point = points[i];
                int count = counts[i];

                pixelCountSums[clusterIndex] += count;
                componentASums[clusterIndex] += point[0] * count;
                componentBSums[clusterIndex] += point[1] * count;
                componentCSums[clusterIndex] += point[2] * count;
            }

            for (int i = 0; i < clusterCount; i++) {
                int count = pixelCountSums[i];
                if (count == 0) {
                    clusters[i] = [0.0, 0.0, 0.0];
                    continue;
                }

                double a = componentASums[i] / count;
                double b = componentBSums[i] / count;
                double c = componentCSums[i] / count;

                clusters[i][0] = a;
                clusters[i][1] = b;
                clusters[i][2] = c;
            }
        }

        var argbToPopulation = new Dictionary<uint, int>();
        for (int i = 0; i < clusterCount; i++) {
            int count = pixelCountSums[i];
            if (count == 0) {
                continue;
            }

            uint possibleNewCluster = pointProvider.ToUInt(clusters[i]);
            if (argbToPopulation.ContainsKey(possibleNewCluster)) {
                continue;
            }

            argbToPopulation[possibleNewCluster] = count;
        }

        return argbToPopulation;
    }
}