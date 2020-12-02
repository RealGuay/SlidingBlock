using Prism.Commands;
using Prism.Mvvm;
using SlidingLogic;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace WpfClient
{
   public class MainViewModel : BindableBase
   {
      private PictureSection _emptyPictureSection;
      private GameRules _game;
      private ObservableCollection<PictureSection> _pictureSections;
      private PictureSection _removedPictureSection;
      private int _xDimension;
      private int _yDimension;

      public MainViewModel()
      {
         XDimension = 3;
         YDimension = 4;
         PictureSections = new ObservableCollection<PictureSection>();
         MoveSectionCommand = new DelegateCommand<object>(MoveSectionExecute, CanMoveSection);
         CreateGme();
      }

      public DelegateCommand<object> MoveSectionCommand { get; private set; }

      public ObservableCollection<PictureSection> PictureSections
      {
         get { return _pictureSections; }
         set { SetProperty(ref _pictureSections, value); }
      }

      public int XDimension { get => _xDimension; set => SetProperty(ref _xDimension, value); }

      public int YDimension { get => _yDimension; set => SetProperty(ref _yDimension, value); }

      internal void Initialize()
      {
         CreateSections(XDimension, YDimension);
         RemoveLastSection();
         ShuffleSections();
      }

      internal void MoveSection(int index)
      {
         _game.MoveBlock(index);
      }

      internal void ShuffleSections()
      {
         _game.ShuffleBlocks();
      }

      private bool CanMoveSection(object obj)
      {
         int index = GetIndexOfSectionId(obj);
         return _game.MoveableBlockIndexes.Contains(index);
      }

      private void CreateGme()
      {
         _game = new GameRules(XDimension, YDimension);
         _game.BlockMoved += Game_BlockMoved;
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
         _emptyPictureSection = new PictureSection() { Id = -1, ImageMember = null };
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

      private void MoveSectionExecute(object id)
      {
         int index = GetIndexOfSectionId(id);
         MoveSection(index);
      }

      private void RemoveLastSection()
      {
         int lastIndex = PictureSections.Count - 1;
         _removedPictureSection = PictureSections[lastIndex];
         PictureSections[lastIndex] = _emptyPictureSection;
      }
   }
}