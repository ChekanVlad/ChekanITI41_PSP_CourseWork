using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GameWPF
{
    /// <summary>
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public int[] bombs;
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void confirmSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int p1SmallBombs = int.Parse(p1SmallBombsTextBox.Text);
                int p1BigBombs = int.Parse(p1BigBombsTextBox.Text);
                int p2SmallBombs = int.Parse(p2SmallBombsTextBox.Text);
                int p2BigBombs = int.Parse(p2BigBombsTextBox.Text);
                if (p1SmallBombs * 3 + p1BigBombs * 6 <= 50 && p2SmallBombs * 3 + p2BigBombs * 6 <= 50)
                {
                    errorLabel.Content = "";
                    bombs = new int[4] { p1SmallBombs, p1BigBombs, p2SmallBombs, p2BigBombs };
                    Hide();
                }
                else
                {
                    errorLabel.Content = "Incorrect bombs count.";
                }
            }
            catch
            {

            }
        }


        //
        private void plusP1sb_Click(object sender, RoutedEventArgs e)
        {
            int value = int.Parse(p1SmallBombsTextBox.Text);
            if (value < 16)
                p1SmallBombsTextBox.Text = (++value).ToString();
        }

        private void minusP1sb_Click(object sender, RoutedEventArgs e)
        {
            int value = int.Parse(p1SmallBombsTextBox.Text);
            if (value > 0)
                p1SmallBombsTextBox.Text = (--value).ToString();
        }

        private void plusP2sb_Click(object sender, RoutedEventArgs e)
        {
            int value = int.Parse(p2SmallBombsTextBox.Text);
            if (value < 16)
                p2SmallBombsTextBox.Text = (++value).ToString();
        }

        private void minusP2sb_Click(object sender, RoutedEventArgs e)
        {
            int value = int.Parse(p2SmallBombsTextBox.Text);
            if (value > 0)
                p2SmallBombsTextBox.Text = (--value).ToString();
        }

        private void plusP1bb_Click(object sender, RoutedEventArgs e)
        {
            int value = int.Parse(p1BigBombsTextBox.Text);
            if (value < 8)
                p1BigBombsTextBox.Text = (++value).ToString();
        }

        private void minusP1bb_Click(object sender, RoutedEventArgs e)
        {
            int value = int.Parse(p1BigBombsTextBox.Text);
            if (value > 0)
                p1BigBombsTextBox.Text = (--value).ToString();
        }

        private void plusP2bb_Click(object sender, RoutedEventArgs e)
        {
            int value = int.Parse(p2BigBombsTextBox.Text);
            if (value < 8)
                p2BigBombsTextBox.Text = (++value).ToString();

        }

        private void minusP2bb_Click(object sender, RoutedEventArgs e)
        {
            int value = int.Parse(p2BigBombsTextBox.Text);
            if (value > 0)
                p2BigBombsTextBox.Text = (--value).ToString();
        }
    }
}
