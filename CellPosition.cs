using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGameOfLife
{
    public class CellPosition
    {
        public int Row { get; set; }
        public int Column { get; set; }

        public CellPosition(int row, int column)
        {
            Row = row;
            Column = column;
        }
        public override bool Equals(object obj)
        {
            if (obj is CellPosition other)
            {
                return Row == other.Row && Column == other.Column;
            }
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Row.GetHashCode();
                hash = hash * 23 + Column.GetHashCode();
                return hash;
            }
        }
    }
}
