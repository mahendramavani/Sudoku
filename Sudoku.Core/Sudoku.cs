﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Sudoku.Core
{
    public class Sudoku
    {
        private readonly IView _view;

        //http://pi.math.cornell.edu/~mec/Summer2009/meerkamp/Site/Solving_any_Sudoku_II.html
        public const int INNER_SIZE = 3;
        public const int SIZE = INNER_SIZE * INNER_SIZE;

        private int _iterationCountForPlaceFindingAndPreemptiveSet;
        private int _iterationCountForCandidateChecking;
        private readonly Point[,] _points = new Point[SIZE,SIZE];
        private readonly Point[,] _knownPoints = new Point[SIZE,SIZE];

        public Sudoku(IView view)
        {
            _view = view;
        }

        public void Solve()
        {
            var inputs = _view.Inputs;

            for (var x = 0; x < SIZE; x++)
            {
                for (var y = 0; y < SIZE; y++)
                {
                    if (inputs[y, x] < 0 || inputs[y, x] > 9)
                    {
                        _view.InValidInput($"Invalid input at Cell({x},{y})");
                        return;
                    }

                    var point = new Point(x, y, inputs[y, x]);
                    _points[x, y] = point;
                }
            }

            Iterate();
            TryWithPotentialCandidate();
        }

        private void Iterate()
        {
            if (_iterationCountForPlaceFindingAndPreemptiveSet++ >= 1000)
            {
                _view.AppendStatus($"Completed {_iterationCountForPlaceFindingAndPreemptiveSet} iteration of PlaceFinding and PreemptiveSet but still No solution is reached.") ;
                return;
            }

            PlaceFinding();
            PreemptiveSet();
        }

        private void PlaceFinding()
        {
            _view.AppendStatus("Taking a pass with PlaceFinding.");
            var anyPointReachedSolution = false;
            var anyPointInViolation = false;

            ForEveryPoint(point =>
            {
                if (point.IsSolved())
                    return;

                var possibleValues = new List<int> {1, 2, 3, 4, 5, 6, 7, 8, 9};
                //Check all other values in the current row
                for (var x = 0; x < SIZE; x++)
                {
                    if (x == point.X)
                        continue;

                    var pointValue = _points[x, point.Y].Value;
                    if (possibleValues.Contains(pointValue))
                    {
                        possibleValues.Remove(pointValue);
                    }
                }

                //Check all other values in the current column
                for (var y = 0; y < SIZE; y++)
                {
                    if (y == point.Y)
                        continue;

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

                var xPosOfInnerBox = currentX / INNER_SIZE; //integer math, gives whole number only
                var yPosOfInnerBox = currentY / INNER_SIZE; //integer math, gives whole number only

                var xStartOfInnerBox = xPosOfInnerBox * INNER_SIZE;
                var xEndOfInnerBox = (xPosOfInnerBox + 1) * INNER_SIZE;
                var yStartOfInnerBox = yPosOfInnerBox * INNER_SIZE;
                var yEndOfInnerBox = (yPosOfInnerBox + 1) * INNER_SIZE;

                //Now check for values within the "current" inner box
                for (var x = xStartOfInnerBox; x < xEndOfInnerBox; x++)
                {
                    for (var y = yStartOfInnerBox; y < yEndOfInnerBox; y++)
                    {
                        if (x == currentX && y == currentY)
                            continue;

                        var pointValue = _points[x, y].Value;
                        if (possibleValues.Contains(pointValue))
                        {
                            possibleValues.Remove(pointValue);
                        }
                    }
                }

                //At this point, whatever is left in the list is the Possible Values for the current point
                if (possibleValues.Count == 0)
                {
                    anyPointInViolation = true;
                }
                else
                {
                    var solutionReachedForThePoint = point.CheckIfSolutionIsReachedAfterSettingThePossibleValues(possibleValues);
                    if (solutionReachedForThePoint)
                    {
                        anyPointReachedSolution = true;
                    }
                }
            });
           
            if (anyPointReachedSolution && !anyPointInViolation)
            {   //Take another pass of PlaceFinding
                PlaceFinding();
            }
        }

        private void PreemptiveSet()
        {
            _view.AppendStatus("Taking a pass with PreemptiveSet.");

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
                Iterate();
            }
        }

        private void TryWithPotentialCandidate()
        {
            if (_iterationCountForCandidateChecking++ > 100)
            {
                _view.AppendStatus($"Completed {_iterationCountForCandidateChecking} iteration of CandidateFinding but still No solution is reached.");
                return;
            }

            _view.AppendStatus("------------------------------------------------");
            _view.AppendStatus($"Round {_iterationCountForCandidateChecking} of PotentialCandidate.");
            _view.Print(_points);
            
            if (IsSolutionReached())
                return;

            var selectedPoint = ChooseFirstPointWithTwoPossibilities();

            if (selectedPoint == null)
            {
                _view.AppendStatus($"No cell with two possibility found. Can't proceed with Try&Check method.");
                return;
            }

            var possibleValues = selectedPoint.GetPossibleValues();
            var smallerValue = possibleValues[0];
            var biggerValue = possibleValues[1];
            
            _view.AppendStatus($"Selecting point ({selectedPoint.X},{selectedPoint.Y}) for substitution. Possible Values=[{possibleValues[0]},{possibleValues[1]}]");

            SaveCurrentStatus();
            selectedPoint.GuessValue(smallerValue);//First try the smaller value
            PlaceFinding();
            _view.Print(_points);
            if (HasViolatedCondition())
            {
                _view.AppendStatus($"Violation detected after PlaceFinding. Reverting back to last known valid state");
                TryWithSecondValue(selectedPoint,biggerValue);
            }
            else
            {
                PreemptiveSet();
                _view.Print(_points);
                if (HasViolatedCondition())
                {
                    _view.AppendStatus($"Violation detected after PreemptiveSet finding. Reverting back to last known valid state");
                    TryWithSecondValue(selectedPoint,biggerValue);
                }
                else
                {
                    PlaceFinding();
                    _view.Print(_points);
                    if (HasViolatedCondition())
                    {
                        _view.AppendStatus($"Violation detected after second round of PlaceFinding. Reverting back to last known valid state");
                        TryWithSecondValue(selectedPoint, biggerValue);
                    }
                    else
                    {
                        selectedPoint.FinalizeGuessedValue();
                        _view.AppendStatus($"All good so far. First value is the right choice");
                    }
                }
            }

            if (!IsSolutionReached())
            {
                TryWithPotentialCandidate();
            }
        }

        private void TryWithSecondValue(Point selectedPoint,int secondValue)
        {
            _view.Print(_points);
            _view.AppendStatus("Status before reverting(above) and after Reverting back to valid state(below)");

            RevertToLastKnownStatus();
            _view.Print(_points);

            selectedPoint = _points[selectedPoint.X, selectedPoint.Y];
            selectedPoint.SetValue(secondValue); //Now, we know the valid value must be the remaining one
            
            _view.AppendStatus($"Now selecting second value [Point({selectedPoint.X},{selectedPoint.Y})={secondValue}] and Iterate (PlaceFinding + PreemptiveSet).");
            
            Iterate();
            _view.Print(_points);
        }

        private Point ChooseFirstPointWithTwoPossibilities()
        {
            for (var x = 0; x < SIZE; x++)
            {
                for (var y = 0; y < SIZE; y++)
                {
                    if (_points[x,y].PossibleValuesAreOfSize(2))
                    {
                        return _points[x,y];
                    }
                }
            }

            return null;
        }

        private bool HasViolatedCondition()
        {
            //First check each row
            for (var y = 0; y < SIZE; y++)
            {
                var pointGroup = new List<Point>(SIZE);
                for (var x = 0; x < SIZE; x++)
                {
                    pointGroup.Add(_points[x, y]);
                }

                if (GroupIsViolatingCondition(pointGroup))
                {
                    return true;
                }
            }
            
            //Now check each Column
            for (var x = 0; x < SIZE; x++)
            {
                var pointGroup = new List<Point>(SIZE);
                for (var y = 0; y < SIZE; y++)
                {
                    pointGroup.Add(_points[x, y]);
                }

                if (GroupIsViolatingCondition(pointGroup))
                {
                    return true;
                }
            }

            //Now check each box
            for (var xBox = 0; xBox < INNER_SIZE; xBox++)
            {
                for (var yBox = 0; yBox < INNER_SIZE; yBox++)
                {
                    //Process the Box[xBox,yBox]
                    var xStartOfInnerBox = xBox * INNER_SIZE;
                    var xEndOfInnerBox = (xBox + 1) * INNER_SIZE;
                    var yStartOfInnerBox = yBox * INNER_SIZE;
                    var yEndOfInnerBox = (yBox + 1) * INNER_SIZE;

                    var pointGroup = new List<Point>();
                    for (var y = yStartOfInnerBox; y < yEndOfInnerBox; y++)
                    {
                        for (var x = xStartOfInnerBox; x < xEndOfInnerBox; x++)
                        {
                            pointGroup.Add(_points[x, y]);
                        }
                    }
                    if (GroupIsViolatingCondition(pointGroup))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool GroupIsViolatingCondition(List<Point> pointGroup)
        {
            foreach (var point in pointGroup)
            {
                if (point.IsSolved())
                {
                    var duplicateValue = pointGroup.Any(x=> !x.Equals(point) && x.Value == point.Value);
                    if (duplicateValue)
                    {
                        _view.AppendStatus($"Duplicate value for Point({point.X},{point.Y})");
                        return true;
                    }
                }
                else
                {
                    var possibleValues = point.GetPossibleValues();
                    if (possibleValues.Length == 0)
                    {
                        _view.AppendStatus($"No possible Candidate left for Point({point.X},{point.Y})");
                        return true;
                    }

                    foreach (var possibleValue in possibleValues)
                    {
                        var duplicateValue = pointGroup.SkipWhile(x => x.Equals(point)).Any(x => x.Value == possibleValue);
                        if (duplicateValue)
                        {
                            _view.AppendStatus($"Invalid Possible values for Point({point.X},{point.Y})");
                            return true;
                        }
                    }
                }
            }

            /*
            var possibleValues = new List<int>(new []{1,2,3,4,5,6,7,8,9});

            foreach (var point in pointGroup)
            {
                if (point.IsSolved())
                {
                    var value = point.Value;

                    if (!possibleValues.Contains(value))
                    {
                        _view.AppendStatus($"Duplicate value for Point({point.X},{point.Y})");
                        return true;
                    }

                    possibleValues.Remove(value);
                }
                else
                {
                    if (point.PossibleValuesAreOfSize(0))
                    {
                        _view.AppendStatus($"No possible Candidate left for Point({point.X},{point.Y})");
                        return true;
                    }
                }
            }*/
            return false;
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
            for (var y = 0; y < SIZE; y++)
            {
                //One row, at a time
                var matchedPoints = new List<Point>();
                for (var x = 0; x < SIZE; x++)
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
            for (var x = 0; x < SIZE; x++)
            {
                //One Column, at a time
                var matchedPoints = new List<Point>();
                for (var y = 0; y < SIZE; y++)
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
            for (var xBox = 0; xBox < INNER_SIZE; xBox++)
            {
                for (var yBox = 0; yBox < INNER_SIZE; yBox++)
                {   //Process the Box[xBox,yBox]
                    var xStartOfInnerBox = xBox * INNER_SIZE;
                    var xEndOfInnerBox = (xBox + 1) * INNER_SIZE;
                    var yStartOfInnerBox = yBox * INNER_SIZE;
                    var yEndOfInnerBox = (yBox + 1) * INNER_SIZE;

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

            for (var x = 0; x < SIZE; x++)
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

            for (var y = 0; y < SIZE; y++)
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
            var foundTripletInRow = FindPreemptiveSetTripletWithinRows();
            var foundTripletInColumn = FindPreemptiveSetTripletWithinColumns();
            var foundTripletInBox = FindPreemptiveSetTripletWithinInnerBoxes();

            return foundTripletInRow || foundTripletInColumn || foundTripletInBox;
        }

        private bool FindPreemptiveSetTripletWithinRows()
        {
            for (var y = 0; y < SIZE; y++)
            {
                //One row, at a time
                var matchedPoints = new List<Point>();
                for (var x = 0; x < SIZE; x++)
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
                        //Find the second exact match
                        for (var j = i + 1; j < matchedPoints.Count; j++)
                        {
                            if (matchedPoints[j].IsSolved())
                                continue;

                            var firstMatchValue = matchedPoints[i].GetAllPossibleValueAsSortedCombinedInteger();
                            var secondMatchValue = matchedPoints[j].GetAllPossibleValueAsSortedCombinedInteger();

                            if (firstMatchValue == secondMatchValue)
                            {
                                //Found the second match. 
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
                                //then try to find third partial match which will be of length 2 or 4.
                                //Remember, this partial match can be anywhere in the same row
                                for (var k = 0; k < SIZE; k++)
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

        private bool FindPreemptiveSetTripletWithinColumns()
        {
            for (var x = 0; x < SIZE; x++)
            {
                //One Column, at a time
                var matchedPoints = new List<Point>();
                for (var y = 0; y < SIZE; y++)
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
                                for (var k = 0; k < SIZE; k++)
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

        private bool FindPreemptiveSetTripletWithinInnerBoxes()
        {
            for (var xBox = 0; xBox < INNER_SIZE; xBox++)
            {
                for (var yBox = 0; yBox < INNER_SIZE; yBox++)
                {   //Process the Box[xBox,yBox]
                    var xStartOfInnerBox = xBox * INNER_SIZE;
                    var xEndOfInnerBox = (xBox + 1) * INNER_SIZE;
                    var yStartOfInnerBox = yBox * INNER_SIZE;
                    var yEndOfInnerBox = (yBox + 1) * INNER_SIZE;

                    var pointsInBox = new List<Point>();
                    for (var y = yStartOfInnerBox; y < yEndOfInnerBox; y++)
                    {
                        for (var x = xStartOfInnerBox; x < xEndOfInnerBox; x++)
                        {
                            pointsInBox.Add(_points[x, y]);
                        }
                    }

                    var matchedPoints = new List<Point>();
                    for (var index = 0; index < pointsInBox.Count; index++)
                    {
                        var point = pointsInBox[index];

                        if (point.IsSolved() || point.ProcessedAsBoxTriplet)
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
                                    //Found the second match. 
                                    //Now, try to find third exact match
                                    for (var k = j + 1; k < matchedPoints.Count; k++)
                                    {
                                        if (matchedPoints[k].IsSolved())
                                            continue;

                                        var thirdMatchValue = matchedPoints[k].GetAllPossibleValueAsSortedCombinedInteger();
                                        if (firstMatchValue == thirdMatchValue)
                                        {
                                            //Found the third exact match
                                            ProcessPreemptiveSetTripletWithinBox(pointsInBox, matchedPoints[i], matchedPoints[j], matchedPoints[k]);
                                            return true;
                                        }
                                    }

                                    //if third exact match is not found,
                                    //then try to find third partial match which will be of length 2 or 4.
                                    //Remember, this partial match can be anywhere in the same row
                                    foreach (var thirdPoint in pointsInBox)
                                    {
                                        if (thirdPoint.Equals(matchedPoints[i]) || thirdPoint.Equals(matchedPoints[j]))
                                            continue;

                                        var thirdPartialMatchValue = thirdPoint.GetAllPossibleValueAsSortedCombinedInteger();

                                        if (firstMatchValue.ToString().Contains(thirdPartialMatchValue.ToString()))
                                        {
                                            //Found third partial match of length 2
                                            ProcessPreemptiveSetTripletWithinBox(pointsInBox,matchedPoints[i], matchedPoints[j], thirdPoint);
                                            return true;
                                        }
                                        else if (thirdPartialMatchValue.ToString().Contains(firstMatchValue.ToString()))
                                        {
                                            //Found third partial match of length 4
                                            thirdPoint.MatchPossibleValuesWith(matchedPoints[i]);
                                            ProcessPreemptiveSetTripletWithinBox(pointsInBox,matchedPoints[i], matchedPoints[j], thirdPoint);
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        private void ProcessPreemptiveSetTripletWithinRow(Point point1, Point point2, Point point3)
        {
            var yPosition = point1.Y;

            for (var x = 0; x < SIZE; x++)
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

            for (var y = 0; y < SIZE; y++)
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
       
        private void ProcessPreemptiveSetTripletWithinBox(List<Point> boxPoints, Point point1, Point point2, Point point3)
        {
            foreach (var point in boxPoints)
            {
                if (point.Equals(point1) || point.Equals(point2) || point.Equals(point3))
                    continue;

                if (!point.IsSolved())
                {
                    point.RemovePossibleValueMatchingGivenPoint(point1);
                }
            }

            point1.ProcessedAsBoxTriplet = true;
            point2.ProcessedAsBoxTriplet = true;
            point3.ProcessedAsBoxTriplet = true;
        }


        private void SaveCurrentStatus()
        {
            for (var y = 0; y < SIZE; y++)
            {
                for (var x = 0; x < SIZE; x++)
                {
                    _knownPoints[x, y] = _points[x, y].ShallowCopy();
                }
            }
        }
        
        private void RevertToLastKnownStatus()
        {
            for (var y = 0; y < SIZE; y++)
            {
                for (var x = 0; x < SIZE; x++)
                {
                    _points[x, y] =_knownPoints[x, y].ShallowCopy();
                }
            }
        }

        public bool IsSolutionReached()
        {
            for (var y = 0; y < SIZE; y++)
            {
                for (var x = 0; x < SIZE; x++)
                {
                    if (!_points[x,y].IsSolved())
                        return false;
                }
            }
            return true;
        }
        
        private void ForEveryPoint(Action<Point> actionForEveryPoint)
        {
            if (actionForEveryPoint == null) throw new ArgumentNullException(nameof(actionForEveryPoint));
            ForEveryPoint(actionForEveryPoint,() => {});
        }

        private void ForEveryPoint(Action<Point> actionForEveryPoint, Action endOfRowAction)
        {
            for (var y = 0; y < SIZE; y++)
            {
                for (var x = 0; x < SIZE; x++)
                {
                    actionForEveryPoint(_points[x, y]);
                }
                endOfRowAction();
            }
        }
    }
}