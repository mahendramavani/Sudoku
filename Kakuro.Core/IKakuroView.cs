namespace Kakuro.Core
{
    public interface IKakuroView
    {
        public string[,] UserInputs { get; }
        void DisplayCurrentStatus(Cell[,] cells);
    }
}