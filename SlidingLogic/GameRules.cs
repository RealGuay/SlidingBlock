using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

[assembly: InternalsVisibleTo("SlidingLogic.Tests")]

namespace SlidingLogic
{
   public class GameRules
   {
      #region Fields

      private readonly int xDim;
      private readonly int yDim;
      private BlockFrame _frame;
      private int? _emptyBlockIndex;
      private bool _isShuffling;
      private bool _isInitialized;

      #endregion Fields

      #region Properties

      public List<int> MoveableBlockIndexes { get; }
      public int NbCells { get; private set; }

      #endregion Properties

      #region Events

      public event EventHandler<MoveBlockEventArgs> BlockMoved;

      public event EventHandler<int> BlockRemoved;

      public event EventHandler RemovedBlockReplaced;

      public event EventHandler EndOfGameDetected;

      #endregion Events

      public GameRules(int xDim, int yDim)
      {
         this.xDim = xDim;
         this.yDim = yDim;
         _isInitialized = false;
         MoveableBlockIndexes = new List<int>();
         CreateFrame();
      }

      private void CreateFrame()
      {
         _frame = new BlockFrame(xDim, yDim);
         NbCells = _frame.NbCells;
      }

      public void InitializeFrame(bool doShuffle = true)
      {
         if (!_isInitialized)
         {
            RemoveLastBlock();
            FindMoveableBlockIndexes();
            if (doShuffle)
            {
               ShuffleBlocks();
            }
            _isInitialized = true;
         }
      }

      internal void RemoveLastBlock()
      {
         int lastIndex = NbCells - 1;
         _frame.RemoveBlock(lastIndex);
         _emptyBlockIndex = lastIndex;
         BlockRemoved?.Invoke(this, lastIndex);
      }

      internal void FindMoveableBlockIndexes()
      {
         MoveableBlockIndexes.Clear();

         if (_emptyBlockIndex is null)
         {
            // no empty block removed then, no move possible
            return;
         }

         int emptyIndex = _emptyBlockIndex.Value;
         // block above
         int aboveIndex = emptyIndex - xDim;
         if (aboveIndex >= 0)
         {
            MoveableBlockIndexes.Add(aboveIndex);
         }

         // block below
         int belowIndex = emptyIndex + xDim;
         if (belowIndex < _frame.NbCells)
         {
            MoveableBlockIndexes.Add(belowIndex);
         }

         // block on left
         int leftIndex = emptyIndex - 1;
         if (_emptyBlockIndex % xDim > 0)
         {
            MoveableBlockIndexes.Add(leftIndex);
         }

         // block on right
         int rightIndex = emptyIndex + 1;
         if (rightIndex % xDim > 0)
         {
            MoveableBlockIndexes.Add(rightIndex);
         }
         // make it predictable for testing
         MoveableBlockIndexes.Sort();
      }

      internal void ShuffleBlocks()
      {
         _isShuffling = true;
         DoShuffleRandomly();

         if (!IsShuffled())
         {
            ShuffleBlocks();  // recursive !! dangerous ???
         }
         _isShuffling = false;
      }

      private void DoShuffleRandomly()
      {
         Random rand = new Random();

         int nbMoves = rand.Next(10, 100);

         for (int i = 0; i < nbMoves; i++)
         {
            int num = rand.Next(10000);
            int selectMoveableIndex = num % MoveableBlockIndexes.Count;

            MoveBlock(MoveableBlockIndexes[selectMoveableIndex]);
            Thread.Sleep(10);
         }
      }

      internal bool IsShuffled()
      {
         for (int i = 0; i < NbCells; i++)
         {
            if (i < NbCells - 1)
            {
               if (GetBlockId(i) != i)
               {
                  return true;
               }
            }
            else // last cell
            {
               if (GetBlockId(i) != -1)
               {
                  return true;
               }
            }
         }
         return false;
      }

      public void MoveBlock(int fromIndex)
      {
         ValidateMoveableBlock(fromIndex);

         int toIndex = _emptyBlockIndex.Value;
         _frame.SwapBlocks(fromIndex, toIndex);
         _emptyBlockIndex = fromIndex;
         FindMoveableBlockIndexes();
         BlockMoved?.Invoke(this, new MoveBlockEventArgs() { FromIndex = fromIndex, ToIndex = toIndex });

         DetectEndOfGame();
      }

      private void ValidateMoveableBlock(int fromIndex)
      {
         if (_emptyBlockIndex is null)
         {
            throw new InvalidOperationException("There is no empty block, therefore no moves are allowed.");
         }
         if (!MoveableBlockIndexes.Contains(fromIndex))
         {
            throw new ArgumentException($"Cannot move block in cell index: {fromIndex}");
         }
      }

      private void DetectEndOfGame()
      {
         if (!_isShuffling && !IsShuffled())
         {
            ReplaceRemovedBlock();
            EndOfGameDetected?.Invoke(this, EventArgs.Empty);
         }
      }

      private void ReplaceRemovedBlock()
      {
         _frame.ReplaceRemovedBlock();
         _emptyBlockIndex = null;
         FindMoveableBlockIndexes();
         _isInitialized = false;
         RemovedBlockReplaced?.Invoke(this, EventArgs.Empty);
      }

      internal int GetBlockId(int index)
      {
         return _frame.GetBlockId(index);
      }
   }
}