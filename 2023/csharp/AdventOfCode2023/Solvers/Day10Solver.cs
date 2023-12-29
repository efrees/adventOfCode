using AdventOfCode2023.Grid;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2023.Solvers;

internal class Day10Solver : ISolver
{
    private const string Name = "Day 10";
    private const string InputFile = "day10input.txt";

    public void Solve()
    {
        Console.WriteLine(Name);
        var lines = Input.GetLinesFromFile(InputFile).ToList();

        Console.WriteLine($"Output (part 1): {GetPart1Answer(lines)}");
        Console.WriteLine($"Output (part 2): {GetPart2Answer(lines)}");
    }

    private static long GetPart1Answer(List<string> lines)
    {
        var grid = SparseGrid<char>.Parse(lines, c => c);
        var start = (Point2D)grid.FindValueLocation('S');

        var frontiers = GetNextAccessiblePoints(start, grid).ToList();

        var stepCounter = 1;
        while (frontiers.Any())
        {
            frontiers.ForEach(point => grid.SetCell(point, '#'));

            frontiers = frontiers.SelectMany(f => GetNextAccessiblePoints(f, grid)).ToList();
            stepCounter++;
        }

        return stepCounter - 1;
    }

    private static long GetPart2Answer(List<string> lines)
    {
        return -1;
    }

    private static IEnumerable<Point2D> GetNextAccessiblePoints(Point2D current, SparseGrid<char> grid)
    {
        var directionalChecks = new Func<(long, long), SparseGrid<char>, bool>[]
            { CanEnterFromEast, CanEnterFromSouth, CanEnterFromWest, CanEnterFromNorth };

        return current.GetNeighbors4()
            .Zip(directionalChecks, (point, isAccessible) => (point, isAccessible))
            .Where(item => item.isAccessible(item.point, grid))
            .Select(item => item.point);
    }

    private static bool CanEnterFromSouth((long x, long y) location, SparseGrid<char> grid)
    {
        return grid.GetCell(location) is '|' or '7' or 'F';
    }

    private static bool CanEnterFromNorth((long x, long y) location, SparseGrid<char> grid)
    {
        return grid.GetCell(location) is '|' or 'L' or 'J';
    }

    private static bool CanEnterFromEast((long x, long y) location, SparseGrid<char> grid)
    {
        return grid.GetCell(location) is '-' or 'L' or 'F';
    }

    private static bool CanEnterFromWest((long x, long y) location, SparseGrid<char> grid)
    {
        return grid.GetCell(location) is '-' or 'J' or '7';
    }
}