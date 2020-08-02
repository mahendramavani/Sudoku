using System.Collections.Generic;
using Kakuro.Core.Domain;

namespace Kakuro.Core
{
    public class SumCell : Cell
    {
        private readonly List<GameCell> _partsXCells = new List<GameCell>();
        private readonly List<GameCell> _partsYCells = new List<GameCell>();
        private readonly List<int> _alreadySolvedX = new List<int>();
        private readonly List<int> _alreadySolvedY = new List<int>();

        public bool HasSumX { get; set; }

        public int SumX { get; set; }

        public int SumXParts => _partsXCells.Count;

        public bool HasSumY { get; set; }

        public int SumY { get; set; }

        public int SumYParts => _partsYCells.Count;
        public string LookupKeyX => NumberSet.KeyFrom(SumXParts, SumX);
        public string LookupKeyY => NumberSet.KeyFrom(SumYParts, SumY);

        public SumCell(int x, int y) : base(x, y)
        {
        }

        public void AddAlreadySolved(int value, Direction direction)
        {
            if (direction == Direction.Horizontal)
            {
                if (!_alreadySolvedX.Contains(value))
                    _alreadySolvedX.Add(value);
            }
            else if (direction == Direction.Vertical)
            {
                if (!_alreadySolvedY.Contains(value))
                    _alreadySolvedY.Add(value);
            }
        }

        public void AddPartGameCell(GameCell gameCell, Direction direction)
        {
            if (direction == Direction.Horizontal)
            {
                gameCell.XSumCell = this;
                _partsXCells.Add(gameCell);
            }
            else if (direction == Direction.Vertical)
            {
                gameCell.YSumCell = this;
                _partsYCells.Add(gameCell);
            }
        }

        public GameCell[] GetXPartCells()
        {
            return _partsXCells.ToArray();
        }

        public GameCell[] GetYPartCells()
        {
            return _partsYCells.ToArray();
        }

        public string GetDisplayValue()
        {
            return @"   \" + (HasSumX ? SumX.ToString() : string.Empty).PadLeft(3)
                           + "\r\n"
                           + (HasSumY ? SumY.ToString() : string.Empty).PadRight(3) + @"\";
        }
    }
}