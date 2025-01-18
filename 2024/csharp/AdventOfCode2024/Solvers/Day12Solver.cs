using AdventOfCode2024.Grid;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2024.Solvers;

internal class Day12Solver : ISolver
{
    private const string Name = "Day 12";
    private const string InputFile = "day12input.txt";

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
        var grid = SparseGrid<char>.Parse(lines, c => c);

        var visited = new SparseGrid<int>();
        var nextRegionNumber = 1;

        var regions = new List<(int Area, int Perimeter)>();
        foreach (var cell in grid.GetAllCells())
        {
            if (visited.GetCell(cell.Coordinates) > 0)
            {
                continue;
            }

            regions.Add(ComputeAreaAndPerimeter(cell.Coordinates, cell.Value, grid, visited, nextRegionNumber));
            nextRegionNumber++;
        }

        return regions
            .Select(r => r.Area * r.Perimeter)
            .Sum();
    }

    private static (int Area, int Perimeter) ComputeAreaAndPerimeter(Point2D cellCoordinates,
        char cellValue,
        SparseGrid<char> grid,
        SparseGrid<int> visited,
        int regionNumber)
    {
        visited.SetCell(cellCoordinates, regionNumber);

        var area = 1;
        var perimeter = 0;
        foreach (var neighbor in cellCoordinates.GetNeighbors4())
        {
            if (grid.GetCell(neighbor) != cellValue)
            {
                perimeter++;
            }
            else if (visited.GetCell(neighbor) == 0)
            {
                var nestedResult = ComputeAreaAndPerimeter(neighbor, cellValue, grid, visited, regionNumber);
                area += nestedResult.Area;
                perimeter += nestedResult.Perimeter;
            }
        }

        return (area, perimeter);
    }

    private static long GetPart2Answer(List<string> lines)
    {
        var grid = SparseGrid<char>.Parse(lines, c => c);

        var visited = new SparseGrid<int>();
        var nextRegionNumber = 1;

        var regions = new Dictionary<int, Region>();

        foreach (var cell in grid.GetAllCells())
        {
            if (visited.GetCell(cell.Coordinates) > 0)
            {
                continue;
            }

            var areaAndPerimeter = ComputeAreaAndPerimeter(cell.Coordinates, cell.Value, grid, visited, nextRegionNumber);

            var region = new Region
            {
                Id = nextRegionNumber,
                StartCoordinates = cell.Coordinates,
                Area = areaAndPerimeter.Area
            };
            regions[nextRegionNumber] = region;
            nextRegionNumber++;
        }

        foreach (var regionId in regions.Keys)
        {
            var region = regions[regionId];
            var (sides, outerNeighbors) = WalkPerimeterToCountSides(
                region.StartCoordinates,
                visited,
                location => visited.GetCell(location) != regionId
            );
            region.OuterSides = sides;
            region.OuterNeighbors = outerNeighbors;
        }

        var innerNeighborsLookup = MapInnerNeighbors(regions);
        var emptyList = new List<int>();
        foreach (var region in regions.Values)
        {
            var innerNeighbors = innerNeighborsLookup.GetValueOrDefault(region.Id, emptyList);
            if (!innerNeighbors.Any())
            {
                continue;
            }

            // To count inner sides, find all the fully-enclosed regions, and walk the sides as a cluster.
            // Add additional regions to the cluster until we have only sides bordering our outer region.
            // Surely there's a smarter way to do this.
            var innerCluster = new List<int>();
            do
            {
                if (innerCluster.Count == 0)
                {
                    innerCluster.Add(innerNeighbors.First());
                }

                var startCoordinates = innerCluster.Select(id => regions[id].StartCoordinates)
                    .OrderBy(point => point.Y)
                    .ThenBy(point => point.X)
                    .First();
                var (sides, outerNeighbors) = WalkPerimeterToCountSides(
                    startCoordinates,
                    visited,
                    location => !innerCluster.Contains(visited.GetCell(location))
                );

                if (outerNeighbors.Count == 1)
                {
                    region.InnerSides += sides;
                    innerNeighbors.RemoveAll(n => innerCluster.Contains(n));
                    innerCluster = [];
                }
                else
                {
                    innerCluster.AddRange(outerNeighbors.Where(n => n != region.Id));
                }
            } while (innerNeighbors.Any());
        }

        return regions
            .Values
            .Select(r => r.Area * (r.OuterSides + r.InnerSides))
            .Sum();
    }

    private static Dictionary<int, List<int>> MapInnerNeighbors(Dictionary<int, Region> regions)
    {
        var innerNeighbors = new Dictionary<int, List<int>>();

        foreach (var region in regions.Values)
        {
            // A region is an inner neighbor of all its outer neighbors that don't have it as an outer neighbor.
            foreach (var neighborId in region.OuterNeighbors)
            {
                var neighbor = regions[neighborId];
                if (!neighbor.OuterNeighbors.Contains(region.Id))
                {
                    if (!innerNeighbors.ContainsKey(neighborId))
                    {
                        innerNeighbors[neighborId] = new List<int>();
                    }

                    innerNeighbors[neighborId].Add(region.Id);
                }
            }
        }

        return innerNeighbors;
    }

    private static (int Sides, HashSet<int> OuterNeighbors) WalkPerimeterToCountSides(Point2D startCoordinates,
        SparseGrid<int> visited,
        Func<Point2D, bool> checkIsOutsideRegion)
    {
        // We encounter each region from the top (minY) side and from the left, so we start walking right.
        var currentSideDirection = new Point2D(1, 0);
        var currentLocation = startCoordinates;

        var sideCount = 0;
        var outerNeighbors = new HashSet<int>();
        while (currentLocation != startCoordinates || currentSideDirection != (1, 0) || sideCount == 0)
        {
            var leftHandCell = LeftHandCell(currentLocation, currentSideDirection);
            if (checkIsOutsideRegion(leftHandCell) && visited.GetCell(leftHandCell) is var knownRegion and > 0)
            {
                outerNeighbors.Add(knownRegion);
            }

            var nextLocation = currentLocation.Add(currentSideDirection);

            if (checkIsOutsideRegion(nextLocation))
            {
                //...?
                //AAA.
                currentSideDirection = TurnRight(currentSideDirection);
                sideCount++;
            }
            else if (!checkIsOutsideRegion(LeftHandCell(nextLocation, currentSideDirection)))
            {
                //...A
                //AAAA
                currentSideDirection = TurnLeft(currentSideDirection);
                currentLocation = nextLocation;
                sideCount++;
            }
            else
            {
                currentLocation = nextLocation;
            }
        }

        return (sideCount, outerNeighbors);
    }

    private static Point2D LeftHandCell(Point2D nextLocation, Point2D currentSideDirection)
    {
        var leftDirection = TurnLeft(currentSideDirection);
        return nextLocation.Add(leftDirection);
    }

    private static (long Y, long) TurnLeft(Point2D currentSideDirection)
    {
        return (currentSideDirection.Y, -currentSideDirection.X);
    }

    private static (long Y, long) TurnRight(Point2D currentSideDirection)
    {
        return (-currentSideDirection.Y, currentSideDirection.X);
    }

    private class Region
    {
        public int Id { get; init; }
        public Point2D StartCoordinates { get; init; }
        public int Area { get; set; }
        public int OuterSides { get; set; }
        public int InnerSides { get; set; }
        public HashSet<int> OuterNeighbors { get; set; }
    }
}