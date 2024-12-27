using AdventOfCode2024.Grid;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2024.Solvers;

internal class Day06Solver : ISolver
{
    private const string Name = "Day 6";
    private const string InputFile = "day06input.txt";

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
        var grid = SimulatePatrol(lines);

        return grid.GetAllCoordinates().Count(cell => grid.GetCell(cell) == 'X');
    }

    private static long GetPart2Answer(List<string> lines)
    {
        var originalPatrol = SimulatePatrol(lines);

        var visitedCoordinates = originalPatrol.GetAllCoordinates()
            .Where(cell => originalPatrol.GetCell(cell) == 'X')
            .ToList();

        var loopCount = 0;
        foreach (var newObstacleLocation in visitedCoordinates)
        {
            var grid = SparseGrid<char>.Parse(lines, c => c);
            var width = lines[0].Length;
            var height = lines.Count;

            if (grid.GetCell(newObstacleLocation) == '^')
            {
                continue;
            }

            grid.SetCell(newObstacleLocation, '#');

            if (SimulatePatrol(grid, width, height).isLoop)
            {
                loopCount++;
            }
        }

        return loopCount;
    }

    private static SparseGrid<char> SimulatePatrol(List<string> lines)
    {
        var grid = SparseGrid<char>.Parse(lines, c => c);
        var width = lines[0].Length;
        var height = lines.Count;
        return SimulatePatrol(grid, width, height).traversal;
    }

    private static (SparseGrid<char> traversal, bool isLoop) SimulatePatrol(SparseGrid<char> grid, int width, int height)
    {
        var hasLoop = false;
        var previousDirections = new SparseGrid<List<Point2D>>();
        Point2D guardPosition = grid.FindValueLocation('^');
        Point2D direction = (0, -1);
        grid.SetCell(guardPosition, 'X');
        previousDirections.SetCell(guardPosition, [direction]);

        while (!hasLoop && IsInBounds(guardPosition.Add(direction), width, height))
        {
            if (grid.GetCell(guardPosition.Add(direction)) == '#')
            {
                direction = (-direction.Y, direction.X);
            }

            if (grid.GetCell(guardPosition.Add(direction)) == '#')
            {
                direction = (-direction.Y, direction.X);
            }

            guardPosition = guardPosition.Add(direction);
            var existingDirections = previousDirections.GetCell(guardPosition, new List<Point2D>());

            if (existingDirections.Contains(direction))
            {
                hasLoop = true;
            }

            grid.SetCell(guardPosition, 'X');
            existingDirections.Add(direction);
            previousDirections.SetCell(guardPosition, existingDirections);
        }

        return (grid, hasLoop);
    }

    private static bool IsInBounds((long x, long y) guardStart, int width, int height)
    {
        return guardStart.x >= 0 && guardStart.x < width
            && guardStart.y >= 0 && guardStart.y < height;
    }
}