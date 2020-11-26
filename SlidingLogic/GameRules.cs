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

      public GameRules(int xDim, int yDim)
      {
         this.xDim = xDim;
         this.yDim = yDim;
         MoveableBlockIndexes = new List<int>();
         InitializeFrame();
      }

      public event EventHandler<MoveBlockEventArgs> BlockMoved;

      public List<int> MoveableBlockIndexes { get; }
      public int NbCells { get; private set; }

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
   }

   public void ShuffleBlocks()
   {
      DoShuffleRandomly();

      if (!IsShuffled())
      {
         ShuffleBlocks();  // recursive !! dangerous ???
      }
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

   private void FindMoveableBlockIndexes()
   {
      MoveableBlockIndexes.Clear();

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

   private void InitializeFrame()
   {
      frame = new BlockFrame(xDim, yDim);
      NbCells = frame.NbCells;
      RemoveLastBlock();
      FindMoveableBlockIndexes();
   }

   private void RemoveLastBlock()
   {
      int lastIndex = NbCells - 1;
      frame.RemoveBlock(lastIndex);
      emptyBlockIndex = lastIndex;
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