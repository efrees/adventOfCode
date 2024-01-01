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
        var beamsVisited = new HashSet<Beam>();

        var start = (0, 0);
        var direction = (1, 0);
        var frontier = new Queue<Beam>();
        frontier.Enqueue(new Beam(start, direction));

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
                    frontier.Enqueue(beam.RightTurn());
                }
                else if (component == '/')
                {
                    frontier.Enqueue(beam.LeftTurn());
                }
                else if (component == '|')
                {
                    frontier.Enqueue(beam.RightTurn());
                    frontier.Enqueue(beam.LeftTurn());
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
                    frontier.Enqueue(beam.LeftTurn());
                }
                else if (component == '/')
                {
                    frontier.Enqueue(beam.RightTurn());
                }
                else if (component == '-')
                {
                    frontier.Enqueue(beam.RightTurn());
                    frontier.Enqueue(beam.LeftTurn());
                }
                else
                {
                    frontier.Enqueue(beam.Advance());
                }
            }
        }

        var energizedSpaces = beamsVisited.Select(b => b.Position).ToHashSet();

        return energizedSpaces.Count;
    }

    private static long GetPart2Answer(List<string> lines)
    {
        return -1;
    }

    private static Point2D RightTurn(Point2D direction)
    {
        return new Point2D(-direction.Y, direction.X);
    }

    private static Point2D LeftTurn(Point2D direction)
    {
        return new Point2D(direction.Y, -direction.X);
    }

    private record Beam(Point2D Position, Point2D Direction)
    {
        public Beam RightTurn()
        {
            var newDirection = new Point2D(-Direction.Y, Direction.X);
            var newPosition = Position.Add(newDirection);
            return new Beam(newPosition, newDirection);
        }

        public Beam LeftTurn()
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