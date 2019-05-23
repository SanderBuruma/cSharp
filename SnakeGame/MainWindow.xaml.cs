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
        private Brain SnakeBrain { get; set; }
        private Timer MyTimer { get; set; }
        private readonly int TimerInterval = 1;
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

            NewBrain();

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
        private void TimerEventFunction()
        {
            if (Board1.GameOver)
                return;

            RedrawField();
            bool gameOver = true;
            for (int i = 0; i < 1000; i++)
            {
                if (Board1.Progress1Tick())
                {
                    RunBrain();
                    gameOver = false;
                }
                else
                {
                    gameOver = true;
                    break;
                }
            }
            if (!gameOver)
                return;

            Board1.GameOver = true;
            //gameover
            double score = Board1.Score;
            score += Math.Pow(Board1.TailLength, 2)/Board1.Tick*1e3;

            if (score > 10e2)
            {
                RedrawField();
                MessageBox.Show("You crashed into your own tail and died or you ran out of time, your final score was " + string.Format("{0:N2}", score) + "\n\nGrow more quickly and grow larger to gain a larger score");
            }

            Board1 = new Board(BoardWH);
            NewBrain();
        }
        private void RunBrain()
        {
            double[] perceptronsValues = new double[11]
            {
                (double)Board1.TailLength/Board1.WidthHeight/Board1.WidthHeight*2,
                (double)Board1.SnakeDirection/3,
                //distance to food left, up, right, down
                (double)1e3*Board1.DistanceLeft(Board1.SnakeHeadX, Board1.FoodX)/Board1.WidthHeight,
                (double)1e3*Board1.DistanceUp(Board1.SnakeHeadY, Board1.FoodY)/Board1.WidthHeight,
                (double)1e3*Board1.DistanceRight(Board1.SnakeHeadX, Board1.FoodX)/Board1.WidthHeight,
                (double)1e3*Board1.DistanceDown(Board1.SnakeHeadY, Board1.FoodY)/Board1.WidthHeight,
                //distance to nearest tail piece
                0,
                0,
                0,
                0,
                1e3 * new Random().NextDouble()
            };

            double[] tailDistances = new double[4] {
                Board1.WidthHeight-1,
                Board1.WidthHeight-1,
                Board1.WidthHeight-1,
                Board1.WidthHeight-1 };
            for (int i = 0; i < Board1.TailLength; i++)
            {
                if (Board1.TailY[i] == Board1.SnakeHeadY)
                {
                    tailDistances[0] = Math.Min(tailDistances[0], Board1.DistanceLeft(Board1.SnakeHeadX, Board1.TailX[i]));
                    tailDistances[2] = Math.Min(tailDistances[2], Board1.DistanceRight(Board1.SnakeHeadX, Board1.TailX[i]));
                }
                if (Board1.TailX[i] == Board1.SnakeHeadX)
                {
                    tailDistances[1] = Math.Min(tailDistances[1], Board1.DistanceUp(Board1.SnakeHeadY, Board1.TailY[i]));
                    tailDistances[3] = Math.Min(tailDistances[3], Board1.DistanceDown(Board1.SnakeHeadY, Board1.TailY[i]));
                }
            }
            perceptronsValues[6] = tailDistances[0] / (Board1.WidthHeight - 1);
            perceptronsValues[7] = tailDistances[1] / (Board1.WidthHeight - 1);
            perceptronsValues[8] = tailDistances[2] / (Board1.WidthHeight - 1);
            perceptronsValues[9] = tailDistances[3] / (Board1.WidthHeight - 1);

            //calculate what the brain "thinks" that it should output
            double[] brainThoughts = SnakeBrain.InputToOutput(perceptronsValues);

            double maxvalue = brainThoughts.Max();
            for (int i = 0; i < 4; i++)
            {
                if (maxvalue == brainThoughts[i])
                {
                    Board1.ChangeDirection((Board.Direction)i);
                    break;
                }
            }
        }

        private void NewBrain()
        {
            SnakeBrain = new Brain(11, 3, 6, 4);
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
                TimerEventFunction();
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
