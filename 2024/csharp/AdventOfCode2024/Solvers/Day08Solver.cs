using AdventOfCode2024.Grid;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2024.Solvers;

internal class Day08Solver : ISolver
{
    private const string Name = "Day 8";
    private const string InputFile = "day08input.txt";

    public void Solve()
    {
        Console.WriteLine(Name);
        var lines = Input.GetLinesFromFile(InputFile)
            .ToList();

        Console.WriteLine($"Output (part 1): {GetPart1Answer(lines)}");
        Console.WriteLine($"Output (part 2): {GetPart2Answer(lines)}");
    }

    private static long GetPart1Answer(List<string> lines)
    {
        var grid = SparseGrid<char>.Parse(lines, x => x);

        var antennaGroups = grid.GetAllCells()
            .Where(cell => cell.Value != '.')
            .ToLookup(cell => cell.Value, cell => cell.Coordinates);

        var antinodes = new HashSet<(long, long)>();
        foreach (var group in antennaGroups)
        {
            var antennaPositions = group.ToList();
            foreach (var antennaPair in antennaPositions.CrossProduct(antennaPositions))
            {
                var vector = antennaPair.Item2.Subtract(antennaPair.Item1);

                if (vector == (0, 0))
                {
                    continue;
                }

                var antinode = antennaPair.Item2.Add(vector);
                if (grid.IsInCurrentBounds(antinode))
                {
                    antinodes.Add(antinode);
                }
            }
        }

        return antinodes.Count;
    }

    private static long GetPart2Answer(List<string> lines)
    {
        var grid = SparseGrid<char>.Parse(lines, x => x);

        var antennaGroups = grid.GetAllCells()
            .Where(cell => cell.Value != '.')
            .ToLookup(cell => cell.Value, cell => cell.Coordinates);

        var antinodes = new HashSet<(long, long)>();
        foreach (var group in antennaGroups)
        {
            var antennaPositions = group.ToList();
            foreach (var antennaPair in antennaPositions.CrossProduct(antennaPositions))
            {
                var vector = antennaPair.Item2.Subtract(antennaPair.Item1);

                if (vector == (0, 0))
                {
                    continue;
                }

                var antinode = antennaPair.Item1.Add(vector);
                while (grid.IsInCurrentBounds(antinode))
                {
                    antinodes.Add(antinode);
                    antinode = antinode.Add(vector);
                }
            }
        }

        return antinodes.Count;
    }
}