using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2023.Solvers;

internal class Day15Solver : ISolver
{
    private const string Name = "Day 15";
    private const string InputFile = "day15input.txt";

    public void Solve()
    {
        Console.WriteLine(Name);
        var lines = Input.GetLinesFromFile(InputFile).ToList();

        Console.WriteLine($"Output (part 1): {GetPart1Answer(lines)}");
        Console.WriteLine($"Output (part 2): {GetPart2Answer(lines)}");
    }

    private static long GetPart1Answer(List<string> lines)
    {
        var line = lines.First();

        return line.Split(',')
            .Select(ComputeHash)
            .Sum();
    }

    private static long GetPart2Answer(List<string> lines)
    {
        var instructions = lines.First().Split(',');

        var boxes = Enumerable.Range(0, 256).Select(_ => new LensBox()).ToArray();

        foreach (var instruction in instructions)
        {
            var label = instruction.Split('-', '=').First();
            var labelLength = label.Length;
            var op = instruction[labelLength];
            var boxNumber = ComputeHash(label);

            if (op == '-')
            {
                boxes[boxNumber].Remove(label);
            }
            else if (op == '=')
            {
                var focus = int.Parse(instruction.Substring(labelLength + 1));
                boxes[boxNumber].Add(label, focus);
            }
        }

        return boxes.Select((box, index) => (index + 1) * box.GetPartialFocusingPower())
            .Sum();
    }

    private static long ComputeHash(string input)
    {
        var currentValue = 0;
        foreach (var c in input)
        {
            currentValue += c;
            currentValue *= 17;
            currentValue %= 256;
        }

        return currentValue;
    }

    private class LensBox
    {
        private List<(string label, int focus)> Lenses { get; } = new();

        public void Remove(string label)
        {
            Lenses.RemoveAll(lens => lens.label == label);
        }

        public void Add(string label, int focus)
        {
            var existingIndex = Lenses.FindIndex(lens => lens.label == label);
            if (existingIndex >= 0)
            {
                Lenses[existingIndex] = (label, focus);
            }
            else
            {
                Lenses.Add((label, focus));
            }
        }

        public int GetPartialFocusingPower()
        {
            return Lenses.Select((lens, index) => (index + 1) * lens.focus).Sum();
        }
    }
}