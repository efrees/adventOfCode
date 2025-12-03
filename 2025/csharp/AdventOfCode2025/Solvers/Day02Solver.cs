using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2025.Solvers;

internal class Day02Solver : ISolver
{
    private const string Name = "Day 2";
    private const string InputFile = "day02input.txt";

    public void Solve()
    {
        Console.WriteLine(Name);
        var lines = Input.GetLinesFromFile(InputFile)
            .SelectMany(line => line.Split(','))
            .Select(range => range.Split('-').Select(long.Parse).ToArray())
            .ToList();

        Console.WriteLine($"Output (part 1): {GetPart1Answer(lines)}");
        Console.WriteLine($"Output (part 2): {GetPart2Answer(lines)}");
    }

    private static long GetPart1Answer(List<long[]> ranges)
    {
        var sum = 0L;
        foreach (var range in ranges)
        {
            var start = range[0];
            var end = range[1];

            // Take off too many digits, rather than too few, for odd counts.
            var trailingDigits = (1 + DigitsIn(start)) / 2;

            var pattern = DecimalShiftRight(start, trailingDigits);

            var lastTry = 0L;
            while (lastTry < end)
            {
                lastTry = DecimalShiftLeft(pattern, DigitsIn(pattern)) + pattern;

                if (start <= lastTry && lastTry <= end)
                {
                    sum += lastTry;
                }
                pattern++;
            }
        }
        return sum;
    }

    private static long GetPart2Answer(List<long[]> ranges)
    {
        var invalidSet = new HashSet<long>();
        foreach (var range in ranges)
        {
            var start = range[0];
            var end = range[1];

            var maxPatternDigits = DigitsIn(end) / 2;

            var pattern = 1L;

            var patternDigits = 1;
            while (patternDigits <= maxPatternDigits)
            {
                var lastTry = pattern;
                while (lastTry < end)
                {
                    lastTry = DecimalShiftLeft(lastTry, patternDigits) + pattern;

                    if (start <= lastTry && lastTry <= end)
                    {
                        invalidSet.Add(lastTry);
                    }
                }
                pattern++;
                patternDigits = DigitsIn(pattern);
            }
        }
        return invalidSet.Sum();

    }

    private static int DigitsIn(long num)
    {
        var count = 0;

        while (num > 0)
        {
            num /= 10;
            count++;
        }
        return count;
    }

    private static long DecimalShiftLeft(long num, int shift)
    {
        while (shift > 0)
        {
            num *= 10;
            shift--;
        }
        return num;
    }

    private static long DecimalShiftRight(long num, int shift)
    {
        while (shift > 0)
        {
            num /= 10;
            shift--;
        }
        return num;
    }
}
