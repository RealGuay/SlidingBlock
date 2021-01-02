using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;

namespace SlidingLogic.Tests
{
   public class GameRulesTests
   {
      private GameRules game;

      [SetUp]
      public void Setup()
      {
         game = new GameRules(4, 5);
      }

      [Test]
      public void ShouldCreateFrameOnInitialize()
      {
         game.CreateFrame();

         Assert.AreEqual(20, game.NbCells);
      }

      [Test]
      public void ShouldRemoveLastBlock()
      {
         game.CreateFrame();

         game.RemoveLastBlock();

         Assert.AreEqual(-1, game.GetBlockId(19));
      }

      [Test]
      public void ShouldMoveBlock()
      {
         int index = 18;
         InitFrameWithoutShuffling();

         game.MoveBlock(index);

         Assert.AreEqual(18, game.GetBlockId(19));
         Assert.AreEqual(-1, game.GetBlockId(18));
      }

      [Test]
      public void ShouldThrowArgumentExceptionIfMoveBlockOnNonMoveableBlock()
      {
         InitFrameWithoutShuffling();

         Assert.Throws<ArgumentException>(() => game.MoveBlock(0));
      }

      [Test]
      public void ShouldThrowArgumentExceptionIfMoveBlockOnEmptyCell()
      {
         InitFrameWithoutShuffling();

         Assert.Throws<ArgumentException>(() => game.MoveBlock(19));
      }

      [Test]
      public void ShouldProvideIndexesOfMoveableBlocksOnBottomRightCorner()
      {
         InitFrameWithoutShuffling();

         // no move : right bottom corner is free at initialization
         List<int> foundIndexes = game.MoveableBlockIndexes;

         List<int> expectedIndexes = new List<int>() { 15, 18 };
         Assert.AreEqual(expectedIndexes, foundIndexes);
      }

      [Test]
      public void ShouldProvideIndexesOfMoveableBlocksOnBottomLeftCorner()
      {
         InitFrameWithoutShuffling();

         // free bottom left corner cell
         game.MoveBlock(18);
         game.MoveBlock(17);
         game.MoveBlock(16);
         List<int> foundIndexes = game.MoveableBlockIndexes;

         List<int> expectedIndexes = new List<int>() { 12, 17 };
         Assert.AreEqual(expectedIndexes, foundIndexes);
      }

      [Test]
      public void ShouldProvideIndexesOfMoveableBlocksOnTopLeftCorner()
      {
         InitFrameWithoutShuffling();

         // free top left corner cell
         game.MoveBlock(18);
         game.MoveBlock(17);
         game.MoveBlock(16);
         game.MoveBlock(12);
         game.MoveBlock(8);
         game.MoveBlock(4);
         game.MoveBlock(0);
         List<int> foundIndexes = game.MoveableBlockIndexes;

         List<int> expectedIndexes = new List<int>() { 1, 4 };
         Assert.AreEqual(expectedIndexes, foundIndexes);
      }

      [Test]
      public void ShouldProvideIndexesOfMoveableBlocksOnTopRightCorner()
      {
         InitFrameWithoutShuffling();

         // free top right corner cell
         game.MoveBlock(15);
         game.MoveBlock(11);
         game.MoveBlock(7);
         game.MoveBlock(3);
         List<int> foundIndexes = game.MoveableBlockIndexes;

         List<int> expectedIndexes = new List<int>() { 2, 7 };
         Assert.AreEqual(expectedIndexes, foundIndexes);
      }

      [Test]
      public void ShouldProvideIndexesOfMoveableBlocksOnTopRow()
      {
         InitFrameWithoutShuffling();

         // free a top row cell
         game.MoveBlock(15);
         game.MoveBlock(14);
         game.MoveBlock(10);
         game.MoveBlock(9);
         game.MoveBlock(5);
         game.MoveBlock(1);
         List<int> foundIndexes = game.MoveableBlockIndexes;

         List<int> expectedIndexes = new List<int>() { 0, 2, 5 };
         Assert.AreEqual(expectedIndexes, foundIndexes);
      }

      [Test]
      public void ShouldProvideIndexesOfMoveableBlocksOnBottomRow()
      {
         InitFrameWithoutShuffling();

         // free a bottom row cell
         game.MoveBlock(18);
         game.MoveBlock(17);
         List<int> foundIndexes = game.MoveableBlockIndexes;

         List<int> expectedIndexes = new List<int>() { 13, 16, 18 };
         Assert.AreEqual(expectedIndexes, foundIndexes);
      }

      [Test]
      public void ShouldProvideIndexesOfMoveableBlocksOnLeftColumn()
      {
         InitFrameWithoutShuffling();

         // free a left column cell
         game.MoveBlock(18);
         game.MoveBlock(17);
         game.MoveBlock(13);
         game.MoveBlock(9);
         game.MoveBlock(8);
         List<int> foundIndexes = game.MoveableBlockIndexes;

         List<int> expectedIndexes = new List<int>() { 4, 9, 12 };
         Assert.AreEqual(expectedIndexes, foundIndexes);
      }

      [Test]
      public void ShouldProvideIndexesOfMoveableBlocksOnRightColumn()
      {
         InitFrameWithoutShuffling();

         // free a right column cell
         game.MoveBlock(15);
         game.MoveBlock(11);
         List<int> foundIndexes = game.MoveableBlockIndexes;

         List<int> expectedIndexes = new List<int>() { 7, 10, 15 };
         Assert.AreEqual(expectedIndexes, foundIndexes);
      }

      [Test]
      public void ShouldProvideIndexesOfMoveableBlocksOnSurrondedCell()
      {
         InitFrameWithoutShuffling();

         // free a surrounded cell
         game.MoveBlock(15);
         game.MoveBlock(14);
         game.MoveBlock(10);
         List<int> foundIndexes = game.MoveableBlockIndexes;

         List<int> expectedIndexes = new List<int>() { 6, 9, 11, 14 };
         Assert.AreEqual(expectedIndexes, foundIndexes);
      }

      [Test]
      public void ShouldInvokeBlockMovedEventOnMove()
      {
         int fromIndex = -1;
         int toIndex = -1;
         InitFrameWithoutShuffling();

         var wait = new AutoResetEvent(false);
         game.BlockMoved += (s, e) => { wait.Set(); fromIndex = e.FromIndex; toIndex = e.ToIndex; };
         game.MoveBlock(18);

         Assert.IsTrue(wait.WaitOne(TimeSpan.FromSeconds(5)));
         Assert.AreEqual(18, fromIndex);
         Assert.AreEqual(19, toIndex);
      }

      [Test]
      public void ShouldProvideBlockIdForEachCellInInitialState()
      {
         InitFrameWithoutShuffling();

         List<int> expectedIds = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, -1 };
         List<int> foundIds = new List<int>();
         for (int i = 0; i < game.NbCells; i++)
         {
            foundIds.Add(game.GetBlockId(i));
         }

         Assert.AreEqual(expectedIds, foundIds);
      }

      [Test]
      public void ShouldProvideBlockIdForEachCellAfterFourMoves()
      {
         InitFrameWithoutShuffling();

         game.MoveBlock(18);
         game.MoveBlock(14);
         game.MoveBlock(13);
         game.MoveBlock(9);

         List<int> expectedIds = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, -1, 10, 11, 12, 9, 13, 15, 16, 17, 14, 18 };
         List<int> foundIds = new List<int>();

         for (int i = 0; i < game.NbCells; i++)
         {
            foundIds.Add(game.GetBlockId(i));
         }

         Assert.AreEqual(expectedIds, foundIds);
      }

      [Test]
      public void ShouldNotBeShuffledInitialy()
      {
         InitFrameWithoutShuffling();

         bool isShuffled = game.IsShuffled();
         Assert.AreEqual(false, isShuffled);  // this is NOT true because it random  !!! (???)
      }

      public void ShouldNotBeShuffledAfterReversedMoves()
      {
         InitFrameWithoutShuffling();

         game.MoveBlock(18);
         game.MoveBlock(14);
         game.MoveBlock(15);

         game.MoveBlock(14);
         game.MoveBlock(18);
         game.MoveBlock(19);

         bool isShuffled = game.IsShuffled();
         Assert.AreEqual(false, isShuffled);
      }

      [Test]
      public void ShouldBeShuffledAfterOneMove()
      {
         InitFrameWithoutShuffling();

         game.MoveBlock(15);

         bool isShuffled = game.IsShuffled();
         Assert.AreEqual(true, isShuffled);
      }

      [Test]
      public void ShouldShuffleBlocks()
      {
         InitFrameWithoutShuffling();

         game.ShuffleBlocks();

         ReadAllCellIds(out List<int> expectedIds, out List<int> foundIds);

         Assert.AreNotEqual(expectedIds, foundIds);
      }

      [Test]
      public void ShouldShuffleBlocksOnInitializeFrame()
      {
         game.InitializeFrame();

         ReadAllCellIds(out List<int> expectedIds, out List<int> foundIds);

         Assert.AreNotEqual(expectedIds, foundIds);
      }

      private void InitFrameWithoutShuffling()
      {
         game.CreateFrame();
         game.RemoveLastBlock();
         game.FindMoveableBlockIndexes();
      }

      private void ReadAllCellIds(out List<int> expectedIds, out List<int> foundIds)
      {
         expectedIds = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, -1 };
         foundIds = new List<int>();
         for (int i = 0; i < game.NbCells; i++)
         {
            foundIds.Add(game.GetBlockId(i));
         }
      }
   }
}