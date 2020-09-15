using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace setcalc
{
    class Program
    {
        static IEnumerable<int> ReadNumbers()
        {
            var s = Console.ReadLine();
            return s.Trim().Split(' ').Select(int.Parse);
        }

        static string IterableToString<T>(IEnumerable<T> ints)
        {
            var sb = new StringBuilder();
            sb.Append("[");
            foreach (var i in ints)
            {
                sb.Append($"{i},");
            }

            sb.Append("]");
            sb.Remove(sb.Length - 2, 1);

            return sb.ToString();
        }

        static void WriteSet<T>(Set<T> set)
        {
            Console.Write($"{IterableToString(set)} (future vec {IterableToString(set.Futures)})\n");
        }

        static void Main(string[] args)
        {
            Console.WriteLine("HOW TO USE: insert each numbers separated by space.");
            
            Console.Write("U = ");
            var u = ReadNumbers().Distinct();

            var a = new Set<int>(u);
            var b = new Set<int>(u);

            try
            {
                Console.Write("A = ");
                var same = a.Include(ReadNumbers().ToArray());
                WriteSet(a);
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
                WriteSet(b);
                if (same != 0)
                    Console.WriteLine($"*warning* found {same} elements");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine("B are out of range of U");
            }

            Console.WriteLine();

            Console.Write("^A = ");
            WriteSet(a.Inverted);

            Console.Write("A v B = ");
            var union = Set<int>.Union(a, b);
            WriteSet(union);

            Console.Write("A ^ B = ");
            var inter = Set<int>.Intersection(a, b);
            WriteSet(inter);

            Console.Write("A c B = ");
            Console.WriteLine(a.IsSubsetOf(b));

            Console.Write("B c A = ");
            Console.WriteLine(b.IsSubsetOf(a));

            Console.WriteLine();
            Main(args);
        }
    }
}