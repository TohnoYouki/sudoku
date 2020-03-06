using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.Collections.Generic;
namespace Sudoku
{
    /// <summary>
    /// SudokuBox.xaml 的交互逻辑
    /// </summary>
    /// 
    public partial class SudokuBox : UserControl
    {   //用于数独单个格子的所有UI绘制操作
        public static List<Color> UnHelightColors = new List<Color>  //数字格子背景颜色取值
        { Color.FromArgb(255, 246, 236, 187), Color.FromArgb(255, 174, 232, 168), Color.FromArgb(255,255,177,230), Color.FromArgb(255,255,163,110), Color.FromArgb(255, 255, 190, 190), Color.FromArgb(255, 234, 190, 255) };

        public enum SelectType { NotSelected,LeftSelected,RightSelected }  //表示数字格子的选择状态
        public static SudokuBox NowSelectedBox = null;  //当前选择的数字格子
        public static Color color = Color.FromArgb(255, 254, 63, 8);  //显示数字的颜色
        //
        public struct Orientation
        {
            public Color East;
            public Color West;
            public Color South;
            public Color North;
            public Orientation(Color north, Color south, Color west, Color east)
            {
                East = east;
                West = west;
                South = south;
                North = north;
            }
        }

        private Orientation boundaryOrientation;
        public Orientation BoundaryOrientation
        {
            get { return boundaryOrientation; }
            set
            {
                boundaryOrientation = value;
                OnBoundaryChanged();
            }
        }

        private void OnBoundaryChanged()  //设置数字格子的特殊显示机制，包括边界和不等号符号
        {
            Orientation boundary = BoundaryOrientation;
            East.Stroke = new SolidColorBrush(boundary.East);
            West.Stroke = new SolidColorBrush(boundary.West);
            North.Stroke = new SolidColorBrush(boundary.North);
            South.Stroke = new SolidColorBrush(boundary.South);
        }

        private Color unHelightColor = Color.FromArgb(255, 246, 236, 187);  //数字格子没有被鼠标选中时的颜色
        private Color helightColor = Color.FromArgb(255, 255, 255, 255);    //数字格子被鼠标选中时的颜色

        public Color UnHelightColor
        {
            get { return unHelightColor; }
            set { unHelightColor = value; if (!IfHelight) GridBackGround.Background = new SolidColorBrush(unHelightColor); }
        }

        public Color HelightColor
        {
            get { return helightColor; }
            set { helightColor = value; if (IfHelight) GridBackGround.Background = new SolidColorBrush(helightColor); }
        }

        private bool IfHelight = false;   //数字格子是否被鼠标选中
        private void OnHelightChanged(object sender, MouseEventArgs e)
        {
            if (!IfHelight)
                GridBackGround.Background = new SolidColorBrush(HelightColor);
            else
                GridBackGround.Background = new SolidColorBrush(UnHelightColor);
            IfHelight = !IfHelight;
        }

        private bool CanNotChange = false;  //数字格子中的数字能否被修改
        public void SetNotChange()
        {
            CanNotChange = true;
            ClearDraftNumber(false);
            NumberContent.Foreground = new SolidColorBrush(Colors.Black);
        }
        //
        private int boxNumber;
        public int BoxNumber    //数字格子中的数字
        {
            get { return boxNumber; }
            set
            {
                if (!CanNotChange)
                {
                    if (value > 0 && value < 10)
                        boxNumber = value;
                    else
                        boxNumber = 0;
                    OnBoxNumberChanged();
                }
            }
        }

        private void OnBoxNumberChanged()
        {
            NumberContent.Foreground = new SolidColorBrush(color);
            if (BoxNumber != 0)
            {
                ClearDraftNumber(false);
                NumberContent.Content = BoxNumber.ToString();
            }
            else
            {
                ClearDraftNumber(true);
                NumberContent.Content = "";
            }
        }

        public bool[] draftNumber = new bool[9];   //该格子所有草稿的显示状态

        public void SetDraftLabel(int draft, Visibility visibility)
        {
            if (draft > 0 && draft < 10 && draftNumber[draft - 1]) 
                (DraftNumberGrid.Children[draft - 1] as Label).Visibility = visibility;
        }

        public bool IfDraftVisibility(int draft)
        {
            return (DraftNumberGrid.Children[draft - 1] as Label).Visibility == Visibility.Visible;
        }

        public void ReverseDraftNumber(int draft)  //将数字格子中的draft草稿的显示状态反转
        {
            if (draft > 0 && draft < 10 && draftNumber[draft - 1])
            {
                if ((DraftNumberGrid.Children[draft - 1] as Label).Visibility == Visibility.Visible)
                    (DraftNumberGrid.Children[draft - 1] as Label).Visibility = Visibility.Hidden;
                else
                    (DraftNumberGrid.Children[draft - 1] as Label).Visibility = Visibility.Visible;
            }
        }

        
        public void ClearDraftNumber(bool value)
        {
            for (int i = 0; i < 9; i++) 
            {
                draftNumber[i] = value;
                (DraftNumberGrid.Children[i] as Label).Visibility = Visibility.Hidden;
            }
        }

        public void SetValue(int value)  //将数字格子的值设为value，将所有草稿设为不显示
        {
            if (value >= 0 && value < 10)
            {
                if (ifSelected == SelectType.LeftSelected)
                    BoxNumber = value;
                else if (ifSelected == SelectType.RightSelected)
                    ReverseDraftNumber(value);
            }
        }

        private SelectType ifSelected = SelectType.NotSelected;
        public SelectType IfSelected
        {
            get { return ifSelected; }
            private set { ifSelected = value; OnSelectedChanged(); }
        }
        private void OnSelected(object sender, MouseEventArgs e)
        {
            SelectType NewSelect = new SelectType();
            if (e.LeftButton == MouseButtonState.Pressed)
                NewSelect = SelectType.LeftSelected;
            else if (e.RightButton == MouseButtonState.Pressed)
                NewSelect = SelectType.RightSelected;
            else
                NewSelect = SelectType.NotSelected;

            if (NowSelectedBox != null && NowSelectedBox != this)
                NowSelectedBox.IfSelected = SelectType.NotSelected;
            if (NowSelectedBox != this) 
            {
                NowSelectedBox = this;
                IfSelected = NewSelect;
            }
            else
            {
                if (ifSelected == NewSelect)
                {
                    NowSelectedBox = null;
                    IfSelected = SelectType.NotSelected;
                }
                else
                    IfSelected = NewSelect;
            }
        }
        private void OnSelectedChanged()
        {
            LeftSelectedImage.Visibility = Visibility.Hidden;
            RightSelectedImage.Visibility = Visibility.Hidden;
            if (IfSelected == SelectType.LeftSelected)
                LeftSelectedImage.Visibility = Visibility.Visible;
            else if (ifSelected == SelectType.RightSelected)
                RightSelectedImage.Visibility = Visibility.Visible;
        }

        public SudokuBox()
        {
            InitializeComponent();
            BoxNumber = 0;
            BoundaryOrientation = new Orientation(Colors.Transparent, Colors.Transparent, Colors.Transparent, Colors.Transparent);
            ClearDraftNumber(true);
            IfHelight = false;
            IfSelected = SelectType.NotSelected;
            GridBackGround.Background = new SolidColorBrush(UnHelightColor);
            MouseEnter += OnHelightChanged;
            MouseLeave += OnHelightChanged;
            MouseLeftButtonDown += OnSelected;
            MouseRightButtonDown += OnSelected;
        }
    }
}
