using System;

namespace common
{
    public static class Helpers
    {
        public static void Display<T>(this T[,] array)
        {
            for (int i = 0; i <= array.GetUpperBound(0); i++)
            {
                for (int x = 0; x <= array.GetUpperBound(1); x++)
                {
                    Console.Write(array[i, x]);
                    Console.Write(" ");
                }
                Console.WriteLine();
            }
        }
    }
}