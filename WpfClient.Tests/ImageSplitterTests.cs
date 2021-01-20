using Moq;
using NUnit.Framework;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace WpfClient
{
   public class ImageSplitterTests
   {
      private const int TestMonitorWidth = 2000;
      private const int TestMonitorHeight = 1000;
      private Mock<IDisplayMonitorInfo> _displayInfoMock;
      private ImageSplitter _imageSplitter;
      private GameParameters _parameters;

      [SetUp]
      public void Setup()
      {
         string imageLocation = "WpfClient.Images.TestImage1_100_100.png";  // namespace is WpfClient (not WpfClient.Tests)
         var bmi = GetEmbeddedImageForTests(imageLocation);
         _parameters = new GameParameters() { XDimension = 3, YDimension = 4, ImageLocation = "", Image = bmi };

         var testWidth = TestMonitorWidth;
         var testHeight = TestMonitorHeight;
         _displayInfoMock = new Mock<IDisplayMonitorInfo>();
         _displayInfoMock.Setup(di => di.GetFirstMonitorPixelSizes(out testWidth, out testHeight));

         _imageSplitter = new ImageSplitter(_displayInfoMock.Object);
      }

      private static BitmapImage GetEmbeddedImageForTests(string imageLocation)
      {
         var resStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(imageLocation);
         BitmapImage img = new BitmapImage();
         img.BeginInit();
         img.StreamSource = resStream;
         img.EndInit();
         return img;
      }

      [Test]
      public void ShouldFindFirstMonitorDimensionOnConstructor()
      {
         Assert.AreEqual(2000, _imageSplitter.MonitorWidthPixelSize);
         Assert.AreEqual(1000, _imageSplitter.MonitorHeightPixelSize);
      }

      [Test]
      public void ShouldSplitImageOnCreateSection()
      {
         var sections = new ObservableCollection<PictureSection>();

         _imageSplitter.CreateSections(_parameters, sections);

         Assert.AreEqual(293, _imageSplitter.SectionWidth);
         Assert.AreEqual(219, _imageSplitter.SectionHeight);
      }
   }
}