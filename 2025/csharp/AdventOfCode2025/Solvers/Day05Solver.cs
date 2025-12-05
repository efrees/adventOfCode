using System;
using System.Collections.Generic;
using System.Linq;

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
        
        var parsedRanges = ParseRanges(rows.Take(verticalSplitIndex));

        return rows.Skip(verticalSplitIndex + 1)
            .Select(long.Parse)
            .Count(id => parsedRanges.Any(range => range.start <= id && id <= range.end));
    }

    private static long GetPart2Answer(List<string> rows)
    {
        var verticalSplitIndex = rows.IndexOf("");
        
        var parsedRanges = ParseRanges(rows.Take(verticalSplitIndex))
            .OrderBy(range => range.start)
            .ThenBy(range => range.end)
            .ToList();

        var sum = 0L;
        var (currentStart, currentEnd) = parsedRanges.First();
        foreach(var range in parsedRanges.Skip(1))
        {
            if (range.start <= currentEnd)
            {
                // overlapping, so extend the range
                currentEnd = Math.Max(currentEnd, range.end);
            } else
            {
                // not overlapping, so (because they're sorted) we can count and move on
                sum += currentEnd - currentStart + 1;
                (currentStart, currentEnd) = range;
            }
        }
        
        sum += currentEnd - currentStart + 1;

        return sum;
    }

    private static List<(long start, long end)> ParseRanges(IEnumerable<string> inputRows)
    {
        return inputRows
            .Select(range => range.Split('-').Select(long.Parse).ToArray())
            .Select(rangeEnds => (rangeEnds[0], rangeEnds[1]))
            .ToList();
    }
}
