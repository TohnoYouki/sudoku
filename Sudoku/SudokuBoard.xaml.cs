using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows;
using System.Threading;
namespace Sudoku
{
    /// <summary>
    /// SudokuBoard.xaml 的交互逻辑
    /// </summary>
    public partial class SudokuBoard : UserControl
    {
        private SudokuInstance Instance;  //显示的数独种类
        private Dictionary<IntVector2, SudokuBox> Boxs = new Dictionary<IntVector2, SudokuBox>();  //所有单个格子的UI显示类
        private List<Color> ColorPrefab = new List<Color> { Colors.Orange, Colors.Green, Colors.Red, Colors.RosyBrown, Colors.SkyBlue, Colors.Pink, Colors.Gold };
        //显示数字的候选颜色
        private int ColorIndex = 0;  //当前显示数字颜色的编号

        private Dictionary<IntVector2, int> LastBoxNumbers = new Dictionary<IntVector2, int>();
        private Dictionary<IntVector2, List<bool>> LastDraftNumbers = new Dictionary<IntVector2, List<bool>>();
        public void GenerateSudoku(string name)  //根据name选择数独种类进行生成
        {
            if (name == "StandardSudoku")
                Instance = new StandardSudoku();
            if (name == "DoubleSudoku")
                Instance = new DoubleSudoku();
            if (name == "DiagonalSudoku")
                Instance = new DiagonalSudoku();
            if (name == "MiniSudoku")
                Instance = new MiniSudoku();
            if (name == "OddEvenSudoku")
                Instance = new OddEvenSudoku();
            if (name == "SumSudoku")
                Instance = sudokuDataSave.GetSingleSumSudoku();
            if (name == "JigsawSudoku")
                Instance = new JigsawSudoku();
            if (name == "CompareSudoku")
                Instance = sudokuDataSave.GetSingleCompareSudoku();
            if (name == "FerrisWheelSudoku")
                Instance = sudokuDataSave.GetSingleFerrisWheelSudoku();
            if (name == "None")
                Instance = new StandardSudoku();
            if (name != "SumSudoku" && name != "CompareSudoku" && name != "FerrisWheelSudoku")
                Instance.GenerateSudoku();
            if (name == "None")
            {
                foreach (IntVector2 Index in Instance.Result.Keys)
                    Instance.PrimaryDisk[Index] = 0;
                foreach (IntVector2 Index in Instance.PrimaryDisk.Keys)
                    Instance.Result[Index] = 0;
            }
            GenerateGrid();
            Instance.SetBoundary(Boxs);
        }

        private List<Dictionary<IntVector2, int>> boxNumbers;
        private List<Dictionary<IntVector2, List<int>>> draftNumbers;
        private int NowStartIndex;
        private int NowLastIndex;
        private int NowIndex;
        private const int MaxMemoryIndex = 200;

        public void RecordBackOrFront(bool Back)
        {
            if (Back && NowIndex != NowStartIndex)
            {
                foreach (IntVector2 Index in boxNumbers[NowIndex].Keys)
                    Boxs[Index].BoxNumber -= boxNumbers[NowIndex][Index];
                foreach (IntVector2 Index in draftNumbers[NowIndex].Keys)
                    foreach (int i in draftNumbers[NowIndex][Index])
                        Boxs[Index].ReverseDraftNumber(i + 1);
                NowIndex = (NowIndex - 1) % MaxMemoryIndex;
            }
            if (!Back && NowIndex != NowLastIndex) 
            {
                NowIndex = (NowIndex + 1) % MaxMemoryIndex;
                foreach (IntVector2 Index in boxNumbers[NowIndex].Keys)
                    Boxs[Index].BoxNumber += boxNumbers[NowIndex][Index];
                foreach (IntVector2 Index in draftNumbers[NowIndex].Keys)
                    foreach (int i in draftNumbers[NowIndex][Index])
                        Boxs[Index].ReverseDraftNumber(i + 1);
            }
            OnlyUpdateLatestStatement();
        }

        private void OnlyUpdateLatestStatement()
        {
            LastBoxNumbers.Clear();
            LastDraftNumbers.Clear();
            foreach (IntVector2 Index in Instance.PrimaryDisk.Keys)
            {
                List<bool> x = new List<bool>();
                LastBoxNumbers.Add(Index, Boxs[Index].BoxNumber);
                for (int i = 0; i < Boxs[Index].draftNumber.Length; i++)
                    x.Add(Boxs[Index].IfDraftVisibility(i + 1));
                LastDraftNumbers.Add(Index, x);
            }
        }

        private void UpdateLastStatement(bool Initial)
        {
            if (Initial)
            {
                boxNumbers = new List<Dictionary<IntVector2, int>>();
                draftNumbers = new List<Dictionary<IntVector2, List<int>>>();
                for (int i = 0; i <= MaxMemoryIndex; i++)
                {
                    boxNumbers.Add(new Dictionary<IntVector2, int>());
                    draftNumbers.Add(new Dictionary<IntVector2, List<int>>());
                }
                NowLastIndex = NowStartIndex = NowIndex = 0;
            }
            else
            {
                NowLastIndex = (NowIndex + 1) % MaxMemoryIndex;
                NowIndex = NowLastIndex;
                if (NowLastIndex == NowStartIndex) NowStartIndex = (NowStartIndex + 1) & MaxMemoryIndex;
                boxNumbers[NowLastIndex].Clear();
                draftNumbers[NowLastIndex].Clear();
                foreach (IntVector2 Index in LastBoxNumbers.Keys)
                {
                    List<int> change = new List<int>();
                    if (Boxs[Index].BoxNumber != LastBoxNumbers[Index])
                        boxNumbers[NowLastIndex].Add(Index, Boxs[Index].BoxNumber - LastBoxNumbers[Index]);
                    for (int i = 0; i < Boxs[Index].draftNumber.Length; i++)
                        if (LastDraftNumbers[Index][i] != Boxs[Index].IfDraftVisibility(i + 1))
                            change.Add(i);
                    if (change.Count != 0) draftNumbers[NowLastIndex].Add(Index, change);
                }
            }
            OnlyUpdateLatestStatement();
        }

        private void GenerateGrid()  //显示当前的数独，绘制数独的UI
        {
            bool IfFirst = true;
            IntVector2 MinIndex = new IntVector2();
            IntVector2 MaxIndex = new IntVector2();
            foreach (IntVector2 Index in Instance.PrimaryDisk.Keys)
                if (IfFirst)
                {
                    MinIndex = Index;
                    MaxIndex = Index;
                    IfFirst = false;
                }
                else
                {
                    if (Index.X < MinIndex.X) MinIndex.X = Index.X;
                    if (Index.Y < MinIndex.Y) MinIndex.Y = Index.Y;
                    if (Index.X > MaxIndex.X) MaxIndex.X = Index.X;
                    if (Index.Y > MaxIndex.Y) MaxIndex.Y = Index.Y;
                }
            Boxs.Clear();
            Sudoku.Children.Clear();
            Sudoku.RowDefinitions.Clear();
            Sudoku.ColumnDefinitions.Clear();
            for (int i = MinIndex.X; i <= MaxIndex.X; i++)
                Sudoku.RowDefinitions.Add(new RowDefinition());
            for (int i = MinIndex.Y; i <= MaxIndex.Y; i++)
                Sudoku.ColumnDefinitions.Add(new ColumnDefinition());

            foreach (IntVector2 Index in Instance.PrimaryDisk.Keys)
            {
                SudokuBox x = new SudokuBox();
                Sudoku.Children.Add(x);
                Boxs.Add(Index, x);
                x.SetValue(Grid.RowProperty, Index.X - MinIndex.X);
                x.SetValue(Grid.ColumnProperty, Index.Y - MinIndex.Y);
                x.BoxNumber = Instance.PrimaryDisk[Index];
                if (x.BoxNumber != 0) x.SetNotChange();
            }

            UpdateLastStatement(true);
        }

        public void SetValue(int Value)   //给当前选择的数字格子赋值
        {
            SudokuBox.color = ColorPrefab[ColorIndex % ColorPrefab.Count];
            if (SudokuBox.NowSelectedBox != null)
                SudokuBox.NowSelectedBox.SetValue(Value);
            Instance.ClearDraftLaber(Boxs, SudokuBox.NowSelectedBox);
            UpdateLastStatement(false);
        }

        public void ShowAnswer()
        {
            foreach (IntVector2 Index in Boxs.Keys)
                if (Boxs[Index].BoxNumber != Instance.Result[Index])
                {
                    SudokuBox.color = Colors.Red;
                    Boxs[Index].BoxNumber = Instance.Result[Index];
                    Boxs[Index].NumberContent.Foreground = new SolidColorBrush(Colors.Red);
                }
                else if (Instance.PrimaryDisk[Index] == 0) 
                    Boxs[Index].NumberContent.Foreground = new SolidColorBrush(Colors.Green);
            UpdateLastStatement(false);
        }

        public void ShowAllDraft()
        {
            foreach (SudokuBox box in Boxs.Values)
                for (int i = 0; i < box.draftNumber.Length; i++)
                    box.SetDraftLabel(i + 1, Visibility.Visible);
            UpdateLastStatement(false);
        }

        public void SpaceKeyDownEvent()
        {
            ColorIndex = (ColorIndex + 1) % ColorPrefab.Count;
        }

        public void Close()
        {
            sudokuDataSave.Close();
        }

        private SudokuDataSave sudokuDataSave = new SudokuDataSave();
        public SudokuBoard()
        {
            InitializeComponent();
            GenerateSudoku("None");
        }
    }
}
