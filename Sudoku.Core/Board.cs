using System;
using System.Collections.Generic;

namespace Sudoku.Core
{
    public class Board
    {
        //http://pi.math.cornell.edu/~mec/Summer2009/meerkamp/Site/Solving_any_Sudoku_II.html
        private const int SUDOKU_INNER_SIZE = 3;
        private const int SUDOKU_SIZE = SUDOKU_INNER_SIZE * SUDOKU_INNER_SIZE;
        private int iterationCount = 0;
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
            if (iterationCount >= 1000)
            {
                Console.Out.WriteLine($"Completed 1000 iteration but still No solution is reached.");
                return;
            }

            iterationCount++;

            PlaceFinding();
            PreemptiveSet();
        }

        private void PlaceFinding()
        {
            var anyPointReachedSolution = false;

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
                var solutionReachedForThePoint = point.CheckIfSolutionIsReachedAfterSettingThePossibleValues(possibleValues);
                if (solutionReachedForThePoint)
                {
                    anyPointReachedSolution = true;
                }
            });

            if (anyPointReachedSolution)
            {   //Take another pass of PlaceFinding
                PlaceFinding();
            }
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
            var foundPairInRow = FindPreemptiveSetPairWithinRows();
            var foundPairInColumn = FindPreemptiveSetPairWithinColumns();
            var foundPairInBox = FindPreemptiveSetPairWithinInnerBoxes();
            
            return foundPairInRow || foundPairInColumn || foundPairInBox;
        }

        private bool FindPreemptiveSetPairWithinRows()
        {
            for (var y = 0; y < SUDOKU_SIZE; y++)
            {
                //One row, at a time
                var matchedPoints = new List<Point>();
                for (var x = 0; x < SUDOKU_SIZE; x++)
                {
                    var point = _points[x, y];
                    if (point.IsSolved() || point.ProcessedAsRowPair)
                        continue;

                    if (point.PossibleValuesAreOfSize(2))
                    {
                        matchedPoints.Add(point);
                    }
                }

                if (matchedPoints.Count > 1)
                {
                    for (var i = 0; i < matchedPoints.Count - 1; i++)
                    {
                        //Find the matching Pair
                        for (var j = i + 1; j < matchedPoints.Count; j++)
                        {
                            if (matchedPoints[j].IsSolved())
                                continue;

                            var currentSetValue = matchedPoints[i].GetAllPossibleValueAsSortedCombinedInteger();
                            var otherSetValue = matchedPoints[j].GetAllPossibleValueAsSortedCombinedInteger();

                            if (currentSetValue == otherSetValue)
                            {
                                //Found a pair
                                ProcessPreemptiveSetPairWithinRow(matchedPoints[i], matchedPoints[j]);
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        private bool FindPreemptiveSetPairWithinColumns()
        {
            for (var x = 0; x < SUDOKU_SIZE; x++)
            {
                //One Column, at a time
                var matchedPoints = new List<Point>();
                for (var y = 0; y < SUDOKU_SIZE; y++)
                {
                    var point = _points[x, y];
                    if (point.IsSolved() || point.ProcessedAsColumnPair)
                        continue;

                    if (point.PossibleValuesAreOfSize(2))
                    {
                        matchedPoints.Add(point);
                    }
                }

                if (matchedPoints.Count > 1)
                {
                    for (var i = 0; i < matchedPoints.Count - 1; i++)
                    {
                        //Find the matching Pair
                        for (var j = i + 1; j < matchedPoints.Count; j++)
                        {
                            if (matchedPoints[j].IsSolved())
                                continue;

                            var currentSetValue = matchedPoints[i].GetAllPossibleValueAsSortedCombinedInteger();
                            var otherSetValue = matchedPoints[j].GetAllPossibleValueAsSortedCombinedInteger();

                            if (currentSetValue == otherSetValue)
                            {
                                //Found a pair
                                ProcessPreemptiveSetPairWithinColumn(matchedPoints[i], matchedPoints[j]);
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        private bool FindPreemptiveSetPairWithinInnerBoxes()
        {
            for (var xBox = 0; xBox < SUDOKU_INNER_SIZE; xBox++)
            {
                for (var yBox = 0; yBox < SUDOKU_INNER_SIZE; yBox++)
                {   //Process the Box[xBox,yBox]
                    var xStartOfInnerBox = xBox * SUDOKU_INNER_SIZE;
                    var xEndOfInnerBox = (xBox + 1) * SUDOKU_INNER_SIZE;
                    var yStartOfInnerBox = yBox * SUDOKU_INNER_SIZE;
                    var yEndOfInnerBox = (yBox + 1) * SUDOKU_INNER_SIZE;

                    var pointsInBox = new List<Point>();
                    for (var y = yStartOfInnerBox; y < yEndOfInnerBox; y++)
                    {
                        for (var x = xStartOfInnerBox; x < xEndOfInnerBox; x++)
                        {
                            pointsInBox.Add(_points[x,y]);
                        }
                    }

                    var matchedPoints = new List<Point>();
                    for (var index = 0; index < pointsInBox.Count; index++)
                    {
                        var point = pointsInBox[index];

                        if (point.IsSolved() || point.ProcessedAsBoxPair)
                            continue;

                        if (point.PossibleValuesAreOfSize(2))
                        {
                            matchedPoints.Add(point);
                        }
                    }

                    if (matchedPoints.Count > 1)
                    {
                        for (var i = 0; i < matchedPoints.Count - 1; i++)
                        {
                            //Find the matching Pair
                            for (var j = i + 1; j < matchedPoints.Count; j++)
                            {
                                if (matchedPoints[j].IsSolved())
                                    continue;

                                var currentSetValue = matchedPoints[i].GetAllPossibleValueAsSortedCombinedInteger();
                                var otherSetValue = matchedPoints[j].GetAllPossibleValueAsSortedCombinedInteger();

                                if (currentSetValue == otherSetValue)
                                {
                                    //Found a pair
                                    ProcessPreemptiveSetPairWithinBox(pointsInBox,matchedPoints[i], matchedPoints[j]);
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            
            return false;
        }

        private void ProcessPreemptiveSetPairWithinRow(Point point1, Point point2)
        {
            var yPosition = point1.Y;

            for (var x = 0; x < SUDOKU_SIZE; x++)
            {
                if (x == point1.X || x == point2.X)
                    continue;

                if (!_points[x, yPosition].IsSolved())
                {
                    _points[x, yPosition].RemovePossibleValueMatchingGivenPoint(point1);
                }
            }

            point1.ProcessedAsRowPair = true;
            point2.ProcessedAsRowPair = true;
        }

        private void ProcessPreemptiveSetPairWithinColumn(Point point1, Point point2)
        {
            var xPosition = point1.X;

            for (var y = 0; y < SUDOKU_SIZE; y++)
            {
                if (y == point1.Y || y == point2.Y)
                    continue;

                if (!_points[xPosition, y].IsSolved())
                {
                    _points[xPosition, y].RemovePossibleValueMatchingGivenPoint(point1);
                }
            }

            point1.ProcessedAsColumnPair = true;
            point2.ProcessedAsColumnPair = true;
        }

        private void ProcessPreemptiveSetPairWithinBox(List<Point> pointsInBox, Point point1, Point point2)
        {
            foreach (var point in pointsInBox)
            {
                if (point.Equals(point1) || point.Equals(point2))
                    continue;

                if (!point.IsSolved())
                {
                    point.RemovePossibleValueMatchingGivenPoint(point1);
                }
            }

            point1.ProcessedAsBoxPair = true;
            point2.ProcessedAsBoxPair = true;
        }



        private bool FindAndProcessThreeValuePreemptiveSet()
        {
            var foundTripletInRow = FindTripletWithinRows();
            var foundTripletInColumn = FindTripletWithinColumns();
            var foundTripletInBox = FindTripletWithinInnerBoxes();

            return foundTripletInRow || foundTripletInColumn || foundTripletInBox;
        }

        private bool FindTripletWithinRows()
        {
            for (var y = 0; y < SUDOKU_SIZE; y++)
            {
                //One row, at a time
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

                if (matchedPoints.Count > 2)
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
                            {
                                //Found the first match. 
                                //Now, try to find third exact match
                                for (var k = j + 1; k < matchedPoints.Count; k++)
                                {
                                    if (matchedPoints[k].IsSolved())
                                        continue;

                                    var thirdMatchValue = matchedPoints[k].GetAllPossibleValueAsSortedCombinedInteger();
                                    if (firstMatchValue == thirdMatchValue)
                                    {
                                        //Found the third exact match
                                        ProcessPreemptiveSetTripletWithinRow(matchedPoints[i], matchedPoints[j], matchedPoints[k]);
                                        return true;
                                    }
                                }

                                //if third exact match is not found,
                                //then try to find third partial match which will be of length 2.
                                //Remember, this partial match can be anywhere in the same row
                                for (var k = 0; k < SUDOKU_SIZE; k++)
                                {
                                    var thirdPoint = _points[k, y];

                                    if (k == matchedPoints[i].X || k == matchedPoints[j].X)
                                        continue;

                                    var thirdPartialMatchValue = thirdPoint.GetAllPossibleValueAsSortedCombinedInteger();

                                    if (firstMatchValue.ToString().Contains(thirdPartialMatchValue.ToString()))
                                    {
                                        //Found third partial match of length 2
                                        ProcessPreemptiveSetTripletWithinRow(matchedPoints[i], matchedPoints[j], thirdPoint);
                                        return true;
                                    }
                                }
                                
                                //if third partial match (of length 2) is not found,
                                //then try to find third partial match of length 4.
                                //Remember, this partial match can be anywhere in the same row
                                for (var k = 0; k < SUDOKU_SIZE; k++)
                                {
                                    var thirdPoint = _points[k, y];
                                    if (k == matchedPoints[i].X || k == matchedPoints[j].X)
                                        continue;
                                    
                                    var thirdPartialMatchValue = thirdPoint.GetAllPossibleValueAsSortedCombinedInteger();

                                    if (thirdPartialMatchValue.ToString().Contains(firstMatchValue.ToString()))
                                    {
                                        //Found third partial match of length 4
                                        thirdPoint.MatchPossibleValuesWith(matchedPoints[i]);
                                        ProcessPreemptiveSetTripletWithinRow(matchedPoints[i], matchedPoints[j], thirdPoint);
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        private bool FindTripletWithinColumns()
        {
            for (var x = 0; x < SUDOKU_SIZE; x++)
            {
                //One Column, at a time
                var matchedPoints = new List<Point>();
                for (var y = 0; y < SUDOKU_SIZE; y++)
                {
                    var point = _points[x, y];
                    if (point.IsSolved() || point.ProcessedAsColumnTriplet)
                        continue;

                    if (point.PossibleValuesAreOfSize(3))
                    {
                        matchedPoints.Add(point);
                    }
                }

                if (matchedPoints.Count > 2)
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
                            {
                                //Found the first match. 
                                //Now, try to find third exact match
                                for (var k = j + 1; k < matchedPoints.Count; k++)
                                {
                                    if (matchedPoints[k].IsSolved())
                                        continue;

                                    var thirdMatchValue = matchedPoints[k].GetAllPossibleValueAsSortedCombinedInteger();
                                    if (firstMatchValue == thirdMatchValue)
                                    {
                                        //Found the third exact match
                                        ProcessPreemptiveSetTripletWithinColumn(matchedPoints[i], matchedPoints[j], matchedPoints[k]);
                                        return true;
                                    }
                                }

                                //if third exact match is not found,
                                //then try to find third partial match which will be of length 2.
                                //Remember, this partial match can be anywhere in the same row
                                for (var k = 0; k < SUDOKU_SIZE; k++)
                                {
                                    var thirdPoint = _points[x, k];

                                    if (k == matchedPoints[i].Y || k == matchedPoints[j].Y)
                                        continue;

                                    var thirdPartialMatchValue = thirdPoint.GetAllPossibleValueAsSortedCombinedInteger();

                                    if (firstMatchValue.ToString().Contains(thirdPartialMatchValue.ToString()))
                                    {
                                        //Found third partial match of length 2
                                        ProcessPreemptiveSetTripletWithinColumn(matchedPoints[i], matchedPoints[j], thirdPoint);
                                        return true;
                                    }
                                }

                                //if third partial match (of length 2) is not found,
                                //then try to find third partial match of length 4.
                                //Remember, this partial match can be anywhere in the same row
                                for (var k = 0; k < SUDOKU_SIZE; k++)
                                {
                                    var thirdPoint = _points[x, k];
                                    if (k == matchedPoints[i].Y || k == matchedPoints[j].Y)
                                        continue;

                                    var thirdPartialMatchValue = thirdPoint.GetAllPossibleValueAsSortedCombinedInteger();

                                    if (thirdPartialMatchValue.ToString().Contains(firstMatchValue.ToString()))
                                    {
                                        //Found third partial match of length 4
                                        thirdPoint.MatchPossibleValuesWith(matchedPoints[i]);
                                        ProcessPreemptiveSetTripletWithinColumn(matchedPoints[i], matchedPoints[j], thirdPoint);
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        private bool FindTripletWithinInnerBoxes()
        {
            return false;
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

        private void ProcessPreemptiveSetTripletWithinColumn(Point point1, Point point2, Point point3)
        {
            var xPosition = point1.X;

            for (var y = 0; y < SUDOKU_SIZE; y++)
            {
                if (y == point1.Y || y == point2.Y || y == point3.Y)
                    continue;

                if (!_points[xPosition, y].IsSolved())
                {
                    _points[xPosition, y].RemovePossibleValueMatchingGivenPoint(point1);
                }
            }

            point1.ProcessedAsColumnTriplet = true;
            point2.ProcessedAsColumnTriplet = true;
            point3.ProcessedAsColumnTriplet = true;
        }
        private void ProcessPreemptiveSetTripletWithinBox(Point point1, Point point2, Point point3)
        {
        }

        public void PrintCurrentSolution()
        {
            Console.Out.WriteLine("".PadRight(145, '—'));

            ForEveryPoint(point =>
            {
                if (point.Value == 0)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write($"[{string.Join("", point.GetPossibleValues())}]".PadRight(10,' '));
                    Console.ResetColor();
                }
                else
                {
                    if (point.Type == ValueType.Calculated)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                        Console.Write(point.Value.ToString().PadRight(10, ' '));
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Write(point.Value.ToString().PadRight(10, ' '));
                    }
                }
                Console.Write("\t");

                if (point.X % SUDOKU_INNER_SIZE == 2)
                {
                    Console.Out.Write("|");
                }

                if (point.X==SUDOKU_SIZE-1 && point.Y % SUDOKU_INNER_SIZE == 2)
                {
                    Console.Out.Write("\n".PadRight(145, '—'));
                }
            }
            ,() => Console.Write("\n"));

            Console.Out.WriteLine();
            Console.Out.WriteLine("".PadRight(145, '='));
            Console.Out.WriteLine();

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