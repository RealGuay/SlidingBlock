using Moq;
using NUnit.Framework;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;

[assembly: Apartment(ApartmentState.STA)]

namespace WpfClient
{
   public class MainViewModelTests
   {
      //private const int NbSections = 12;
      //private GameParameters _parameters;
//      private ObservableCollection<PictureSection> _pictureSections;

      private Mock<IImageSplitter> _imageSpitterMock;
      private MainViewModel _mvm;

      [SetUp]
      public void Setup()
      {
         //         _parameters = new GameParameters();
//         _pictureSections = new ObservableCollection<PictureSection>();
         _imageSpitterMock = new Mock<IImageSplitter>();
         _imageSpitterMock.Setup(x => x.CreateSections(It.IsAny<GameParameters>(), It.IsAny<ObservableCollection<PictureSection>>())).Callback<GameParameters, ObservableCollection<PictureSection>>(GenerateSections);

         _mvm = new MainViewModel(_imageSpitterMock.Object);
      }

      private void GenerateSections(GameParameters parameters, ObservableCollection<PictureSection> sections)
      {
         int nbSections = parameters.XDimension * parameters.YDimension;
         PictureSection ps = new PictureSection();
         for (int i = 0; i < nbSections; i++)
         {
            sections.Add(ps);
         }
      }

      [Test]
      public void ShouldSetDimensionsInContructor()
      {
         Assert.AreEqual(3, _mvm.XDimension);
         Assert.AreEqual(4, _mvm.YDimension);
      }

      [Test]
      public void ShouldCreateSectionsOnInitialize()
      {
         _mvm.Initialize();

         Assert.AreEqual(12, _mvm.PictureSections.Count);
      }

      [Test]
      public void ShouldNotDisplayHintsOnStartup()
      {
         _mvm.Initialize();

         Assert.AreEqual(Visibility.Collapsed, _mvm.HintsDisplayed);
      }

      [Test]
      public void ShouldDisplayHintsOnFirstShowHints()
      {
         _mvm.Initialize();
         _mvm.ShowHintsCommand.Execute();

         Assert.AreEqual(Visibility.Visible, _mvm.HintsDisplayed);
      }

      [Test]
      public void ShouldNotDisplayHintsOnSecondShowHints()
      {
         _mvm.Initialize();
         _mvm.ShowHintsCommand.Execute();
         _mvm.ShowHintsCommand.Execute();

         Assert.AreEqual(Visibility.Collapsed, _mvm.HintsDisplayed);
      }
   }
}