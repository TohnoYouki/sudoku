using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    class SelectContinusMinNumber
    {
        private bool IfFirst = true;
        private int Min = 0;
        public bool IfMin(int number)
        {
            if (IfFirst || Min > number)
            {
                IfFirst = false;
                Min = number;
                return true;
            }
            else
                return false;
        }
    }
}
