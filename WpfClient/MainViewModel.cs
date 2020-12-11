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
      private int _totalImagePixelHeight;
      private int _totalImagePixelWidth;
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

      public int TotalImagePixelHeight { get => _totalImagePixelHeight; set => SetProperty(ref _totalImagePixelHeight, value); }
      public int TotalImagePixelWidth { get => _totalImagePixelWidth; set => SetProperty(ref _totalImagePixelWidth, value); }
      public int XDimension { get => _xDimension; set => SetProperty(ref _xDimension, value); }

      public int YDimension { get => _yDimension; set => SetProperty(ref _yDimension, value); }

      internal void Initialize()
      {
         CreateSections();
         RemoveLastSection();
         ShuffleSections();
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

      private void CreateSections()
      {
         CroppedBitmap cb;
         Image imageSection;
         Int32Rect rect;
         int index;
         //Uri imageLocation = new Uri(@"D:\RG\Pictures\Saved Pictures\2020\IMG_5049.JPG");
         Uri imageLocation = new Uri("pack://application:,,,/Images/Cloe1.jpg");

         var sysWidth = System.Windows.SystemParameters.WorkArea.Width;
         var sysHeight = System.Windows.SystemParameters.WorkArea.Height;


         BitmapImage biTmp = new BitmapImage(imageLocation);
         double xFactor = biTmp.PixelWidth / biTmp.Width;
         double yFactor = biTmp.PixelHeight / biTmp.Height;
         TotalImagePixelWidth = biTmp.PixelWidth;
         TotalImagePixelHeight = biTmp.PixelHeight;
         bool scaleVertical = sysWidth / biTmp.Width > sysHeight / biTmp.Height;

        //BitmapImage bitmapImage = new BitmapImage(new Uri("pack://application:,,,/Images/Cloe.jpg"));
        //BitmapImage bi = new BitmapImage(new Uri(@"D:\RG\Pictures\Saved Pictures\2020\IMG_5060.JPG"));
        BitmapImage bi = new BitmapImage();
         bi.BeginInit();
         bi.UriSource = imageLocation;
         if (scaleVertical)
         {
            bi.DecodePixelHeight = (int)(sysHeight * yFactor * 0.9);
         }
         else
         {
            bi.DecodePixelWidth = (int)(sysWidth * xFactor * 0.9);
         }

         bi.EndInit();

         int sectionWidth = bi.PixelWidth / XDimension;
         int sectionHeight = bi.PixelHeight / YDimension;

         for (int j = 0; j < YDimension; j++)
         {
            for (int i = 0; i < XDimension; i++)
            {
               index = j * XDimension + i;
               rect = new Int32Rect(i * sectionWidth,
                                    j * sectionHeight,
                                    sectionWidth,
                                    sectionHeight);
               cb = new CroppedBitmap(bi, rect);
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

      private void MoveSection(int index)
      {
         _game.MoveBlock(index);
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

      private void ShuffleSections()
      {
         _game.ShuffleBlocks();
      }
   }
}