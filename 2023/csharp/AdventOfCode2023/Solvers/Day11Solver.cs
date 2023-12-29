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
        var inputHeight = lines.Count;
        var inputWidth = lines[0].Length;

        var rowHasGalaxy = new bool[inputHeight];
        var columnHasGalaxy = new bool[inputWidth];

        for (var row = 0; row < inputHeight; row++)
        {
            var line = lines[row];

            foreach (var galaxyIndex in GetAllGalaxyIndexes(line))
            {
                columnHasGalaxy[galaxyIndex] = true;
                rowHasGalaxy[row] = true;
            }
        }

        var galaxyPositions = new HashSet<Point2D>();

        var yExpansion = 0;
        for (var row = 0; row < inputHeight; row++)
        {
            var xExpansion = 0;
            var line = lines[row];
            for (var col = 0; col < line.Length; col++)
            {
                if (line[col] == '#')
                {
                    galaxyPositions.Add((col + xExpansion, row + yExpansion));
                }

                if (!columnHasGalaxy[col])
                {
                    xExpansion++;
                }
            }

            if (!rowHasGalaxy[row])
            {
                yExpansion++;
            }
        }

        var allDistances = galaxyPositions.CrossProduct(galaxyPositions)
            .Select(pair => pair.Item1.ManhattanDistance(pair.Item2))
            .Sum();

        return allDistances / 2;
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

    private static long GetPart2Answer(List<string> lines)
    {
        return -1;
    }
}