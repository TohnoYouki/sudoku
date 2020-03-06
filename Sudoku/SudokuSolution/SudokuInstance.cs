using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    abstract class SudokuInstance
    {     //所有数独的基类，抽象了所有数独的特征
        public Dictionary<IntVector2, int> PrimaryDisk = new Dictionary<IntVector2, int>(); 
        //表示数独的初盘数字，为0表示当前坐标的格子上初始没有数字
        public Dictionary<IntVector2, int> Result = new Dictionary<IntVector2, int>();
        //表示数独的终盘数字，即数独的答案
        public Dictionary<IntVector2, SolutionNumber> solutionNumbers = new Dictionary<IntVector2, SolutionNumber>();
        //数独中的所有数字格子
        public Dictionary<SolutionNumber, IntVector2> IndexSet = new Dictionary<SolutionNumber, IntVector2>();
        public List<Ruler> Rulers = new List<Ruler>(); //数独中所包含的所有规则
        protected List<SolutionNumber> numbersInList = new List<SolutionNumber>();
        abstract public bool Initial(int maxDelay = -1); //在maxDelay的时间限制内生成数独的初盘，为-1不考虑限制
        abstract public bool GenerateSudoku(); //生成数独的初盘，包括从终盘中去除数字和相关多余的规则
        abstract public void SetBoundary(Dictionary<IntVector2, SudokuBox> boxes);   //提供该数独UI绘制机制
        abstract public void ClearDraftLaber(Dictionary<IntVector2, SudokuBox> Boxs, SudokuBox nowSelectedBox);  //提供该数独草稿消除机制

        protected static bool run = true;
        public static void Close() { run = false; }
        public void AddIndexSet()
        {
            IndexSet.Clear();
            foreach (IntVector2 Index in solutionNumbers.Keys)
                IndexSet.Add(solutionNumbers[Index], Index);
        }
    }
}
