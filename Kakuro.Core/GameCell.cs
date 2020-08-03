using System.Collections.Generic;
using System.Linq;
using Kakuro.Core.Domain;

namespace Kakuro.Core
{
    public class GameCell : Cell
    {
        private readonly List<int> _eliminatedValues = new List<int>();
        
        public int Value { get; private set; }
        public bool IsSolved { get; set; }
        public SumCell XSumCell { get; set; }
        public SumCell YSumCell { get; set; }

        public GameCell(int x, int y) : base(x, y)
        {
        }

        public bool EliminateTheNumbers(params int[] numbers)
        {
            var anythingSolved = false;

            foreach (var number in numbers)
            {
                if (number > 0 && number < 10 && !_eliminatedValues.Contains(number))
                {
                    _eliminatedValues.Add(number);
                }
            }

            if (CanBeSolvedWithLastNonEliminatedValue(out var solvedValue))
            {
                MarkAsSolved(solvedValue);
                anythingSolved = true;
            }

            return anythingSolved;
        }

        private bool CanBeSolvedWithLastNonEliminatedValue(out int solvedValue)
        {
            solvedValue = 0;
            if (_eliminatedValues.Count != 8) 
                return false;
            
            var allValue = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            solvedValue = allValue.Except(_eliminatedValues).ElementAt(0);
            return true;
        }

        public void MarkAsSolved(int value)
        {
            Value = value;
            IsSolved = true;

            XSumCell.FoundNewSolution(value, Direction.Horizontal);
            YSumCell.FoundNewSolution(value, Direction.Vertical);
        }

        public string DisplaySolutionValue()
        {
            return Value.ToString();
        }

        public string GetEliminationValueDisplayValue()
        {
            var displayValue = (_eliminatedValues.Contains(1) ? "_" : " ") + string.Empty.PadRight(6) + (_eliminatedValues.Contains(2) ? "_" : " ") + string.Empty.PadRight(6) + (_eliminatedValues.Contains(3) ? "_" : " ")
                               + "\r\n" + (_eliminatedValues.Contains(4) ? "_" : " ") + string.Empty.PadRight(6) + (_eliminatedValues.Contains(5) ? "_" : " ") + string.Empty.PadRight(6) + (_eliminatedValues.Contains(6) ? "_" : " ")
                               + "\r\n" + (_eliminatedValues.Contains(7) ? "_" : " ") + string.Empty.PadRight(6) + (_eliminatedValues.Contains(8) ? "_" : " ") + string.Empty.PadRight(6) + (_eliminatedValues.Contains(9) ? "_" : " ");

            return displayValue;
        }

        public bool IsTheValuePossibleSolution(in int value)
        {
            return !_eliminatedValues.Contains(value);
        }
    }
}