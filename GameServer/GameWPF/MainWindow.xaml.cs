using System;
using System.Windows;
using Udp;

namespace GameWPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Action<string> action;
        private Client client;
        private MainViewModel vm;
        public SettingsWindow settingsWindow;
        int[] bombs = new int[4];
        int playerID, mapId;

        public MainWindow()
        {
            InitializeComponent();
            action = OpenGame;
        }

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            string ip = ServerIPTextBox.Text;
            int localPort = 4000;
            int remotePort = 5555;
            client = new Client(ip, remotePort, localPort, bombs[0], bombs[1]);
            client.Notify += OpenGameDispatcher;
            ConnectionStatusLabel.Content = "Waiting for connection...";
        }

        public void OpenGameDispatcher(string message)
        {
            Dispatcher.Invoke(action, message);
        }

        public void OpenGame(string message)
        {
            ConnectionStatusLabel.Content = message;
            if (message == "Connect")
            {
                client.ClearNotify();
                               
                try
                {
                    vm = new MainViewModel
                    {
                        Content = new Renderer(bombs, playerID, mapId, client)
                    };
                    DataContext = vm;
                }
                catch
                {

                }

            }
            else
            {
                var prms = message.Split('|');
                playerID = int.Parse(prms[3]);
                bombs[2] = int.Parse(prms[0]);
                bombs[3] = int.Parse(prms[1]);
                mapId = int.Parse(prms[2]);
           }

        }

        private void settingsButton_Click(object sender, RoutedEventArgs e)
        {
            settingsWindow = new SettingsWindow();
            settingsWindow.Owner = this;
            settingsWindow.Show();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (client != null)
            {
                client.CloseClient();
            }
        }
    }
}
