namespace WpfClient
{
   public interface IDisplayMonitorInfo
   {
      void GetFirstMonitorPixelSizes(out int widthPixelSize, out int heightPixelSize);
   }
}