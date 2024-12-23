using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2024.Solvers;

internal class Day02Solver : ISolver
{
    private const string Name = "Day 2";
    private const string InputFile = "day02input.txt";

    public void Solve()
    {
        Console.WriteLine(Name);
        var lines = Input.GetLinesFromFile(InputFile)
            .Select(line => line.Split(" ").Select(int.Parse).ToArray())
            .ToList();

        Console.WriteLine($"Output (part 1): {GetPart1Answer(lines)}");
        Console.WriteLine($"Output (part 2): {GetPart2Answer(lines)}");
    }

    private static long GetPart1Answer(List<int[]> lines)
    {
        return lines.Count(ReportIsSafe);
    }

    private static long GetPart2Answer(List<int[]> lines)
    {
        return lines.Count(ReportIsSafeWithTolerance);
    }

    private static bool ReportIsSafe(int[] report)
    {
        var direction = report.Last() - report.First();

        if (Math.Abs(direction) < report.Length - 2)
        {
            return false;
        }

        direction /= Math.Abs(direction);

        var previous = report.First();
        foreach (var next in report.Skip(1))
        {
            if (!IsInRangeInclusive(next, previous + direction, previous + 3 * direction))
            {
                return false;
            }

            previous = next;
        }

        return true;
    }

    private static bool ReportIsSafeWithTolerance(int[] report)
    {
        if (ReportIsSafe(report))
        {
            return true;
        }

        var modifiedReport = new int[report.Length - 1];

        for (var iToSkip = 0; iToSkip < report.Length; iToSkip++)
        {
            var copyTo = 0;
            for (var j = 0; j < report.Length; j++)
            {
                if (j == iToSkip)
                {
                    continue;
                }

                modifiedReport[copyTo++] = report[j];
            }

            if (ReportIsSafe(modifiedReport))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsInRangeInclusive(int next, int start, int end)
    {
        if (start > end)
        {
            (start, end) = (end, start);
        }

        return start <= next && end >= next;
    }
}