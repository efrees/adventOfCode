using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode2024.Solvers;

internal class Day03Solver : ISolver
{
    private const string Name = "Day 3";
    private const string InputFile = "day03input.txt";

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
        var pattern = new Regex(@"mul\((\d+),(\d+)\)");

        return lines.SelectMany(line => pattern.Matches(line))
            .Select(match => int.Parse(match.Groups[1].Value) * int.Parse(match.Groups[2].Value))
            .Sum();
    }

    private static long GetPart2Answer(List<string> lines)
    {
        var fullMemory = string.Join("", lines);

        var dos = new Regex(@"do\(\)");
        var donts = new Regex(@"don't\(\)");
        var pattern = new Regex(@"mul\((\d+),(\d+)\)");

        var enablePositions = dos.Matches(fullMemory).Select(match => (match.Index, true)).Prepend((0, true));
        var disablePositions = donts.Matches(fullMemory).Select(match => (match.Index, false));

        var controlSwitches =
            new Queue<(int index, bool enable)>(enablePositions.Concat(disablePositions).OrderBy(control => control.Item1));
        var currentControl = controlSwitches.Dequeue();
        var sum = 0;

        foreach (Match mulMatch in pattern.Matches(fullMemory))
        {
            while (controlSwitches.Count > 0 && controlSwitches.Peek().index < mulMatch.Index)
            {
                currentControl = controlSwitches.Dequeue();
            }

            if (currentControl.enable)
            {
                sum += int.Parse(mulMatch.Groups[1].Value) * int.Parse(mulMatch.Groups[2].Value);
            }
        }

        return sum;
    }
}