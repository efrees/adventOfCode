using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2023.Solvers;

internal class Day04Solver : ISolver
{
    private const string Name = "Day 4";
    private const string InputFile = "day04input.txt";

    public void Solve()
    {
        Console.WriteLine(Name);
        var lines = Input.GetLinesFromFile(InputFile).ToList();

        Console.WriteLine($"Output (part 1): {GetPart1Answer(lines)}");
        Console.WriteLine($"Output (part 2): {GetPart2Answer(lines)}");
    }

    private static long GetPart1Answer(List<string> lines)
    {
        var sumOfWinningCardScores = 0;

        foreach (var line in lines)
        {
            var myWinningNumbers = GetWinningNumbersFromCard(line);

            sumOfWinningCardScores += myWinningNumbers.Any()
                ? 1 << (myWinningNumbers.Count - 1)
                : 0;
        }

        return sumOfWinningCardScores;
    }

    private static long GetPart2Answer(List<string> lines)
    {
        var cardCounts = new int[lines.Count];
        for (var card = 0; card < lines.Count; card++)
        {
            cardCounts[card]++;
            var myWinningNumbers = GetWinningNumbersFromCard(lines[card]);

            for (var offset = 1; offset <= myWinningNumbers.Count; offset++)
            {
                cardCounts[card + offset] += cardCounts[card];
            }
        }

        return cardCounts.Sum();
    }

    private static List<string> GetWinningNumbersFromCard(string line)
    {
        var cardHalves = line.Substring(line.IndexOf(':') + 1).Split('|');

        var allWinningNumbers = cardHalves[0].SplitRemovingEmpty(' ');
        var myWinningNumbers = cardHalves[1].SplitRemovingEmpty(' ').Intersect(allWinningNumbers).ToList();
        return myWinningNumbers;
    }
}