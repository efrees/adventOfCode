using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode2023.Solvers;

internal class Day19Solver : ISolver
{
    private const string Name = "Day 19";
    private const string InputFile = "day19input.txt";

    public void Solve()
    {
        Console.WriteLine(Name);
        var lines = Input.GetLinesFromFile(InputFile).ToList();

        Console.WriteLine($"Output (part 1): {GetPart1Answer(lines)}");
        Console.WriteLine($"Output (part 2): {GetPart2Answer(lines)}");
    }

    private static long GetPart1Answer(List<string> lines)
    {
        var workflows = new Dictionary<string, Workflow>();

        var index = 0;
        while (lines[index] != string.Empty)
        {
            if (lines[index] == string.Empty)
            {
                break;
            }

            var workflow = Workflow.Parse(lines[index]);
            workflows[workflow.Name] = workflow;
            index++;
        }

        var acceptedTotal = 0;
        var partPattern = new Regex(@"{x=(\d+),m=(\d+),a=(\d+),s=(\d+)}");
        while (++index < lines.Count)
        {
            var match = partPattern.Match(lines[index]);
            var partValues = match.Groups.Values.Skip(1).Select(g => int.Parse(g.Value)).ToArray();
            var part = (x: partValues[0], m: partValues[1], a: partValues[2], s: partValues[3]);
            var workflowName = "in";

            while (workflowName is not "R" and not "A")
            {
                var workflow = workflows[workflowName];
                workflowName = workflow.ApplyToPart(part);
            }

            if (workflowName == "A")
            {
                acceptedTotal += part.x + part.m + part.a + part.s;
            }
        }

        return acceptedTotal;
    }

    private static long GetPart2Answer(List<string> lines)
    {
        return -1;
    }

    private class Workflow
    {
        public string Name { get; init; }
        public List<SortRule> Rules { get; init; } = new List<SortRule>();

        public static Workflow Parse(string input)
        {
            var ruleBlockIndex = input.IndexOf('{');
            return new Workflow
            {
                Name = input.Substring(0, ruleBlockIndex),
                Rules = input.Substring(ruleBlockIndex + 1).Split(',').Select(SortRule.Parse).ToList()
            };
        }

        public string ApplyToPart((int x, int m, int a, int s) part)
        {
            return Rules.First(rule => rule.Condition(part)).Destination;
        }
    }

    private class SortRule
    {
        public Func<(int x, int m, int a, int s), bool> Condition { get; init; }
        public string Destination { get; init; }

        public static SortRule Parse(string input)
        {
            if (input.EndsWith('}'))
            {
                return new SortRule
                {
                    Condition = _ => true,
                    Destination = input[..^1],
                };
            }

            var variable = input[0];
            var op = input[1];
            var colonIndex = input.IndexOf(':');
            var filterValue = int.Parse(input[2..colonIndex]);
            var destination = input[(colonIndex + 1)..];

            return new SortRule
            {
                Condition = part => (variable, op) switch
                {
                    ('x', '>') => part.x > filterValue,
                    ('x', '<') => part.x < filterValue,
                    ('m', '>') => part.m > filterValue,
                    ('m', '<') => part.m < filterValue,
                    ('a', '>') => part.a > filterValue,
                    ('a', '<') => part.a < filterValue,
                    ('s', '<') => part.s < filterValue,
                    ('s', '>') => part.s > filterValue,
                },
                Destination = destination,
            };
        }
    }
}
