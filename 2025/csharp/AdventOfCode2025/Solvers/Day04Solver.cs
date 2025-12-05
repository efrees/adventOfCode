using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2025.Grid;

namespace AdventOfCode2025.Solvers;

internal class Day04Solver : ISolver
{
    private const string Name = "Day 4";
    private const string InputFile = "day04input.txt";

    public void Solve()
    {
        Console.WriteLine(Name);
        var lines = Input.GetLinesFromFile(InputFile)
            .ToList();

        Console.WriteLine($"Output (part 1): {GetPart1Answer(lines)}");
        Console.WriteLine($"Output (part 2): {GetPart2Answer(lines)}");
    }

    private static long GetPart1Answer(List<string> rows)
    {
        var grid = SparseGrid<char>.Parse(rows, c => c);
        return grid.GetAllCells()
            .Where(pair => pair.Value == '@')
            .Count(pair => HasFewerThanFourAdjacentRolls(pair.Coordinates, grid));
    }

    private static bool HasFewerThanFourAdjacentRolls(Point2D point, SparseGrid<char> grid)
    {
        return point
            .GetNeighbors8()
            .Count(neighbor => grid.GetCell(neighbor) == '@') < 4;
    }

    private static long GetPart2Answer(List<string> rows)
    {
        var sum = 0L;
        var grid = SparseGrid<char>.Parse(rows, c => c);
        var checkAgain = true;
        while (checkAgain)
        {
            var removableRolls = grid.GetAllCells()
                .Where(pair => pair.Value == '@' && HasFewerThanFourAdjacentRolls(pair.Coordinates, grid))
                .ToList();

            removableRolls.ForEach(roll => grid.SetCell(roll.Coordinates, '.'));

            var removableCount = removableRolls.Count;

            sum += removableCount;

            if (removableCount == 0)
            {
                checkAgain = false;
            }
        }
        return sum;
    }
}
