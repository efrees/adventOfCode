using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2024.Solvers;

internal class Day01Solver : ISolver
{
    private const string Name = "Day 1";
    private const string InputFile = "day01input.txt";

    public void Solve()
    {
        Console.WriteLine(Name);
        var lines = Input.GetLinesFromFile(InputFile)
            .Select(line => line.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray())
            .ToList();

        Console.WriteLine($"Output (part 1): {GetPart1Answer(lines)}");
        Console.WriteLine($"Output (part 2): {GetPart2Answer(lines)}");
    }

    private static long GetPart1Answer(List<int[]> pairs)
    {
        var leftList = pairs.Select(p => p[0]).Order();
        var rightList = pairs.Select(p => p[1]).Order();

        return leftList.Zip(rightList, (left, right) => Math.Abs(left - right))
            .Sum();
    }

    private static long GetPart2Answer(List<int[]> pairs)
    {
        var leftList = pairs.Select(p => p[0]);
        var counts = pairs.Select(p => p[1]).Aggregate(new Dictionary<int, int>(), (agg, next) =>
        {
            agg[next] = agg.GetValueOrDefault(next, 0) + 1;
            return agg;
        });

        return leftList.Select(value => value * counts.GetValueOrDefault(value, 0)).Sum();
    }
}