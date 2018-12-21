using System;
using System.Collections.Generic;
using System.Linq;
using common;
using day16;

namespace day21
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> lines;
            using(var reader = new InputReader("test.txt"))
            {
                lines = reader.GetLines().ToList();
            }
        }
    }
}
