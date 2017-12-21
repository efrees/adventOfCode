﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2017.Solvers
{
    internal class Day21Solver : IProblemSolver
    {
        public static IProblemSolver Create() => new Day21Solver();

        public void Solve(string fileText)
        {
            var rules = fileText.SplitIntoLines()
                .Select(EnhancementRule.Parse)
                .ToList();
            var image = new[,] { { '.', '#', '.' }, { '.', '.', '#' }, { '#', '#', '#' } };

            image = IterateMultipleTimes(5, image, rules);

            Output.Answer(CountPixels(image), "P1");

            image = IterateMultipleTimes(18 - 5, image, rules);

            Output.Answer(CountPixels(image), "P2");
        }

        private char[,] IterateMultipleTimes(int iterations, char[,] image, IList<EnhancementRule> rules)
        {
            while (iterations > 0)
            {
                image = IterateImage(image, rules);
                iterations--;
            }
            return image;
        }

        private char[,] IterateImage(char[,] image, IList<EnhancementRule> rules)
        {
            var size = image.GetLength(0);
            var blockSize = size % 2 == 0 ? 2 : 3;
            var numberOfBlocks = size / blockSize; //in each row
            var newImage = new char[size + numberOfBlocks, size + numberOfBlocks];
            for (var i = 0; i < numberOfBlocks; i++)
            {
                for (var j = 0; j < numberOfBlocks; j++)
                {
                    var blockY = i * blockSize;
                    var blockX = j * blockSize;
                    var patternToReplace = SnipFromImage(image, blockX, blockY, blockSize);
                    var rule = rules.First(r => r.TargetSize == blockSize && r.PatternMatches(patternToReplace));
                    if (rule.Output.Cast<char>().Any(c => c == 0)) System.Diagnostics.Debugger.Break();
                    CopyToImage(rule.Output, newImage, blockX + j, blockY + i, blockSize + 1);
                }
            }
            return newImage;
        }

        private char[,] SnipFromImage(char[,] image, int blockX, int blockY, int blockSize)
        {
            var snippet = new char[blockSize, blockSize];
            for (var i = 0; i < blockSize; i++)
            {
                for (var j = 0; j < blockSize; j++)
                {
                    snippet[i, j] = image[blockY + i, blockX + j];
                }
            }
            return snippet;
        }

        private void CopyToImage(char[,] ruleOutput, char[,] newImage, int blockX, int blockY, int blockSize)
        {
            for (var i = 0; i < blockSize; i++)
            {
                for (var j = 0; j < blockSize; j++)
                {
                    newImage[blockY + i, blockX + j] = ruleOutput[i, j];
                }
            }
        }

        private int CountPixels(char[,] image)
        {
            return image.Cast<char>().Count(p => p == '#');
        }

        internal class EnhancementRule
        {
            public static EnhancementRule Parse(string line)
            {
                var parts = line.Split(new[] { " => " }, StringSplitOptions.RemoveEmptyEntries);
                var rule = new EnhancementRule()
                {
                    Pattern = ParsePart(parts[0]),
                    Output = ParsePart(parts[1])
                };
                return rule;
            }

            private static char[,] ParsePart(string part)
            {
                var rows = part.Split('/');
                var result = new char[rows.Length, rows.Length];
                for (var i = 0; i < rows.Length; i++)
                {
                    for (var j = 0; j < rows.Length; j++)
                    {
                        result[i, j] = rows[i][j];
                    }
                }
                return result;
            }

            public char[,] Pattern { get; private set; }
            public char[,] Output { get; private set; }
            public int TargetSize => Pattern.GetLength(0);

            public bool PatternMatches(char[,] patternToReplace)
            {
                var size = patternToReplace.GetLength(0);
                foreach (var manipulatedPattern in Manipulate(patternToReplace))
                {
                    var matchFound = true;
                    for (var i = 0; i < size; i++)
                    {
                        for (var j = 0; j < size; j++)
                        {
                            if (Pattern[i, j] != manipulatedPattern[i, j])
                            {
                                matchFound = false;
                            }
                        }
                    }
                    if (matchFound) return true;
                }

                return false;
            }

            private IEnumerable<char[,]> Manipulate(char[,] patternToReplace)
            {
                for (var direction = 0; direction < 4; direction++)
                {
                    yield return patternToReplace;
                    Rotate(patternToReplace);
                }

                FlipVertically(patternToReplace);

                for (var direction = 0; direction < 4; direction++)
                {
                    yield return patternToReplace;
                    Rotate(patternToReplace);
                }
            }

            private void Rotate(char[,] patternToReplace)
            {
                var size = patternToReplace.GetLength(0);
                //only need to handle 2x2 and 3x3
                for (var i = 0; i < (size + 1) / 2; i++)
                {
                    var t = patternToReplace[0, i];
                    patternToReplace[0, i] = patternToReplace[i, size - 1];
                    patternToReplace[i, size - 1] = patternToReplace[size - 1, size - 1 - i];
                    patternToReplace[size - 1, size - 1 - i] = patternToReplace[size - 1 - i, 0];
                    patternToReplace[size - 1 - i, 0] = t;
                }
            }

            private void FlipVertically(char[,] patternToReplace)
            {
                var size = patternToReplace.GetLength(0);
                for (var i = 0; i < size / 2; i++)
                {
                    for (var j = 0; j < size; j++)
                    {
                        var t = patternToReplace[i, j];
                        patternToReplace[i, j] = patternToReplace[size - 1 - i, j];
                        patternToReplace[size - 1 - i, j] = t;
                    }
                }
            }
        }
    }
}