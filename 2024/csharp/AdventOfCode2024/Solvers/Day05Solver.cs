using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2024.Solvers;

internal class Day05Solver : ISolver
{
    private const string Name = "Day 5";
    private const string InputFile = "day05input.txt";

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
        var middleSum = 0;
        var dependentPages = GetDependentPageRules(lines);

        foreach (var update in lines.SkipWhile(line => line != string.Empty).Skip(1))
        {
            var isValid = true;
            var parsedUpdate = update.Split(',').Select(int.Parse).ToList();
            var earlierPagesInUpdate = new HashSet<int>();
            foreach (var page in parsedUpdate)
            {
                if (dependentPages.ContainsKey(page) && dependentPages[page].Intersect(earlierPagesInUpdate).Any())
                {
                    isValid = false;
                    break;
                }

                earlierPagesInUpdate.Add(page);
            }

            if (isValid)
            {
                middleSum += parsedUpdate.Middle();
            }
        }

        return middleSum;
    }

    private static long GetPart2Answer(List<string> lines)
    {
        var middleSum = 0;
        var dependentPages = GetDependentPageRules(lines);

        foreach (var update in lines.SkipWhile(line => line != string.Empty).Skip(1))
        {
            var isValid = true;
            var parsedUpdate = update.Split(',').Select(int.Parse).ToList();
            var earlierPagesInUpdate = new HashSet<int>();
            foreach (var page in parsedUpdate)
            {
                if (dependentPages.ContainsKey(page) && dependentPages[page].Intersect(earlierPagesInUpdate).Any())
                {
                    isValid = false;
                    break;
                }

                earlierPagesInUpdate.Add(page);
            }

            if (!isValid)
            {
                var fixedUpdate = CorrectUpdate(parsedUpdate, dependentPages);
                middleSum += fixedUpdate.Middle();
            }
        }

        return middleSum;
    }

    private static Dictionary<int, HashSet<int>> GetDependentPageRules(List<string> lines)
    {
        var dependentPages = new Dictionary<int, HashSet<int>>();

        var lineIndex = 0;
        while (lines[lineIndex] != string.Empty)
        {
            var rule = lines[lineIndex].Split('|').Select(int.Parse).ToArray();
            if (dependentPages.ContainsKey(rule[0]))
            {
                dependentPages[rule[0]].Add(rule[1]);
            }
            else
            {
                dependentPages[rule[0]] = [rule[1]];
            }

            lineIndex++;
        }

        return dependentPages;
    }

    private static List<int> CorrectUpdate(List<int> parsedUpdate, Dictionary<int, HashSet<int>> dependentPages)
    {
        var emptySet = new HashSet<int>();
        var fixedUpdate = new List<int>();
        foreach (var page in parsedUpdate)
        {
            var pagesAfterThis = dependentPages.GetValueOrDefault(page, emptySet);
            var insertPosition = 0;
            while (insertPosition < fixedUpdate.Count
                   && !pagesAfterThis.Contains(fixedUpdate[insertPosition]))
            {
                insertPosition++;
            }

            fixedUpdate.Insert(insertPosition, page);
        }

        return fixedUpdate;
    }
}