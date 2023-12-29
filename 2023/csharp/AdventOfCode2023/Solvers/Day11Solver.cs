using AdventOfCode2023.Grid;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2023.Solvers;

internal class Day11Solver : ISolver
{
    private const string Name = "Day 11";
    private const string InputFile = "day11input.txt";

    public void Solve()
    {
        Console.WriteLine(Name);
        var lines = Input.GetLinesFromFile(InputFile).ToList();

        Console.WriteLine($"Output (part 1): {GetPart1Answer(lines)}");
        Console.WriteLine($"Output (part 2): {GetPart2Answer(lines)}");
    }

    private static long GetPart1Answer(List<string> lines)
    {
        var (rowHasGalaxy, columnHasGalaxy) = MarkRowsAndColumnsWithGalaxy(lines);

        var expandedRowPositions = ComputeExpandedPositionMap(rowHasGalaxy);
        var expandedColumnPositions = ComputeExpandedPositionMap(columnHasGalaxy);

        var galaxyPositions = GetExpandedGalaxyPositions(lines, expandedColumnPositions, expandedRowPositions);

        var allDistances = galaxyPositions.CrossProduct(galaxyPositions)
            .Select(pair => pair.Item1.ManhattanDistance(pair.Item2))
            .Sum();

        return allDistances / 2;
    }

    private static long GetPart2Answer(List<string> lines)
    {
        var (rowHasGalaxy, columnHasGalaxy) = MarkRowsAndColumnsWithGalaxy(lines);

        var expandedRowPositions = ComputeExpandedPositionMap(rowHasGalaxy, 1_000_000);
        var expandedColumnPositions = ComputeExpandedPositionMap(columnHasGalaxy, 1_000_000);

        var galaxyPositions = GetExpandedGalaxyPositions(lines, expandedColumnPositions, expandedRowPositions);

        var allDistances = galaxyPositions.CrossProduct(galaxyPositions)
            .Select(pair => pair.Item1.ManhattanDistance(pair.Item2))
            .Sum();

        return allDistances / 2;
    }

    private static (bool[] rowHasGalaxy, bool[] columnHasGalaxy) MarkRowsAndColumnsWithGalaxy(List<string> lines)
    {
        var rowHasGalaxy = new bool[lines.Count];
        var columnHasGalaxy = new bool[lines[0].Length];

        for (var row = 0; row < lines.Count; row++)
        {
            var line = lines[row];

            foreach (var galaxyIndex in GetAllGalaxyIndexes(line))
            {
                columnHasGalaxy[galaxyIndex] = true;
                rowHasGalaxy[row] = true;
            }
        }

        return (rowHasGalaxy, columnHasGalaxy);
    }

    private static int[] ComputeExpandedPositionMap(bool[] hasGalaxy, int weight = 2)
    {
        var impacts = new int[hasGalaxy.Length];

        var countWithoutGalaxy = 0;
        for (var i = 0; i < impacts.Length; i++)
        {
            impacts[i] = i + countWithoutGalaxy * (weight - 1);

            if (!hasGalaxy[i])
            {
                countWithoutGalaxy++;
            }
        }

        return impacts;
    }

    private static HashSet<Point2D> GetExpandedGalaxyPositions(List<string> lines,
        int[] expandedColumnPositions,
        int[] expandedRowPositions)
    {
        var galaxyPositions = new HashSet<Point2D>();

        for (var row = 0; row < lines.Count; row++)
        {
            var line = lines[row];
            foreach (var col in GetAllGalaxyIndexes(line))
            {
                galaxyPositions.Add((expandedColumnPositions[col], expandedRowPositions[row]));
            }
        }

        return galaxyPositions;
    }

    private static IEnumerable<int> GetAllGalaxyIndexes(string line)
    {
        var nextSearchIndex = 0;

        var nextIndex = line.IndexOf('#', nextSearchIndex);
        while (nextIndex >= 0)
        {
            yield return nextIndex;
            nextSearchIndex = nextIndex + 1;
            nextIndex = line.IndexOf('#', nextSearchIndex);
        }
    }
}