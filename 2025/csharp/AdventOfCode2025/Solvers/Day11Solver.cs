using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2025.Grid;

namespace AdventOfCode2025.Solvers;

internal class Day11Solver : ISolver
{
    private const string Name = "Day 11";
    private const string InputFile = "day11input.txt";

    public void Solve()
    {
        Console.WriteLine(Name);
        var lines = Input.GetLinesFromFile(InputFile).ToList();

        Console.WriteLine($"Output (part 1): {GetPart1Answer(lines)}");
        Console.WriteLine($"Output (part 2): {GetPart2Answer(lines)}");
    }

    private static long GetPart1Answer(List<string> rows)
    {
        var connections = rows.Select(row => row.Split(": "))
            .ToDictionary(parts => parts[0], parts => parts[1].Split(' '));

        var start = "you";

        return GetDfsPathsFromTo(start, "out", connections);
    }

    private static long GetDfsPathsFromTo(
        string current,
        string target,
        Dictionary<string, string[]> connections
    )
    {
        if (current == target)
        {
            return 1;
        }

        return connections[current].Sum(next => GetDfsPathsFromTo(next, target, connections));
    }

    private static long GetPart2Answer(List<string> rows)
    {
        var connections = rows.Select(row => row.Split(": "))
            .ToDictionary(parts => parts[0], parts => parts[1].Split(' '));

        var start = "svr";

        var requirements = new List<string>() { "fft", "dac" };
        return GetDfsPathsFromToWithRequirements([start], "out", requirements, connections);
    }

    private static Dictionary<(string, bool, bool), long> searchMemos = new();

    private static long GetDfsPathsFromToWithRequirements(
        List<string> currentPath,
        string target,
        List<string> requirements,
        Dictionary<string, string[]> connections
    )
    {
        var current = currentPath.Last();
        if (current == target)
        {
            if (requirements.All(req => currentPath.Contains(req)))
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        var hasFirstReq = currentPath.Contains(requirements[0]);
        var hasSecondReq = currentPath.Contains(requirements[1]);

        var memoKey = (current, hasFirstReq, hasSecondReq);
        if (searchMemos.ContainsKey(memoKey))
        {
            return searchMemos[memoKey];
        }

        var totalSubPaths = connections[current]
            .Where(next => !currentPath.Contains(next))
            .Sum(next =>
                GetDfsPathsFromToWithRequirements(
                    currentPath.Append(next).ToList(),
                    target,
                    requirements,
                    connections
                )
            );
        searchMemos[memoKey] = totalSubPaths;
        return totalSubPaths;
    }
}
