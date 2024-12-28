using AdventOfCode2024.Grid;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2024.Solvers;

internal class Day12Solver : ISolver
{
    private const string Name = "Day 12";
    private const string InputFile = "day12input_sample.txt";

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
        var grid = SparseGrid<char>.Parse(lines, c => c);

        var visited = new HashSet<Point2D>();

        var regions = new List<(int Area, int Perimeter)>();
        foreach (var cell in grid.GetAllCells())
        {
            if (visited.Contains(cell.Coordinates))
            {
                continue;
            }

            regions.Add(ComputeAreaAndPerimeter(cell.Coordinates, cell.Value, grid, visited));
        }

        return regions
            .Select(r => r.Area * r.Perimeter)
            .Sum();
    }

    private static (int Area, int Perimeter) ComputeAreaAndPerimeter(Point2D cellCoordinates,
        char cellValue,
        SparseGrid<char> grid,
        HashSet<Point2D> visited)
    {
        visited.Add(cellCoordinates);

        var area = 1;
        var perimeter = 0;
        foreach (var neighbor in cellCoordinates.GetNeighbors4())
        {
            if (grid.GetCell(neighbor) != cellValue)
            {
                perimeter++;
            }
            else if (!visited.Contains(neighbor))
            {
                var nestedResult = ComputeAreaAndPerimeter(neighbor, cellValue, grid, visited);
                area += nestedResult.Area;
                perimeter += nestedResult.Perimeter;
            }
        }

        return (area, perimeter);
    }

    private static long GetPart2Answer(List<string> lines)
    {
        var grid = SparseGrid<char>.Parse(lines, c => c);

        var visited = new HashSet<Point2D>();

        var regions = new List<(int Area, int Sides)>();
        foreach (var cell in grid.GetAllCells())
        {
            if (visited.Contains(cell.Coordinates))
            {
                continue;
            }

            var areaAndPerimeter = ComputeAreaAndPerimeter(cell.Coordinates, cell.Value, grid, visited);
            var sides = WalkPerimeterToCountSides(cell.Coordinates, cell.Value, grid);
            regions.Add((areaAndPerimeter.Area, sides));
        }

        // > 812010
        return regions
            .Select(r => r.Area * r.Sides)
            .Sum();
    }

    private static int WalkPerimeterToCountSides(Point2D cellCoordinates, char cellValue, SparseGrid<char> grid)
    {
        var sides = new SparseGrid<char>();
        // We encounter each region from the top (minY) side and from the left, so we start walking right.
        var currentSideDirection = new Point2D(1, 0);
        var currentLocation = cellCoordinates;
        sides.SetCell(currentLocation, '#');

        var sideCount = 1;
        while (currentLocation != cellCoordinates || currentSideDirection != (0, -1))
        {
            var nextLocation = currentLocation.Add(currentSideDirection);

            if (grid.GetCell(nextLocation) != cellValue)
            {
                //...?
                //AAA.
                currentSideDirection = TurnRight(currentSideDirection);
                sideCount++;
            }
            else if (grid.GetCell(LeftHandCell(nextLocation, currentSideDirection)) == cellValue)
            {
                //...A
                //AAAA
                currentSideDirection = TurnLeft(currentSideDirection);
                currentLocation = nextLocation;
                sideCount++;
            }
            else
            {
                currentLocation = nextLocation;
            }

            sides.SetCell(currentLocation, '#');
        }

        Console.WriteLine($"{cellValue}: {sideCount}");
        Console.WriteLine(sides.RenderAsString(c => c == '\0'
            ? '.'
            : c));

        return sideCount;
    }

    private static Point2D LeftHandCell(Point2D nextLocation, Point2D currentSideDirection)
    {
        var leftDirection = TurnLeft(currentSideDirection);
        return nextLocation.Add(leftDirection);
    }

    private static (long Y, long) TurnLeft(Point2D currentSideDirection)
    {
        return (currentSideDirection.Y, -currentSideDirection.X);
    }

    private static (long Y, long) TurnRight(Point2D currentSideDirection)
    {
        return (-currentSideDirection.Y, currentSideDirection.X);
    }
}