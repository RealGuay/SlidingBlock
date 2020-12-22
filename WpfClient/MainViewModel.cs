using Prism.Commands;
using Prism.Mvvm;
using SlidingLogic;
using System;
using System.Collections.ObjectModel;

namespace WpfClient
{
   public class MainViewModel : BindableBase
   {
      private int _xDimension;
      private int _yDimension;
      private GameRules _game;
      private ObservableCollection<PictureSection> _pictureSections;
      private PictureSection _removedPictureSection;
      private PictureSection _emptyPictureSection;

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
         ImageSplitter splitter = new ImageSplitter(XDimension, YDimension, "pack://application:,,,/Images/Cloe1.jpg");
         splitter.CreateSections(PictureSections);
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
         if (!(obj is PictureSection ps))
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