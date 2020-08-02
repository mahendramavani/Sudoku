using System.Collections.Generic;

namespace Kakuro.Core
{
    public interface IKakuroView
    {
        public string[,] UserInputs { get; }
        void DisplayCurrentStatus(IList<SumCell> sumCells);
        void RemoveCell(Cell cell);
    }
}