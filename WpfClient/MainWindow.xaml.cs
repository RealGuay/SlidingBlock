using System.Windows;

namespace WpfClient
{
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window
   {
      private MainViewModel mainViewModel;

      public MainWindow()
      {
         InitializeComponent();
         mainViewModel = new MainViewModel();
         DataContext = mainViewModel;
         this.Loaded += MainWindow_Loaded;
      }

      private void MainWindow_Loaded(object sender, RoutedEventArgs e)
      {
         mainViewModel.Initialize();
      }
   }
}