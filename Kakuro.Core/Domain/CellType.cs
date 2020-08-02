using System;
using System.Collections.Generic;

namespace Kakuro.Core.Domain
{
    public class CellType : IEquatable<CellType>, IComparer<CellType>
    {
        public string DisplayName { get; }
        public int Id { get; }

        public static readonly CellType RemovedCell = new CellType("Removed");
        public static readonly CellType SumCell = new CellType("Sum");
        public static readonly CellType GameCell = new CellType("Game");

        private CellType(string displayName)
        {
            Id = displayName.GetHashCode();
            DisplayName = displayName;
        }

        public override string ToString()
        {
            return DisplayName;
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public bool Equals(CellType other)
        {
            return other != null && Id.Equals(other.Id);
        }

        public int Compare(CellType x, CellType y)
        {
            if (x == null && y == null) return 0;
            if (x == null) return -1;
            if (y == null) return 1;

            return x.Id.CompareTo(y.Id);
        }
    }
}