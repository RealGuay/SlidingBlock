using System;

namespace SlidingLogic
{
   public class MoveBlockEventArgs : EventArgs
   {
      public int FromIndex { get; set; }
      public int ToIndex { get; set; }
   }
}