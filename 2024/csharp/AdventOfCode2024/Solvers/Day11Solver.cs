using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2024.Solvers;

internal class Day11Solver : ISolver
{
    private const string Name = "Day 11";
    private const string InputFile = "day11input.txt";

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
        var lineOfNumbers = lines.First().Split(' ').Select(long.Parse).ToList();

        var blinkCount = 25;
        while (blinkCount > 0)
        {
            lineOfNumbers = GetNextEvolution(lineOfNumbers).ToList();
            blinkCount--;
        }

        return lineOfNumbers.Count;
    }

    private static IEnumerable<long> GetNextEvolution(IEnumerable<long> lineOfNumbers)
    {
        foreach (var number in lineOfNumbers)
        {
            if (number == 0)
            {
                yield return 1;
            }
            else if (number.ToString().Length % 2 == 0)
            {
                var digits = number.ToString();
                yield return long.Parse(digits[..(digits.Length / 2)]);
                yield return long.Parse(digits[(digits.Length / 2)..]);
            }
            else
            {
                yield return number * 2024;
            }
        }
    }

    private static long GetPart2Answer(List<string> lines)
    {
        return 0;
    }
}