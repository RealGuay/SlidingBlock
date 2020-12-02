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
      private GameRules game;
      private ObservableCollection<PictureSection> pictureSections;
      private PictureSection removedPictureSection;
      public MainViewModel()
      {
         XDimension = 3;
         YDimension = 4;
         PictureSections = new ObservableCollection<PictureSection>();
         MoveSectionCommand = new DelegateCommand<object>(MoveSectionExecute, CanMoveSection);
         CreateGme();
      }

      public DelegateCommand<object> MoveSectionCommand { get; set; }

      public ObservableCollection<PictureSection> PictureSections { get => pictureSections; set => pictureSections = value; }

      public int XDimension { get; set; }

      public int YDimension { get; set; }

      internal void Initialize()
      {
         CreateSections(XDimension, YDimension);
         RemoveLastSection();
         ShuffleSections();
      }

      private void CreateGme()
      {
         game = new GameRules(XDimension, YDimension);
         game.BlockMoved += Game_BlockMoved;
      }

      internal void MoveSection(int index)
      {
         game.MoveBlock(index);
      }

      internal void ShuffleSections()
      {
         game.ShuffleBlocks();
      }

      private bool CanMoveSection(object obj)
      {
         int index = GetIndexOfSectionId(obj);
         return game.MoveableBlockIndexes.Contains(index);
      }

      private void Game_BlockMoved(object sender, MoveBlockEventArgs e)
      {
         var tempSection = PictureSections[e.ToIndex];
         PictureSections[e.ToIndex] = PictureSections[e.FromIndex];
         PictureSections[e.FromIndex] = tempSection;

         MoveSectionCommand.RaiseCanExecuteChanged();
      }

      private int GetIndexOfSectionId(object obj)
      {
         PictureSection ps = obj as PictureSection;
         if (ps is null)
         {
            throw new ArgumentException("Invalid object type", nameof(obj));
         }

         int index = PictureSections.IndexOf(ps);
         return index;
      }

      private void CreateSections(int xDimension, int yDimension)
      {
         CroppedBitmap cb;
         Image imageSection;
         Int32Rect rect;
         int index;

         BitmapImage bitmapImage = new BitmapImage(new Uri("pack://application:,,,/Images/Cloe1.jpg"));
         //BitmapImage bitmapImage = new BitmapImage(new Uri(@"D:\RG\Pictures\Saved Pictures\2020\IMG_5056.JPG"));
         int sectionWidth = (int)bitmapImage.PixelWidth / xDimension;
         int sectionHeight = (int)bitmapImage.PixelHeight / yDimension;

         for (int j = 0; j < yDimension; j++)
         {
            for (int i = 0; i < xDimension; i++)
            {
               index = j * xDimension + i;
               rect = new Int32Rect(i * sectionWidth, j * sectionHeight, sectionWidth, sectionHeight);
               cb = new CroppedBitmap(bitmapImage, rect);
               imageSection = new Image();
               imageSection.Source = cb;
               PictureSections.Add(new PictureSection() { Id = index, ImageMember = imageSection });
            }
         }
         emptyPictureSection = new PictureSection() { Id = -1, ImageMember = null };
      }

      private void MoveSectionExecute(object id)
      {
         int index = GetIndexOfSectionId(id);
         MoveSection(index);
      }
      private void RemoveLastSection()
      {
         int lastIndex = PictureSections.Count - 1;
         removedPictureSection = PictureSections[lastIndex];
         PictureSections[lastIndex] = emptyPictureSection;
      }
   }
}