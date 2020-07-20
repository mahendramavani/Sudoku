using System;
using Sudoku.Core;

namespace Sudoku.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            int[,] initialInputs = new int[9,9];

            for (int y = 0; y < 9; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    while (true)
                    {
                        int key = System.Console.Read();
                        var number = key - 48;
                        if (number >= 0 && number <= 9)
                        {
                            initialInputs[x, y] = number;
                            break;
                        }
                    }
                }
            }

            for (int y = 0; y < 9; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    System.Console.Out.Write($"{initialInputs[x, y]}".PadRight(10,' '));
                }

                System.Console.Out.WriteLine();
            }

            return;

            
            initialInputs = new[,]
            {
                {2, 9, 5,   7, 0, 0,   8, 6, 0},
                {0, 3, 1,   8, 6, 5,   9, 2, 0},
                {8, 0, 6,   0, 0, 0,   0, 0, 0},
 
                {0, 0, 7,   0, 5, 0,   0, 0, 6},
                {0, 0, 0,   3, 8, 7,   0, 0, 5},
                {5, 0, 0,   0, 1, 6,   7, 0, 0},
                
                {0, 0, 0,   5, 0, 0,   1, 0, 9},
                {0, 2, 0,   6, 0, 0,   3, 0, 0},
                {0, 5, 4,   0, 0, 8,   6, 7, 2},
            };

            var transposedInput = new int[9, 9];
            for (var i = 0; i < 9; i++)
            {
                for (var j = 0; j < 9; j++)
                {
                    transposedInput[i, j] = initialInputs[j, i];
                }
            }

            Board board;
            board = new Board(initialInputs);
            //board.PrintCurrentSolution();
            board.Solve();
            board.PrintCurrentSolution();

            // board = new Board(transposedInput);
            // board.PrintCurrentSolution();
            // board.Solve();
            // board.PrintCurrentSolution();
        }
    }
}
