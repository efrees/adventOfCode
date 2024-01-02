using AdventOfCode2023.Grid;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace AdventOfCode2023.Solvers;

internal class Day18Solver : ISolver
{
    private const string Name = "Day 18";
    private const string InputFile = "day18input.txt";

    public void Solve()
    {
        Console.WriteLine(Name);
        var lines = Input.GetLinesFromFile(InputFile).ToList();

        Console.WriteLine($"Output (part 1): {GetPart1Answer(lines)}");
        Console.WriteLine($"Output (part 2): {GetPart2Answer(lines)}");
    }

    private static long GetPart1Answer(List<string> lines)
    {
        var digSite = new SparseGrid<char>();
        var potentialInsidePoints = new HashSet<Point2D>();

        var currentPoint = Point2D.Origin;

        foreach (var line in lines)
        {
            var splits = line.Split(' ');
            var direction = splits[0][0];
            var distance = int.Parse(splits[1]);
            var color = splits[2];

            foreach (var _ in Enumerable.Range(0, distance))
            {
                digSite.SetCell(currentPoint, '#');

                //Assumes first trench cuts to the right.
                var rightPoint = direction switch
                {
                    'U' => currentPoint.GetEastNeighbor(),
                    'D' => currentPoint.GetWestNeighbor(),
                    'L' => currentPoint.GetNorthNeighbor(),
                    'R' => currentPoint.GetSouthNeighbor()
                };
                potentialInsidePoints.Add(rightPoint);

                currentPoint = direction switch
                {
                    'U' => currentPoint.GetNorthNeighbor(),
                    'D' => currentPoint.GetSouthNeighbor(),
                    'L' => currentPoint.GetWestNeighbor(),
                    'R' => currentPoint.GetEastNeighbor()
                };
            }
        }

        foreach (var inside in potentialInsidePoints)
        {
            if (digSite.GetCell(inside) == '#')
            {
                continue;
            }

            FloodFill(digSite, '#', inside);
        }

        var dugPointCount = digSite.GetAllCoordinates().Count();
        return dugPointCount;
    }

    private static long GetPart2Answer(List<string> lines)
    {
        var digInstructions = lines
            .Select(line => line.Split(' ')[2])
            .Select(colorString => colorString[2..^1])
            .Select(hexDigits => (direction: hexDigits[5] - '0', distance: int.Parse(hexDigits[..5], NumberStyles.HexNumber)));

        return ComputeEnclosedArea(digInstructions);
    }

    private static long ComputeEnclosedArea(IEnumerable<(int direction, int distance)> digInstructions)
    {
        var totalArea = 1L;
        var currentCutDepth = 1L;

        foreach (var instruction in digInstructions)
        {
            if (instruction.direction == 0)
            {
                currentCutDepth += instruction.distance;
                totalArea += instruction.distance;
            }
            else if (instruction.direction == 1)
            {
                totalArea += currentCutDepth * instruction.distance;
            }
            else if (instruction.direction == 2)
            {
                currentCutDepth -= instruction.distance;
            }
            else if (instruction.direction == 3)
            {
                totalArea -= (currentCutDepth - 1) * instruction.distance;
            }
        }

        return totalArea;
    }

    private static void FloodFill(SparseGrid<char> digSite, char fillChar, Point2D seed)
    {
        var frontier = new Queue<Point2D>();
        frontier.Enqueue(seed);
        while (frontier.Any())
        {
            var fillPoint = frontier.Dequeue();

            if (digSite.GetCell(fillPoint) == fillChar)
            {
                continue;
            }

            digSite.SetCell(fillPoint, fillChar);

            foreach (var neighbor in fillPoint.GetNeighbors4())
            {
                frontier.Enqueue(neighbor);
            }
        }
    }
}