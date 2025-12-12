using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2025.Grid;

namespace AdventOfCode2025.Solvers;

internal class Day08Solver : ISolver
{
    private const string Name = "Day 8";
    private const string InputFile = "day08input.txt";

    public void Solve()
    {
        Console.WriteLine(Name);
        var lines = Input.GetLinesFromFile(InputFile).ToList();

        Console.WriteLine($"Output (part 1): {GetPart1Answer(lines)}");
        Console.WriteLine($"Output (part 2): {GetPart2Answer(lines)}");
    }

    private static bool IsSample => InputFile.Contains("_sample");

    private static long GetPart1Answer(List<string> rows)
    {
        var points = rows.Select(row => row.Split(','))
            .Select(split =>
                (Point3D)(long.Parse(split[0]), long.Parse(split[1]), long.Parse(split[2]))
            )
            .ToList();

        var connected = new HashSet<(Point3D, Point3D)>();
        var pairsByDistance = points
            .CrossProduct(points)
            .Where(pair => pair.Item1 != pair.Item2)
            .OrderBy(pair => pair.Item1.StraightLineDistance(pair.Item2))
            .ToList();

        var numberToConnect = IsSample ? 10 : 1000;

        while (numberToConnect > 0)
        {
            numberToConnect--;

            var nextClosest = pairsByDistance.First();
            pairsByDistance.RemoveAt(0);

            // I've left both equivalent pairs in the list
            if (connected.Contains(nextClosest))
            {
                numberToConnect++; //doesn't count
                continue;
            }

            connected.Add(nextClosest);
            connected.Add((nextClosest.Item2, nextClosest.Item1));
        }

        // Determine the number and size of clusters
        var adjacencyMap = new Dictionary<Point3D, HashSet<Point3D>>();
        foreach (var (item1, item2) in connected)
        {
            if (adjacencyMap.ContainsKey(item1))
            {
                adjacencyMap[item1].Add(item2);
            }
            else
            {
                adjacencyMap[item1] = new HashSet<Point3D>() { item2 };
            }
        }

        var unclustered = points.ToHashSet();

        var clusterSizes = new List<int>();
        while (unclustered.Any())
        {
            var seed = unclustered.First();
            unclustered.Remove(seed);

            var currentCluster = new HashSet<Point3D>() { seed };
            var frontier = adjacencyMap.GetValueOrDefault(seed, []).ToList();

            while (frontier.Any())
            {
                var next = frontier.First();
                frontier.RemoveAt(0);
                if (currentCluster.Contains(next))
                {
                    continue;
                }

                unclustered.Remove(next);
                currentCluster.Add(next);
                frontier.AddRange(adjacencyMap.GetValueOrDefault(next, []));
            }

            clusterSizes.Add(currentCluster.Count);
        }

        return clusterSizes.OrderDescending().Take(3).Aggregate((agg, next) => agg * next);
    }

    private static long GetPart2Answer(List<string> rows)
    {
        var points = rows.Select(row => row.Split(','))
            .Select(split =>
                (Point3D)(long.Parse(split[0]), long.Parse(split[1]), long.Parse(split[2]))
            )
            .ToList();

        var pairsByDistance = points
            .CrossProduct(points)
            .Where(pair => pair.Item1 != pair.Item2)
            .OrderBy(pair => pair.Item1.StraightLineDistance(pair.Item2))
            .ToList();

        // initialize each point as its own cluster
        var clusterMap = new Dictionary<Point3D, int>();
        for (var i = 0; i < points.Count; i++)
        {
            clusterMap[points[i]] = i;
        }

        // We'll pick the smaller "id" when we merge clusters, so we'll converge to 0
        bool IsSingleCluster(Dictionary<Point3D, int> map)
        {
            return !map.Values.Any(v => v > 0);
        }

        void MergeClusters(Dictionary<Point3D, int> map, int cluster1, int cluster2)
        {
            if (cluster1 > cluster2)
            {
                (cluster1, cluster2) = (cluster2, cluster1);
            }

            var sourceCluster = map.Where(pair => pair.Value == cluster2)
                .Select(pair => pair.Key)
                .ToList();
            sourceCluster.ForEach(point => map[point] = cluster1);
        }

        var adjacencyMap = new Dictionary<Point3D, HashSet<Point3D>>();

        var lastPairXProduct = 0L;
        foreach (var (item1, item2) in pairsByDistance)
        {
            if (clusterMap[item1] != clusterMap[item2])
            {
                MergeClusters(clusterMap, clusterMap[item1], clusterMap[item2]);
            }

            if (IsSingleCluster(clusterMap))
            {
                lastPairXProduct = item1.X * item2.X;
                break;
            }
        }

        // < 15650261281478
        return lastPairXProduct;
    }
}
