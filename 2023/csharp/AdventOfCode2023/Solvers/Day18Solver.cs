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
        var digInstructions = lines.Select(line => line.Split(' '))
            .Select(splits =>
            {
                var direction = splits[0][0];
                var distance = int.Parse(splits[1]);
                return (direction, distance);
            });
        return ComputeEnclosedArea(digInstructions);
    }

    private static long GetPart2Answer(List<string> lines)
    {
        var digInstructions = lines
            .Select(line => line.Split(' ')[2])
            .Select(colorString => colorString[2..^1])
            .Select(hexDigits => (
                direction: hexDigits[5] switch
                {
                    '0' => 'R',
                    '1' => 'D',
                    '2' => 'L',
                    '3' => 'U',
                    _ => throw new ArgumentOutOfRangeException()
                },
                distance: int.Parse(hexDigits[..5], NumberStyles.HexNumber)
            ));

        return ComputeEnclosedArea(digInstructions);
    }

    private static long ComputeEnclosedArea(IEnumerable<(char direction, int distance)> digInstructions)
    {
        var totalArea = 1L;
        var currentCutDepth = 1L;

        foreach (var (direction, distance) in digInstructions)
        {
            switch (direction)
            {
                case 'R':
                    currentCutDepth += distance;
                    totalArea += distance;
                    break;
                case 'D':
                    totalArea += currentCutDepth * distance;
                    break;
                case 'L':
                    currentCutDepth -= distance;
                    break;
                case 'U':
                    totalArea -= (currentCutDepth - 1) * distance;
                    break;
            }
        }

        return totalArea;
    }
}