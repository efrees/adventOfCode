using AdventOfCode2024.Grid;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2024.Solvers;

internal class Day10Solver : ISolver
{
    private const string Name = "Day 10";
    private const string InputFile = "day10input.txt";

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
        var grid = SparseGrid<int>.Parse(lines, c => c - '0');

        var scoreTotal = 0;
        foreach (var cell in grid.GetAllCells().Where(cell => cell.Value == 0))
        {
            var reachableNines = FindReachableNines(cell, grid).Distinct();
            scoreTotal += reachableNines.Count();
        }

        return scoreTotal;
    }

    private static IEnumerable<Point2D> FindReachableNines((Point2D Coordinates, int Value) cell, SparseGrid<int> grid)
    {
        if (cell.Value == 9)
        {
            return [cell.Coordinates];
        }

        return cell.Coordinates.GetNeighbors4()
            .Select(neighbor => (coords: neighbor, value: grid.GetCell(neighbor, -1)))
            .Where(neighborCell => neighborCell.value == cell.Value + 1)
            .SelectMany(neighborCell => FindReachableNines(neighborCell, grid));
    }

    private static long GetPart2Answer(List<string> lines)
    {
        var grid = SparseGrid<int>.Parse(lines, c => c - '0');

        var scoreTotal = 0;
        foreach (var cell in grid.GetAllCells().Where(cell => cell.Value == 0))
        {
            var countTrailsToNine = CountTrailsToNine(cell, grid);
            scoreTotal += countTrailsToNine;
        }

        return scoreTotal;
    }

    private static int CountTrailsToNine((Point2D Coordinates, int Value) cell, SparseGrid<int> grid)
    {
        if (cell.Value == 9)
        {
            return 1;
        }

        return cell.Coordinates.GetNeighbors4()
            .Select(neighbor => (coords: neighbor, value: grid.GetCell(neighbor, -1)))
            .Where(neighborCell => neighborCell.value == cell.Value + 1)
            .Select(neighborCell => CountTrailsToNine(neighborCell, grid))
            .Sum();
    }
}