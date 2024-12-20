using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2023.Solvers;

internal class Day20Solver : ISolver
{
    private const string Name = "Day 20";
    private const string InputFile = "day20input.txt";

    public void Solve()
    {
        Console.WriteLine(Name);
        var lines = Input.GetLinesFromFile(InputFile).ToList();

        Console.WriteLine($"Output (part 1): {GetPart1Answer(lines)}");
        Console.WriteLine($"Output (part 2): {GetPart2Answer(lines)}");
    }

    private static long GetPart1Answer(List<string> lines)
    {
        var modules = ParseModules(lines);

        var highCount = 0;
        var lowCount = 0;
        var pulsesToProcess = new Queue<Pulse>();
        var pulseFromButton = new Pulse("button", "broadcaster", false);

        foreach (var _ in Enumerable.Range(0, 1000))
        {
            pulsesToProcess.Enqueue(pulseFromButton);

            while (pulsesToProcess.Count > 0)
            {
                var pulse = pulsesToProcess.Dequeue();
                if (pulse.IsHigh)
                {
                    highCount++;
                }
                else
                {
                    lowCount++;
                }

                if (!modules.ContainsKey(pulse.Destination))
                {
                    //Console.WriteLine($"Unregistered module '{pulse.Destination}' received {pulse.IsHigh} from {pulse.Source}");
                    continue;
                }

                var handlingModule = modules[pulse.Destination];
                var nextPulses = handlingModule.ProcessPulse(pulse);
                EnqueueAll(pulsesToProcess, nextPulses);
            }
        }

        return highCount * lowCount;
    }

    private static long GetPart2Answer(List<string> lines)
    {
        var modules = ParseModules(lines);

        var pulsesToProcess = new Queue<Pulse>();
        var pulseFromButton = new Pulse("button", "broadcaster", false);
        var targetModule = "rx";

        var pressCount = 0;
        var targetModulePulseCount = 0;
        while (targetModulePulseCount != 1)
        {
            pulsesToProcess.Enqueue(pulseFromButton);
            pressCount++;

            targetModulePulseCount = 0;
            while (pulsesToProcess.Count > 0)
            {
                var pulse = pulsesToProcess.Dequeue();

                if (pulse.Destination == targetModule)
                {
                    targetModulePulseCount += pulse.IsHigh
                        ? 2
                        : 1;
                }

                if (!modules.ContainsKey(pulse.Destination))
                {
                    Console.WriteLine($"Unregistered module '{pulse.Destination}' received {pulse.IsHigh} from {pulse.Source}");
                    continue;
                }

                var handlingModule = modules[pulse.Destination];
                var nextPulses = handlingModule.ProcessPulse(pulse);
                EnqueueAll(pulsesToProcess, nextPulses);
            }
        }

        return pressCount;
    }

    private static void EnqueueAll(Queue<Pulse> pulsesToProcess, IEnumerable<Pulse> nextPulses)
    {
        foreach (var nextPulse in nextPulses)
        {
            pulsesToProcess.Enqueue(nextPulse);
        }
    }

    private static Dictionary<string, PulseModule> ParseModules(List<string> lines)
    {
        var modules = new Dictionary<string, PulseModule>();
        var conjunctions = new Dictionary<string, Conjunction>();

        foreach (var line in lines)
        {
            var inputOutputParts = line.Split(" -> ");

            var inputPart = inputOutputParts[0];
            var outputNames = inputOutputParts[1].Split(", ");
            if (inputPart == "broadcaster")
            {
                modules[inputPart] = new Broadcast
                {
                    Name = inputPart,
                    OutputModuleNames = outputNames
                };
            }
            else if (inputPart.StartsWith('%'))
            {
                var name = inputPart[1..];
                modules[name] = new FlipFlop
                {
                    Name = name,
                    OutputModuleNames = outputNames
                };
            }
            else if (inputPart.StartsWith('&'))
            {
                var name = inputPart[1..];
                var conjunction = new Conjunction
                {
                    Name = name,
                    OutputModuleNames = outputNames
                };
                modules[name] = conjunction;
                conjunctions[name] = conjunction;
            }
        }

        foreach (var moduleKey in modules.Keys)
        {
            var outputs = modules[moduleKey].OutputModuleNames;
            foreach (var output in outputs)
            {
                if (conjunctions.ContainsKey(output))
                {
                    conjunctions[output].ConnectInput(moduleKey);
                    break;
                }
            }
        }

        return modules;
    }

    private abstract class PulseModule
    {
        public string Name { get; init; }
        public IList<string> OutputModuleNames { get; init; } = new List<string>();

        public abstract IEnumerable<Pulse> ProcessPulse(Pulse input);

        protected IEnumerable<Pulse> CreateOutputPulses(bool outputValue)
        {
            return OutputModuleNames.Select(outName => new Pulse(Name, outName, outputValue));
        }
    }

    private class Broadcast : PulseModule
    {
        public override IEnumerable<Pulse> ProcessPulse(Pulse input)
        {
            return CreateOutputPulses(input.IsHigh);
        }
    }

    private class FlipFlop : PulseModule
    {
        private bool IsOn { get; set; }

        public override IEnumerable<Pulse> ProcessPulse(Pulse input)
        {
            if (input.IsHigh)
            {
                return Array.Empty<Pulse>();
            }

            IsOn = !IsOn;
            return CreateOutputPulses(IsOn);
        }
    }

    private class Conjunction : PulseModule
    {
        private Dictionary<string, bool> Memory { get; } = new Dictionary<string, bool>();

        public void ConnectInput(string inputName)
        {
            Memory[inputName] = false;
        }

        public override IEnumerable<Pulse> ProcessPulse(Pulse input)
        {
            Memory[input.Source] = input.IsHigh;
            var outputValue = !Memory.Values.All(isHigh => isHigh);
            return CreateOutputPulses(outputValue);
        }
    }

    private record Pulse(string Source, string Destination, bool IsHigh);
}