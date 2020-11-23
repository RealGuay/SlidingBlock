using NUnit.Framework;
using System;

namespace SlidingLogic.Tests
{
   public class BlockFrameTests
   {
      private BlockFrame frame;

      [SetUp]
      public void Setup()
      {
         frame = new BlockFrame(4, 5);
      }

      [Test]
      public void ShouldCreateFrameWithNbyMCells()
      {
         Assert.AreEqual(20, frame.NbCells);
      }

      [Test]
      public void ShouldThrowArgumentExceptionIfXDimensionLessThanTwo()
      {
         Assert.Throws<ArgumentException>(() => new BlockFrame(1, 5), "xDim must be 2 or greater");
      }

      [Test]
      public void ShouldThrowArgumentExceptionIfYDimensionLessThanTwo()
      {
         Assert.Throws<ArgumentException>(() => new BlockFrame(4, 1), "yDim must be 2 or greater");
      }

      [Test]
      public void ShouldCreateOneBlockPerCell()
      {

      }

      [Test]
      public void ShouldCreateBlocksInOrder()
      {
         int[] frameCellIndexes, foundBlockIds;
         ExtractAllBlockIds(out frameCellIndexes, out foundBlockIds);

         Assert.AreEqual(frameCellIndexes, foundBlockIds);
      }

      private void ExtractAllBlockIds(out int[] frameCellIndexes, out int[] foundBlockIds)
      {
         frameCellIndexes = new int[frame.NbCells];
         foundBlockIds = new int[frame.NbCells];
         for (int i = 0; i < frame.NbCells; i++)
         {
            frameCellIndexes[i] = i;
            foundBlockIds[i] = frame.GetBlockId(i);
         }
      }

      // valid test ?? : the exception is thrown but not by my code !!!
      [Test]
      public void ShouldThrowArgumentExceptionIfCellIndexIsTooLarge()
      {
         Assert.Throws<ArgumentOutOfRangeException>(() => frame.GetBlockId(20));
      }

      [Test]
      public void ShouldRemoveLastBlockFromFrame()
      {
         frame.RemoveLastBlock();

         int[] frameCellIndexes, foundBlockIds;
         ExtractAllBlockIds(out frameCellIndexes, out foundBlockIds);

         frameCellIndexes[frame.NbCells - 1] = -1; // empty cell indicator

         Assert.AreEqual(frameCellIndexes, foundBlockIds);
      }

      [Test]
      public void ShouldSwapBlocks()
      {
         frame.SwapBlocks(2, 5);

         int[] frameCellIndexes, foundBlockIds;
         ExtractAllBlockIds(out frameCellIndexes, out foundBlockIds);

         frameCellIndexes[2] = 5;
         frameCellIndexes[5] = 2;

         Assert.AreEqual(frameCellIndexes, foundBlockIds);
      }
   }
}