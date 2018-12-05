using System;
using System.Linq;
using common;

namespace day1
{
    class Program
    {
        static void Main(string[] args)
        {
            using(InputReader reader = new InputReader("input.txt"))
            {
                Console.WriteLine(reader.GetLines().Select(x => int.Parse(x)).Sum());
            }
        }
    }
}
