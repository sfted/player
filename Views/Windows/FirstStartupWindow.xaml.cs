using Player.ViewModels.Windows;
using System.ComponentModel;
using System.Windows;

namespace Player.Views.Windows
{
    /// <summary>
    /// Interaction logic for LibraryScanWindow.xaml
    /// </summary>
    public partial class FirstStartupWindow : Window
    {
        public FirstStartupWindow()
        {
            InitializeComponent();
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            var vm = DataContext as FirstStartupViewModel;
            // TODO: прописать логику когда загрузка еще не завершена (по типу: "Уверен, что хочешь прервать загрузку?")
            if (!vm.FinishedLoading)
                Application.Current.Shutdown();
        }
    }
}
