using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode2023.Solvers;

internal class Day03Solver : ISolver
{
    private const string Name = "Day 3";
    private const string InputFile = "day03input.txt";

    public void Solve()
    {
        Console.WriteLine(Name);
        var lines = Input.GetLinesFromFile(InputFile).ToList();

        Console.WriteLine($"Output (part 1): {GetPart1Answer(lines)}");
        Console.WriteLine($"Output (part 2): {GetPart2Answer(lines)}");
    }

    private static long GetPart1Answer(List<string> lines)
    {
        var sumOfPartNumbers = 0;

        var numberPattern = new Regex(@"\d+");
        var symbolLocations = GetSymbolLocations(lines).ToHashSet();

        var row = 0;
        foreach (var line in lines)
        {
            foreach (Match numberMatch in numberPattern.Matches(line))
            {
                if (GetSurroundingCoordinates(row, numberMatch.Index, numberMatch.Length).Intersect(symbolLocations).Any())
                {
                    var partNumber = Convert.ToInt32(numberMatch.Value);
                    sumOfPartNumbers += partNumber;
                }
            }

            row++;
        }

        return sumOfPartNumbers;
    }

    private static IEnumerable<(int x, int y)> GetSurroundingCoordinates(int row, int numberMatchIndex, int numberMatchLength)
    {
        for (var y = row - 1; y <= row + 1; y++)
        {
            for (var x = numberMatchIndex - 1; x <= numberMatchIndex + numberMatchLength; x++)
            {
                yield return (x, y);
            }
        }
    }

    private static IEnumerable<(int x, int y)> GetSymbolLocations(List<string> lines)
    {
        var symbolPattern = new Regex(@"[^.0-9]");
        var y = 0;

        foreach (var line in lines)
        {
            var symbolLocationsInLine = symbolPattern.Matches(line).Select(m => m.Index);
            foreach (var matchLocation in symbolLocationsInLine)
            {
                yield return (matchLocation, y);
            }

            y++;
        }
    }

    private static long GetPart2Answer(List<string> lines)
    {
        var numberPattern = new Regex(@"\d+");
        var potentialGearConnections = GetSymbolLocations(lines).ToDictionary(x => x, _ => new List<int>());

        var row = 0;
        foreach (var line in lines)
        {
            foreach (Match numberMatch in numberPattern.Matches(line))
            {
                var partNumber = Convert.ToInt32(numberMatch.Value);
                foreach (var potentialGearLocation in GetSurroundingCoordinates(row, numberMatch.Index, numberMatch.Length)
                             .Where(potentialGearConnections.ContainsKey))
                {
                    potentialGearConnections[potentialGearLocation].Add(partNumber);
                }
            }

            row++;
        }

        var sumOfGearRatios = potentialGearConnections.Values
            .Where(x => x.Count == 2)
            .Select(x => x[0] * x[1])
            .Sum();
        return sumOfGearRatios;
    }
}