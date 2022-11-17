using System.Windows;
using WpfTracker.Services;

namespace WpfTracker
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new ApplicationViewModel(new DefaultDialogService(), new JsonFileService());
        }
    }
}
