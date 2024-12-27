using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2024.Solvers;

internal class Day07Solver : ISolver
{
    private const string Name = "Day 7";
    private const string InputFile = "day07input.txt";

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
        var totalValid = 0L;
        foreach (var rawEquation in lines)
        {
            var equationSplit = rawEquation.Split(':');
            var testValue = long.Parse(equationSplit[0]);
            var numbers = equationSplit[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray();

            var operatorCount = numbers.Length - 1;

            for (var opMap = 0; opMap < 1 << operatorCount; opMap++)
            {
                if (testValue == CalculateWithTwoOperators(numbers, opMap))
                {
                    totalValid += testValue;
                    break;
                }
            }
        }

        return totalValid;
    }

    private static long CalculateWithTwoOperators(long[] numbers, int opMap)
    {
        var result = numbers[0];
        for (var i = 0; i < numbers.Length - 1; i++)
        {
            var opFlag = opMap & (1 << i);
            if (opFlag > 0)
            {
                result *= numbers[i + 1];
            }
            else
            {
                result += numbers[i + 1];
            }
        }

        return result;
    }

    private static long GetPart2Answer(List<string> lines)
    {
        var totalValid = 0L;
        foreach (var rawEquation in lines)
        {
            var equationSplit = rawEquation.Split(':');
            var testValue = long.Parse(equationSplit[0]);
            var numbers = equationSplit[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray();

            var operatorCount = numbers.Length - 1;

            for (var opMap = 0; opMap < Math.Pow(3, operatorCount); opMap++)
            {
                if (testValue == CalculateWithThreeOperators(numbers, opMap))
                {
                    totalValid += testValue;
                    break;
                }
            }
        }

        return totalValid;
    }

    private static long CalculateWithThreeOperators(long[] numbers, int opMap)
    {
        var result = numbers[0];
        for (var i = 0; i < numbers.Length - 1; i++)
        {
            var opId = opMap % 3;
            opMap /= 3;
            if (opId == 2)
            {
                if (long.TryParse(result.ToString() + numbers[i + 1], out var validLong))
                {
                    result = validLong;
                }
                else
                {
                    return -1;
                }
            }
            else if (opId == 1)
            {
                result *= numbers[i + 1];
            }
            else
            {
                result += numbers[i + 1];
            }
        }

        return result;
    }
}