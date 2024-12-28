using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2024.Solvers;

internal class Day09Solver : ISolver
{
    private const string Name = "Day 9";
    private const string InputFile = "day09input.txt";

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
        var diskMap = lines.First();

        var checksum = 0L;
        var processingIndex = 1; //First file is zeros in the checksum anyway
        var movingFileIndex = (diskMap.Length + 1) / 2 * 2;

        var overallBlockPosition = diskMap[0] - '0';
        var leftoverBlocksToMove = 0;
        while (processingIndex < movingFileIndex)
        {
            var spaceToFill = diskMap[processingIndex] - '0';

            while (spaceToFill > 0 && movingFileIndex > processingIndex)
            {
                if (leftoverBlocksToMove == 0)
                {
                    movingFileIndex -= 2;
                    leftoverBlocksToMove = diskMap[movingFileIndex] - '0';
                }

                checksum += overallBlockPosition * (movingFileIndex / 2);
                overallBlockPosition++;
                leftoverBlocksToMove--;
                spaceToFill--;
            }

            processingIndex++;

            if (processingIndex < movingFileIndex)
            {
                var unmovedBlocks = diskMap[processingIndex] - '0';
                while (unmovedBlocks > 0)
                {
                    checksum += overallBlockPosition * (processingIndex / 2);
                    overallBlockPosition++;
                    unmovedBlocks--;
                }
            }

            processingIndex++;
        }

        while (leftoverBlocksToMove > 0)
        {
            checksum += overallBlockPosition * (movingFileIndex / 2);
            overallBlockPosition++;
            leftoverBlocksToMove--;
        }

        return checksum;
    }

    private static long GetPart2Answer(List<string> lines)
    {
        var diskMap = lines.First();

        if (diskMap.Length % 2 == 0)
        {
            diskMap = diskMap[..^1];
        }

        var checksum = 0L;
        var processingIndex = 1; //First file is zeros in the checksum anyway
        var movedFiles = new HashSet<int>();

        var overallBlockPosition = diskMap[0] - '0';
        while (processingIndex < diskMap.Length)
        {
            if (processingIndex % 2 == 1)
            {
                var spaceSize = diskMap[processingIndex] - '0';
                for (var fileIndex = diskMap.Length - 1; fileIndex > processingIndex && spaceSize > 0; fileIndex -= 2)
                {
                    var fileSize = diskMap[fileIndex] - '0';
                    if (fileSize > spaceSize || movedFiles.Contains(fileIndex))
                    {
                        continue;
                    }

                    while (fileSize > 0)
                    {
                        checksum += overallBlockPosition * (fileIndex / 2);
                        overallBlockPosition++;
                        fileSize--;
                        spaceSize--;
                    }

                    movedFiles.Add(fileIndex);
                }

                overallBlockPosition += spaceSize; //Any unfillable space
            }
            else
            {
                var fileSize = diskMap[processingIndex] - '0';
                if (movedFiles.Contains(processingIndex))
                {
                    overallBlockPosition += fileSize;
                }
                else
                {
                    while (fileSize > 0)
                    {
                        checksum += overallBlockPosition * (processingIndex / 2);
                        overallBlockPosition++;
                        fileSize--;
                    }
                }
            }

            processingIndex++;
        }

        return checksum;
    }
}