using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2025.Solvers;

internal class Day06Solver : ISolver
{
    private const string Name = "Day 6";
    private const string InputFile = "day06input.txt";

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
        var operators = rows.Last().Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var parsedRows = rows
            .Take(rows.Count - 1)
            .Select(row =>
                    row.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                       .Select(int.Parse)
                       .ToArray()
            ).ToList();

        var grandTotal = 0L;
        for (int prob = 0; prob < operators.Length; prob++)
        {
            long result = operators[prob] == "*" ? 1 : 0;
            foreach (var valueRow in parsedRows)
            {
                if (operators[prob] == "*")
                {
                    result *= valueRow[prob];
                }
                else if (operators[prob] == "+")
                {
                    result += valueRow[prob];
                }
            }

            grandTotal += result;
        }

        return grandTotal;
    }

    private static long GetPart2Answer(List<string> rows)
    {
        var grandTotal = 0L;
        var maxLength = rows.Select(r => r.Length).Max();
        var numRows = rows.Count;
        var operands = new List<long>();
        var operatorChar = '\0';
        for (var index = maxLength - 1; index >= 0; index--)
        {
            var operandBuffer = 0;

            var allSpaces = true;
            for (var j = 0; j < numRows; j++)
            {
                if (index >= rows[j].Length)
                {
                    continue;
                }

                var ch = rows[j][index];

                if (ch == ' ')
                {
                    continue;
                }

                allSpaces = false;

                if (ch is '*' or '+')
                {
                    operatorChar = ch;
                }
                else // assume numeric
                {
                    operandBuffer *= 10;
                    operandBuffer += ch - '0';
                }
            }

            if (!allSpaces)
            {
                operands.Add(operandBuffer);
            }

            if (allSpaces || index == 0)
            {
                if (operatorChar == '*')
                {
                    grandTotal += operands.Aggregate((agg, next) => agg * next);
                }
                else if (operatorChar == '+')
                {
                    grandTotal += operands.Sum();
                }
                operands.Clear();
                operatorChar = '\0';
            }
        }
        return grandTotal;
    }
}
