using System.Collections.Generic;
using Kakuro.Core.Domain;

namespace Kakuro.Core
{
    public class KakuroBoard
    {
        private static Dictionary<string, NumberSet>  _numberSet = NumberSetBuilder.BuildNumberSets();
        private Cell[,] _cells = new Cell[0, 0];
        
        private readonly IKakuroView _view;

        private int _xSize;
        private int _ySize;

        public KakuroBoard(IKakuroView view, int xSize, int ySize)
        {
            _view = view;
            _xSize = xSize;
            _ySize = ySize;
        }

        public void Solve()
        {
            var userInputs = _view.UserInputs;

            _cells = new Cell[_xSize, _ySize];

            for (var y = 0; y < _ySize; y++)
            {
                for (var x = 0; x < _xSize; x++)
                {
                    _cells[x, y] = new Cell(userInputs[x, y]);
                }
            }

            _view.DisplayCurrentStatus(_cells);

            FindParts();
            CalculateEliminatedValueSet();
            CalculateLastNumberOfSet();    

            _view.DisplayCurrentStatus(_cells);
        }

        private void FindParts()
        {
            for (var y = 0; y < _ySize; y++)
            {
                for (var x = 0; x < _xSize; x++)
                {
                    if (_cells[x, y].IsSumXCell)
                    {
                        var numberOfParts = 0;
                        for (var i = x + 1; i < _xSize; i++)
                        {
                            if (_cells[i, y].IsGameCell())
                                numberOfParts++;
                            else
                                break;
                        }

                        _cells[x, y].SumXParts = numberOfParts;
                    }

                    if (_cells[x, y].IsSumYCell)
                    {
                        if (y == 1 && x == 4)
                        {
                            var stop = true;
                        }
                        var numberOfParts = 0;
                        for (var i = y + 1; i < _ySize; i++)
                        {
                            if (_cells[x, i].IsGameCell())
                                numberOfParts++;
                            else 
                                break;
                        }

                        _cells[x, y].SumYParts = numberOfParts;
                    }
                }
            }
        }

        private void CalculateEliminatedValueSet()
        {
            for (var y = 0; y < _ySize; y++)
            {
                for (var x = 0; x < _xSize; x++)
                {
                    if (_cells[x, y].IsSumXCell)
                    {
                        var key = string.Format(NumberSet.KEY_FORMAT, _cells[x,y].SumXParts, _cells[x, y].SumX);
                        var neverUsed = _numberSet[key].NeverUsed;

                        for (var i = x + 1; i < _xSize; i++)
                        {
                            if (_cells[i, y].IsGameCell())
                                _cells[i, y].EliminateNumber(neverUsed.ToArray());
                            else
                                break;
                        }
                    }

                    if (_cells[x, y].IsSumYCell)
                    {
                        var key = string.Format(NumberSet.KEY_FORMAT, _cells[x, y].SumYParts, _cells[x, y].SumY);
                        var neverUsed = _numberSet[key].NeverUsed;

                        for (var i = y + 1; i < _ySize; i++)
                        {
                            if (_cells[x, i].IsGameCell())
                                _cells[x, i].EliminateNumber(neverUsed.ToArray());
                            else
                                break;
                        }
                    }
                }
            }
        }

        private void CalculateLastNumberOfSet()
        {
            for (var y = 0; y < _ySize; y++)
            {
                for (var x = 0; x < _xSize; x++)
                {
                    if (_cells[x, y].IsSumXCell)
                    {
                        var sum = _cells[x, y].SumX;
                        var unSolvedCells = new List<Cell>();
                        for (var i = x + 1; i < _xSize; i++)
                        {
                            if (_cells[i, y].IsGameCell())
                            {
                                if (_cells[i, y].IsSolved)
                                {
                                    sum -= _cells[i, y].Value;
                                }
                                else
                                {
                                    unSolvedCells.Add(_cells[i,y]);
                                }
                            }
                            else
                                break;
                        }

                        if (unSolvedCells.Count == 1)
                        {
                            unSolvedCells[0].MarkAsSolved(sum);
                        }
                    }

                    if (_cells[x, y].IsSumYCell)
                    {
                        var sum = _cells[x, y].SumY;
                        var unSolvedCells = new List<Cell>();

                        for (var i = y + 1; i < _ySize; i++)
                        {
                            if (_cells[x, i].IsGameCell())
                            {
                                if (_cells[x, i].IsSolved)
                                {
                                    sum -= _cells[x, i].Value;
                                }
                                else
                                {
                                    unSolvedCells.Add(_cells[x, i]);
                                }
                            }
                            else
                                break;
                        }

                        if (unSolvedCells.Count == 1)
                        {
                            unSolvedCells[0].MarkAsSolved(sum);
                        }
                    }
                }
            }

        }
    }
}
