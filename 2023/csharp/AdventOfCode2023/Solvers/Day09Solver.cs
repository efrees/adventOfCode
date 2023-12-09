using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2023.Solvers;

internal class Day09Solver : ISolver
{
    private const string Name = "Day 9";
    private const string InputFile = "day09input.txt";

    public void Solve()
    {
        Console.WriteLine(Name);
        var lines = Input.GetLinesFromFile(InputFile).ToList();

        Console.WriteLine($"Output (part 1): {GetPart1Answer(lines)}");
        Console.WriteLine($"Output (part 2): {GetPart2Answer(lines)}");
    }

    private static long GetPart1Answer(List<string> lines)
    {
        var sequences = lines.Select(l => l.SplitRemovingEmpty().Select(int.Parse).ToList());

        var nextNumbers = sequences.Select(ExtrapolateSequence);

        return nextNumbers.Sum();
    }

    private static int ExtrapolateSequence(List<int> sequence)
    {
        var diffSequence = sequence.Zip(sequence.Skip(1), (first, second) => second - first).ToList();

        if (diffSequence.All(diff => diff == 0))
        {
            return sequence.Last();
        }

        var nextDiff = ExtrapolateSequence(diffSequence);
        return sequence.Last() + nextDiff;
    }

    private static long GetPart2Answer(List<string> lines)
    {
        var sequences = lines.Select(l => l.SplitRemovingEmpty().Select(int.Parse).ToList());

        var nextNumbers = sequences.Select(ExtrapolateSequenceBackward);

        return nextNumbers.Sum();
    }

    private static int ExtrapolateSequenceBackward(List<int> sequence)
    {
        var diffSequence = sequence.Zip(sequence.Skip(1), (first, second) => second - first).ToList();

        if (diffSequence.All(diff => diff == 0))
        {
            return sequence.First();
        }

        var nextDiff = ExtrapolateSequenceBackward(diffSequence);
        return sequence.First() - nextDiff;
    }
}