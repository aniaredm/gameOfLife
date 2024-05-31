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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TheGameOfLife.Views;
using Microsoft.Win32;
using System.IO;

namespace TheGameOfLife
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string selectedFilePath;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            int rows = 0, cols = 0;

            if (int.TryParse(BoardRows.Text, out rows) && int.TryParse(BoardCols.Text, out cols) && rows > 0 && cols > 0)
            {
                // Input is valid, create and show the GameWindow
                GameWindow gameWindow = new GameWindow(rows, cols);
                gameWindow.Show();
                Close();
            }
            else
            {
                // Input is not valid, show an error message
                MessageBoxResult result = MessageBox.Show("Please enter valid integer values for rows and columns.", "Invalid Input", MessageBoxButton.OK);

                if (result == MessageBoxResult.OK)
                {
                    // Clear the TextBoxes or take other appropriate action to re-enter values
                    BoardRows.Text = string.Empty;
                    BoardCols.Text = string.Empty;
                    BoardRows.Focus();
                }
            }
        }

        private void ChooseShape_Click(object sender, RoutedEventArgs e)
        {
            ChooseShape chooseShape = new ChooseShape();
            chooseShape.Show();
            Close();
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            string currentDirectory = System.IO.Directory.GetCurrentDirectory();
            string projectDirectory = Directory.GetParent(Directory.GetParent(Directory.GetParent(currentDirectory).FullName).FullName).FullName;
            string savedGamesFolderPath = System.IO.Path.Combine(projectDirectory, "SavedGames");

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Select a Game File",
                Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
                InitialDirectory = savedGamesFolderPath
            };

            if (openFileDialog.ShowDialog() == true)
            {
                FileName.Content = System.IO.Path.GetFileName(openFileDialog.FileName);
                selectedFilePath = openFileDialog.FileName;
                StartWithFile.Visibility = Visibility.Visible;
            }
        }

        private void StartWithFile_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(selectedFilePath))
            {
                // Open the GameWindow and pass the selected file path as a parameter
                GameWindow gameWindow = new GameWindow(selectedFilePath);
                gameWindow.Show();
                Close();
            }
        }
    }
}
