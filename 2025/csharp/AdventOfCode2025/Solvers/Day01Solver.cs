using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2025.Solvers;

internal class Day01Solver : ISolver
{
    private const string Name = "Day 1";
    private const string InputFile = "day01input.txt";

    public void Solve()
    {
        Console.WriteLine(Name);
        var lines = Input.GetLinesFromFile(InputFile)
            .ToList();

        Console.WriteLine($"Output (part 1): {GetPart1Answer(lines)}");
        Console.WriteLine($"Output (part 2): {GetPart2Answer(lines)}");
    }

    private static long GetPart1Answer(List<string> turns)
    {
        var currentValue = 50;
        var zeroCount = 0;
        foreach (var turn in turns)
        {
            var direction = turn[0];
            var turnAmount = int.Parse(turn[1..]);

            if (direction == 'L')
            {
                turnAmount = -turnAmount;
            }

            currentValue = (100 + currentValue + turnAmount) % 100;

            if (currentValue == 0)
            {
                zeroCount++;
            }
        }

        return zeroCount;
    }

    private static long GetPart2Answer(List<string> turns)
    {
        var currentValue = 50;
        var zeroCount = 0;
        foreach (var turn in turns)
        {
            var direction = turn[0];
            var turnAmount = int.Parse(turn[1..]);

            var fullCycles = Math.Abs(turnAmount / 100);
            if (fullCycles > 0)
            {
                zeroCount += fullCycles;
                turnAmount %= 100;
            }

            if (direction == 'L')
            {
                turnAmount = -turnAmount;
            }

            var newUnwrappedValue = currentValue + turnAmount;

            if (newUnwrappedValue >= 100
                || (newUnwrappedValue < 0 && currentValue > 0)
                || newUnwrappedValue is -100 or 0 && turnAmount != 0)
            {
                zeroCount++;
            }

            currentValue = (100 + newUnwrappedValue) % 100;
        }

        return zeroCount;
    }
}
