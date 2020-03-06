using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    class GenerateComponent
    {//采用随机搜索的方法，通过搜索回溯找到解空间中的一个随机解，并利用这个随机解生成相应的不重复规则
        public List<List<int>> numbers = new List<List<int>>();

        public GenerateComponent()
        {
            for (int i = 0; i < 9; i++)
            {
                List<int> x = new List<int>();
                for (int j = 0; j < 9; j++) x.Add(0);
                numbers.Add(x);
            }
        }

        private void AddNeighbour(IntVector2 select, List<IntVector2> x)
        {
            if (select.X > 0 && numbers[select.X - 1][select.Y] == 0 && !x.Contains(select.Up())) x.Add(select.Up());
            if (select.X < 8 && numbers[select.X + 1][select.Y] == 0 && !x.Contains(select.Down())) x.Add(select.Down());
            if (select.Y > 0 && numbers[select.X][select.Y - 1] == 0 && !x.Contains(select.Left())) x.Add(select.Left());
            if (select.Y < 8 && numbers[select.X][select.Y + 1] == 0 && !x.Contains(select.Right())) x.Add(select.Right());
        }

        private IntVector2 SelectOneIndex()
        {
            Random x = new Random();
            return pendingNeighbours[number][x.Next(pendingNeighbours[number].Count)];
        }

        private int DFS(int i, int j, List<List<bool>> x)
        {
            int result = 1;
            x[i][j] = true;
            if (i > 0 && !x[i - 1][j]) result += DFS(i - 1, j, x);
            if (i < 8 && !x[i + 1][j]) result += DFS(i + 1, j, x);
            if (j > 0 && !x[i][j - 1]) result += DFS(i, j - 1, x);
            if (j < 8 && !x[i][j + 1]) result += DFS(i, j + 1, x);
            return result;
        }

        private bool IfLeftConnected()
        {
            List<List<bool>> IfVisited = new List<List<bool>>();
            for (int i = 0; i < 9; i++)
            {
                List<bool> x = new List<bool>();
                IfVisited.Add(x);
                for (int j = 0; j < 9; j++)
                    if (numbers[i][j] == 0) x.Add(false); else x.Add(true);
            }

            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    if (!IfVisited[i][j] && DFS(i, j, IfVisited) % 9 != 0) return false;
            return true;
        }

        private bool GoBack()
        {
            pendingNeighbours.RemoveAt(number);
            IntVector2 remove = neighbourSelect[number];
            neighbourSelect.RemoveAt(number);
            numbers[remove.X][remove.Y] = 0;
            number--;
            if (number == 0) return false;
            pendingNeighbours[number].Remove(remove);
            return true;
        }

        private List<List<IntVector2>> pendingNeighbours = new List<List<IntVector2>>();
        private List<IntVector2> neighbourSelect = new List<IntVector2>();
        private int number;
        public bool GenerateOneComponent(IntVector2 center, int componentIndex)
        {
            number = 0;

            pendingNeighbours.Clear();
            neighbourSelect.Clear();

            neighbourSelect.Add(center);
            pendingNeighbours.Add(new List<IntVector2>());
            AddNeighbour(center, pendingNeighbours[0]);

            while (true)
            {
                while (pendingNeighbours[number].Count == 0)
                    if (!GoBack()) return false;

                IntVector2 select = SelectOneIndex();
                neighbourSelect.Add(select);
                numbers[select.X][select.Y] = componentIndex;

                List<IntVector2> y = new List<IntVector2>();
                foreach (IntVector2 Index in pendingNeighbours[number]) if (Index != select) y.Add(Index);
                AddNeighbour(select, y);
                pendingNeighbours.Add(y);

                number++;
                if (number >= 9)
                    if (IfLeftConnected()) break; else if (!GoBack()) return false;
            }
            return true;
        }

        private IntVector2 FindCenter()
        {
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    if (numbers[i][j] == 0) return new IntVector2(i, j);
            return new IntVector2(0, 0);
        }

        public void GenerateAllComponent()
        {
            int componentIndex = 1;
            while (componentIndex <= 9)
            {
                componentIndex = 1;
                for (int i = 0; i < 9; i++)
                    for (int j = 0; j < 9; j++) numbers[i][j] = 0;
                while (componentIndex <= 9)
                {
                    if (!GenerateOneComponent(FindCenter(), componentIndex)) break;
                    componentIndex++;
                }
            }
        }
    }
}
