using AdventOfCode2024.Solvers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AdventOfCode2024;

public static class Program
{
    private static readonly IReadOnlyList<ISolver> Solvers = new ISolver[]
    {
        new Day01Solver(),
        new Day02Solver(),
        new Day03Solver(),
        new Day04Solver(),
        new Day05Solver(),
    };

    public static void Main()
    {
        ReportTime(SolveAll, "Total time:");
#if RUNTIMES
            ReportAllAverages();
#endif
    }

    private static void SolveAll()
    {
        foreach (var solver in Solvers)
        {
            ReportTime(solver.Solve);
        }
    }

    private static void ReportTime(Action action, string label = "Solved in")
    {
        var timeInMillis = TimeAction(action);
        Console.WriteLine($"{label} {timeInMillis / 1000:F9}s\n");
    }

    private static void ReportAllAverages()
    {
        const int repetitions = 5;
        Console.WriteLine("Beginning actual runs for average");

        var results = Solvers.Select((solver, i) => (day: i + 1, time: GetAverageTime(solver.Solve, repetitions))).ToList();

        Console.WriteLine($"------\nAverage Runtime in Seconds ({repetitions} attempts)");
        Console.WriteLine("Day\tC#");
        foreach (var (day, time) in results)
        {
            Console.WriteLine($"{day}\t{time / 1000:F9}");
        }
    }

    private static double GetAverageTime(Action action, int repetitions)
    {
        var times = new List<double>();
        for (var i = 0; i < repetitions; i++)
        {
            times.Add(TimeAction(action));
        }

        Console.WriteLine(string.Join(", ", times));
        return times.Average();
    }

    private static double TimeAction(Action action)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Restart();
        action();
        stopwatch.Stop();
        return stopwatch.Elapsed.TotalMilliseconds;
    }
}