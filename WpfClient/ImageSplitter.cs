using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace WpfClient
{
   public class ImageSplitter
   {
      private int _xDimension;
      private int _yDimension;
      private string _imageLocation;
      private readonly double _screenWidth;
      private readonly double _screenHeight;

      private enum ScalingOrientation
      {
         Horizontal,
         Vertical
      }

      public ImageSplitter(int x, int y, string imageLocation)
      {
         _xDimension = x;
         _yDimension = y;
         _imageLocation = imageLocation;

         _screenWidth = System.Windows.SystemParameters.WorkArea.Width;
         _screenHeight = System.Windows.SystemParameters.WorkArea.Height;
      }

      public void CreateSections(ObservableCollection<PictureSection> pictureSections)
      {
         Uri imageUri = new Uri(_imageLocation);
         CalculateMaxPixelSizes(imageUri, out int maxDecodePixelHeight, out int maxDecodePixelWidth);
         int requestedPixelHeight = (int)(maxDecodePixelHeight * 0.8);
         int requestedPixelWidth = (int)(maxDecodePixelWidth * 0.8);

         BitmapImage bi = new BitmapImage();
         ResizeToFitOnScreen(bi, imageUri, requestedPixelHeight, requestedPixelWidth);
         SplitInSections(bi, pictureSections);
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

      private void CalculateMaxPixelSizes(Uri imageUri, out int maxDecodePixelHeight, out int maxDecodePixelWidth)
      {
         BitmapImage biTmp = new BitmapImage(imageUri);

         GetPixelSize(biTmp, out int pixelHeight, out int pixelWidth);
         GetDisplaySize(biTmp, out double displayHeight, out double displayWidth);
         // factors
         double xFactor = pixelWidth / displayWidth;
         double yFactor = pixelHeight / displayHeight;

         // resize orientation
         ScalingOrientation orientation = _screenWidth / displayWidth > _screenHeight / displayHeight
            ? ScalingOrientation.Vertical
            : ScalingOrientation.Horizontal;
         if (orientation == ScalingOrientation.Horizontal)
         {
            maxDecodePixelHeight = 0;
            maxDecodePixelWidth = (int)(_screenWidth * xFactor * 0.9);
         }
         else
         {
            maxDecodePixelHeight = (int)(_screenHeight * yFactor * 0.9);
            maxDecodePixelWidth = 0;
         }
      }

      private void ResizeToFitOnScreen(BitmapImage bi, Uri imageUri, int requestedPixelHeight, int requestedPixelWidth)
      {
         bi.BeginInit();
         bi.UriSource = imageUri;
         bi.DecodePixelHeight = requestedPixelHeight;
         bi.DecodePixelWidth = requestedPixelWidth;
         bi.EndInit();
      }

      private void SplitInSections(BitmapImage bi, ObservableCollection<PictureSection> pictureSections)
      {
         int sectionWidth = bi.PixelWidth / _xDimension;
         int sectionHeight = bi.PixelHeight / _yDimension;

         CroppedBitmap cb;
         Image imageSection;
         Int32Rect rect;
         int index;
         for (int j = 0; j < _yDimension; j++)
         {
            for (int i = 0; i < _xDimension; i++)
            {
               index = j * _xDimension + i;
               rect = new Int32Rect(i * sectionWidth,
                                    j * sectionHeight,
                                    sectionWidth,
                                    sectionHeight);
               cb = new CroppedBitmap(bi, rect);
               imageSection = new Image() { Source = cb };
               pictureSections.Add(new PictureSection() { Id = index, ImageMember = imageSection });
            }
         }
      }
   }
}