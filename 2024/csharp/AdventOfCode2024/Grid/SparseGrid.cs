﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode2024.Grid
{
    public class SparseGrid<TCell>
    {
        private readonly Dictionary<(long x, long y), TCell> _data = new();
        public long xMin {get; private set;} = long.MaxValue;
        public long xMax {get; private set;} = long.MinValue;
        public long yMin {get; private set;} = long.MaxValue;
        public long yMax {get; private set; } = long.MinValue;

        public static SparseGrid<TCell> Parse(IEnumerable<string> input, Func<char, TCell> valueSelector)
        {
            var grid = new SparseGrid<TCell>();
            var allPairs = input
                .SelectMany((line, y) => line.Select((ch, x) => (coords: (x, y), val: valueSelector(ch))));
            foreach (var (coords, val) in allPairs)
            {
                grid.SetCell(coords, val);
            }

            return grid;
        }

        public IEnumerable<Point2D> GetAllCoordinates()
        {
            return _data.Keys.Select(k => (Point2D)k);
        }

        public IEnumerable<(Point2D Coordinates, TCell Value)> GetAllCells()
        {
            return _data.Keys.Select(k => ((Point2D)k, GetCell(k)));
        }

        public void SetCell(Point2D coordinates, TCell value)
        {
            xMin = Math.Min(xMin, coordinates.X);
            xMax = Math.Max(xMax, coordinates.X);
            yMin = Math.Min(yMin, coordinates.Y);
            yMax = Math.Max(yMax, coordinates.Y);
            _data[coordinates] = value;
        }

        public TCell GetCell(Point2D coordinates, TCell defaultValue = default)
        {
            return _data.GetValueOrDefault(coordinates, defaultValue);
        }

        public (long x, long y) FindValueLocation(TCell searchValue)
        {
            return _data.FirstOrDefault(p => p.Value.Equals(searchValue)).Key;
        }

        public bool IsInCurrentBounds(Point2D coordinates)
        {
            return xMin <= coordinates.X
                && yMin <= coordinates.Y
                && xMax >= coordinates.X
                && yMax >= coordinates.Y;
        }

        /// <summary>
        /// Only use this if the grid is expected to be fairly small.
        /// </summary>
        /// <param name="renderFunc"></param>
        /// <returns></returns>
        public string RenderAsString(Func<TCell, char> renderFunc)
        {
            var sb = new StringBuilder();
            for (var i = yMin; i <= yMax; i++)
            {
                for (var j = xMin; j <= xMax; j++)
                {
                    sb.Append(renderFunc(_data.GetValueOrDefault((j, i))));
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}