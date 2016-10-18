using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexagonGenerator
{
    public class HexagonData
    {
        public int RowIndex { get; private set; }
        public int ColIndex { get; private set; }
        public double DistanceToMainAxis { get; private set; }


        public HexagonData(int rowIndex, int colIndex, double distanceToMainAxis)
        {
            RowIndex = rowIndex;
            ColIndex = colIndex;
            DistanceToMainAxis = distanceToMainAxis;
        }
    }
}
