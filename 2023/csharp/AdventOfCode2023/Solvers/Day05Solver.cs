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
        var seeds = ParseSeeds(lines.First());
        var maps = ParseRangeMaps(lines);

        return seeds.Select(seed => MapSeedToLocation(seed, maps))
            .Min();
    }

    private static long GetPart2Answer(List<string> lines)
    {
        var seedRanges = ParseSeedsAsRanges(lines.First()).ToList();
        var maps = ParseRangeMaps(lines);
        var reverseLookup = maps.Values.ToDictionary(map => map.ToType);

        var currentMap = reverseLookup["location"];

        while (currentMap.FromType != "seed")
        {
            var previous = reverseLookup[currentMap.FromType];

            currentMap = FlattenRangeMaps(previous, currentMap);
        }

        var lowestLocationRanges = currentMap.Ranges.OrderBy(r => r.sourceMin + r.shift);

        // Assumes (naively) that the lowest value will actually come from a mapped point, not an unmapped.

        foreach (var mappedRange in lowestLocationRanges)
        {
            var overlappingSeedRanges = seedRanges
                .Select(seedRange => (
                    start: Math.Max(seedRange.start, mappedRange.sourceMin),
                    end: Math.Min(seedRange.end, mappedRange.sourceMax)
                ))
                .Where(overlap => overlap.start < overlap.end);
            if (overlappingSeedRanges.Any())
            {
                return overlappingSeedRanges.First().start + mappedRange.shift;
            }
        }

        return -1;
    }

    private static long MapSeedToLocation(long seed, Dictionary<string, RangeMapper> maps)
    {
        var mappedType = "seed";
        var mappedValue = seed;

        while (mappedType != "location")
        {
            var mapper = maps[mappedType];

            mappedValue = mapper.Map(mappedValue);
            mappedType = mapper.ToType;
        }

        return mappedValue;
    }

    private static IEnumerable<long> ParseSeeds(string seedsLine)
    {
        return seedsLine.Substring("seeds: ".Length)
            .Split(' ')
            .Select(long.Parse);
    }

    private static IEnumerable<(long start, long end)> ParseSeedsAsRanges(string seedsLine)
    {
        var seedNumbers = seedsLine.Substring("seeds: ".Length)
            .Split(' ')
            .Select(long.Parse)
            .ToList();

        for (var i = 0; i < seedNumbers.Count; i += 2)
        {
            var seedCount = seedNumbers[i + 1];
            yield return (seedNumbers[i], seedNumbers[i] + seedCount);
        }
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

    private static RangeMapper FlattenRangeMaps(RangeMapper leftMapper, RangeMapper rightMapper)
    {
        //Flatten the two mappers by splitting ranges
        var flattenedMap = new RangeMapper(leftMapper.FromType, rightMapper.ToType);
        var orderedIntermediateRanges = rightMapper.Ranges.OrderBy(r => r.sourceMin).ToList();

        foreach (var left in leftMapper.Ranges)
        {
            var leftDestination = (min: left.sourceMin + left.shift, max: left.sourceMax + left.shift);

            var overlappingIntermediates = orderedIntermediateRanges
                .Where(r => r.sourceMax > leftDestination.min && r.sourceMin < leftDestination.max).ToList();
            foreach (var right in overlappingIntermediates)
            {
                if (leftDestination.min < right.sourceMin)
                {
                    flattenedMap.Ranges.Add((leftDestination.min - left.shift, right.sourceMin - left.shift, left.shift));

                    var maxOverlap = Math.Min(leftDestination.max, right.sourceMax);
                    flattenedMap.Ranges.Add((right.sourceMin - left.shift, maxOverlap - left.shift, left.shift + right.shift));

                    leftDestination = leftDestination with { min = maxOverlap };
                }
                else if (right.sourceMin <= leftDestination.min)
                {
                    var maxOverlap = Math.Min(leftDestination.max, right.sourceMax);
                    flattenedMap.Ranges.Add((leftDestination.min - left.shift, maxOverlap - left.shift, left.shift + right.shift));

                    leftDestination = leftDestination with { min = maxOverlap };
                }
            }

            //Add any remaining part of the left range, since it doesn't overlap with any range in the right mapper
            if (leftDestination.min < leftDestination.max)
            {
                flattenedMap.Ranges.Add((leftDestination.min - left.shift, left.sourceMax, left.shift));
            }
        }

        var orderedLeftRanges = leftMapper.Ranges.OrderBy(l => l.sourceMin).ToList();
        foreach (var right in rightMapper.Ranges)
        {
            var remainderOfRight = (min: right.sourceMin, max: right.sourceMax);
            var overlappingLeftRanges = orderedLeftRanges.Where(l => l.sourceMax > right.sourceMin && l.sourceMin < right.sourceMax);
            foreach (var left in overlappingLeftRanges)
            {
                if (remainderOfRight.min < left.sourceMin)
                {
                    flattenedMap.Ranges.Add((remainderOfRight.min, left.sourceMin, right.shift)); //example: 4,5
                }

                remainderOfRight = remainderOfRight with { min = left.sourceMax };
            }

            //Add any remaining part of the range, since it doesn't overlap with any source range already mapped
            if (remainderOfRight.min < remainderOfRight.max)
            {
                flattenedMap.Ranges.Add((remainderOfRight.min, remainderOfRight.max, right.shift));
            }
        }

        return flattenedMap;
    }

    private static bool IsEmptyRange((long sourceMin, long sourceMax, long shift) range)
    {
        return range.sourceMax <= range.sourceMin;
    }

    private class RangeMapper(string fromType, string toType)
    {
        public string FromType { get; set; } = fromType;
        public string ToType { get; set; } = toType;

        public List<(long sourceMin, long sourceMax, long shift)> Ranges { get; set; } = new();

        public long Map(long inputValue)
        {
            var matchingRange = Ranges.FirstOrDefault(range => inputValue >= range.sourceMin && inputValue < range.sourceMax);

            return matchingRange != default
                ? inputValue + matchingRange.shift
                : inputValue;
        }
    }
}