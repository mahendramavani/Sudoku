using System;

namespace Sudoku.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var initialInputs = new[,]
            {
                {2, 9, 5, 7, 0, 0, 8, 6, 0},
                {0, 3, 1, 8, 6, 5, 9, 2, 0},
                {8, 0, 6, 0, 0, 0, 0, 0, 0},
                {0, 0, 7, 0, 5, 0, 0, 0, 6},
                {0, 0, 0, 3, 8, 7, 0, 0, 0},
                {5, 0, 0, 0, 1, 6, 7, 0, 0},
                {0, 0, 0, 5, 0, 0, 1, 0, 9},
                {0, 2, 0, 6, 0, 0, 3, 5, 0},
                {0, 5, 4, 0, 0, 8, 6, 7, 2},
            };

            var board = new Board(initialInputs);
            board.Solve();

            Action<string> display = System.Console.Out.Write;
            Action unSolvedColor = () => System.Console.ForegroundColor = ConsoleColor.DarkRed;
            Action resetColor = System.Console.ResetColor;

            board.PrintCurrentSolution(display, unSolvedColor, resetColor);
        }
    }
}
