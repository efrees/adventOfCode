using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2023.Solvers;

internal class Day13Solver : ISolver
{
    private const string Name = "Day 13";
    private const string InputFile = "day13input.txt";

    public void Solve()
    {
        Console.WriteLine(Name);
        var lines = Input.GetLinesFromFile(InputFile).ToList();

        Console.WriteLine($"Output (part 1): {GetPart1Answer(lines)}");
        Console.WriteLine($"Output (part 2): {GetPart2Answer(lines)}");
    }

    private static long GetPart1Answer(List<string> lines)
    {
        var image = new List<string>();
        var sum = 0;

        foreach (var line in lines)
        {
            if (!string.IsNullOrEmpty(line))
            {
                image.Add(line);
                continue;
            }

            sum += GetReflectionSummary(image);
            image.Clear();
        }

        if (image.Any())
        {
            sum += GetReflectionSummary(image);
        }

        return sum;
    }

    private static int GetReflectionSummary(List<string> image)
    {
        var reflectionRowIndex = FindVerticalReflection(image);
        if (reflectionRowIndex >= 0)
        {
            return reflectionRowIndex * 100;
        }

        var transposedImage = Transpose(image);

        return FindVerticalReflection(transposedImage);
    }

    private static int FindVerticalReflection(List<string> image)
    {
        for (var i = 1; i < image.Count; i++)
        {
            var allMatch = true;
            for (var j = 0; allMatch && i - j - 1 >= 0 && i + j < image.Count; j++)
            {
                var preImage = image[i - j - 1];
                var postImage = image[i + j];
                if (preImage != postImage)
                {
                    allMatch = false;
                }
            }

            if (allMatch)
            {
                return i;
            }
        }

        return -1;
    }

    private static long GetPart2Answer(List<string> lines)
    {
        var image = new List<string>();
        var sum = 0;

        foreach (var line in lines)
        {
            if (!string.IsNullOrEmpty(line))
            {
                image.Add(line);
                continue;
            }

            sum += GetReflectionSummaryWithSingleError(image);
            image.Clear();
        }

        if (image.Any())
        {
            sum += GetReflectionSummaryWithSingleError(image);
        }

        return sum;
    }

    private static int GetReflectionSummaryWithSingleError(List<string> image)
    {
        var reflectionRowIndex = FindVerticalReflectionWithSingleError(image);
        if (reflectionRowIndex >= 0)
        {
            return reflectionRowIndex * 100;
        }

        var transposedImage = Transpose(image);

        return FindVerticalReflectionWithSingleError(transposedImage);
    }

    private static int FindVerticalReflectionWithSingleError(List<string> image)
    {
        for (var i = 1; i < image.Count; i++)
        {
            var errorCount = 0;
            for (var j = 0; errorCount <= 1 && i - j - 1 >= 0 && i + j < image.Count; j++)
            {
                var preImage = image[i - j - 1];
                var postImage = image[i + j];
                if (preImage != postImage)
                {
                    errorCount += CountDifferencesWithMaxOfTwo(preImage, postImage);
                }
            }

            if (errorCount == 1)
            {
                return i;
            }
        }

        return -1;
    }

    private static int CountDifferencesWithMaxOfTwo(string preImage, string postImage)
    {
        var diffCount = 0;
        for (var i = 0; diffCount < 2 && i < preImage.Length; i++)
        {
            if (preImage[i] != postImage[i])
            {
                diffCount++;
            }
        }

        return diffCount;
    }

    private static List<string> Transpose(List<string> image)
    {
        var newImage = new List<string>();

        for (var i = 0; i < image[0].Length; i++)
        {
            var columnAsRow = new string(image.Select(row => row[i]).ToArray());
            newImage.Add(columnAsRow);
        }

        return newImage;
    }
}