using System;
using System.Collections.Generic;
using System.Text;

namespace common
{
    public class CartesianPlane <T> where T : Point
    {
        SortedDictionary<long, SortedDictionary<long,T>> _plane = new SortedDictionary<long, SortedDictionary<long,T>>(new BackwardsSorter());
        protected long _minX = 0, _maxX = 0, _minY = 0, _maxY = 0;

        private class BackwardsSorter : IComparer<long>
        {
            public int Compare(long x, long y)
            {
                return y.CompareTo(x);
            }
        }

        public virtual string Print(bool toConsole = true)
        {
            StringBuilder sb = new StringBuilder();

                for (long y = _minY; y < _maxY; y++)
                {
                    for (long x = _minX; x < _maxX; x++)
                    {
                        var point = this[x,y];
                        if(point == null)
                        {
                            if(toConsole) Console.Write(" ");
                            else sb.Append(" ");
                        }
                        else
                        {
                            if(toConsole) Console.Write(".");
                            else sb.Append(".");
                        }
                    }
                    if(toConsole) Console.WriteLine();
                    else sb.AppendLine();
                }

                if(!toConsole) return sb.ToString();
                return null;
        }

        public virtual T this[long x, long y]
        {
            get
            {
                SortedDictionary<long, T> xValues;
                T foundValue;
                if(!_plane.TryGetValue(y, out xValues) ||
                    !xValues.TryGetValue(x, out foundValue))
                {
                    foundValue = null;   
                }

                return foundValue;
            }
            set 
            {
                SortedDictionary<long, T> xValues;
                if(!_plane.TryGetValue(y, out xValues))
                {
                    xValues = new SortedDictionary<long, T>();
                    _plane[y] = xValues;
                    if(y < _minY) _minY = y;
                    else if(y > _maxY) _maxY = y;
                }

                xValues.Add(x, value);
                if(x < _minX) _minX = x;
                else if(x > _maxX) _maxX = x;
            }
        }
    }
}