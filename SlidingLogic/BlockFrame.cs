using System;
using System.Collections.Generic;

namespace SlidingLogic
{
   internal class BlockFrame
   {
      private readonly List<Block> _cells;
      private readonly Block _emptyCellBlock = new Block(-1);
      private Block _removedBlock;
      private int _removedBlockIndex;
      public int NbCells { get; set; }

      public BlockFrame(int xDim, int yDim)
      {
         ValidateFrameDimensions(xDim, yDim);
         _cells = new List<Block>();
         InitializeFrame(xDim, yDim);
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
            _cells.Add(new Block(i));
         }
         NbCells = _cells.Count;
      }

      public void RemoveBlock(int index)
      {
         _removedBlock = _cells[index];
         _cells[index] = _emptyCellBlock;
         _removedBlockIndex = index;
      }

      public void ReplaceRemovedBlock()
      {
         _cells[_removedBlockIndex] = _removedBlock;
      }

      public void SwapBlocks(int index1, int index2)
      {
         Block tempBlock = _cells[index2];

         _cells[index2] = _cells[index1];
         _cells[index1] = tempBlock;
      }

      internal int GetBlockId(int index)
      {
         return _cells[index].Id;
      }
   }
}