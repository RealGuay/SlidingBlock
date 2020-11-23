using System;
using System.Collections.Generic;

namespace SlidingLogic
{
   public class BlockFrame
   {
      public int NbCells { get; set; }
      private int xDim;
      private int yDim;
      private List<Block> cells;
      private Block emptyCellBlock = new Block(-1);

      public BlockFrame(int xDim, int yDim)
      {
         ValidateFrameDimensions(xDim, yDim);
         this.xDim = xDim;
         this.yDim = yDim;
         cells = new List<Block>();
         InitializeFrame(xDim, yDim);
         NbCells = cells.Count;
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

      private void InitializeFrame(int xDim, int yDim)
      {
         for (int i = 0; i < xDim * yDim; i++)
         {
            cells.Add(new  Block(i));
         }
      }

      public int GetBlockId(int i)
      {
         return cells[i].Id;
      }

      public void RemoveLastBlock()
      {
         cells[NbCells - 1] = emptyCellBlock;
      }

      public void SwapBlocks(int fromIndex, int toIndex)
      {
         Block tempBlock = cells[toIndex];

         cells[toIndex] = cells[fromIndex];
         cells[fromIndex] = tempBlock;
      }
   }
}