using AdventOfCode2023.Grid;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2023.Solvers;

internal class Day17Solver : ISolver
{
    private const string Name = "Day 17";
    private const string InputFile = "day17input.txt";

    public void Solve()
    {
        Console.WriteLine(Name);
        var lines = Input.GetLinesFromFile(InputFile).ToList();

        Console.WriteLine($"Output (part 1): {GetPart1Answer(lines)}");
        Console.WriteLine($"Output (part 2): {GetPart2Answer(lines)}");
    }

    private static long GetPart1Answer(List<string> lines)
    {
        var grid = SparseGrid<int>.Parse(lines, x => x - '0');

        var start = new Point2D(0, 0);
        var end = new Point2D((int)grid.xMax, (int)grid.yMax);

        long EstimateRemaining(Point2D current) => current.ManhattanDistance(end);

        var bestResults = new Dictionary<TraversalState, long>();
        var searchFrontier = new PriorityQueue<TraversalState, long>();

        var nextToSearch = new TraversalState(start, (0, 0), 0, 0);

        while (!nextToSearch.Position.Equals(end))
        {
            var traversalKey = nextToSearch with { ActualCost = 0 };

            if (!bestResults.TryGetValue(traversalKey, out var bestValue) || bestValue > nextToSearch.ActualCost)
            {
                bestResults[traversalKey] = nextToSearch.ActualCost;

                var reachablePositions = nextToSearch.Position.GetNeighbors4();
                foreach (var reachablePosition in reachablePositions)
                {
                    var costOfEntry = grid.GetCell(reachablePosition);
                    if (costOfEntry == default)
                    {
                        continue; //outside of grid
                    }

                    var direction = reachablePosition.Subtract(nextToSearch.Position);

                    var newDistanceInDirection = nextToSearch.EntryDirection == direction
                        ? nextToSearch.DistanceInDirection + 1
                        : 1;

                    if (newDistanceInDirection > 3 || direction.Add(nextToSearch.EntryDirection).Equals(Point2D.Origin))
                    {
                        continue;
                    }

                    var newCost = nextToSearch.ActualCost + costOfEntry;
                    var nextPossibility = new TraversalState(reachablePosition, direction, newDistanceInDirection, newCost);
                    searchFrontier.Enqueue(nextPossibility, newCost + EstimateRemaining(reachablePosition));
                }
            }

            nextToSearch = searchFrontier.Dequeue();
        }

        return nextToSearch.ActualCost;
    }

    private static long GetPart2Answer(List<string> lines)
    {
        var grid = SparseGrid<int>.Parse(lines, x => x - '0');

        var start = new Point2D(0, 0);
        var end = new Point2D((int)grid.xMax, (int)grid.yMax);

        long EstimateRemaining(Point2D current) => current.ManhattanDistance(end);

        var bestResults = new Dictionary<TraversalState, long>();
        var searchFrontier = new PriorityQueue<TraversalState, long>();

        var nextToSearch = new TraversalState(start, (0, 0), 0, 0);

        while (!nextToSearch.Position.Equals(end) || nextToSearch.DistanceInDirection < 4)
        {
            var traversalKey = nextToSearch with { ActualCost = 0 };

            if (!bestResults.TryGetValue(traversalKey, out var bestValue) || bestValue > nextToSearch.ActualCost)
            {
                bestResults[traversalKey] = nextToSearch.ActualCost;

                var reachablePositions = nextToSearch.Position.GetNeighbors4();
                foreach (var reachablePosition in reachablePositions)
                {
                    var costOfEntry = grid.GetCell(reachablePosition);
                    if (costOfEntry == default)
                    {
                        continue; //outside of grid
                    }

                    var direction = reachablePosition.Subtract(nextToSearch.Position);

                    var isSameDirection = nextToSearch.EntryDirection == Point2D.Origin || nextToSearch.EntryDirection == direction;
                    var newDistanceInDirection = isSameDirection
                        ? nextToSearch.DistanceInDirection + 1
                        : 1;

                    if (newDistanceInDirection > 10
                        || (!isSameDirection && nextToSearch.DistanceInDirection < 4)
                        || direction.Add(nextToSearch.EntryDirection) == Point2D.Origin)
                    {
                        continue;
                    }

                    var newCost = nextToSearch.ActualCost + costOfEntry;
                    var nextPossibility = new TraversalState(reachablePosition, direction, newDistanceInDirection, newCost);
                    searchFrontier.Enqueue(nextPossibility, newCost + EstimateRemaining(reachablePosition));
                }
            }

            nextToSearch = searchFrontier.Dequeue();
        }

        return nextToSearch.ActualCost;
    }

    private record TraversalState(Point2D Position, Point2D EntryDirection, int DistanceInDirection, long ActualCost);
}