using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode2023.Solvers;

internal class Day05Solver : ISolver
{
    private const string Name = "Day 5";
    private const string InputFile = "day05input.txt";

    public void Solve()
    {
        Console.WriteLine(Name);
        var lines = Input.GetLinesFromFile(InputFile).ToList();

        Console.WriteLine($"Output (part 1): {GetPart1Answer(lines)}");
        Console.WriteLine($"Output (part 2): {GetPart2Answer(lines)}");
    }

    private static long GetPart1Answer(List<string> lines)
    {
        var locationNumbers = new List<long>();

        var seeds = ParseSeeds(lines.First());
        var maps = ParseRangeMaps(lines);

        foreach (var seed in seeds)
        {
            var mappedType = "seed";
            var mappedValue = seed;

            while (mappedType != "location")
            {
                var mapper = maps[mappedType];

                mappedValue = mapper.Map(mappedValue);
                mappedType = mapper.ToType;
            }

            locationNumbers.Add(mappedValue);
        }

        return locationNumbers.Min();
    }

    private static IEnumerable<long> ParseSeeds(string seedsLine)
    {
        return seedsLine.Substring("seeds: ".Length)
            .Split(' ')
            .Select(long.Parse);
    }

    private static Dictionary<string, RangeMapper> ParseRangeMaps(List<string> lines)
    {
        var maps = new Dictionary<string, RangeMapper>();
        var mapTitlePattern = new Regex(@"^(?<leftResource>\w+)-to-(?<rightResource>\w+) map:");

        RangeMapper currentMapper = default;
        for (var lineIndex = 1; lineIndex < lines.Count; lineIndex++)
        {
            var line = lines[lineIndex];
            if (string.IsNullOrEmpty(line))
            {
                lineIndex++;
                line = lines[lineIndex];

                var titleMatch = mapTitlePattern.Match(line);
                var sourceType = titleMatch.Groups["leftResource"].Value;
                currentMapper = new RangeMapper(sourceType, titleMatch.Groups["rightResource"].Value);
                maps[sourceType] = currentMapper;

                continue;
            }

            var rangeSplits = line.Split(' ').Select(long.Parse).ToList();
            var sourceMin = rangeSplits[1];
            var shift = rangeSplits[0] - sourceMin;
            currentMapper.Ranges.Add((sourceMin, sourceMin + rangeSplits[2], shift));
        }

        return maps;
    }

    private static long GetPart2Answer(List<string> lines)
    {
        return 0;
    }

    private class RangeMapper(string fromType, string toType)
    {
        public string FromType { get; set; } = fromType;
        public string ToType { get; set; } = toType;

        public List<(long sourceMin, long sourceMax, long shift)> Ranges { get; set; } = new();

        public long Map(long inputValue)
        {
            var matchingRange = Ranges.FirstOrDefault(range => inputValue >= range.sourceMin && inputValue <= range.sourceMax);

            return matchingRange != default
                ? inputValue + matchingRange.shift
                : inputValue;
        }
    }
}