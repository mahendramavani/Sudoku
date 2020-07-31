using System;
using System.Collections.Generic;
using System.Text;

namespace Kakuro.Core.Domain
{
    public class NumberSet : IEquatable<NumberSet>, IComparer<NumberSet>
    {
        public const string KEY_FORMAT = "{0}_{1}"; 
        public int NumberOfCells { get; set; }
        public int Sum { get; set; }
        public List<List<int>> Sets { get; set; }
        public List<int> AlwaysUsed { get; set; }
        public List<int> NeverUsed { get; set; }
        public string Key => string.Format(KEY_FORMAT,NumberOfCells,Sum);

        public bool Equals(NumberSet other)
        {
            if (other == null)
                return false;

            return NumberOfCells == other.NumberOfCells && Sum == other.Sum;
        }

        public int Compare(NumberSet x, NumberSet y)
        {
            if (x == null || y == null)
                return -1;

            return x.NumberOfCells.CompareTo(y.NumberOfCells) + x.Sum.CompareTo(y.Sum);
        }

        public override string ToString()
        {
            var possibleSets = new StringBuilder();
            Sets.ForEach(x =>
            {
                x.ForEach(y => possibleSets.Append(y));
                possibleSets.Append(" ");
            });

            var alwaysUsedDigits = string.Join("", AlwaysUsed);
            if (alwaysUsedDigits == string.Empty)
                alwaysUsedDigits = "{}";

            var neverUsedDigits = string.Join("", NeverUsed);
            if (neverUsedDigits == string.Empty)
                neverUsedDigits = "{}";

            return $"{NumberOfCells}\t{Sum}\t{possibleSets}\t{alwaysUsedDigits}\t{neverUsedDigits}";
        }
    }
}