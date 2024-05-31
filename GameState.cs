using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGameOfLife
{
    public class GameState
    {
        public bool[,] CellStates { get; set; }
        public bool[,] BornCells { get; set; }
        public bool[,] DeadCells { get; set; }
        public int CellsDiedNumber { get; set; }
        public int CellsBornNumber { get; set; }

        public GameState(bool[,] cellStates, int cellsDiedNumber, int cellsBornNumber)
        {
            CellStates = (bool[,])cellStates.Clone();
            CellsDiedNumber = cellsDiedNumber;
            CellsBornNumber = cellsBornNumber;
        }
    }
}
