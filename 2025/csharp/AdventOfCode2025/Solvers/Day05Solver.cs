using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2025.Grid;

namespace AdventOfCode2025.Solvers;

internal class Day05Solver : ISolver
{
    private const string Name = "Day 5";
    private const string InputFile = "day05input.txt";

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
        var verticalSplitIndex = rows.IndexOf("");
        
        var parsedRanges = new List<(long start, long end)>();
        foreach(var range in rows.Take(verticalSplitIndex))
        {
            var rangeEnds = range.Split('-').Select(long.Parse).ToArray();

            parsedRanges.Add((rangeEnds[0], rangeEnds[1]));
        }

        return rows.Skip(verticalSplitIndex + 1)
            .Select(long.Parse)
            .Count(id => parsedRanges.Any(range => range.start <= id && id <= range.end));
    }

    private static long GetPart2Answer(List<string> rows)
    {
        var sum = 0L;
        return sum;
    }
}
