using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace WpfClient
{
   public class ImageSplitter : IImageSplitter
   {
      private const int _FrameReservedPixel = 120;

      private GameParameters _parameters;

      public int MonitorWidthPixelSize { get; private set; }
      public int MonitorHeightPixelSize { get; private set; }

      public int SectionWidth { get; private set; }
      public int SectionHeight { get; private set; }

      private enum ResizeOrientation
      {
         Horizontal,
         Vertical
      }

      public ImageSplitter(IDisplayMonitorInfo displayInfo)
      {
         displayInfo.GetFirstMonitorPixelSizes(out int monitorWidth, out int monitorHeight);
         MonitorWidthPixelSize = monitorWidth;
         MonitorHeightPixelSize = monitorHeight;
      }

      public void CreateSections(GameParameters parameters, ObservableCollection<PictureSection> pictureSections)
      {
         _parameters = parameters;
         CalculateMaxPixelSizes(parameters.Image, out int maxDecodePixelHeight, out int maxDecodePixelWidth);

         BitmapImage bi = new BitmapImage();
         ResizeToFitOnScreen(bi, parameters.Image, maxDecodePixelHeight, maxDecodePixelWidth);
         SplitInSections(bi, pictureSections);
      }

      private void CalculateMaxPixelSizes(BitmapImage image, out int maxDecodePixelHeight, out int maxDecodePixelWidth)
      {
         BitmapImage biTmp = image.Clone();

         GetPixelSize(biTmp, out int pixelHeight, out int pixelWidth);
         GetDisplaySize(biTmp, out double displayHeight, out double displayWidth);

         // factors
         double xDisplayToPixelFactor = pixelWidth / displayWidth;
         double yDisplayToPixelFactor = pixelHeight / displayHeight;

         // resize orientation
         ResizeOrientation orientation = (MonitorWidthPixelSize / displayWidth) > (MonitorHeightPixelSize / displayHeight)
            ? ResizeOrientation.Vertical
            : ResizeOrientation.Horizontal;

         if (orientation == ResizeOrientation.Horizontal)
         {
            maxDecodePixelHeight = 0;
            maxDecodePixelWidth = (int)(MonitorWidthPixelSize * xDisplayToPixelFactor) - _FrameReservedPixel;
         }
         else
         {
            maxDecodePixelHeight = (int)(MonitorHeightPixelSize * yDisplayToPixelFactor) - _FrameReservedPixel;
            maxDecodePixelWidth = 0;
         }
      }

      private void GetDisplaySize(BitmapImage bi, out double displayHeight, out double displayWidth)
      {
         displayHeight = bi.Height;
         displayWidth = bi.Width;
      }

      private void GetPixelSize(BitmapImage bi, out int pixelHeight, out int pixelWidth)
      {
         pixelHeight = bi.PixelHeight;
         pixelWidth = bi.PixelWidth;
      }

      private void ResizeToFitOnScreen(BitmapImage bi, BitmapImage paramImage, int requestedPixelHeight, int requestedPixelWidth)
      {
         bi.BeginInit();
         bi.UriSource = paramImage.UriSource;
         bi.StreamSource = paramImage.StreamSource;
         bi.DecodePixelHeight = requestedPixelHeight;
         bi.DecodePixelWidth = requestedPixelWidth;
         bi.EndInit();
      }

      private void SplitInSections(BitmapImage bi, ObservableCollection<PictureSection> pictureSections)
      {
         CalculateSectionSizes(bi);
         GenerateSections(bi, pictureSections);
      }

      private void CalculateSectionSizes(BitmapImage bi)
      {
         SectionWidth = bi.PixelWidth / _parameters.XDimension;
         SectionHeight = bi.PixelHeight / _parameters.YDimension;
      }

      private void GenerateSections(BitmapImage bi, ObservableCollection<PictureSection> pictureSections)
      {
         int index;
         Int32Rect rect;
         CroppedBitmap cb;
         Image imageSection;

         for (int j = 0; j < _parameters.YDimension; j++)
         {
            for (int i = 0; i < _parameters.XDimension; i++)
            {
               index = j * _parameters.XDimension + i;
               rect = new Int32Rect(i * SectionWidth,
                                    j * SectionHeight,
                                    SectionWidth,
                                    SectionHeight);
               cb = new CroppedBitmap(bi, rect);
               imageSection = new Image { Source = cb };
               pictureSections.Add(new PictureSection() { Id = index, ImageMember = imageSection });
            }
         }
      }
   }
}