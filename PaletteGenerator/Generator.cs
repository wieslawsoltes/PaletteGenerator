using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SkiaSharp;

namespace PaletteGenerator;

public class PaletteGenerator
{
    public static List<SKColor> Generate(string filePath, int numClusters)
    {
        // Load the image file into an SKBitmap object
        SKBitmap bitmap;
        using (var stream = File.OpenRead(filePath))
        {
            bitmap = SKBitmap.Decode(stream);
        }

        // Extract the colors from the bitmap
        var colors = ExtractColors(bitmap);

        // Cluster the colors using K-Means
        var clusters = KMeansCluster(colors, numClusters);

        // Select the most representative color from each cluster
        var dominantColors = new List<SKColor>();
        foreach (var cluster in clusters)
        {
            var representative = cluster.OrderByDescending(c => c.Value).First().Key;
            dominantColors.Add(representative);
        }

        return dominantColors;
    }

    static Dictionary<SKColor, int> ExtractColors(SKBitmap bitmap)
    {
        // Create a dictionary to store the colors and their frequencies
        var colors = new Dictionary<SKColor, int>();

        // Iterate over the pixels in the bitmap
        for (int y = 0; y < bitmap.Height; y++)
        {
            for (int x = 0; x < bitmap.Width; x++)
            {
                var color = bitmap.GetPixel(x, y);
                if (colors.ContainsKey(color))
                {
                    colors[color]++;
                }
                else
                {
                    colors[color] = 1;
                }
            }
        }

        // Return the dictionary of colors
        return colors;
    }

    static List<Dictionary<SKColor, int>> KMeansCluster(Dictionary<SKColor, int> colors, int numClusters)
    {
        // Initialize the clusters
        var clusters = new List<Dictionary<SKColor, int>>();
        for (int i = 0; i < numClusters; i++)
        {
            clusters.Add(new Dictionary<SKColor, int>());
        }

        // Select the initial cluster centers randomly
        var centers = colors.Keys.OrderBy(c => Guid.NewGuid()).Take(numClusters).ToArray();

        // Loop until the clusters stabilize
        var changed = true;
        while (changed)
        {
            changed = false;
            // Assign each color to the nearest cluster center
            foreach (var color in colors.Keys)
            {
                var nearest = FindNearestCenter(color, centers);
                var clusterIndex = Array.IndexOf(centers, nearest);
                clusters[clusterIndex][color] = colors[color];
            }

            // Recompute the cluster centers
            for (int i = 0; i < numClusters; i++)
            {
                var sumR = 0;
                var sumG = 0;
                var sumB = 0;
                var count = 0;
                foreach (var color in clusters[i].Keys)
                {
                    sumR += color.Red;
                    sumG += color.Green;
                    sumB += color.Blue;
                    count++;
                }

                var r = (byte) (sumR / count);
                var g = (byte) (sumG / count);
                var b = (byte) (sumB / count);
                var newCenter = new SKColor(r, g, b);
                if (!newCenter.Equals(centers[i]))
                {
                    centers[i] = newCenter;
                    changed = true;
                }
            }
        }

        // Return the clusters
        return clusters;
    }

    static SKColor FindNearestCenter(SKColor color, SKColor[] centers)
    {
        var nearest = centers[0];
        var minDist = double.MaxValue;
        foreach (var center in centers)
        {
            var dist = Distance(color, center);
            if (dist < minDist)
            {
                nearest = center;
                minDist = dist;
            }
        }

        return nearest;
    }

    static double Distance(SKColor c1, SKColor c2)
    {
        var r = c1.Red - c2.Red;
        var g = c1.Green - c2.Green;
        var b = c1.Blue - c2.Blue;
        return Math.Sqrt(r * r + g * g + b * b);
    }
}
