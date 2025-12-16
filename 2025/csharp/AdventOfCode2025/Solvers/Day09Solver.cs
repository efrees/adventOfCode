using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2025.Grid;

namespace AdventOfCode2025.Solvers;

internal class Day09Solver : ISolver
{
    private const string Name = "Day 9";
    private const string InputFile = "day09input.txt";

    public void Solve()
    {
        Console.WriteLine(Name);
        var lines = Input.GetLinesFromFile(InputFile).ToList();

        Console.WriteLine($"Output (part 1): {GetPart1Answer(lines)}");
        Console.WriteLine($"Output (part 2): {GetPart2Answer(lines)}");
    }

    private static bool IsSample => InputFile.Contains("_sample");

    private static long GetPart1Answer(List<string> rows)
    {
        var points = rows.Select(row => row.Split(','))
            .Select(split => (Point2D)(long.Parse(split[0]), long.Parse(split[1])))
            .ToList();

        var maxArea = 0L;
        for (var i = 0; i < points.Count; i++)
        {
            for (var j = i + 1; j < points.Count; j++)
            {
                var area = (points[i].X - points[j].X + 1) * (points[i].Y - points[j].Y + 1);

                if (area < 0)
                {
                    area *= -1;
                }

                if (area > maxArea)
                {
                    maxArea = area;
                }
            }
        }

        return maxArea;
    }

    private static long GetPart2Answer(List<string> rows)
    {
        return 0;
    }
}
