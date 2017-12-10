﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode2017.Solvers
{
    internal class Day10Solver : IProblemSolver
    {
        public static IProblemSolver Create()
        {
            return new Day10Solver();
        }

        public void Solve(string fileText)
        {
            SolvePart1(fileText);
            SolvePart2(fileText);
        }

        private void SolvePart1(string fileText)
        {
            var lengths = Array.ConvertAll(fileText.Split(','), int.Parse);
            var list = Enumerable.Range(0, 256).ToArray();
            var skip = 0;

            var startPosition = 0;
            foreach (var len in lengths)
            {
                list = ReverseInArray(list, len, startPosition);
                startPosition = (startPosition + len + skip) % list.Length;
                skip++;
            }

            var answerOne = list[0] * list[1];
            Console.WriteLine($"P1: {answerOne}");
        }

        private void SolvePart2(string fileText)
        {
            var lengths = Encoding.ASCII.GetBytes(fileText.Trim());
            lengths = lengths.Concat(new byte[] { 17, 31, 73, 47, 23 })
                .ToArray();

            var list = Enumerable.Range(0, 256).ToArray();
            var skip = 0;
            var startPosition = 0;
            foreach (var n in Enumerable.Range(1, 64))
            {
                foreach (var len in lengths)
                {
                    list = ReverseInArray(list, len, startPosition);
                    startPosition = (startPosition + len + skip) % list.Length;
                    skip++;
                }
            }

            var sparseHash = list;
            var denseBytes = new List<int>();
            foreach (var n in Enumerable.Range(0, 16))
            {
                var denseByte = GetXorResult(sparseHash, n * 16);
                denseBytes.Add(denseByte);
            }

            var hash = denseBytes.Select(b => b.ToString("X2"));

            Console.WriteLine($"P2: {string.Join("", hash)}");
        }

        private int GetXorResult(int[] sparseHash, int start)
        {
            var xor = sparseHash[start];
            for (var i = 1; i < 16; i++)
            {
                xor ^= sparseHash[start + i];
            }
            return xor;
        }

        private int[] ReverseInArray(int[] list, int len, int startPosition)
        {
            var reversedPart = list.Concat(list)
                .Skip(startPosition)
                .Take(len)
                .Reverse()
                .ToArray();

            for (var i = 0; i < len; i++)
            {
                var listIndex = (startPosition + i) % list.Length;
                list[listIndex] = reversedPart[i];
            }
            return list;
        }
    }
}