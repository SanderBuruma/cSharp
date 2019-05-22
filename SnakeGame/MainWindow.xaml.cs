using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SnakeGame
{
    public partial class MainWindow : Window
    {
        private readonly int BoardWH = 16;
        internal Board Board1 { get; set; }
        private Rectangle[] FieldRectangles { get; set; }
        private Timer MyTimer { get; set; }
        private readonly int TimerInterval = 75;
        private readonly int RectWidth = 20;
        private readonly int RectSpacing = 2;
        public MainWindow()
        {
            InitializeComponent();
            Board1 = new Board(BoardWH);

            MainWindow1.Width = 22 + (RectWidth + RectSpacing) * BoardWH;
            MainWindow1.Height = 42 + (RectWidth + RectSpacing) * BoardWH;

            FieldRectangles = new Rectangle[BoardWH * BoardWH];
            for (int i = 0; i < BoardWH; i++)
                for (int j = 0; j < BoardWH; j++)
                {
                    Brush brush = Brushes.White;
                    Rectangle rect = new Rectangle
                    {
                        Width = RectWidth,
                        Height = RectWidth,
                        Margin = new Thickness(5 + (RectWidth + RectSpacing) * i, 0, 0, 5 + (RectWidth + RectSpacing) * j),
                        Fill = ReturnFieldBrush(Board1.Fields[i+j*BoardWH]),
                        VerticalAlignment = VerticalAlignment.Bottom,
                        HorizontalAlignment = HorizontalAlignment.Left
                    };
                    FieldRectangles[i + j * BoardWH] = rect;
                    MainGrid.Children.Add(FieldRectangles[i + j * BoardWH]);
                }

            MyTimer = new Timer();
            MyTimer.Elapsed += new ElapsedEventHandler(ProgressEvent);
            MyTimer.Interval = TimerInterval;
            MyTimer.Enabled = true;

            Brain brain = new Brain(5,7,3,11);

        }
        private void RedrawField()
        {
            for (int i = 0; i < BoardWH; i++)
                for (int j = 0; j < BoardWH; j++)
                {
                    int t = i + j * BoardWH;
                    FieldRectangles[t].Fill = ReturnFieldBrush(Board1.Fields[t]);
                }
        }
        private void Progress1Tick()
        {
            if (Board1.GameOver)
                return;
            if (Board1.Progress1Tick())
            {
                RedrawField();
                return;
            }

            Board1.GameOver = true;
            //gameover
            MessageBox.Show("You crashed into your own tail and died, your final score was " + (Board1.Score).ToString());
            Board1 = new Board(BoardWH);
        }
        private Brush ReturnFieldBrush(Board.Field field)
        {

            switch (field)
            {
                case Board.Field.empty:
                    return Brushes.White;
                case Board.Field.head:
                    return Brushes.Black;
                case Board.Field.tail:
                    return Brushes.Green;
                case Board.Field.food:
                    return Brushes.LightGreen;
            }
            return Brushes.White;
        }
        private void Pause()
        {
            if (MyTimer.Enabled)
                MyTimer.Enabled = false;
            else
                MyTimer.Enabled = true;
        }
        private void ProgressEvent(object source, ElapsedEventArgs e) {
            Dispatcher.Invoke(() =>
            {
                Progress1Tick();
            });
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.W:
                    Board1.ChangeDirection(Board.Direction.up);
                    break;
                case Key.A:
                    Board1.ChangeDirection(Board.Direction.left);
                    break;
                case Key.S:
                    Board1.ChangeDirection(Board.Direction.down);
                    break;
                case Key.D:
                    Board1.ChangeDirection(Board.Direction.right);
                    break;
                case Key.Escape:
                    Application.Current.Shutdown();
                    break;
                case Key.P:
                    Pause();
                    break;
                default:
                    break;
            }
        }
    }
}
