using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using System.ComponentModel;
namespace Sudoku
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            //应用动画
            System.Windows.Media.Animation.Storyboard s = (System.Windows.Media.Animation.Storyboard)TryFindResource("sb");
            s.Begin();  // Start animation
            KeyDown += InputHandle;
            string translate = System.Environment.CurrentDirectory.Replace('\\', '/');
            player.Open(new System.Uri(translate + "/music/RADWIMPS - 御宅訪問.mp3", System.UriKind.Absolute));
            player.Play();
            player.MediaEnded += (sender, e) => { player.Position = new System.TimeSpan(0); };
            this.Closing += DoWithClosing;
        }

        private void DoWithClosing(object sender, CancelEventArgs e)
        {
            player.Close();
            Board.Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private bool IfPlay = true;
        private MediaPlayer player = new MediaPlayer();
        private void OpenAndCloseMusic(object sender, MouseButtonEventArgs e)
        {
            if (IfPlay) player.Pause(); else player.Play();
            IfPlay = !IfPlay;
        }

        private void Logout_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void ShowAllDraftButton(object sender, MouseButtonEventArgs e)
        {
            Board.ShowAllDraft();
        }

        private void ShowAnswerButton(object sender, MouseButtonEventArgs e)
        {
            Board.ShowAnswer();
        }

        private void GoBackButtonDown(object sender, MouseButtonEventArgs e)
        {
            Board.RecordBackOrFront(true);
        }

        private void GoFrontButtonDown(object sender, MouseButtonEventArgs e)
        {
            Board.RecordBackOrFront(false);
        }

        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Board.GenerateSudoku((sender as Label).Name);
            if ((sender as Label).Name == "StandardSudoku")
                userInfo.Text = "每一行、每一列、每一个粗线宫（3*3）内的数字均含1-9，不重复";
            if ((sender as Label).Name == "DoubleSudoku")
                userInfo.Text = "两个重叠数独均满足标准数独的规则";
            if ((sender as Label).Name == "DiagonalSudoku")
                userInfo.Text = "每一行、每一列、每一个粗线宫（3*3）以及对角线内的数字均含1-9，不重复";
            if ((sender as Label).Name == "MiniSudoku")
                userInfo.Text = "每一行、每一列、每一个粗线宫（2*3）内的数字均含1-6，不重复";
            if ((sender as Label).Name == "OddEvenSudoku")
                userInfo.Text = "每一行、每一列、每一个粗线宫（3*3）内的数字均含1-9，不重复，绿色区域必须是奇数，粉红色区域必须是偶数";
            if ((sender as Label).Name == "SumSudoku")
                userInfo.Text = "每一行、每一列、每一个粗线宫（3*3）内的数字均含1-9，不重复且每个颜色区域内的和为上方的数字";
            if ((sender as Label).Name == "JigsawSudoku")
                userInfo.Text = "每一行、每一列、每一个粗线宫区域内的数字均含1-9，不重复";
            if ((sender as Label).Name == "CompareSudoku")
                userInfo.Text = "每一行、每一列、每一个粗线宫（3*3）内的数字均含1-9，不重复，且相邻区域要满足大小关系";
            if ((sender as Label).Name == "FerrisWheelSudoku")
                userInfo.Text = "规则同标准数独一样且周围的数字,表示从此处观察该行或列,看到的数字个数(小数会被前面的大数挡住)";
        }

        private void TabItem_MouseMove_1(object sender, MouseEventArgs e)
        {
            //var part_text= this.LeftTabControl.Template.FindName("PART_Text", this.LeftTabControl);
            //null
        }

        private void InputHandle(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9)
                Board.SetValue(e.Key - Key.D0);
        }
    }
}
