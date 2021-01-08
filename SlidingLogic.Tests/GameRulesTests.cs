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
      public void ShouldGenerateCellsOnConstructor()
      {
         Assert.AreEqual(20, game.NbCells);
      }

      [Test]
      public void ShouldRemoveLastBlock()
      {
         game.RemoveLastBlock();

         Assert.AreEqual(-1, game.GetBlockId(19));
      }

      [Test]
      public void ShouldMoveBlock()
      {
         int index = 18;
         game.InitializeFrame(false);

         game.MoveBlock(index);

         Assert.AreEqual(18, game.GetBlockId(19));
         Assert.AreEqual(-1, game.GetBlockId(18));
      }

      [Test]
      public void ShouldThrowArgumentExceptionIfMoveBlockOnNonMoveableBlock()
      {
         game.InitializeFrame(false);

         Assert.Throws<ArgumentException>(() => game.MoveBlock(0));
      }

      [Test]
      public void ShouldThrowArgumentExceptionIfMoveBlockOnEmptyCell()
      {
         game.InitializeFrame(false);

         Assert.Throws<ArgumentException>(() => game.MoveBlock(19));
      }

      [Test]
      public void ShouldProvideIndexesOfMoveableBlocksOnBottomRightCorner()
      {
         game.InitializeFrame(false);

         // no move : right bottom corner is free at initialization
         List<int> foundIndexes = game.MoveableBlockIndexes;

         List<int> expectedIndexes = new List<int>() { 15, 18 };
         Assert.AreEqual(expectedIndexes, foundIndexes);
      }

      [Test]
      public void ShouldProvideIndexesOfMoveableBlocksOnBottomLeftCorner()
      {
         game.InitializeFrame(false);

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
         game.InitializeFrame(false);

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
         game.InitializeFrame(false);

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
         game.InitializeFrame(false);

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
         game.InitializeFrame(false);

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
         game.InitializeFrame(false);

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
         game.InitializeFrame(false);

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
         game.InitializeFrame(false);

         // free a surrounded cell
         game.MoveBlock(15);
         game.MoveBlock(14);
         game.MoveBlock(10);
         List<int> foundIndexes = game.MoveableBlockIndexes;

         List<int> expectedIndexes = new List<int>() { 6, 9, 11, 14 };
         Assert.AreEqual(expectedIndexes, foundIndexes);
      }

      [Test]
      public void ShouldInvokeBlockMovedEventOnMoveBlock()
      {
         int fromIndex = -1;
         int toIndex = -1;
         game.InitializeFrame(false);

         var wait = new AutoResetEvent(false);
         game.BlockMoved += (s, e) => { wait.Set(); fromIndex = e.FromIndex; toIndex = e.ToIndex; };
         game.MoveBlock(18);

         Assert.IsTrue(wait.WaitOne(TimeSpan.FromSeconds(5)));
         Assert.AreEqual(18, fromIndex);
         Assert.AreEqual(19, toIndex);
      }

      [Test]
      public void ShouldInvokeBlockRemovedEventOnRemoveBlock()
      {
         int lastIndex = 0;
         var wait = new AutoResetEvent(false);
         game.BlockRemoved += (s, i) => { wait.Set(); lastIndex = i; };

         game.RemoveLastBlock();

         Assert.IsTrue(wait.WaitOne(TimeSpan.FromSeconds(5)));
         Assert.AreEqual(19, lastIndex);
      }

      [Test]
      public void ShouldInvokeRemovedBlockReplacedEventWhenPuzzleSolved()
      {
         var wait = new AutoResetEvent(false);
         game.RemovedBlockReplaced += (s, i) => { wait.Set(); };
         game.InitializeFrame(false);

         game.MoveBlock(18);
         game.MoveBlock(19);

         Assert.IsTrue(wait.WaitOne(TimeSpan.FromSeconds(5)));
      }

      [Test]
      public void ShouldReplaceRemovedBlockWhenPuzzleSolved()
      {
         game.InitializeFrame(false);

         game.MoveBlock(18);
         game.MoveBlock(19);

         Assert.AreEqual(19, game.GetBlockId(19));
      }

      [Test]
      public void ShouldEmptyMoveableIndexesWhenPuzzleSolved()
      {
         game.InitializeFrame(false);

         game.MoveBlock(18);
         game.MoveBlock(19);

         Assert.IsEmpty(game.MoveableBlockIndexes);
      }

      [Test]
      public void ShouldInvokeEndOfGameDetectedEventWhenPuzzleSolved()
      {
         var wait = new AutoResetEvent(false);
         game.EndOfGameDetected += (s, i) => { wait.Set(); };
         game.InitializeFrame(false);

         game.MoveBlock(18);
         game.MoveBlock(19);

         Assert.IsTrue(wait.WaitOne(TimeSpan.FromSeconds(5)));
      }

      [Test]
      public void ShouldProvideBlockIdForEachCellInInitialState()
      {
         game.InitializeFrame(false);

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
         game.InitializeFrame(false);

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
         game.InitializeFrame(false);

         bool isShuffled = game.IsShuffled();
         Assert.AreEqual(false, isShuffled);
      }

      public void ShouldNotBeShuffledAfterReversedMoves()
      {
         game.InitializeFrame(false);

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
         game.InitializeFrame(false);

         game.MoveBlock(15);

         bool isShuffled = game.IsShuffled();
         Assert.AreEqual(true, isShuffled);
      }

      [Test]
      public void ShouldShuffleBlocks()
      {
         game.InitializeFrame(false);

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