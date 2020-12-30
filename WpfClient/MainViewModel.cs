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

      public DelegateCommand<PictureSection> MoveSectionCommand { get; private set; }

      public ObservableCollection<PictureSection> PictureSections
      {
         get { return _pictureSections; }
         set { SetProperty(ref _pictureSections, value); }
      }

      public int XDimension { get => _xDimension; set => SetProperty(ref _xDimension, value); }
      public int YDimension { get => _yDimension; set => SetProperty(ref _yDimension, value); }

      public MainViewModel()
      {
         XDimension = 3;
         YDimension = 4;
         PictureSections = new ObservableCollection<PictureSection>();
         MoveSectionCommand = new DelegateCommand<PictureSection>(MoveSectionExecute, CanMoveSection);
      }

      private void MoveSectionExecute(PictureSection ps)
      {
         int index = GetIndexOfSection(ps);
         _game.MoveBlock(index);
      }

      private bool CanMoveSection(PictureSection ps)
      {
         int index = GetIndexOfSection(ps);
         return _game.MoveableBlockIndexes.Contains(index);
      }

      internal void Initialize()
      {
         CreateSections();
         CreateGame();
      }

      private void CreateSections()
      {
         ImageSplitter splitter = new ImageSplitter(XDimension, YDimension, "pack://application:,,,/Images/Cloe1.jpg");
         splitter.CreateSections(PictureSections);
         _emptyPictureSection = new PictureSection() { Id = -1, ImageMember = null };
      }

      private void CreateGame()
      {
         _game = new GameRules(XDimension, YDimension);
         _game.BlockMoved += Game_BlockMoved;
         _game.BlockRemoved += Game_BlockRemoved;
         _game.RemovedBlockReplaced += Game_RemovedBlockReplaced;
         _game.InitializeFrame();
      }

      private void Game_BlockMoved(object sender, MoveBlockEventArgs e)
      {
         var tempSection = PictureSections[e.ToIndex];
         PictureSections[e.ToIndex] = PictureSections[e.FromIndex];
         PictureSections[e.FromIndex] = tempSection;
         MoveSectionCommand.RaiseCanExecuteChanged();
      }

      private void Game_BlockRemoved(object sender, int index)
      {
         _removedPictureSection = PictureSections[index];
         PictureSections[index] = _emptyPictureSection;
      }

      private void Game_RemovedBlockReplaced(object sender, EventArgs e)
      {
         PictureSections[^1] = _removedPictureSection; // ^1 : one "from the end"
      }

      private int GetIndexOfSection(PictureSection ps)
      {
         int index = PictureSections.IndexOf(ps);
         if (index == -1)
         {
            throw new InvalidOperationException($"Picture section not found {ps.Id}");
         }
         return index;
      }
   }
}