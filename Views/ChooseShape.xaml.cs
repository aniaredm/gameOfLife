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
using System.IO;

namespace TheGameOfLife.Views
{
    public partial class ChooseShape : Window
    {
        public ChooseShape()
        {
            InitializeComponent();
        }
        private void Crocodile_Click(object sender, RoutedEventArgs e)
        {
            StartGame("Crocodile.txt");
        }
        private void Fountain_Click(object sender, RoutedEventArgs e)
        {
            StartGame("Fountain.txt");
        }
        private void Frog_Click(object sender, RoutedEventArgs e)
        {
            StartGame("Frog.txt");
        }
        private void Glider_Click(object sender, RoutedEventArgs e)
        {
            StartGame("Glider.txt");
        }
        private void Dakota_Click(object sender, RoutedEventArgs e)
        {
            StartGame("Dakota.txt");
        }

        private void StartGame(string fileName)
        {
            string currentDirectory = System.IO.Directory.GetCurrentDirectory();
            string projectDirectory = Directory.GetParent(Directory.GetParent(Directory.GetParent(currentDirectory).FullName).FullName).FullName;
            string filePath = System.IO.Path.Combine(projectDirectory, "InitShapes", fileName);
            if (File.Exists(filePath))
            {
                GameWindow gameWindow = new GameWindow(filePath);
                gameWindow.Show();
                Close();
            }
            else
            {
                MessageBox.Show(fileName + " not found.");
            }
        }

        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }
    }
}
