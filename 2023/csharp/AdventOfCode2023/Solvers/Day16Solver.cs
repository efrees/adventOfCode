using AdventOfCode2023.Grid;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2023.Solvers;

internal class Day16Solver : ISolver
{
    private const string Name = "Day 16";
    private const string InputFile = "day16input.txt";

    public void Solve()
    {
        Console.WriteLine(Name);
        var lines = Input.GetLinesFromFile(InputFile).ToList();

        Console.WriteLine($"Output (part 1): {GetPart1Answer(lines)}");
        Console.WriteLine($"Output (part 2): {GetPart2Answer(lines)}");
    }

    private static long GetPart1Answer(List<string> lines)
    {
        var grid = SparseGrid<char>.Parse(lines, x => x);

        var start = (0, 0);
        var direction = (1, 0);
        var startBeam = new Beam(start, direction);

        return CountEnergizedSpaces(grid, startBeam);
    }

    private static long GetPart2Answer(List<string> lines)
    {
        var grid = SparseGrid<char>.Parse(lines, x => x);

        return Enumerable.Range(0, lines.Count)
            .SelectMany(i => new[]
            {
                new Beam((0, i), (1, 0)),
                new Beam(((int)grid.xMax, i), (-1, 0)),
                new Beam((i, 0), (0, 1)),
                new Beam((i, (int)grid.yMax), (0, -1)),
            }).Select(startBeam => CountEnergizedSpaces(grid, startBeam))
            .Max();
    }

    private static int CountEnergizedSpaces(SparseGrid<char> grid, Beam startBeam)
    {
        var frontier = new Queue<Beam>();
        var beamsVisited = new HashSet<Beam>();
        frontier.Enqueue(startBeam);

        while (frontier.Any())
        {
            var beam = frontier.Dequeue();

            var component = grid.GetCell(beam.Position);

            if (component == default || beamsVisited.Contains(beam))
            {
                continue;
            }

            beamsVisited.Add(beam);

            if (beam.Direction.Y == 0)
            {
                if (component == '\\')
                {
                    frontier.Enqueue(beam.TurnRight());
                }
                else if (component == '/')
                {
                    frontier.Enqueue(beam.TurnLeft());
                }
                else if (component == '|')
                {
                    frontier.Enqueue(beam.TurnRight());
                    frontier.Enqueue(beam.TurnLeft());
                }
                else
                {
                    frontier.Enqueue(beam.Advance());
                }
            }
            else if (beam.Direction.X == 0)
            {
                if (component == '\\')
                {
                    frontier.Enqueue(beam.TurnLeft());
                }
                else if (component == '/')
                {
                    frontier.Enqueue(beam.TurnRight());
                }
                else if (component == '-')
                {
                    frontier.Enqueue(beam.TurnRight());
                    frontier.Enqueue(beam.TurnLeft());
                }
                else
                {
                    frontier.Enqueue(beam.Advance());
                }
            }
        }

        return beamsVisited.Select(b => b.Position)
            .Distinct()
            .Count();
    }

    private record Beam(Point2D Position, Point2D Direction)
    {
        public Beam TurnRight()
        {
            var newDirection = new Point2D(-Direction.Y, Direction.X);
            var newPosition = Position.Add(newDirection);
            return new Beam(newPosition, newDirection);
        }

        public Beam TurnLeft()
        {
            var newDirection = new Point2D(Direction.Y, -Direction.X);
            var newPosition = Position.Add(newDirection);
            return new Beam(newPosition, newDirection);
        }

        public Beam Advance()
        {
            return this with { Position = Position.Add(Direction) };
        }
    }
}