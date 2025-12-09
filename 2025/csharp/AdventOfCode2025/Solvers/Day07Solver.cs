using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2025.Solvers;

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

    private static long GetPart1Answer(List<string> rows)
    {
        var totalSplits = 0;

        var currentBeams = rows.First().Replace('S', '|').ToCharArray();
        var nextBeams = new char[currentBeams.Length];

        foreach (var row in rows.Skip(1))
        {
            for (var i = 0; i < row.Length; i++)
            {
                if (currentBeams[i] == '|')
                {
                    if (row[i] == '^')
                    {
                        totalSplits++;
                        nextBeams[i - 1] = '|';
                        nextBeams[i + 1] = '|';
                    }
                    else
                    {
                        nextBeams[i] = '|';
                    }
                }
            }

            currentBeams = nextBeams;
            nextBeams = new char[currentBeams.Length];
        }

        return totalSplits;
    }

    private static long GetPart2Answer(List<string> rows)
    {
        var pathsBelow = Enumerable.Repeat(1L, rows.First().Length).ToArray();
        var nextCounts = new long[pathsBelow.Length];

        for (var i = rows.Count - 1; i >= 0; i--)
        {
            for (var j = 0; j < nextCounts.Length; j++)
            {
                if (rows[i][j] == '^')
                {
                    nextCounts[j] = pathsBelow[j - 1] + pathsBelow[j + 1];
                }
                else
                {
                    nextCounts[j] = pathsBelow[j];
                }

            }

            pathsBelow = nextCounts;
            nextCounts = new long[pathsBelow.Length];
        }

        var beamStartIndex = rows.First().IndexOf('S');

        return pathsBelow[beamStartIndex];
    }
}
