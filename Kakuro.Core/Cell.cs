using System;

namespace Kakuro.Core
{
    public class Cell
    {
        public int XPosition { get; private set; }
        public int YPosition { get; private set; }

        protected Cell(int x, int y)
        {
            XPosition = x;
            YPosition = y;
        }
        public static Cell NewCell(string initialValue, int xPosition, int yPosition)
        {
            if (initialValue.Equals("x",StringComparison.InvariantCultureIgnoreCase))
            {
                return new RemovedCell(xPosition,yPosition);
            }

            if (initialValue.Contains(@"\"))
            {
                var sumCell = new SumCell(xPosition, yPosition);
                
                var parts = initialValue.Split(@"\");
                if (parts[0].Length == 0)
                {
                    sumCell.HasSumY = false;
                }
                else
                {
                    sumCell.HasSumY = true;
                    sumCell.SumY = int.Parse(parts[0]);
                }
                
                if (parts[1].Length == 0)
                {
                    sumCell.HasSumX = false;
                }
                else
                {
                    sumCell.HasSumX = true;
                    sumCell.SumX = int.Parse(parts[1]);
                }

                return sumCell;
            }

            return new GameCell(xPosition, yPosition);
        }
    }
}