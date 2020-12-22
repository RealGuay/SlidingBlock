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
         BitmapImage bi = new BitmapImage();
         CalculateXXX(imageUri, bi);
         SplitImage(pictureSections, bi);
      }

      private void CalculateXXX(Uri imageUri, BitmapImage bi)
      {
         double xFactor, yFactor;
         bool scaleVertical;
         CalculateScalingFactors(imageUri, out xFactor, out yFactor, out scaleVertical);
         DecodePixel(imageUri, xFactor, yFactor, scaleVertical, bi);
      }

      private void CalculateScalingFactors(Uri imageUri, out double xFactor, out double yFactor, out bool scaleVertical)
      {
         BitmapImage biTmp = new BitmapImage(imageUri);
         xFactor = biTmp.PixelWidth / biTmp.Width;
         yFactor = biTmp.PixelHeight / biTmp.Height;
         scaleVertical = _screenWidth / biTmp.Width > _screenHeight / biTmp.Height;
      }

      private void DecodePixel(Uri imageUri, double xFactor, double yFactor, bool scaleVertical, BitmapImage bi)
      {
         bi.BeginInit();
         bi.UriSource = imageUri;
         if (scaleVertical)
         {
            bi.DecodePixelHeight = (int)(_screenHeight * yFactor * 0.9);
         }
         else
         {
            bi.DecodePixelWidth = (int)(_screenWidth * xFactor * 0.9);
         }
         bi.EndInit();
      }

      private void SplitImage(ObservableCollection<PictureSection> pictureSections, BitmapImage bi)
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