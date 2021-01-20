using System;
using System.Windows.Media.Imaging;

namespace WpfClient
{
   public class GameParameters
   {
      private const string DefaultImageLocation = "pack://application:,,,/Images/Cloe1.jpg";
      public int XDimension { get; set; }
      public int YDimension { get; set; }
      public string ImageLocation { get; set; }

      private BitmapImage image;

      public BitmapImage Image
      {
         get
         {
            if (image == null)
            {
               image = GetDefaultImage();
            }
            return image;
         }
         set { image = value; }
      }

      private BitmapImage GetDefaultImage()
      {
         var defaultImageUri = new Uri(DefaultImageLocation);
         var defaultImage = new BitmapImage(defaultImageUri);
         return defaultImage;
      }

      public GameParameters()
      {
         // default values;
         XDimension = 3;
         YDimension = 4;
         ImageLocation = "pack://application:,,,/WpfClient;component/Images/Cloe1.jpg";
         ImageLocation = "pack://application:,,,/Images/Cloe1.jpg";
      }
   }
}