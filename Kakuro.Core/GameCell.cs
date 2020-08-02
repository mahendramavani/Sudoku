using System.Collections.Generic;
using System.Linq;

namespace Kakuro.Core
{
    public class GameCell : Cell
    {
        private readonly List<int> _eliminatedValues = new List<int>();
        
        public int Value { get; set; }
        public bool IsSolved { get; set; }
        public SumCell XSumCell { get; set; }
        public SumCell YSumCell { get; set; }

        public GameCell(int x, int y) : base(x, y)
        {
        }

        public void EliminateTheNumbers(params int[] numbers)
        {
            foreach (var number in numbers)
            {
                if (number > 0 && number < 10 && !_eliminatedValues.Contains(number))
                {
                    _eliminatedValues.Add(number);
                }
            }

            var allValue = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            if (_eliminatedValues.Count == 8)
            {
                var value = allValue.Except(_eliminatedValues).ElementAt(0);
                MarkAsSolved(value);
            }
        }

        public void MarkAsSolved(int value)
        {
            Value = value;
            IsSolved = true;
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
    }
}