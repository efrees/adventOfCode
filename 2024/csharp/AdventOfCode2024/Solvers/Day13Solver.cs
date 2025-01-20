using AdventOfCode2024.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode2024.Solvers;

internal class Day13Solver : ISolver
{
    private const string Name = "Day 13";
    private const string InputFile = "day13input.txt";

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
        var games = ParseGames(lines);

        return games
            .Select(ComputeCheapestWin)
            .Sum();
    }

    private static int ComputeCheapestWin(Game game)
    {
        var lowestCost = int.MaxValue;
        var buttonATimes = game.Goal.X / game.IncrementA.X;
        while (buttonATimes >= 0)
        {
            var remainingIncrement = game.Goal.Subtract(game.IncrementA.TimesScalar(buttonATimes));
            var buttonBTimes = remainingIncrement.X / game.IncrementB.X;

            if (remainingIncrement == game.IncrementB.TimesScalar(buttonBTimes)
                && GetCost(buttonATimes, buttonBTimes) < lowestCost)
            {
                lowestCost = GetCost(buttonATimes, buttonBTimes);
            }

            buttonATimes--;
        }

        return lowestCost == int.MaxValue
            ? 0
            : lowestCost;
    }

    private static int GetCost(long buttonATimes, long buttonBTimes)
    {
        var buttonACost = 3;
        var buttonBCost = 1;

        return (int)(buttonACost * buttonATimes + buttonBCost * buttonBTimes);
    }

    private static long GetPart2Answer(List<string> lines)
    {
        return 0;
    }

    private static List<Game> ParseGames(List<string> lines)
    {
        var incrementPattern = new Regex(@"X[+=](\d+), Y[+=](\d+)");

        var games = new List<Game>();
        var nextGame = new Game();
        foreach (var line in lines.Where(l => !string.IsNullOrEmpty(l)))
        {
            var xyMatch = incrementPattern.Match(line);
            var point = new Point2D(int.Parse(xyMatch.Groups[1].Value), int.Parse(xyMatch.Groups[2].Value));

            if (line.StartsWith("Button A"))
            {
                nextGame.IncrementA = point;
            }
            else if (line.StartsWith("Button B"))
            {
                nextGame.IncrementB = point;
            }
            else if (line.StartsWith("Prize"))
            {
                nextGame.Goal = point;
                games.Add(nextGame);
                nextGame = new Game();
            }
        }

        return games;
    }

    private class Game
    {
        public Point2D IncrementA { get; set; }
        public Point2D IncrementB { get; set; }
        public Point2D Goal { get; set; }
    }
}