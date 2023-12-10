using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        var startNodes = transitions.Keys.Where(k => k.EndsWith('A'));

        // After running the more general solution, it turns out all the starts loop with a simple period and offset
        //foreach (var start in startNodes)
        //{
        //    var loopingCountSequence = GetCountSequenceToFinishingNodes(start, sequence, transitions).Take(3);

        //    Console.WriteLine(string.Join(", ", loopingCountSequence));
        //}

        var loopLengths = startNodes.Select(start => GetCountToFirstFinishingNode(start, sequence, transitions));

        return loopLengths
            .Select(x => (long)x)
            .Aggregate(LeastCommonMultiple);
    }

    private static long LeastCommonMultiple(long first, long second)
    {
        return first / GreatestCommonDivisor(first, second) * second;
    }

    private static long GreatestCommonDivisor(long first, long second)
    {
        if (first < second)
        {
            (first, second) = (second, first);
        }

        while (second != 0)
        {
            var remainder = first % second;
            (first, second) = (second, remainder);
        }

        return first;
    }

    private static int GetCountToFirstFinishingNode(string startNode,
        string instructionSequence,
        Dictionary<string, (string left, string right)> transitions)
    {
        var current = startNode;
        var stepCount = 0;

        while (!current.EndsWith('Z'))
        {
            var instruction = instructionSequence[stepCount % instructionSequence.Length];

            current = instruction == 'L'
                ? transitions[current].left
                : transitions[current].right;

            stepCount++;
        }

        return stepCount;
    }

    private static IEnumerable<int> GetCountSequenceToFinishingNodes(string startNode,
        string instructionSequence,
        Dictionary<string, (string left, string right)> transitions)
    {
        var nextNode = startNode;
        var stepCount = 0;
        var nextInstructionIndex = 0;
        var visited = new Dictionary<string, int>();
        var finishingNodes = new List<(string node, int stepReached)>();

        while (!visited.ContainsKey(nextNode + nextInstructionIndex))
        {
            visited[nextNode + nextInstructionIndex] = stepCount;
            if (nextNode.EndsWith('Z'))
            {
                finishingNodes.Add((nextNode, stepCount));
            }

            var instruction = instructionSequence[stepCount % instructionSequence.Length];

            nextNode = instruction == 'L'
                ? transitions[nextNode].left
                : transitions[nextNode].right;

            stepCount++;
            nextInstructionIndex = stepCount % instructionSequence.Length;
        }

        // Loop likely isn't looping all the way to the beginning
        var firstTimeReached = visited[nextNode + nextInstructionIndex];
        var lengthOfLoop = stepCount - firstTimeReached;

        // I didn't see these conditions guaranteed in the statement, but they simplify the logic a good bit
        Debug.Assert(finishingNodes.Count == 1);
        Debug.Assert(lengthOfLoop == finishingNodes[0].stepReached);

        var repeatedFinishers = finishingNodes.Where(n => n.stepReached >= firstTimeReached).ToList();

        for (var i = 0;; i++)
        {
            foreach (var (finishingNode, stepReached) in repeatedFinishers)
            {
                var nextStepReached = stepReached + lengthOfLoop * i;
                yield return nextStepReached;
            }
        }
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