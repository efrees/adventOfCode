using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2023.Solvers;

internal class Day06Solver : ISolver
{
    private const string Name = "Day 6";
    private const string InputFile = "day06input.txt";

    public void Solve()
    {
        Console.WriteLine(Name);
        var lines = Input.GetLinesFromFile(InputFile).ToList();

        Console.WriteLine($"Output (part 1): {GetPart1Answer(lines)}");
        Console.WriteLine($"Output (part 2): {GetPart2Answer(lines)}");
    }

    private static long GetPart1Answer(List<string> lines)
    {
        var raceDurations = lines[0].SplitRemovingEmpty(' ').Skip(1).Select(int.Parse).ToList();
        var recordDistances = lines[1].SplitRemovingEmpty(' ').Skip(1).Select(int.Parse).ToList();

        return raceDurations.Zip(recordDistances, (duration, record) => CountWaysOfWinning(duration, record))
            .MultiplyAll();
    }

    private static long GetPart2Answer(List<string> lines)
    {
        var raceDuration = CollectDigitsAsLong(lines[0]);
        var recordDistance = CollectDigitsAsLong(lines[1]);

        return CountWaysOfWinning(raceDuration, recordDistance);
    }

    private static int CountWaysOfWinning(long duration, long record)
    {
        var winCount = 0;

        var chargeTime = duration / 2;
        while (chargeTime * (duration - chargeTime) > record)
        {
            winCount += 2;
            chargeTime--;
        }

        //Even has odd number of wins, and we double-counted one above
        if (duration % 2 == 0)
        {
            winCount--;
        }

        return winCount;
    }

    private static long CollectDigitsAsLong(string line)
    {
        return long.Parse(new string(line.Where(char.IsDigit).ToArray()));
    }
}