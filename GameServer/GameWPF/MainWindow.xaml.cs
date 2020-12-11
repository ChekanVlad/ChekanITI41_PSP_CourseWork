using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SharpDX.Direct2D1;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GameWPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel vm;
        public SettingsWindow settingsWindow;

        public MainWindow()
        {
            
                vm = new MainViewModel
                {
                    Content = null
                };
                
                InitializeComponent();
            
                DataContext = vm;    
        }

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vm.Content = new Renderer(settingsWindow.bombs);
            }
            catch
            {

            }
        }

        private void settingsButton_Click(object sender, RoutedEventArgs e)
        {
            settingsWindow = new SettingsWindow();
            settingsWindow.Owner = this;
            settingsWindow.Show();
        }
    }
}
