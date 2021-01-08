using System;
using System.Collections.Generic;
using System.Threading;

namespace SlidingLogic
{
   public class GameRules
   {
      private readonly int xDim;
      private readonly int yDim;
      private int emptyBlockIndex;
      private BlockFrame frame;
      private bool _isShuffling;
      private bool _isInitialized;

      public event EventHandler<MoveBlockEventArgs> BlockMoved;

      public event EventHandler<int> BlockRemoved;

      public event EventHandler RemovedBlockReplaced;

      public event EventHandler EndOfGameDetected;

      public List<int> MoveableBlockIndexes { get; }
      public int NbCells { get; private set; }

      public GameRules(int xDim, int yDim)
      {
         this.xDim = xDim;
         this.yDim = yDim;
         _isInitialized = false;
         emptyBlockIndex = -1;
         MoveableBlockIndexes = new List<int>();
         CreateFrame();
      }
      private void CreateFrame()
      {
         frame = new BlockFrame(xDim, yDim);
         NbCells = frame.NbCells;
      }

      public void InitializeFrame()
      {
         if (!_isInitialized)
         {
            RemoveLastBlock();
            FindMoveableBlockIndexes();
            ShuffleBlocks();
            _isInitialized = true;
         }
      }

      public void RemoveLastBlock()
      {
         int lastIndex = NbCells - 1;
         frame.RemoveBlock(lastIndex);
         emptyBlockIndex = lastIndex;
         BlockRemoved?.Invoke(this, lastIndex);
      }

      public void FindMoveableBlockIndexes()
      {
         MoveableBlockIndexes.Clear();

         if (emptyBlockIndex == -1)
         {
            // no block removed; no move possible
            return;
         }

         // block above
         int aboveIndex = emptyBlockIndex - xDim;
         if (aboveIndex >= 0)
         {
            MoveableBlockIndexes.Add(aboveIndex);
         }

         // block below
         int belowIndex = emptyBlockIndex + xDim;
         if (belowIndex < frame.NbCells)
         {
            MoveableBlockIndexes.Add(belowIndex);
         }

         // block on left
         int leftIndex = emptyBlockIndex - 1;
         if (emptyBlockIndex % xDim > 0)
         {
            MoveableBlockIndexes.Add(leftIndex);
         }

         // block on right
         int rightIndex = emptyBlockIndex + 1;
         if (rightIndex % xDim > 0)
         {
            MoveableBlockIndexes.Add(rightIndex);
         }
         // make it predictable for testing
         MoveableBlockIndexes.Sort();
      }

      public int GetBlockId(int index)
      {
         return frame.GetBlockId(index);
      }

      public bool IsShuffled()
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

         int toIndex = emptyBlockIndex;
         frame.SwapBlocks(fromIndex, toIndex);
         emptyBlockIndex = fromIndex;
         FindMoveableBlockIndexes();
         BlockMoved?.Invoke(this, new MoveBlockEventArgs() { FromIndex = fromIndex, ToIndex = toIndex });

         DetectEndOfGame();
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
         frame.ReplaceRemovedBlock();
         emptyBlockIndex = -1;
         FindMoveableBlockIndexes();
         _isInitialized = false;
         RemovedBlockReplaced?.Invoke(this, EventArgs.Empty);
      }

      public void ShuffleBlocks()
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

      private void ValidateMoveableBlock(int fromIndex)
      {
         if (!MoveableBlockIndexes.Contains(fromIndex))
         {
            throw new ArgumentException($"Cannot move block in cell index: {fromIndex}");
         }
      }
   }
}