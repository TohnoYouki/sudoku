using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    class ContinusRandomSelect
    {
        private int SelectNumber = 0;
        private Random RandomSeed = new Random();

        public bool SelectThis()
        {
            SelectNumber++;
            return RandomSeed.Next(SelectNumber) == SelectNumber - 1;
        }
    }
}
