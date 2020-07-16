using System;
using System.Collections.Generic;
using System.Linq;

namespace Sudoku.Core
{
    public class Point : IEquatable<Point>
    {
        private List<int> _possibleValues = new List<int>();
        private List<int> _eliminatedValues = new List<int>();

        public bool ProcessedAsRowPair { get; set; }
        public bool ProcessedAsColumnPair { get; set; }
        public bool ProcessedAsBoxPair { get; set; }
        public bool ProcessedAsRowTriplet { get; set; }
        public bool ProcessedAsColumnTriplet { get; set; }
        public bool ProcessedAsBoxTriplet { get; set; }

        public int X { get; }
        public int Y { get; }
        public int Value { get; private set; }
        public ValueType Type { get; private set; }

        public Point(int x, int y, int initialValue)
        {
            X = x;
            Y = y;
            Value = initialValue;
            Type = (initialValue == 0 ? ValueType.Unknown : ValueType.Given);
        }
        public int[] GetPossibleValues()
        {
            _possibleValues.Sort();
            return _possibleValues.ToArray();
        }

        public bool IsSolved()
        {
            return Value > 0;
        }

        public void SetValue(int val)
        {
            if (val != 0)
            {
                _possibleValues.Clear();
                Type = ValueType.Calculated;
                Value = val;
            }
        }

        public void AddPossibleValue(int possibleValue)
        {
            _possibleValues.Add(possibleValue);
        }

        public bool PossibleValuesAreOfSize(int size)
        {
            return _possibleValues.Count == size;
        }

        public int GetAllPossibleValueAsSortedCombinedInteger()
        {
            int combinedInteger=0;
            try
            {
                combinedInteger = IsSolved() ? Value : int.Parse(string.Join("", GetPossibleValues()));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
            }
            return combinedInteger;
        }

        public override int GetHashCode()
        {
            return X*17+Y*19;
        }

        public bool Equals(Point other)
        {
            return other!= null && X == other.X && Y == other.Y;
        }

        public void RemovePossibleValueMatchingGivenPoint(Point point)
        {
            var possibleValues = point.GetPossibleValues();
            foreach (var value in possibleValues)
            {
                if (_possibleValues.Contains(value))
                {
                    _possibleValues.Remove(value);
                    _eliminatedValues.Add(value);
                }
            }

            if (_possibleValues.Count == 1)
            {
                SetValue(_possibleValues[0]);
            }
        }

        public bool CheckIfSolutionIsReachedAfterSettingThePossibleValues(List<int> possibleValues)
        {
            _possibleValues = possibleValues?? new List<int>();

            foreach (var eliminatedValue in _eliminatedValues)
            {
                _possibleValues.Remove(eliminatedValue);
            }
            if (_possibleValues.Count == 1)
            {
                SetValue(_possibleValues[0]);
                return true;
            }

            return false;
        }

        public void MatchPossibleValuesWith(Point point)
        {
            var possibleValues = point.GetPossibleValues();
            var removeList = _possibleValues.Except(possibleValues).ToArray();

            foreach (var value in removeList)
            {
                _possibleValues.Remove(value);
                _eliminatedValues.Add(value);
            }

            if (_possibleValues.Count == 1)
            {
                SetValue(_possibleValues[0]);
            }
        }
    }

    public enum ValueType
    {
        Unknown,
        Given,
        Calculated,
        Guessed
    }
}
