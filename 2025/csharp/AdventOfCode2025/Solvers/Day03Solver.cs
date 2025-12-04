using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2025.Solvers;

internal class Day03Solver : ISolver
{
    private const string Name = "Day 3";
    private const string InputFile = "day03input.txt";

    public void Solve()
    {
        Console.WriteLine(Name);
        var lines = Input.GetLinesFromFile(InputFile)
            .ToList();

        Console.WriteLine($"Output (part 1): {GetPart1Answer(lines)}");
        Console.WriteLine($"Output (part 2): {GetPart2Answer(lines)}");
    }

    private static long GetPart1Answer(List<string> banks)
    {
        var sum = 0;
        foreach (var bank in banks)
        {
            var firstDigit = '0';
            var secondDigit = bank[^1];
            for (var i = bank.Length - 2; i >= 0; i--)
            {
                if (bank[i] >= firstDigit)
                {
                    if (firstDigit > secondDigit)
                    {
                        secondDigit = firstDigit;
                    }
                    firstDigit = bank[i];
                }
            }

            sum += (firstDigit - '0') * 10 + (secondDigit - '0');
        }
        return sum;
    }

    private static long GetPart2Answer(List<string> banks)
    {
        var sum = 0L;
        foreach (var bank in banks)
        {
            //var selectedDigits = 0L;

            //var lastSelectedDigitIndex = -1;
            //for (var d = 0; d < 12; d++)
            //{
            //    var maxDigit = '0';
            //    var maxIndex = -1;
            //    for (var i = lastSelectedDigitIndex + 1; i <= bank.Length + d - 12; i++)
            //    {
            //        if (bank[i] > maxDigit)
            //        {
            //            maxDigit = bank[i];
            //            maxIndex = i;
            //        }
            //    }

            //    selectedDigits *= 10;
            //    selectedDigits += maxDigit - '0';
            //    lastSelectedDigitIndex = maxIndex;
            //}
             var selectedDigits = bank[^12..].ToArray();
            for (var i = bank.Length - 13; i >= 0; i--)
            {
                var insertingDigit = bank[i];

                var j = 0;
                while (j < selectedDigits.Length && insertingDigit >= selectedDigits[j])
                {
                    (insertingDigit, selectedDigits[j]) = (selectedDigits[j], insertingDigit);
                    j++;
                }
            }

            sum += long.Parse(selectedDigits);
        }
        return sum;
    }
}
