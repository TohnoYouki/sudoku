using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    public interface IUniqueElement
    {
        int Index { get; set; }
    }

    public class UniqueStack
    {
        private bool[] InStack;
        private Stack<IUniqueElement> element = new Stack<IUniqueElement>();
        public UniqueStack(List<IUniqueElement> TolList)
        {
            InStack = new bool[TolList.Count];
            for (int i = 0; i < TolList.Count; i++)
            {
                InStack[i] = false;
                TolList[i].Index = i;
            }
        }

        public void Push(IUniqueElement item)
        {
            if (!InStack[item.Index])
            {
                element.Push(item);
                InStack[item.Index] = true;
            }
        }

        public IUniqueElement Pop()
        {
            if (element.Count != 0)
            {
                IUniqueElement x = element.Pop();
                InStack[x.Index] = false;
                return x;
            }
            return null;
        }

        public void Clear()
        {
            while (element.Count != 0)
                Pop();
        }

        public int Count { get { return element.Count; } }
    }
}
