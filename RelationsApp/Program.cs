using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using static System.Math;
using static System.Console;

namespace RelationsApp
{
    class Program
    {
        private static readonly int ACharCode = Convert.ToInt16('A');

        static IEnumerable<string> GenerateNames(string prefix, int n) =>
            Enumerable.Range(0, n)
                .Select(i => prefix + Convert.ToChar(ACharCode + i));

        static void WithContrast(bool isInverted, Action action)
        {
            if (isInverted)
            {
                BackgroundColor = ConsoleColor.White;
                ForegroundColor = ConsoleColor.Black;
            }

            action();

            if (isInverted)
            {
                BackgroundColor = ConsoleColor.Black;
                ForegroundColor = ConsoleColor.White;
            }
        }

        static void BackWrite(string s)
        {
            var l = s.Length;
            SetCursorPosition(CursorLeft - l, CursorTop);
            Write(s);
            //Console.SetCursorPosition(Console.CursorLeft+1, Console.CursorTop);
        }

        static void Main(string[] args)
        {
            Write("Rows: ");
            var rows = int.Parse(ReadLine());
            Write("Columns: ");
            var cols = int.Parse(ReadLine());

            var rowLabels = GenerateNames("A-", rows).ToArray();
            var colLabels = GenerateNames("B-", cols).ToArray();

            var relationsIndirect = new Set<string>[cols];
            for (int i = 0; i < cols; i++)
            {
                relationsIndirect[i] = new Set<string>(rowLabels);
            }

            var relationsDirect = new Set<string>[rows];
            for (int iRow = 0; iRow < rows; iRow++)
            {
                relationsDirect[iRow] = new Set<string>(colLabels);
                relationsDirect[iRow].Randomize();

                var aEl = rowLabels[iRow];
                for (int iIndirectCol = 0;
                    iIndirectCol < cols;
                    iIndirectCol++) // insert copy into the transposed relations
                {
                    if (relationsDirect[iRow][iIndirectCol])
                        relationsIndirect[iIndirectCol].FlipElement(aEl);
                }
            }
            
            //TODO: move into new class

            int xCursor = 0, yCursor = 0;
            var xActivity = new bool[cols];
            var yActivity = new bool[rows];

            void RenderTable()
            {
                var cornerPlaceholder = new string(' ', 7) + "|";
                Write(cornerPlaceholder);
                for (int i = 0; i < xActivity.Length; i++)
                {
                    Write($" {(xActivity[i] ? '+' : '-')} |");
                }

                WriteLine();

                Write(cornerPlaceholder);
                for (int x = 0; x < colLabels.Length; x++)
                {
                    WithContrast(x == xCursor, () =>
                    {
                        if (x == xCursor) BackWrite("|");
                        Write($"{colLabels[x]}|");
                    });
                }

                WriteLine();

                for (int y = 0; y < rows; y++)
                {
                    var row = relationsDirect[y];

                    Write($" {(yActivity[y] ? '+' : '-')} ");

                    WithContrast(y == yCursor,
                        () => Write($"|{rowLabels[y]}|"));

                    var x = 0;
                    foreach (var r in row.ActivityPairs)
                    {
                        WithContrast(y == yCursor || x == xCursor,
                            () =>
                            {
                                if (x == xCursor) BackWrite("|");
                                Write(r.isActive ? " 1 |" : " 0 |");
                            });
                        x++;
                    }

                    WriteLine();
                }
            }

            void RenderAgenda()
            {
                WriteLine("R - enable row in calcs");
                WriteLine("C - enable column in calcs");
                WriteLine("F - flip vec's bit");
            }

            void ShowGalua(Set<string>[] sets, string[] labels, bool[] active)
            {
                var galua = active
                    .Zip(labels)
                    .Select((row, i) =>
                    {
                        var (isActive, id) = row;
                        return (isActive, id, i);
                    })
                    .Where(row => row.isActive)
                    .Select(row => sets[row.i]); // filter by activation
                //TODO: move into new class

                try
                {
                    var result = Set<string>.Intersection(galua.ToArray());
                    Write("{");
                    foreach (var g in result)
                    {
                        Write($" {g} ");
                    }

                    WriteLine("}");
                }
                catch (ArgumentException ex)
                {
                    WriteLine("0");
                }
            }

            CursorVisible = false;

            while (true)
            {
                Clear();
                RenderTable();
                RenderAgenda();
                Write("Г(Х) = ");
                ShowGalua(relationsDirect, rowLabels, yActivity);
                Write("Г^(Х) = ");
                ShowGalua(relationsIndirect, colLabels, xActivity);

                var c = ReadKey();
                switch (c.Key)
                {
                    case ConsoleKey.LeftArrow:
                        xCursor = Max(0, xCursor - 1);
                        break;
                    case ConsoleKey.UpArrow:
                        yCursor = Max(0, yCursor - 1);
                        break;
                    case ConsoleKey.RightArrow:
                        xCursor = Min(cols - 1, xCursor + 1);
                        break;
                    case ConsoleKey.DownArrow:
                        yCursor = Min(rows - 1, yCursor + 1);
                        break;
                    case ConsoleKey.R:
                        yActivity[yCursor] = !yActivity[yCursor];
                        break;
                    case ConsoleKey.C:
                        xActivity[xCursor] = !xActivity[xCursor];
                        break;
                    case ConsoleKey.F:
                        relationsDirect[yCursor].FlipElement(colLabels[xCursor]);
                        relationsIndirect[xCursor].FlipElement(rowLabels[yCursor]);
                        break;
                }
            }
        }
    }
}