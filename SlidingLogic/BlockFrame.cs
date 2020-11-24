using System;
using System.Collections.Generic;

namespace SlidingLogic
{
   public class BlockFrame
   {
      public int NbCells { get; set; }
      private List<Block> cells;
      private Block emptyCellBlock = new Block(-1);

      public BlockFrame(int xDim, int yDim)
      {
         ValidateFrameDimensions(xDim, yDim);
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

      public int GetBlockId(int index)
      {
         return cells[index].Id;
      }

      public void RemoveBlock(int index)
      {
         cells[index] = emptyCellBlock;
      }

      public void SwapBlocks(int index1, int index2)
      {
         Block tempBlock = cells[index2];

         cells[index2] = cells[index1];
         cells[index1] = tempBlock;
      }
   }
}