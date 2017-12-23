using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;

namespace AdventOfCode2017.Solvers
{
    internal class Output
    {
        public static void Info(string info)
        {
            Console.WriteLine(info);
        }

        public static void Answer(object answer, [CallerMemberName]string part = "P1")
        {
            Console.WriteLine($"{part}: {answer}");
            if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
                Clipboard.SetText(answer.ToString());
        }
    }
}