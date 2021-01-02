using System;
using System.Collections.Generic;

namespace SlidingLogic
{
   public class BlockFrame
   {
      private readonly List<Block> cells;
      private readonly Block emptyCellBlock = new Block(-1);
      private Block removedBlock;
      private int removedBlockIndex;
      public int NbCells { get; set; }

      public BlockFrame(int xDim, int yDim)
      {
         ValidateFrameDimensions(xDim, yDim);
         cells = new List<Block>();
         InitializeFrame(xDim, yDim);
      }

      private void InitializeFrame(int xDim, int yDim)
      {
         for (int i = 0; i < xDim * yDim; i++)
         {
            cells.Add(new Block(i));
         }
         NbCells = cells.Count;
      }

      public void RemoveBlock(int index)
      {
         removedBlock = cells[index];
         cells[index] = emptyCellBlock;
         removedBlockIndex = index;
      }

      public int GetBlockId(int index)
      {
         return cells[index].Id;
      }

      public void ReplaceRemovedBlock()
      {
         cells[removedBlockIndex] = removedBlock;
      }

      public void SwapBlocks(int index1, int index2)
      {
         Block tempBlock = cells[index2];

         cells[index2] = cells[index1];
         cells[index1] = tempBlock;
      }

      private static void ValidateFrameDimensions(int xDim, int yDim)
      {
         if (xDim < 2)
         {
            throw new ArgumentException("xDim must be greater than 1");
         }
         if (yDim < 2)
         {
            throw new ArgumentException("yDim must be greater than 1");
         }
      }
   }
}