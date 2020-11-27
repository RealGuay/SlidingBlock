using SlidingLogic;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace WpfClient
{
   public class MainViewModel
   {
      private PictureSection emptyPictureSection;
      private ObservableCollection<PictureSection> pictureSections;
      private PictureSection removedPictureSection;
      private GameRules game;

      public MainViewModel()
      {
         XDimension = 3;
         YDimension = 4;
         InitializeSections(XDimension, YDimension);
         RemoveLastSection();

         game = new GameRules(XDimension, YDimension);
         game.BlockMoved += Game_BlockMoved;
      }

      public ObservableCollection<PictureSection> PictureSections { get => pictureSections; set => pictureSections = value; }

      internal void ShuffleSections()
      {
         game.ShuffleBlocks();
      }



      public int XDimension { get; set; }

      public int YDimension { get; set; }

      internal void MoveSection()
      {
         game.MoveBlock(game.MoveableBlockIndexes.Last());
      }

      private void Game_BlockMoved(object sender, MoveBlockEventArgs e)
      {
         var tempSection = PictureSections[e.ToIndex];
         PictureSections[e.ToIndex] = PictureSections[e.FromIndex];
         PictureSections[e.FromIndex] = tempSection;
      }
      private void InitializeSections(int xDimension, int yDimension)
      {
         CroppedBitmap cb;
         Image imageSection;
         Int32Rect rect;
         int index;

         BitmapImage bitmap = new BitmapImage(new Uri(@"D:\RG\Pictures\Saved Pictures\2020\IMG_5381.JPG"));
         int sectionWidth = (int)bitmap.PixelWidth / xDimension;
         int sectionHeight = (int)bitmap.PixelHeight / yDimension;

         PictureSections = new ObservableCollection<PictureSection>();
         for (int j = 0; j < yDimension; j++)
         {
            for (int i = 0; i < xDimension; i++)
            {
               index = j * xDimension + i;
               rect = new Int32Rect(i * sectionWidth, j * sectionHeight, sectionWidth, sectionHeight);
               cb = new CroppedBitmap(bitmap, rect);
               imageSection = new Image();
               imageSection.Source = cb;
               PictureSections.Add(new PictureSection() { Id = index, ImageMember = imageSection });
            }
         }
         emptyPictureSection = new PictureSection() { Id = -1, ImageMember = null };
      }

      private void RemoveLastSection()
      {
         int lastIndex = PictureSections.Count - 1;
         removedPictureSection = PictureSections[lastIndex];
         PictureSections[lastIndex] = emptyPictureSection;
      }
   }
}