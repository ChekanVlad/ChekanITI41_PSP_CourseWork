using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using Udp;

namespace GameServer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Action<string> action;
        Server server;
        List<string> consoleText;
        public MainWindow()
        {
            InitializeComponent();
            action = ShowMessage;
            consoleText = new List<string>();
            string Host = System.Net.Dns.GetHostName();
            ServerIPLabel.Content = "Use " + System.Net.Dns.GetHostByName(Host).AddressList[1].ToString() + " to connect to the server.";
        }

        private void RunServerButton_Click(object sender, RoutedEventArgs e)
        {
            server = new Server(5555);
            server.Notify += NotifyDispatcher;
            server.RunServer();
            RunServerButton.IsEnabled = false;
            ConsoleTextBlock.Text = "Server launched.";
        }

        private void NotifyDispatcher(string message)
        {
            Dispatcher.Invoke(action, message);
        }

        private void ShowMessage(string message)
        {
            if (consoleText.Count > 10)
                consoleText.RemoveAt(0);
            consoleText.Add(DateTime.Now.ToString() + ": " + message + "\n");
            StringBuilder builder = new StringBuilder();
            foreach (string str in consoleText)
            {
                builder.Append(str);
            }
            ConsoleTextBlock.Text = builder.ToString();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (server != null)
            {
                server.Close();
            }
        }
    }
}
