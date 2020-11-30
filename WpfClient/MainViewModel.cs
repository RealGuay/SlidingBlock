using Prism.Commands;
using SlidingLogic;
using System;
using System.Collections.ObjectModel;
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

      public DelegateCommand<object> MoveSectionCommand { get; set; }


      public MainViewModel()
      {
         XDimension = 3;
         YDimension = 4;
         InitializeSections(XDimension, YDimension);
         RemoveLastSection();

         MoveSectionCommand = new DelegateCommand<object>(MoveSectionExecute, CanMoveSection);
         game = new GameRules(XDimension, YDimension);
         game.BlockMoved += Game_BlockMoved;
      }

      private bool CanMoveSection(object arg)
      {
         return game.MoveableBlockIndexes.Contains((int)arg);
      }

      private void MoveSectionExecute(object obj)
      {
         MoveSection((int)obj);
      }

      public ObservableCollection<PictureSection> PictureSections { get => pictureSections; set => pictureSections = value; }

      internal void ShuffleSections()
      {
         game.ShuffleBlocks();
      }



      public int XDimension { get; set; }

      public int YDimension { get; set; }

      internal void MoveSection(int id)
      {
         game.MoveBlock(id);
      }

      private void Game_BlockMoved(object sender, MoveBlockEventArgs e)
      {
         var tempSection = PictureSections[e.ToIndex];
         PictureSections[e.ToIndex] = PictureSections[e.FromIndex];
         PictureSections[e.FromIndex] = tempSection;

         foreach (var item in PictureSections)
         {
            MoveSectionCommand.RaiseCanExecuteChanged();
         }

      }
      private void InitializeSections(int xDimension, int yDimension)
      {
         CroppedBitmap cb;
         Image imageSection;
         Int32Rect rect;
         int index;

         BitmapImage bitmap = new BitmapImage(new Uri("pack://application:,,,/Images/Cloe1.jpg"));
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