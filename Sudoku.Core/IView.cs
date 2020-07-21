namespace Sudoku.Core
{
    public interface IView
    {
        int[,] Inputs { get; }
        void AppendStatus(string status);
        void Print(Point[,] points);
        void InValidInput(string message);
    }
}