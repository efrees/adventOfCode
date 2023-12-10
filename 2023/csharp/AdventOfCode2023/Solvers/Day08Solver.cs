using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode2023.Solvers;

internal class Day08Solver : ISolver
{
    private const string Name = "Day 8";
    private const string InputFile = "day08input.txt";

    public void Solve()
    {
        Console.WriteLine(Name);
        var lines = Input.GetLinesFromFile(InputFile).ToList();

        Console.WriteLine($"Output (part 1): {GetPart1Answer(lines)}");
        Console.WriteLine($"Output (part 2): {GetPart2Answer(lines)}");
    }

    private static long GetPart1Answer(List<string> lines)
    {
        var sequence = lines.First();

        var transitions = ParseTransitions(lines);

        var current = "AAA";
        var stepCount = 0;

        while (current != "ZZZ")
        {
            var instruction = sequence[stepCount % sequence.Length];

            current = instruction == 'L'
                ? transitions[current].left
                : transitions[current].right;
            stepCount++;
        }

        return stepCount;
    }

    private static long GetPart2Answer(List<string> lines)
    {
        var sequence = lines.First();

        var transitions = ParseTransitions(lines);

        var current = transitions.Keys.Where(k => k.EndsWith('A')).ToList();
        var stepCount = 0L;
        var allEndInZ = false;

        while (!allEndInZ)
        {
            var instruction = sequence[(int)(stepCount % sequence.Length)];

            allEndInZ = true;
            for (var i = 0; i < current.Count; i++)
            {
                current[i] = instruction == 'L'
                    ? transitions[current[i]].left
                    : transitions[current[i]].right;

                if (!current[i].EndsWith('Z'))
                {
                    allEndInZ = false;
                }
            }

            stepCount++;
        }

        return stepCount;
    }

    private static Dictionary<string, (string left, string right)> ParseTransitions(List<string> lines)
    {
        var transitionPattern = new Regex(@"(?<source>\w{3}) = \((?<left>\w{3}), (?<right>\w{3})\)");
        var transitions = new Dictionary<string, (string left, string right)>();

        foreach (var line in lines.Skip(2))
        {
            var match = transitionPattern.Match(line);
            var source = match.Groups["source"].Value;
            var left = match.Groups["left"].Value;
            var right = match.Groups["right"].Value;

            transitions[source] = (left, right);
        }

        return transitions;
    }
}