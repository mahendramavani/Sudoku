using System;
using System.Collections.Generic;

namespace Sudoku
{
    public class Board
    {
        //http://pi.math.cornell.edu/~mec/Summer2009/meerkamp/Site/Solving_any_Sudoku_II.html
        private const int SUDOKU_INNER_SIZE = 3;
        private const int SUDOKU_SIZE = SUDOKU_INNER_SIZE * SUDOKU_INNER_SIZE;

        private readonly Point[,] _points = new Point[SUDOKU_SIZE,SUDOKU_SIZE];

        public Board(int[,] initialInputs)
        {
            for (var x = 0; x < SUDOKU_SIZE; x++)
            {
                for (var y = 0; y < SUDOKU_SIZE; y++)
                {
                    var point = new Point(x,y,initialInputs[y,x]);
                    _points[x,y] = point;
                }
            }
        }

        public void Solve()
        {
            PlaceFinding();
            PreemptiveSet();
        }

        private void PlaceFinding()
        {
            ForEveryPoint(point =>
            {
                if (point.IsSolved())
                    return;

                var possibleValues = new List<int> {1, 2, 3, 4, 5, 6, 7, 8, 9};
                //Check all other values in the current row
                for (var x = 0; x < SUDOKU_SIZE; x++)
                {
                    var pointValue = _points[x, point.Y].Value;
                    if (possibleValues.Contains(pointValue))
                    {
                        possibleValues.Remove(pointValue);
                    }
                }

                //Check all other values in the current column
                for (var y = 0; y < SUDOKU_SIZE; y++)
                {
                    var pointValue = _points[point.X, y].Value;
                    if (possibleValues.Contains(pointValue))
                    {
                        possibleValues.Remove(pointValue);
                    }
                }

                //Check all other values in the inner box

                //First determine, to which inner box, the current point belongs to
                var currentX = point.X;
                var currentY = point.Y;

                var xPosOfInnerBox = currentX / SUDOKU_INNER_SIZE; //integer math, gives whole number only
                var yPosOfInnerBox = currentY / SUDOKU_INNER_SIZE; //integer math, gives whole number only

                var xStartOfInnerBox = xPosOfInnerBox * SUDOKU_INNER_SIZE;
                var xEndOfInnerBox = (xPosOfInnerBox + 1) * SUDOKU_INNER_SIZE;
                var yStartOfInnerBox = yPosOfInnerBox * SUDOKU_INNER_SIZE;
                var yEndOfInnerBox = (yPosOfInnerBox + 1) * SUDOKU_INNER_SIZE;

                //Now check for values within the "current" inner box
                for (var x = xStartOfInnerBox; x < xEndOfInnerBox; x++)
                {
                    for (var y = yStartOfInnerBox; y < yEndOfInnerBox; y++)
                    {
                        var pointValue = _points[x, y].Value;
                        if (possibleValues.Contains(pointValue))
                        {
                            possibleValues.Remove(pointValue);
                        }
                    }
                }

                //At this point, whatever is left in the list is the Possible Values for the current point
                point.SetPossibleValues(possibleValues);
            });
        }

        private void PreemptiveSet()
        {
            bool solvedAtLeastOne = false;
            var takeAnotherPass = true;
            while (takeAnotherPass)
            {
                var foundPair = FindAndProcessTwoValuePreemptiveSet();
                var foundTriplet = FindAndProcessThreeValuePreemptiveSet();

                takeAnotherPass = foundPair || foundTriplet;
                solvedAtLeastOne |= takeAnotherPass;
            }

            if (solvedAtLeastOne)
            {
                Solve();
            }
        }

        private bool FindAndProcessTwoValuePreemptiveSet()
        {
            var foundAtLeastOnePair = false;

            //Find PreemptiveSet within Rows
            for (var y = 0; y < SUDOKU_SIZE; y++)
            {   //One row, at a time
                var matchedPoints = new List<Point>();
                for (var x = 0; x < SUDOKU_SIZE; x++)
                {
                    var point = _points[x,y];
                    if (point.IsSolved() || point.ProcessedAsRowPair)
                        continue;

                    if (point.PossibleValuesAreOfSize(2))
                    {
                        matchedPoints.Add(point);
                    }
                }

                if (matchedPoints.Count > 1)
                {
                    for (var i = 0; i < matchedPoints.Count-1; i++)
                    {
                        //Find the matching Pair
                        for (var j = i+1; j < matchedPoints.Count; j++)
                        {
                            if (matchedPoints[j].IsSolved())
                                continue;

                            var currentSetValue = matchedPoints[i].GetAllPossibleValueAsSortedCombinedInteger();
                            var otherSetValue = matchedPoints[j].GetAllPossibleValueAsSortedCombinedInteger();

                            if (currentSetValue == otherSetValue)
                            {   //Found a pair
                                ProcessPreemptiveSetPairWithinRow(matchedPoints[i], matchedPoints[j]);
                                foundAtLeastOnePair = true;
                            }
                        }
                    }
                }
            }

            //Find PreemptiveSet within Columns

            //Find PreemptiveSet within Inner box
            
            return foundAtLeastOnePair;
        }

        private bool FindAndProcessThreeValuePreemptiveSet()
        {
            var foundAtLeastOneTriplet = false;

            //Find PreemptiveSet within Rows
            for (var y = 0; y < SUDOKU_SIZE; y++)
            {   //One row, at a time
                var matchedPoints = new List<Point>();
                for (var x = 0; x < SUDOKU_SIZE; x++)
                {
                    var point = _points[x, y];
                    if (point.IsSolved() || point.ProcessedAsRowTriplet)
                        continue;

                    if (point.PossibleValuesAreOfSize(3))
                    {
                        matchedPoints.Add(point);
                    }
                }

                if (matchedPoints.Count > 1)
                {
                    for (var i = 0; i < matchedPoints.Count - 1; i++)
                    {
                        //Find the first exact match
                        for (var j = i + 1; j < matchedPoints.Count; j++)
                        {
                            if (matchedPoints[j].IsSolved())
                                continue;

                            var firstMatchValue = matchedPoints[i].GetAllPossibleValueAsSortedCombinedInteger();
                            var secondMatchValue = matchedPoints[j].GetAllPossibleValueAsSortedCombinedInteger();

                            if (firstMatchValue == secondMatchValue)
                            {   //Found the first match. 
                                //Now, try to find third exact match
                                for (var k = j+1; k < matchedPoints.Count; k++)
                                {
                                    if (matchedPoints[k].IsSolved())
                                        continue;

                                    var thirdMatchValue = matchedPoints[k].GetAllPossibleValueAsSortedCombinedInteger();
                                    if (firstMatchValue == thirdMatchValue)
                                    {   //Found the third exact match
                                        ProcessPreemptiveSetTripletWithinRow(matchedPoints[i],matchedPoints[j],matchedPoints[k]);
                                        foundAtLeastOneTriplet = true;
                                    }
                                }

                                //if third match is not found,
                                //then try to find third partial match which will be of length 2.
                                //Remember, this partial match can be anywhere in the same row
                                for (var k = 0; k < SUDOKU_SIZE; k++)
                                {
                                    if (k == matchedPoints[i].X || k == matchedPoints[j].X)
                                        continue;

                                    var thirdPoint = _points[k,y];
                                   
                                    var thirdPartialMatchValue = thirdPoint.GetAllPossibleValueAsSortedCombinedInteger();

                                    if (firstMatchValue.ToString().Contains(thirdPartialMatchValue.ToString()))
                                    {   //Found third partial match
                                        ProcessPreemptiveSetTripletWithinRow(matchedPoints[i],matchedPoints[j],thirdPoint);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            //Find PreemptiveSet within Columns

            //Find PreemptiveSet within Inner box

            return foundAtLeastOneTriplet;
        }

        private void ProcessPreemptiveSetPairWithinRow(Point point1, Point point2)
        {
            var yPosition = point1.Y;

            for (var x = 0; x < SUDOKU_SIZE; x++)
            {
                if (x==point1.X || x==point2.X)
                    continue;

                if (!_points[x, yPosition].IsSolved())
                {
                    _points[x, yPosition].RemovePossibleValueMatchingGivenPoint(point1);
                }
            }

            point1.ProcessedAsRowPair = true;
            point2.ProcessedAsRowPair = true;
        }

        private void ProcessPreemptiveSetTripletWithinRow(Point point1, Point point2, Point point3)
        {
            var yPosition = point1.Y;

            for (var x = 0; x < SUDOKU_SIZE; x++)
            {
                if (x == point1.X || x == point2.X || x == point3.X)
                    continue;

                if (!_points[x, yPosition].IsSolved())
                {
                    _points[x, yPosition].RemovePossibleValueMatchingGivenPoint(point1);
                }
            }

            point1.ProcessedAsRowTriplet = true;
            point2.ProcessedAsRowTriplet = true;
            point3.ProcessedAsRowTriplet = true;
        }

        public void PrintCurrentSolution(Action<string> display, Action unSolvedColor, Action resetColor)
        {
            ForEveryPoint(point =>
            {
                if (point.Value == 0)
                {
                    unSolvedColor();
                    display($"[{string.Join("", point.GetPossibleValues())}]".PadRight(10,' '));
                    resetColor();
                }
                else
                {
                    display(point.Value.ToString().PadRight(10,' '));
                }
                display("\t");
            }
            ,() => display("\n"));
        }

        private void ForEveryPoint(Action<Point> actionForEveryPoint)
        {
            ForEveryPoint(actionForEveryPoint,() => {});
        }

        private void ForEveryPoint(Action<Point> actionForEveryPoint, Action endOfRowAction)
        {
            for (var y = 0; y < SUDOKU_SIZE; y++)
            {
                for (var x = 0; x < SUDOKU_SIZE; x++)
                {
                    actionForEveryPoint(_points[x, y]);
                }
                endOfRowAction();
            }
        }
    }
}