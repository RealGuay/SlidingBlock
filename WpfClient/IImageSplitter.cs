using System.Collections.ObjectModel;

namespace WpfClient
{
   public interface IImageSplitter
   {
      void CreateSections(GameParameters parameters, ObservableCollection<PictureSection> pictureSections);
   }
}