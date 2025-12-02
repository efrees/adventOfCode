using System;

namespace AdventOfCode2025.Solvers;

internal class NullSolver : ISolver
{
    private readonly string _name;

    public NullSolver(string name)
    {
        _name = name;
    }

    public void Solve()
    {
        Console.WriteLine(_name);
        Console.WriteLine("Not solved yet!");
    }
}