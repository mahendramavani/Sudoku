using System;
using System.Collections.Generic;
using System.Linq;
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
                _view.DisplayCurrentStatus(_sumCells);

                var anythingSolved2 = CalculateLastNumberOfSet();
                _view.DisplayCurrentStatus(_sumCells);
                if (!anythingSolved1 && !anythingSolved2)
                    break;
            } 

            //_view.DisplayCurrentStatus(_sumCells);
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
                    if (EliminateNeverUsedNumbers(sumCell, sumCell.LookupKeyX, sumCell.GetXPartCells(), Direction.Horizontal))
                        anythingSolved = true;
                }
                if (sumCell.HasSumY)
                {
                    if (EliminateNeverUsedNumbers(sumCell, sumCell.LookupKeyY, sumCell.GetYPartCells(), Direction.Vertical))
                        anythingSolved = true;
                }
            });

            return anythingSolved;
        }

        private bool EliminateNeverUsedNumbers(SumCell sumCell, string lookupKey, GameCell[] partCells, Direction direction)
        {
            foreach (var gameCell in partCells.Where(x => x.IsSolved))
            {
                sumCell.AddAlreadySolved(gameCell.Value, direction);
            }

            var neverUsed = NumberSets[lookupKey].NeverUsed;
            var alreadySolved = sumCell.GetAlreadySolved(direction);
            var eliminationList = neverUsed.Union(alreadySolved);

            var anythingSolved = false;
            foreach (var gameCell in partCells.Where(x=>!x.IsSolved))
            {
                if (gameCell.EliminateTheNumbers(eliminationList.ToArray()))
                {
                    anythingSolved = true;
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
                    if (CheckForLastMissingPart(sumCell,sumCell.SumX, sumCell.GetXPartCells(), Direction.Horizontal))
                        anythingSolved = true;
                }

                if (sumCell.HasSumY)
                {
                    if (CheckForLastMissingPart(sumCell,sumCell.SumY, sumCell.GetYPartCells(), Direction.Vertical))
                        anythingSolved = true;
                }
            });

            return anythingSolved;
        }

        private bool CheckForLastMissingPart(SumCell sumCell, int sum, GameCell[] parts, Direction direction)
        {
            var anythingSolved = false;

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

            var unSolvedCount = unSolvedCells.Count;
            if (unSolvedCount == 1)
            {
                unSolvedCells[0].MarkAsSolved(sum);
                anythingSolved = true;
            }
            else if (unSolvedCount > 1)
            {
                //Work on the remaining (unsolved subset) and see if anymore possibilities can be eliminated
                var neverUsed = NumberSets[NumberSet.KeyFrom(unSolvedCount, sum)].NeverUsed;
                foreach (var unSolvedCell in unSolvedCells)
                {
                    if (unSolvedCell.EliminateTheNumbers(neverUsed.ToArray()))
                        anythingSolved = true;
                }
            }

            return anythingSolved;
        }
    }
}
