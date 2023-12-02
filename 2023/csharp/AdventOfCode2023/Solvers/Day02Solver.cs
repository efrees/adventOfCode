using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode2023.Solvers;

internal class Day02Solver : ISolver
{
    private const string Name = "Day 2";
    private const string InputFile = "day02input.txt";

    private static readonly Dictionary<string, int> MaxConstraints = new Dictionary<string, int>
    {
        { "red", 12 },
        { "green", 13 },
        { "blue", 14 },
    };

    public void Solve()
    {
        Console.WriteLine(Name);
        var lines = Input.GetLinesFromFile(InputFile).ToList();

        Console.WriteLine($"Output (part 1): {GetPart1Answer(lines)}");
        Console.WriteLine($"Output (part 2): {GetPart2Answer(lines)}");
    }

    private static long GetPart1Answer(List<string> lines)
    {
        var sumOfPossibleIds = 0;
        var gameIdPattern = new Regex(@"Game (\d+):");
        var countPattern = new Regex(@" (?<count>\d+) (?<color>red|green|blue)");
        foreach (var line in lines)
        {
            var gameId = Convert.ToInt32(gameIdPattern.Match(line).Groups[1].Value);
            var gameContents = line.Substring(line.IndexOf(':') + 1);

            var isPossible = countPattern.Matches(gameContents)
                .Select(countMatch => (color: countMatch.Groups["color"].Value, count: countMatch.Groups["count"].Value))
                .All(pair => Convert.ToInt32(pair.count) <= MaxConstraints[pair.color]);
            if (isPossible)
            {
                sumOfPossibleIds += gameId;
            }
        }

        return sumOfPossibleIds;
    }

    private static long GetPart2Answer(List<string> lines)
    {
        var sumOfPowers = 0;
        var countPattern = new Regex(@" (?<count>\d+) (?<color>red|green|blue)");
        foreach (var line in lines)
        {
            var gameContents = line.Substring(line.IndexOf(':') + 1);

            var maxByColor = countPattern.Matches(gameContents)
                .Select(countMatch => (color: countMatch.Groups["color"].Value, count: countMatch.Groups["count"].Value))
                .GroupBy(pair => pair.color)
                .Select(group => group.Select(g => int.Parse(g.count)).Max());
            var power = maxByColor.Aggregate((product, next) => product * next);
            sumOfPowers += power;
        }

        return sumOfPowers;
    }
}