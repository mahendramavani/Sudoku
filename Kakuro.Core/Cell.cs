using System;
using System.Collections.Generic;
using System.Linq;

namespace Kakuro.Core
{
    public class Cell
    {
        private readonly List<int> _eliminatedValues = new List<int>();
        
        public bool IsRemoved { get; set; }
        public bool IsSumXCell { get; set; }
        public int SumX { get; set; }
        public int SumXParts { get; set; }
        public bool IsSumYCell { get; set; }
        public int SumY { get; set; }
        public int SumYParts { get; set; }
        public int Value { get; set; }

        public Cell(string initialValue)
        {
            if (initialValue.Equals("x",StringComparison.InvariantCultureIgnoreCase))
            {
                IsRemoved = true;
            }
            else if (initialValue.Contains(@"\"))
            {
                var parts = initialValue.Split(@"\");
                if (parts[0].Length == 0)
                {
                    IsSumYCell = false;
                }
                else
                {
                    IsSumYCell = true;
                    SumY = int.Parse(parts[0]);
                }
                
                if (parts[1].Length == 0)
                {
                    IsSumXCell = false;
                }
                else
                {
                    IsSumXCell = true;
                    SumX = int.Parse(parts[1]);
                }
            }
        }

        public bool IsSolved { get; set; }

        public void EliminateNumber(params int[] numbers)
        {
            foreach (var number in numbers)
            {
                if (number > 0 && number < 10 && !_eliminatedValues.Contains(number))
                {
                    _eliminatedValues.Add(number);
                }
            }

            var allValue = new[] {1, 2, 3, 4, 5, 6, 7, 8, 9}; 
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

        public bool IsGameCell()
        {
            return !IsRemoved && !IsSumYCell && !IsSumXCell;
        }

        public bool IsSumCell()
        {
            return IsSumXCell || IsSumYCell;
        }

        public string DisplaySolutionValue()
        {
            return Value.ToString();
        }
        public string DisplaySumCellValue() 
        { 
            return @"   \" + (IsSumXCell ? SumX.ToString() : string.Empty).PadLeft(3)
                           + "\r\n" 
                           + (IsSumYCell ? SumY.ToString() : string.Empty).PadRight(3) + @"\";
        }

        public string DisplayEliminationValue()
        {
            var displayValue = (_eliminatedValues.Contains(1) ? "_" : " ") + string.Empty.PadRight(6) + (_eliminatedValues.Contains(2) ? "_" : " ") + string.Empty.PadRight(6) + (_eliminatedValues.Contains(3) ? "_" : " ")
                               + "\r\n" + (_eliminatedValues.Contains(4) ? "_" : " ") + string.Empty.PadRight(6) + (_eliminatedValues.Contains(5) ? "_" : " ") + string.Empty.PadRight(6) + (_eliminatedValues.Contains(6) ? "_" : " ")
                               + "\r\n" + (_eliminatedValues.Contains(7) ? "_" : " ") + string.Empty.PadRight(6) + (_eliminatedValues.Contains(8) ? "_" : " ") + string.Empty.PadRight(6) + (_eliminatedValues.Contains(9) ? "_" : " ");

            return displayValue;
        }
    }
}