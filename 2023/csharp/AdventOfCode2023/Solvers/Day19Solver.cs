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
        var workflows = ParseWorkflowsSection(lines);

        var index = workflows.Count;

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
        var workflows = ParseWorkflowsSection(lines);

        var startWorkflow = "in";
        var fullPartRange = new PartRange();

        var searchQueue = new Queue<(string workflow, PartRange partRange)>();
        searchQueue.Enqueue((startWorkflow, fullPartRange));

        var acceptedCombinations = 0L;

        while (searchQueue.Any())
        {
            var (workflowName, range) = searchQueue.Dequeue();

            if (workflowName == "A")
            {
                acceptedCombinations += range.GetNumberOfCombinations();
            }
            else if (workflowName != "R")
            {
                var workflow = workflows[workflowName];
                var rangesToProcess = workflow.ApplyRulesToSplitRange(range);
                foreach (var targetedRange in rangesToProcess)
                {
                    searchQueue.Enqueue(targetedRange);
                }
            }
        }

        return acceptedCombinations;
    }

    private static Dictionary<string, Workflow> ParseWorkflowsSection(List<string> lines)
    {
        return lines
            .TakeWhile(line => line != string.Empty)
            .Select(Workflow.Parse)
            .ToDictionary(workflow => workflow.Name);
    }

    private class Workflow
    {
        public string Name { get; init; }
        public List<SortRule> Rules { get; init; } = new();

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
            return Rules.First(rule => rule.MatchesPart(part)).DestinationWorkflow;
        }

        public IEnumerable<(string workflow, PartRange matchingRange)> ApplyRulesToSplitRange(PartRange range)
        {
            // Split into non-overlapping ranges, based on each rule
            var remainingRange = range;
            foreach (var rule in Rules)
            {
                if (remainingRange == PartRange.Empty)
                {
                    break;
                }

                var (matchedRange, unmatchedRange) = rule.SplitRange(remainingRange);

                if (matchedRange != PartRange.Empty)
                {
                    yield return (rule.DestinationWorkflow, matchedRange);
                }

                remainingRange = unmatchedRange;
            }
        }
    }

    private class SortRule
    {
        public char Parameter { get; init; }
        public char Operator { get; init; }
        public int FilterValue { get; init; }
        public string DestinationWorkflow { get; init; }

        public static SortRule Parse(string input)
        {
            if (input.EndsWith('}'))
            {
                return new SortRule
                {
                    Parameter = '_',
                    DestinationWorkflow = input[..^1],
                };
            }

            var variable = input[0];
            var op = input[1];
            var colonIndex = input.IndexOf(':');
            var filterValue = int.Parse(input[2..colonIndex]);
            var destination = input[(colonIndex + 1)..];

            return new SortRule
            {
                Parameter = variable,
                Operator = op,
                FilterValue = filterValue,
                DestinationWorkflow = destination,
            };
        }

        public bool MatchesPart((int x, int m, int a, int s) part)
        {
            if (Parameter == '_')
            {
                return true;
            }

            return (Parameter, Operator) switch
            {
                ('x', '>') => part.x > FilterValue,
                ('x', '<') => part.x < FilterValue,
                ('m', '>') => part.m > FilterValue,
                ('m', '<') => part.m < FilterValue,
                ('a', '>') => part.a > FilterValue,
                ('a', '<') => part.a < FilterValue,
                ('s', '<') => part.s < FilterValue,
                ('s', '>') => part.s > FilterValue,
            };
        }

        public (PartRange matched, PartRange unmatched) SplitRange(PartRange partRange)
        {
            if (Parameter == '_')
            {
                return (partRange, PartRange.Empty);
            }

            var attributeRangeToSplit = Parameter switch
            {
                'x' => partRange.XRange,
                'm' => partRange.MRange,
                'a' => partRange.ARange,
                's' => partRange.SRange
            };

            var replacementRanges = SplitAttributeRangeByFilterValue(attributeRangeToSplit);
            var matched = replacementRanges.matched == (0, 0)
                ? PartRange.Empty
                : partRange.ReplaceAttributeRange(Parameter, replacementRanges.matched);
            var unmatched = replacementRanges.unmatched == (0, 0)
                ? PartRange.Empty
                : partRange.ReplaceAttributeRange(Parameter, replacementRanges.unmatched);
            return (matched, unmatched);
        }

        private ((int min, int max) matched, (int min, int max) unmatched) SplitAttributeRangeByFilterValue((int min, int max) range)
        {
            var emptyRange = (0, 0);

            if (Operator == '>')
            {
                var matched = range.max > FilterValue
                    ? (FilterValue + 1, range.max)
                    : emptyRange;
                var unmatched = range.min <= FilterValue
                    ? (range.min, FilterValue)
                    : emptyRange;
                return (matched, unmatched);
            }
            else
            {
                var matched = range.min < FilterValue
                    ? (range.min, FilterValue - 1)
                    : emptyRange;
                var unmatched = range.max >= FilterValue
                    ? (FilterValue, range.max)
                    : emptyRange;
                return (matched, unmatched);
            }
        }
    }

    private record PartRange
    {
        public static readonly PartRange Empty = new()
        {
            XRange = (0, -1),
            MRange = (0, -1),
            ARange = (0, -1),
            SRange = (0, -1),
        };

        public (int min, int max) XRange { get; init; } = (1, 4000);
        public (int min, int max) MRange { get; init; } = (1, 4000);
        public (int min, int max) ARange { get; init; } = (1, 4000);
        public (int min, int max) SRange { get; init; } = (1, 4000);

        public PartRange ReplaceAttributeRange(char attribute, (int min, int max) newRange)
        {
            return attribute switch
            {
                'x' => this with { XRange = newRange },
                'm' => this with { MRange = newRange },
                'a' => this with { ARange = newRange },
                's' => this with { SRange = newRange },
            };
        }

        public long GetNumberOfCombinations()
        {
            return (long)(XRange.max - XRange.min + 1)
                * (MRange.max - MRange.min + 1)
                * (ARange.max - ARange.min + 1)
                * (SRange.max - SRange.min + 1);
        }
    }
}