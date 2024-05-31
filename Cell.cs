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
            IsAlive = false;

            CellContainer = new Border
            {
                Width = 20, 
                Height = 20, 
                BorderBrush = Brushes.Black, 
                Background = Brushes.White, 
                Child = new Rectangle() 
            };

        }

        public void ToggleState()
        {
            IsAlive = !IsAlive;
            CellContainer.Background = IsAlive ? Brushes.Black : Brushes.White;
        }
    }
}
