using System;
using System.Collections.Generic;
using Kakuro.Core.Domain;

namespace Kakuro.Core
{
    public class KakuroBoard
    {
        private static readonly Dictionary<string, NumberSet>  NumberSets = NumberSetBuilder.BuildNumberSets();
        private readonly List<SumCell> _sumCells = new List<SumCell>();

        private readonly IKakuroView _view;

        private readonly int _xSize;
        private readonly int _ySize;

        public KakuroBoard(IKakuroView view, int xSize, int ySize)
        {
            _view = view;
            _xSize = xSize;
            _ySize = ySize;
        }

        public void Solve()
        {
            BuildKakuroBoardFromUserInput();
            _view.DisplayCurrentStatus(_sumCells);

            while (true)
            {
                var anythingSolved1 = CalculateEliminatedValueSet();
                var anythingSolved2 = CalculateLastNumberOfSet();

                if (!anythingSolved1 && !anythingSolved2)
                    break;
            } 

            _view.DisplayCurrentStatus(_sumCells);
        }

        private void BuildKakuroBoardFromUserInput()
        {
            var userInputs = _view.UserInputs;

            var cells = new Cell[_xSize,_ySize];
            for (var y = 0; y < _ySize; y++)
            {
                for (var x = 0; x < _xSize; x++)
                {
                    var cell = Cell.NewCell(userInputs[x, y],x,y);

                    if (cell is RemovedCell)
                        _view.RemoveCell(cell);

                    if (cell is SumCell sumCell)
                        _sumCells.Add(sumCell);

                    cells[x,y] = cell;
                }
            }
            BuildSumPartsChain(cells);
        }

        private void BuildSumPartsChain(Cell[,] cells)
        {
            for (var y = 0; y < _ySize; y++)
            {
                for (var x = 0; x < _xSize; x++)
                {
                    if (!(cells[x, y] is SumCell currentCell))
                        continue;

                    if (currentCell.HasSumX)
                    {
                        BuildGameCellChainForSumCell(currentCell, i => cells[i, y], x + 1, _xSize, Direction.Horizontal);
                    }
                    if (currentCell.HasSumY)
                    {
                        BuildGameCellChainForSumCell(currentCell, i => cells[x, i], y + 1, _ySize, Direction.Vertical);
                    }
                }
            }
        }

        private void BuildGameCellChainForSumCell(SumCell currentCell, Func<int,Cell> cellAt, int startPosition, int endPosition, Direction direction)
        {
            for (var i = startPosition; i < endPosition; i++)
            {
                if (cellAt(i) is GameCell gameCell)
                {
                    currentCell.AddPartGameCell(gameCell, direction);
                }
                else
                    break;
            }
        }

        private bool CalculateEliminatedValueSet()
        {
            var anythingSolved = false;

            _sumCells.ForEach(sumCell =>
            {
                if (sumCell.HasSumX)
                {
                    anythingSolved  = EliminateNeverUsedNumbers(sumCell, sumCell.LookupKeyX, sumCell.GetXPartCells(), Direction.Horizontal) || anythingSolved;
                }
                if (sumCell.HasSumY)
                {
                    anythingSolved = EliminateNeverUsedNumbers(sumCell, sumCell.LookupKeyY, sumCell.GetYPartCells(), Direction.Vertical) || anythingSolved;
                }
            });

            return anythingSolved;
        }

        private bool EliminateNeverUsedNumbers(SumCell sumCell, string lookupKey, GameCell[] partCells, Direction direction)
        {
            var anythingSolved = false;
            var neverUsed = NumberSets[lookupKey].NeverUsed;
            foreach (var gameCell in partCells)
            {
                if (gameCell.IsSolved)
                    sumCell.AddAlreadySolved(gameCell.Value, direction);
                else
                {
                    gameCell.EliminateTheNumbers(neverUsed.ToArray());
                    if (gameCell.CanSolveIfAllButOneAreEliminated(out var solvedValue))
                    {
                        sumCell.AddAlreadySolved(solvedValue, direction);
                        anythingSolved = true;
                    }
                }
            }
            return anythingSolved;
        }

        private bool CalculateLastNumberOfSet()
        {
            var anythingSolved = false;

            _sumCells.ForEach(sumCell =>
            {
                if (sumCell.HasSumX)
                {
                    anythingSolved = CheckForLastMissingPart(sumCell,sumCell.SumX, sumCell.GetXPartCells(), Direction.Horizontal) || anythingSolved;
                }

                if (sumCell.HasSumY)
                {
                    anythingSolved = CheckForLastMissingPart(sumCell,sumCell.SumY, sumCell.GetYPartCells(), Direction.Vertical) || anythingSolved;
                }
            });

            return anythingSolved;
        }

        private bool CheckForLastMissingPart(SumCell sumCell, int sum, GameCell[] parts, Direction direction)
        {
            var unSolvedCells = new List<GameCell>();
            foreach (var gameCell in parts)
            {
                if (gameCell.IsSolved)
                {
                    var gameCellValue = gameCell.Value;
                    sumCell.AddAlreadySolved(gameCellValue,direction);
                    sum -= gameCellValue;
                }
                else
                {
                    unSolvedCells.Add(gameCell);
                }
            }

            if (unSolvedCells.Count == 1)
            {
                unSolvedCells[0].MarkAsSolved(sum);
                return true;
            }

            return false;
        }
    }
}
