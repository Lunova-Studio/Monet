using Bless.Monet.Provider;

namespace Bless.Monet.Media.Quantize;

/// <summary>
/// An image quantizer that improves on the speed of a standard K-Means algorithm by implementing
/// several optimizations, including deduping identical pixels and a triangle inequality rule that
/// reduces the number of comparisons needed to identify which cluster a point should be moved to.
/// </summary>
/// <remarks>
/// Wsmeans stands for Weighted Square Means.
/// This algorithm was designed by M. Emre Celebi, and was found in their 2011 paper, Improving
/// the Performance of K-Means for Color Quantization. https://arxiv.org/abs/1101.0395
/// </remarks>
public class WsmeansQuantizer {
    private const int MAX_ITERATIONS = 10;
    private const double MIN_MOVEMENT_DISTANCE = 3.0;
    private static readonly Random _random = new(0x42688);

    public static Dictionary<uint, uint> Quantize(uint[] inputPixels, uint[] startingClusters, int maxColorCount) {
        var pixelToCount = new Dictionary<uint, uint>();
        var points = new List<double[]>();
        var pixels = new List<uint>();
        var pointProvider = new LabPointProvider();

        int pointCount = 0;
        for (int i = 0; i < inputPixels.Length; i++) {
            var inputPixel = inputPixels[i];

            if (!pixelToCount.TryGetValue(inputPixel, out var pixelCount)) {
                points.Add(pointProvider.FromUInt32(inputPixel));
                pixels.Add(inputPixel);
                pointCount++;

                pixelToCount[inputPixel] = 1;
            } else 
                pixelToCount[inputPixel]++;
        }

        var counts = new uint[pointCount];
        for (int i = 0; i < pointCount; i++) {
            var pixel = pixels[i];
            counts[i] = pixelToCount[pixel];
        }

        int clusterCount = Math.Min(maxColorCount, pointCount);
        if (startingClusters.Length != 0) {
            clusterCount = Math.Min(clusterCount, startingClusters.Length);
        }

        var clusters = new double[clusterCount][];
        int clustersCreated = 0;
        for (int i = 0; i < startingClusters.Length; i++) {
            clusters[i] = pointProvider.FromUInt32(startingClusters[i]);
            clustersCreated++;
        }

        int additionalClustersNeeded = clusterCount - clustersCreated;
        if (additionalClustersNeeded > 0) {
            for (int i = 0; i < additionalClustersNeeded; i++) { }
        }

        var clusterIndices = new int[pointCount];
        for (int i = 0; i < pointCount; i++) {
            clusterIndices[i] = _random.Next(clusterCount);
        }

        var indexMatrix = new int[clusterCount][];
        for (int i = 0; i < clusterCount; i++) {
            indexMatrix[i] = new int[clusterCount];
        }

        var distanceToIndexMatrix = new DistanceComparable[clusterCount][];
        for (int i = 0; i < clusterCount; i++) {
            distanceToIndexMatrix[i] = new DistanceComparable[clusterCount];
            for (int j = 0; j < clusterCount; j++) {
                distanceToIndexMatrix[i][j] = new();
            }
        }

        var pixelCountSums = new uint[clusterCount];
        for (int iteration = 0; iteration < MAX_ITERATIONS; iteration++) {
            for (int i = 0; i < clusterCount; i++) {
                for (int j = i + 1; j < clusterCount; j++) {
                    double distance = pointProvider.Distance(clusters[i], clusters[j]);
                    distanceToIndexMatrix[j][i].Distance = distance;
                    distanceToIndexMatrix[j][i].Index = i;
                    distanceToIndexMatrix[i][j].Distance = distance;
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
                    if (distanceToIndexMatrix[previousClusterIndex][j].Distance >= 4 * previousDistance) {
                        continue;
                    }
                    double distance = pointProvider.Distance(point, clusters[j]);
                    if (distance < minimumDistance) {
                        minimumDistance = distance;
                        newClusterIndex = j;
                    }
                }
                if (newClusterIndex != -1) {
                    double distanceChange = Math.Abs(Math.Sqrt(minimumDistance) - Math.Sqrt(previousDistance));
                    if (distanceChange > MIN_MOVEMENT_DISTANCE) {
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
            Array.Fill<uint>(pixelCountSums, 0);

            for (int i = 0; i < pointCount; i++) {
                int clusterIndex = clusterIndices[i];
                double[] point = points[i];
                var count = counts[i];

                pixelCountSums[clusterIndex] += count;
                componentASums[clusterIndex] += (point[0] * count);
                componentBSums[clusterIndex] += (point[1] * count);
                componentCSums[clusterIndex] += (point[2] * count);
            }

            for (int i = 0; i < clusterCount; i++) {
                var count = pixelCountSums[i];

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

        var argbToPopulation = new Dictionary<uint, uint>();
        for (int i = 0; i < clusterCount; i++) {
            var count = pixelCountSums[i];
            if (count == 0) {
                continue;
            }

            uint possibleNewCluster = pointProvider.ToUInt32(clusters[i]);
            if (argbToPopulation.ContainsKey(possibleNewCluster)) {
                continue;
            }

            argbToPopulation[possibleNewCluster] = count;
        }

        return argbToPopulation;
    }
}

internal class DistanceComparable: IComparable<DistanceComparable> {
    public int Index { get; set; }
    public double Distance { get; set; }

    internal DistanceComparable() {
        Index = -1;
        this.Distance = -1;
    }

    public int CompareTo(DistanceComparable? other) {
        return Distance.CompareTo(other?.Distance);
    }
}