using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2024.Solvers;

internal class Day04Solver : ISolver
{
    private const string Name = "Day 4";
    private const string InputFile = "day04input.txt";

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
        // Search for reversed word, too, so we have half as many directions and associated boundary cases to watch.
        var word = "XMAS";
        var wordReversed = "SAMX";
        var foundCount = 0;
        for (var i = 0; i < lines.Count; i++)
        {
            for (var j = 0; j < lines[i].Length; j++)
            {
                if (lines[i][j] == 'X')
                {
                    var hasSpaceUp = i >= 3;
                    var hasSpaceRight = j + 3 < lines[i].Length;
                    var hasSpaceDown = i + 3 < lines.Count;

                    var directionsValid = new[]
                    {
                        hasSpaceUp && hasSpaceRight,
                        hasSpaceRight,
                        hasSpaceRight && hasSpaceDown,
                        hasSpaceDown
                    };
                    for (var k = 0; k < word.Length; k++)
                    {
                        if (directionsValid[0] && word[k] != lines[i - k][j + k])
                        {
                            directionsValid[0] = false;
                        }

                        if (directionsValid[1] && word[k] != lines[i][j + k])
                        {
                            directionsValid[1] = false;
                        }

                        if (directionsValid[2] && word[k] != lines[i + k][j + k])
                        {
                            directionsValid[2] = false;
                        }

                        if (directionsValid[3] && word[k] != lines[i + k][j])
                        {
                            directionsValid[3] = false;
                        }
                    }

                    foundCount += directionsValid.Count(x => x);
                }

                if (lines[i][j] == 'S')
                {
                    var hasSpaceUp = i >= 3;
                    var hasSpaceRight = j + 3 < lines[i].Length;
                    var hasSpaceDown = i + 3 < lines.Count;

                    var directionsValid = new[]
                    {
                        hasSpaceUp && hasSpaceRight,
                        hasSpaceRight,
                        hasSpaceRight && hasSpaceDown,
                        hasSpaceDown
                    };
                    for (var k = 0; k < wordReversed.Length; k++)
                    {
                        if (directionsValid[0] && wordReversed[k] != lines[i - k][j + k])
                        {
                            directionsValid[0] = false;
                        }

                        if (directionsValid[1] && wordReversed[k] != lines[i][j + k])
                        {
                            directionsValid[1] = false;
                        }

                        if (directionsValid[2] && wordReversed[k] != lines[i + k][j + k])
                        {
                            directionsValid[2] = false;
                        }

                        if (directionsValid[3] && wordReversed[k] != lines[i + k][j])
                        {
                            directionsValid[3] = false;
                        }
                    }

                    foundCount += directionsValid.Count(x => x);
                }
            }
        }

        return foundCount;
    }

    private static long GetPart2Answer(List<string> lines)
    {
        var foundCount = 0;

        for (var i = 1; i < lines.Count - 1; i++)
        {
            for (var j = 1; j < lines[i].Length - 1; j++)
            {
                // Look for the middle 'A' and then make sure we have two M's and two S's, not alternating
                if (lines[i][j] == 'A')
                {
                    var corners = new[]
                    {
                        lines[i - 1][j - 1],
                        lines[i - 1][j + 1],
                        lines[i + 1][j + 1],
                        lines[i + 1][j - 1]
                    };

                    foundCount += new string(corners) switch
                    {
                        "MMSS" or "MSSM" or "SSMM" or "SMMS" => 1,
                        _ => 0
                    };
                }
            }
        }

        return foundCount;
    }
}