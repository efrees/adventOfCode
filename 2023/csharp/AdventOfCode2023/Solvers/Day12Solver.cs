using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2023.Solvers;

internal class Day12Solver : ISolver
{
    private const string Name = "Day 12";
    private const string InputFile = "day12input.txt";

    public void Solve()
    {
        Console.WriteLine(Name);
        var lines = Input.GetLinesFromFile(InputFile).ToList();

        Console.WriteLine($"Output (part 1): {GetPart1Answer(lines)}");
        Console.WriteLine($"Output (part 2): {GetPart2Answer(lines)}");
    }

    private static long GetPart1Answer(List<string> lines)
    {
        return lines.Select(CountConsistentPossibilities).Sum();
    }

    private static long GetPart2Answer(List<string> lines)
    {
        return -1;
    }

    private static int CountConsistentPossibilities(string line)
    {
        var splits = line.Split(' ');

        var pattern = splits[0];
        var blockSizes = splits[1].Split(',').Select(x => Convert.ToInt32(x)).ToArray();

        var constraint = GetConstraintBitMap(pattern);
        var constraintMask = GetMaskOfKnown(pattern);

        var totalSizeOfGaps = pattern.Length - blockSizes.Sum();

        // 1. Distribute the slack among the boundaries in all possible ways.
        // 2. Construct the resulting value and compare to the constraint.
        var successCount = 0;

        // Any gap at the beginning is implicit here. The bitmap will have zeros there anyway, and leaving
        // the first gap implicit keeps the arrays conveniently the same size and reduces the number of invalid
        // combinations we have to generate.
        var combinationsOfGapSizes = GetCombinationsOfGapSizes(totalSizeOfGaps, blockSizes.Length).ToList();
        foreach (var gapSizes in combinationsOfGapSizes)
        {
            var valueToTest = ConstructBitMapFromOnOffSequence(blockSizes, gapSizes);

            if ((valueToTest & constraintMask) == constraint)
            {
                successCount++;
            }
        }

        return successCount;
    }

    private static IEnumerable<int[]> GetCombinationsOfGapSizes(int totalSizeOfGaps, int numberOfGroupings)
    {
        if (numberOfGroupings == 1)
        {
            for (var current = 0; current <= totalSizeOfGaps; current++)
            {
                yield return new[] { current };
            }

            yield break;
        }

        //All but last gap has to be non-zero
        for (var current = 1; current <= totalSizeOfGaps; current++)
        {
            var remainingGapCombinations = GetCombinationsOfGapSizes(totalSizeOfGaps - current, numberOfGroupings - 1);
            foreach (var subCombination in remainingGapCombinations)
            {
                yield return new[] { current }.Concat(subCombination).ToArray();
            }
        }
    }

    private static int ConstructBitMapFromOnOffSequence(int[] blockSizes, int[] gapSizes)
    {
        var setBits = 0;
        for (var i = 0; i < blockSizes.Length; i++)
        {
            var blockSize = blockSizes[i];
            var gapSize = gapSizes[i];

            for (var j = 0; j < blockSize; j++)
            {
                setBits <<= 1;
                setBits |= 1;
            }

            setBits <<= gapSize;
        }

        return setBits;
    }

    private static int GetConstraintBitMap(string pattern)
    {
        var setBits = 0;
        foreach (var c in pattern)
        {
            setBits <<= 1;
            if (c == '#')
            {
                setBits |= 1;
            }
        }

        return setBits;
    }

    private static int GetMaskOfKnown(string pattern)
    {
        var mask = 0;
        foreach (var c in pattern)
        {
            mask <<= 1;
            if (c != '?')
            {
                mask |= 1;
            }
        }

        return mask;
    }
}