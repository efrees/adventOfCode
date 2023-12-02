using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode2023.Solvers;

internal class Day01Solver : ISolver
{
    private const string Name = "Day 1";
    private const string InputFile = "day01input.txt";

    public void Solve()
    {
        Console.WriteLine(Name);
        var lines = Input.GetLinesFromFile(InputFile).ToList();

        Console.WriteLine($"Output (part 1): {GetPart1Answer(lines)}");
        Console.WriteLine($"Output (part 2): {GetPart2Answer(lines)}");
    }

    private static long GetPart1Answer(List<string> lines)
    {
        var firstDigits = lines.Select(line => line.First(char.IsDigit));
        var lastDigits = lines.Select(line => line.Last(char.IsDigit));

        return firstDigits
            .Zip(lastDigits, (first, last) => Convert.ToInt32($"{first}{last}"))
            .Sum();
    }

    private static long GetPart2Answer(List<string> lines)
    {
        // Just getting the whole list of matches with one Regex misses the overlapping ones
        var firstDigitPattern = new Regex("[1-9]|one|two|three|four|five|six|seven|eight|nine");
        var lastDigitPattern = new Regex("[1-9]|one|two|three|four|five|six|seven|eight|nine", RegexOptions.RightToLeft);

        var firstDigits = lines.Select(line => firstDigitPattern.Match(line).Value).Select(GetDigit);
        var lastDigits = lines.Select(line => lastDigitPattern.Match(line).Value).Select(GetDigit);

        return firstDigits
            .Zip(lastDigits, (first, last) => Convert.ToInt32($"{first}{last}"))
            .Sum();
    }

    private static long GetDigit(string digit)
    {
        return digit switch
        {
            "one" => 1,
            "two" => 2,
            "three" => 3,
            "four" => 4,
            "five" => 5,
            "six" => 6,
            "seven" => 7,
            "eight" => 8,
            "nine" => 9,
            var other => Convert.ToInt32(other)
        };
    }
}