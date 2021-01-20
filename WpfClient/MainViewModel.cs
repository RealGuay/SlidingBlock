using Prism.Commands;
using Prism.Mvvm;
using SlidingLogic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

[assembly: InternalsVisibleTo("WpfClient.Tests")]

namespace WpfClient
{
   public class MainViewModel : BindableBase
   {
      enum GameState
      {
         ReadyToStart,
         Shuffling,
         Playing,
         Solving
      }

      private Dispatcher _uiDispatcher;
      private GameRules _game;
      private DateTime _startTime;
      private TimeSpan _playTime;
      private Timer _timer;
      private PictureSection _removedPictureSection;
      private PictureSection _emptyPictureSection;
      private Stack<int> _undoMoves;
      private bool _inSolvingMode;
      private IImageSplitter _imageSplitter;
      private GameParameters _parameters;

      #region Properties

      private int xDimension;
      public int XDimension { get => xDimension; set => SetProperty(ref xDimension, value); }

      private int yDimension;
      public int YDimension { get => yDimension; set => SetProperty(ref yDimension, value); }

      private int moveCount;
      public int MovesCount { get => moveCount; set => SetProperty(ref moveCount, value); }

      private string playTimeString;
      public string PlayTimeString { get => playTimeString; set => SetProperty(ref playTimeString, value); }

      private bool isGameStarted;

      public bool IsGameStarted
      {
         get { return isGameStarted; }
         set
         {
            SetProperty(ref isGameStarted, value);
            IsGameEnded = !value;
         }
      }

      private bool isGameEnded;
      public bool IsGameEnded { get => isGameEnded; set => SetProperty(ref isGameEnded, value); }

      private Visibility hintsDisplayed;

      public Visibility HintsDisplayed { get => hintsDisplayed; set => SetProperty(ref hintsDisplayed, value); }

      public DelegateCommand<PictureSection> MoveSectionCommand { get; private set; }
      public DelegateCommand StartGameCommand { get; private set; }
      public DelegateCommand SolveGameCommand { get; private set; }
      public DelegateCommand ShowHintsCommand { get; private set; }

      private ObservableCollection<PictureSection> _pictureSections;

      public ObservableCollection<PictureSection> PictureSections
      {
         get { return _pictureSections; }
         set { SetProperty(ref _pictureSections, value); }
      }

      #endregion Properties

      public MainViewModel(IImageSplitter imageSplitter)
      {
         _imageSplitter = imageSplitter;
         _parameters = new GameParameters();
         XDimension = _parameters.XDimension;
         YDimension = _parameters.YDimension;
         _playTime = new TimeSpan(0);
         PlayTimeString = _playTime.ToString(@"hh\:mm\:ss");
         MovesCount = 0;
         IsGameStarted = false;
         HintsDisplayed = Visibility.Collapsed;
         PictureSections = new ObservableCollection<PictureSection>();
         _undoMoves = new Stack<int>();
         _uiDispatcher = Dispatcher.CurrentDispatcher;

         //         StartGameCommand = new DelegateCommand(StartGameExecute, CanStartGame);
         StartGameCommand = new DelegateCommand(StartGameExecute).ObservesCanExecute(() => IsGameEnded);
         MoveSectionCommand = new DelegateCommand<PictureSection>(MoveSectionExecute, CanMoveSection);
         ShowHintsCommand = new DelegateCommand(ShowHintsExecute).ObservesCanExecute(() => IsGameStarted);
         SolveGameCommand = new DelegateCommand(SolveGameExecute).ObservesCanExecute(() => IsGameStarted);
      }

      internal void Initialize()
      {
         CreateSections();
         CreateGame();
      }

      private void CreateSections()
      {
//         ImageSplitter splitter = new ImageSplitter();
//       ImageSplitter splitter = new ImageSplitter(XDimension, YDimension, "pack://application:,,,/Images/Cloe1.jpg");
//       ImageSplitter splitter = new ImageSplitter(XDimension, YDimension, @"D:\RG\Pictures\Saved Pictures\2021\DroneLauzon\Photo 2021-01-01 12 55 57.jpg");
         _imageSplitter.CreateSections(_parameters, PictureSections);
         _emptyPictureSection = new PictureSection() { Id = -1, ImageMember = null };
      }

      private void CreateGame()
      {
         _game = new GameRules(XDimension, YDimension);
         _game.BlockMoved += Game_BlockMoved;
         _game.BlockRemoved += Game_BlockRemoved;
         _game.RemovedBlockReplaced += Game_RemovedBlockReplaced;
         _game.EndOfGameDetected += Game_EndOfGameDetected;
      }

      private void Game_BlockMoved(object sender, MoveBlockEventArgs e)
      {
         var tempSection = PictureSections[e.ToIndex];
         PictureSections[e.ToIndex] = PictureSections[e.FromIndex];
         PictureSections[e.FromIndex] = tempSection;
         if (!_inSolvingMode) _undoMoves.Push(e.ToIndex);
         MoveSectionCommand.RaiseCanExecuteChanged();
      }

      private void Game_BlockRemoved(object sender, int index)
      {
         _removedPictureSection = PictureSections[index];
         PictureSections[index] = _emptyPictureSection;
      }

      private void Game_RemovedBlockReplaced(object sender, EventArgs e)
      {
         StopSolving();
         PictureSections[^1] = _removedPictureSection; // ^1 : one "from the end"
      }

      private void StopSolving()
      {
         if (_inSolvingMode)
         {
            _undoMoves.Clear();
            _inSolvingMode = false;
         }
      }

      private void Game_EndOfGameDetected(object sender, EventArgs e)
      {
         ReleaseTimer();
         IsGameStarted = false;
         MessageBox.Show("End of game !!!", "WOW!!!");
      }

      private void ReleaseTimer()
      {
         _timer.Dispose();
         _timer = null;
      }

      private void StartGameExecute()
      {
         _game.InitializeFrame();
         MovesCount = 0;
         IsGameStarted = true;
         _startTime = DateTime.Now;
         _timer = new Timer(UpdatePlayTime, null, 0, 1000);
         _inSolvingMode = false;
      }

      private void MoveSectionExecute(PictureSection ps)
      {
         MovesCount++;
         int index = GetIndexOfSection(ps);
         _game.MoveBlock(index);
      }

      private bool CanMoveSection(PictureSection ps)
      {
         int index = GetIndexOfSection(ps);
         return _game.MoveableBlockIndexes.Contains(index);
      }

      private void UpdatePlayTime(object state)
      {
         _playTime = DateTime.Now - _startTime;
         PlayTimeString = _playTime.ToString(@"hh\:mm\:ss");
      }

      private void ShowHintsExecute()
      {
         HintsDisplayed = HintsDisplayed == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
      }

      private void SolveGameExecute()
      {
         ReleaseTimer();
         _inSolvingMode = true;
         _timer = new Timer(DoSolveOneMove, null, 0, 500);
      }

      private void DoSolveOneMove(object state)
      {
         if (_inSolvingMode && _undoMoves.TryPop(out int index))
         {
            _uiDispatcher.Invoke(() => _game.MoveBlock(index));
         }
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