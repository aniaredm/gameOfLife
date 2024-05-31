using System.Windows.Ink;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;

namespace TheGameOfLife
{
    public class Cell
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public bool IsAlive { get; set; }
        public Border CellContainer { get; set; }

        public Cell(int row, int column)
        {
            Row = row;
            Column = column;
            IsAlive = false; // Initialize IsAlive to false by default

            // Create the underlying Rectangle
            CellContainer = new Border
            {
                Width = 20, // Adjust as needed
                Height = 20, // Adjust as needed
                BorderBrush = Brushes.Black, // Border color
                Background = Brushes.White, // Initial color
                Child = new Rectangle() // Actual cell content
            };

            // Attach any event handlers here, if needed
        }

        public void ToggleState()
        {
            IsAlive = !IsAlive;
            // Update the appearance of the Rectangle to reflect the new state.
            CellContainer.Background = IsAlive ? Brushes.Black : Brushes.White;
        }
    }
}
