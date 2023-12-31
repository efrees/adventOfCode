using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode2023.Solvers;

internal class Day14Solver : ISolver
{
    private const string Name = "Day 14";
    private const string InputFile = "day14input.txt";

    public void Solve()
    {
        Console.WriteLine(Name);
        var lines = Input.GetLinesFromFile(InputFile).ToList();

        Console.WriteLine($"Output (part 1): {GetPart1Answer(lines)}");
        Console.WriteLine($"Output (part 2): {GetPart2Answer(lines)}");
    }

    private static long GetPart1Answer(List<string> lines)
    {
        var emptyCellCountsAboveCurrentRow = new int[lines[0].Length];

        var totalLoad = 0;
        var currentRowScore = lines.Count;
        foreach (var line in lines)
        {
            for (var i = 0; i < line.Length; i++)
            {
                if (line[i] == '.')
                {
                    emptyCellCountsAboveCurrentRow[i]++;
                }
                else if (line[i] == '#')
                {
                    emptyCellCountsAboveCurrentRow[i] = 0;
                }
                else if (line[i] == 'O')
                {
                    totalLoad += currentRowScore + emptyCellCountsAboveCurrentRow[i];
                }
            }

            currentRowScore--;
        }

        return totalLoad;
    }

    private static long GetPart2Answer(List<string> lines)
    {
        var grid = ParseDenseGrid(lines);
        var rows = grid.GetLength(0);
        var cols = grid.GetLength(1);

        //In reality, the input is square, but this shouldn't be required
        var temporaryTransposedGrid = new char[cols, rows];

        var statesSeen = new Dictionary<string, int>();

        var targetSpinCount = 1_000_000_000;
        for (var i = 0; i < targetSpinCount; i++)
        {
            SpinCycle(grid, temporaryTransposedGrid);

            // Watch for loops
            var state = GetFlatString(grid);

            if (statesSeen.TryGetValue(state, out var previousOccurrence))
            {
                var loopLength = i - previousOccurrence;
                var loopCount = (targetSpinCount - i) / loopLength;
                i += loopCount * loopLength;
            }

            statesSeen[state] = i;
        }

        return ComputeNorthLoad(grid);
    }

    private static char[,] ParseDenseGrid(List<string> lines)
    {
        var grid = new char[lines.Count, lines[0].Length];
        for (var i = 0; i < lines.Count; i++)
        {
            for (var j = 0; j < lines[i].Length; j++)
            {
                grid[i, j] = lines[i][j];
            }
        }

        return grid;
    }

    private static string GetFlatString(char[,] grid)
    {
        var builder = new StringBuilder();
        var enumerator = grid.GetEnumerator();
        while (enumerator.MoveNext())
        {
            builder.Append(enumerator.Current);
        }

        return builder.ToString();
    }

    private static void SpinCycle(char[,] grid, char[,] tempGrid)
    {
        TiltNorth(grid);
        RotateCW(grid, tempGrid);
        TiltNorth(tempGrid);
        RotateCW(tempGrid, grid);
        TiltNorth(grid);
        RotateCW(grid, tempGrid);
        TiltNorth(tempGrid);
        RotateCW(tempGrid, grid);
    }

    private static void TiltNorth(char[,] grid)
    {
        var rows = grid.GetLength(0);
        var cols = grid.GetLength(1);

        var emptyCellCountsAboveCurrentRow = new int[cols];
        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < cols; j++)
            {
                var cell = grid[i, j];

                if (cell == '.')
                {
                    emptyCellCountsAboveCurrentRow[j]++;
                }
                else if (cell == '#')
                {
                    emptyCellCountsAboveCurrentRow[j] = 0;
                }
                else if (cell == 'O' && emptyCellCountsAboveCurrentRow[j] != 0)
                {
                    var newRow = i - emptyCellCountsAboveCurrentRow[j];
                    grid[newRow, j] = grid[i, j];
                    grid[i, j] = '.';
                }
            }
        }
    }

    private static long ComputeNorthLoad(char[,] grid)
    {
        var rows = grid.GetLength(0);
        var cols = grid.GetLength(1);

        var totalLoad = 0;
        var currentRowScore = rows;

        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < cols; j++)
            {
                if (grid[i, j] == 'O')
                {
                    totalLoad += currentRowScore;
                }
            }

            currentRowScore--;
        }

        return totalLoad;
    }

    private static void RotateCW(char[,] grid, char[,] nextGrid)
    {
        var rows = grid.GetLength(0);
        var cols = grid.GetLength(1);

        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < cols; j++)
            {
                nextGrid[i, j] = grid[rows - j - 1, i];
            }
        }
    }
}