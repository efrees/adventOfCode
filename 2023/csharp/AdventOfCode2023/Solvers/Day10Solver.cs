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

        var grid = SparseGrid<char>.Parse(lines, c => c);
        Console.WriteLine($"Output (part 1): {GetPart1Answer(grid)}");
        Console.WriteLine($"Output (part 2): {GetPart2Answer(grid)}");
    }

    private static long GetPart1Answer(SparseGrid<char> grid)
    {
        var start = (Point2D)grid.FindValueLocation('S');

        var mainPathCoords = FollowMainPath(start, grid);

        return mainPathCoords.Count() / 2;
    }

    private static long GetPart2Answer(SparseGrid<char> grid)
    {
        var start = (Point2D)grid.FindValueLocation('S');

        // 1. Follow path, marking left and right, non-path cells
        // 2. If left or right is ever outside the extremes of the path, that will be "outside"; the other is "inside"
        // 3. Flood-fill from each "inside" point and count
        var mainPath = FollowMainPath(start, grid).ToList();
        var pathPoints = mainPath.ToHashSet();
        var minX = mainPath.Select(point => point.X).Min();
        var maxX = mainPath.Select(point => point.X).Max();

        var leftPoints = new HashSet<Point2D>();
        var rightPoints = new HashSet<Point2D>();

        var rightIsInside = false;

        var previous = start;
        foreach (var current in mainPath.Skip(1).Concat(new[] { start }))
        {
            var direction = current.Subtract(previous);
            var left = (direction.Y, -direction.X);
            var right = (-direction.Y, direction.X);

            var leftPoint = current.Add(left);
            var rightPoint = current.Add(right);

            if (!pathPoints.Contains(leftPoint))
            {
                leftPoints.Add(leftPoint);
            }

            if (!pathPoints.Contains(rightPoint))
            {
                rightPoints.Add(rightPoint);
            }

            var previousLeft = previous.Add(left);
            var previousRight = previous.Add(right);

            if (!pathPoints.Contains(previousLeft))
            {
                leftPoints.Add(previousLeft);
            }

            if (!pathPoints.Contains(previousRight))
            {
                rightPoints.Add(previousRight);
            }

            if (leftPoint.X < minX || leftPoint.X > maxX)
            {
                rightIsInside = true;
            }

            previous = current;
        }

        var insidePoints = rightIsInside
            ? FloodFillAll(rightPoints, pathPoints)
            : FloodFillAll(leftPoints, pathPoints);

        return insidePoints.Count;
    }

    private static HashSet<Point2D> FloodFillAll(HashSet<Point2D> pointsInSet, HashSet<Point2D> boundaryPoints)
    {
        var pointsToSearch = new Queue<Point2D>(pointsInSet);
        pointsInSet = new();

        while (pointsToSearch.Count > 0)
        {
            var point = pointsToSearch.Dequeue();

            if (pointsInSet.Contains(point))
            {
                continue;
            }

            pointsInSet.Add(point);
            foreach (var neighbor in point.GetNeighbors4().Except(boundaryPoints).Except(pointsInSet))
            {
                pointsToSearch.Enqueue(neighbor);
            }
        }

        return pointsInSet;
    }

    private static IEnumerable<Point2D> FollowMainPath(Point2D start, SparseGrid<char> grid)
    {
        yield return start;
        var nextPoint = GetPointsAccessibleFromStart(start, grid).First();
        var previousPoint = start;

        while (nextPoint is not null)
        {
            yield return nextPoint;

            var current = nextPoint;
            nextPoint = GetNextAccessiblePoints(nextPoint, grid)
                .FirstOrDefault(x => x != start && x != previousPoint);
            previousPoint = current;
        }
    }

    private static IEnumerable<Point2D> GetPointsAccessibleFromStart(Point2D start, SparseGrid<char> grid)
    {
        var directionalChecks = new Func<(long, long), SparseGrid<char>, bool>[]
            { CanEnterFromEast, CanEnterFromSouth, CanEnterFromWest, CanEnterFromNorth };

        return start.GetNeighbors4()
            .Zip(directionalChecks, (point, isAccessible) => (point, isAccessible))
            .Where(item => item.isAccessible(item.point, grid))
            .Select(item => item.point);
    }

    private static IEnumerable<Point2D> GetNextAccessiblePoints(Point2D current, SparseGrid<char> grid)
    {
        var currentShape = grid.GetCell(current);

        var (west, north, east, south)
            = (current.GetWestNeighbor(), current.GetNorthNeighbor(), current.GetEastNeighbor(), current.GetSouthNeighbor());

        return currentShape switch
        {
            '|' => new List<Point2D> { north, south },
            'L' => new List<Point2D> { north, east },
            'J' => new List<Point2D> { north, west },
            '-' => new List<Point2D> { west, east },
            '7' => new List<Point2D> { west, south },
            'F' => new List<Point2D> { south, east },
            _ => throw new ArgumentOutOfRangeException(nameof(current), "We might not be on the main path")
        };
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