using System;
using System.Collections.Generic;
using System.Text;

namespace WpfClient
{
   public class DisplayMonitorInfo : IDisplayMonitorInfo
   {
      public void GetFirstMonitorPixelSizes(out int widthPixelSize, out int heightPixelSize)
      {
         widthPixelSize = (int)System.Windows.SystemParameters.WorkArea.Width;
         heightPixelSize = (int)System.Windows.SystemParameters.WorkArea.Height;
      }
   }
}
