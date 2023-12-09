using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2023.Solvers;

internal class Day07Solver : ISolver
{
    private const string Name = "Day 7";
    private const string InputFile = "day07input.txt";

    public void Solve()
    {
        Console.WriteLine(Name);
        var lines = Input.GetLinesFromFile(InputFile).ToList();

        Console.WriteLine($"Output (part 1): {GetPart1Answer(lines)}");
        Console.WriteLine($"Output (part 2): {GetPart2Answer(lines)}");
    }

    private static long GetPart1Answer(List<string> lines)
    {
        var hands = lines
            .Select(l => l.Split(' '))
            .Select(rawPair => (cards: rawPair[0], bid: int.Parse(rawPair[1])))
            .OrderBy(GetHandType)
            .ThenBy(GetCardValuesAsBase13)
            .ToList();

        return hands
            .Select((hand, index) => hand.bid * (index + 1))
            .Sum();
    }

    private static long GetPart2Answer(List<string> lines)
    {
        var hands = lines
            .Select(l => l.Split(' '))
            .Select(rawPair => (cards: rawPair[0], bid: int.Parse(rawPair[1])))
            .OrderBy(GetHandTypeWithJokers)
            .ThenBy(GetCardValuesAsBase13WithJokers)
            .ToList();

        return hands
            .Select((hand, index) => hand.bid * (index + 1))
            .Sum();
    }

    private static int GetHandType((string cards, int bid) hand)
    {
        var cardCounts = hand.cards
            .GroupBy(c => c)
            .Select(group => group.Count())
            .OrderDescending()
            .ToArray();

        return GetHandTypeFromCounts(cardCounts);
    }

    private static int GetHandTypeWithJokers((string cards, int bid) hand)
    {
        var cardGroups = hand.cards.ToLookup(c => c);
        var jokerCount = cardGroups['J'].Count();
        var cardCounts = cardGroups
            .Where(group => group.Key != 'J')
            .Select(group => group.Count())
            .OrderDescending()
            .DefaultIfEmpty(0)
            .ToList();

        cardCounts[0] += jokerCount;

        return GetHandTypeFromCounts(cardCounts);
    }

    private static int GetHandTypeFromCounts(IList<int> cardCounts)
    {
        // types: 5, 4, (3,2), (3), (2,2), 2, 1
        var biggestCount = cardCounts.First();
        if (biggestCount >= 4)
        {
            return biggestCount + 1;
        }

        var nextCount = cardCounts[1];

        return (biggestCount, nextCount) switch
        {
            (3, 2) => 4,
            (3, 1) => 3,
            (2, 2) => 2,
            (2, 1) => 1,
            (1, 1) => 0
        };
    }

    private static int GetCardValuesAsBase13((string cards, int bid) hand)
    {
        var value = 0;
        foreach (var digit in hand.cards.Select(GetDigit))
        {
            value *= 13;
            value += digit;
        }

        return value;
    }

    private static int GetDigit(char card)
    {
        return card switch
        {
            'A' => 12,
            'K' => 11,
            'Q' => 10,
            'J' => 9,
            'T' => 8,
            _ => card - '0' - 2
        };
    }

    private static int GetCardValuesAsBase13WithJokers((string cards, int bid) hand)
    {
        var value = 0;
        foreach (var digit in hand.cards.Select(GetDigitWithJokers))
        {
            value *= 13;
            value += digit;
        }

        return value;
    }

    private static int GetDigitWithJokers(char card)
    {
        return card switch
        {
            'A' => 12,
            'K' => 11,
            'Q' => 10,
            'J' => 0,
            'T' => 9,
            _ => card - '0' - 1
        };
    }
}