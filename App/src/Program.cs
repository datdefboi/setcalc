using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace setcalc
{
    class Program
    {
        static IEnumerable<int> ReadNumbers()
        {
            var s = Console.ReadLine();
            return s.Trim().Split(' ').Select(int.Parse);
        }

        static void WriteNumbers(IEnumerable<int> ints)
        {
            foreach (var i in ints)
            {
                Console.Write($"{i},");
            }

            Console.WriteLine();
        }

        static void Main(string[] args)
        {
            Console.Write("U = ");
            var u = ReadNumbers().Distinct();

            var a = new Set<int>(u);
            var b = new Set<int>(u);

            try
            {
                Console.Write("A = ");
                var same = a.Include(ReadNumbers().ToArray());
                if (same != 0)
                    Console.WriteLine($"*warning* found {same} elements");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine("A are out of range of U");
            }

            try
            {
                Console.Write("B = ");
                var same = b.Include(ReadNumbers().ToArray());
                if (same != 0)
                    Console.WriteLine($"*warning* found {same} elements");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine("B are out of range of U");
            }

            Console.WriteLine();

            Console.Write("Ā = ");
            WriteNumbers(a.Inverted);

            Console.Write("A ∪ B = ");
            WriteNumbers(Set<int>.Union(a, b));

            Console.Write("A ∩ B = ");
            WriteNumbers(Set<int>.Intersection(a, b));

            Console.Write("A ⊂ B = ");
            Console.WriteLine(a.IsSubsetOf(b));

            Console.Write("B ⊂ A = ");
            Console.WriteLine(b.IsSubsetOf(a));

            /*foreach (var pair in a.Relations)
            {
                Console.Write($"{pair},");
            }*/

            Console.WriteLine();
            Main(args);
        }
    }
}