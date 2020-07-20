using System;
using Sudoku.Core;
using ValueType = Sudoku.Core.ValueType;

namespace Sudoku.Console
{
    public class Program : IView
    {
        //Sample Inputs
        /*
019 000 000
600 003 040
734 589 610
006 817 430
051 390 000
403 200 000
000 470 103
002 900 867
090 030 020

2, 9, 5,7, 0, 0,8, 6, 0
0, 3, 1,8, 6, 5,9, 2, 0
8, 0, 6,0, 0, 0,0, 0, 0
0, 0, 7,0, 5, 0,0, 0, 6
0, 0, 0,3, 8, 7,0, 0, 5
5, 0, 0,0, 1, 6,7, 0, 0
0, 0, 0,5, 0, 0,1, 0, 9
0, 2, 0,6, 0, 0,3, 0, 0
0, 5, 4,0, 0, 8,6, 7, 2
         */
        private Core.Sudoku _sudoku;

        public static void Main(string[] args)
        {
            var program = new Program();
            program.Run();
        }

        private void Run()
        {
            Inputs = new int[Core.Sudoku.SIZE, Core.Sudoku.SIZE];

            for (int y = 0; y < Core.Sudoku.SIZE; y++)
            {
                System.Console.Out.WriteLine($"Enter line {y + 1}, followed by enter key");
                for (int x = 0; x < Core.Sudoku.SIZE; x++)
                {
                    while (true)
                    {
                        int key = System.Console.Read();
                        var number = key - 48;
                        if (number >= 0 && number <= 9)
                        {
                            Inputs[x, y] = number;
                            break;
                        }
                    }
                }
            }

            // var transposedInput = new int[Core.Sudoku.SIZE, Core.Sudoku.SIZE];
            // for (var i = 0; i < Core.Sudoku.SIZE; i++)
            // {
            //     for (var j = 0; j < Core.Sudoku.SIZE; j++)
            //     {
            //         transposedInput[i, j] = Inputs[j, i];
            //     }
            // }

            for (int y = 0; y < 9; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    System.Console.Out.Write($"{Inputs[x, y]}".PadRight(10, ' '));
                }

                System.Console.Out.WriteLine();
            }

            _sudoku = new Core.Sudoku(this);
            _sudoku.Solve();
        }

        // var initialInputs = new[,]
        // {
        //     {2, 9, 5,   7, 0, 0,   8, 6, 0},
        //     {0, 3, 1,   8, 6, 5,   9, 2, 0},
        //     {8, 0, 6,   0, 0, 0,   0, 0, 0},
        //     
        //     {0, 0, 7,   0, 5, 0,   0, 0, 6},
        //     {0, 0, 0,   3, 8, 7,   0, 0, 5},
        //     {5, 0, 0,   0, 1, 6,   7, 0, 0},
        //         
        //     {0, 0, 0,   5, 0, 0,   1, 0, 9},
        //     {0, 2, 0,   6, 0, 0,   3, 0, 0},
        //     {0, 5, 4,   0, 0, 8,   6, 7, 2},
        // };

        public int[,] Inputs { get; private set; }

        public void AppendStatus(string status)
        {
            System.Console.WriteLine(status);
        }

        public void Print(Point[,] points)
        {
            System.Console.Out.WriteLine("".PadRight(145, '—'));

            for (var y = 0; y < Core.Sudoku.SIZE; y++)
            {
                for (var x = 0; x < Core.Sudoku.SIZE; x++)
                {
                    var point = points[x, y];
                    if (point.Value == 0)
                    {
                        System.Console.ForegroundColor = ConsoleColor.DarkRed;
                        System.Console.Write($"[{string.Join("", point.GetPossibleValues())}]".PadRight(10, ' '));
                        System.Console.ResetColor();
                    }
                    else
                    {
                        if (point.Type == ValueType.Calculated)
                        {
                            System.Console.ForegroundColor = ConsoleColor.DarkBlue;
                            System.Console.Write(point.Value.ToString().PadRight(10, ' '));
                            System.Console.ResetColor();
                        }
                        else if (point.Type == ValueType.Guessed)
                        {
                            System.Console.ForegroundColor = ConsoleColor.DarkCyan;
                            System.Console.Write(point.Value.ToString().PadRight(10, ' '));
                            System.Console.ResetColor();
                        }
                        else
                        {
                            System.Console.Write(point.Value.ToString().PadRight(10, ' '));
                        }
                    }
                    System.Console.Write("\t");

                    if (point.X % Core.Sudoku.INNER_SIZE == 2)
                    {
                        System.Console.Out.Write("|");
                    }

                    if (point.X == Core.Sudoku.SIZE - 1 && point.Y % Core.Sudoku.INNER_SIZE == 2)
                    {
                        System.Console.Out.Write("\n".PadRight(145, '—'));
                    }
                }

                System.Console.WriteLine();
            }
            System.Console.Out.WriteLine();
            System.Console.Out.WriteLine("".PadRight(145, '='));
            System.Console.Out.WriteLine();
        }

        public void InValidInput(string message)
        {
            System.Console.Out.WriteLine(message);
        }
    }
}
