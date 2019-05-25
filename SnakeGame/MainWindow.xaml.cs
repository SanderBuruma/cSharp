using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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
        private double ScoreTreshold { get; set; } = 2e3;
        private int TimerInterval { get; set; } = 75;
        private readonly int RectWidth = 20;
        private readonly int RectSpacing = 2;
        private readonly Random rng = new Random();
        private readonly BinaryFormatter Formatter = new BinaryFormatter();
        private int savedFilesCount = 0;
        private int savedFilesMax = 1000;

        public MainWindow()
        {
            InitializeComponent();

            MainWindow1.Width = 22 + (RectWidth + RectSpacing) * BoardWH;
            MainWindow1.Height = 42 + (RectWidth + RectSpacing) * BoardWH;
        }
        /// <summary>
        /// This mode should generate new SnakeBrain####.dat files which have scored above a certain treshold
        /// </summary>
        private void DrawFieldRectangles()
        {
            FieldRectangles = new Rectangle[BoardWH * BoardWH];
            for (int i = 0; i < BoardWH; i++)
                for (int j = 0; j < BoardWH; j++)
                {
                    Rectangle rect = new Rectangle
                    {
                        Width = RectWidth,
                        Height = RectWidth,
                        Margin = new Thickness(5 + (RectWidth + RectSpacing) * i, 0, 0, 5 + (RectWidth + RectSpacing) * j),
                        Fill = ReturnFieldBrush(Board1.Fields[i + j * BoardWH]),
                        VerticalAlignment = VerticalAlignment.Bottom,
                        HorizontalAlignment = HorizontalAlignment.Left
                    };
                    FieldRectangles[i + j * BoardWH] = rect;
                    MainGrid.Children.Add(FieldRectangles[i + j * BoardWH]);
                }

        }
        private void ModeGenerateNewBrains(int hlWidth, int hlHeight)
        {
            NewBoard();
            DrawFieldRectangles();
            NewBrain(hlWidth, hlHeight);
            while (savedFilesCount < savedFilesMax)
            {
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
                    continue;

                Board1.GameOver = true;
                //gameover
                double score = CalculateScore();

                if (score > ScoreTreshold)
                {
                    RedrawField();

                    try
                    {
                        string filename = "SnakeBrainFile" + Math.Floor(score).ToString() + "-" + DateTime.Now.Ticks.ToString() + ".dat";
                        FileStream SnakeBrainFile =
                            new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write);
                        Formatter.Serialize(SnakeBrainFile, SnakeBrain);
                        SnakeBrainFile.Close();
                        savedFilesCount++;
                        //MessageBox.Show("Saved snakebrain as " + filename);
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show("Error saving snake brain.");
                        MessageBox.Show(err.Message);
                    }

                    //MessageBox.Show("You crashed into your own tail and died or you ran out of time, your final score was " + string.Format("{0:N2}", score) + "\n\nGrow more quickly and grow larger to gain a larger score");
                }

                NewBoard();
                NewBrain(hlWidth, hlHeight);
            }
            MessageBox.Show("End of program, " + savedFilesCount.ToString() + " brain files generated.\nReview the executables' folder to see the brain files");
        }
        /// <summary>
        /// this mode is intended to observably demonstrate an AI (aka. snake brain) from a file
        /// </summary>
        private void ModeDemoAi()
        {
            NewBoard();
            DrawFieldRectangles();

            MyTimer = new Timer();
            MyTimer.Elapsed += new ElapsedEventHandler(DemoAIEvent);
            MyTimer.Interval = TimerInterval;
            MyTimer.Enabled = true;
        }
        /// <summary>
        /// runs the main brain a few times after mutating it and returns its average score
        /// </summary>
        /// <param name="mutateMagnitude">The magnitude by which mutations can happen</param>
        /// <param name="iterations">The number of times the snake plays the game before returning the average score.</param>
        /// <param name="mutateNRR">Determines how much mutation magnitude distrubtion conforms to a normal distrubtion.</param>
        /// <returns>the average score</returns>
        private double ModeTrainAI(double mutateMagnitude = .1, int mutateNRR = 4, int iterations = 100, int mutationChance = 5)
        {
            SnakeBrain.Mutate(mutateMagnitude, mutateNRR, mutationChance);
            double averageScore = 0;
            for (int i = 0; i < iterations; i++)
            {
                NewBoard();
                while (Board1.Progress1Tick())
                    RunBrain();
                averageScore += CalculateScore();
            }
            return averageScore / iterations;

        }
        private void DemoAIEvent(object source, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                DemoAICycle();
            });
        }
        private void DemoAICycle()
        {
            RedrawField();
            if (Board1.Progress1Tick())
                RunBrain();
            else
            {//gameOver
                MyTimer.Enabled = false;
                //MessageBox.Show("This bot scored " + string.Format("{0:N2}", CalculateScore()) + " points!\nIt crashed or it ran out of time.");
                MyTimer.Enabled = true;
                NewBoard();
            }
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
        private void RunBrain()
        {
            double[] perceptronsValues = new double[5]
            {
                (double)Board1.TailLength/Board1.WidthHeight/Board1.WidthHeight*2,
                //an rng factor to allow the snake to have some randomness (and prevent repeating patterns)
                rng.NextDouble(),
                //distance to food left, up, right, down
                0,
                0,
                0,
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
            double[] foodDistances = new double[4] {
                Board1.DistanceLeft(Board1.SnakeHeadX, Board1.FoodX),
                Board1.DistanceUp(Board1.SnakeHeadY, Board1.FoodY),
                Board1.DistanceRight(Board1.SnakeHeadX, Board1.FoodX),
                Board1.DistanceDown(Board1.SnakeHeadY, Board1.FoodY)
            };

            switch (Board1.SnakeDirection)
            {
                case Board.Direction.left:
                    if (tailDistances[0] < foodDistances[0])
                        perceptronsValues[2] = -(Board1.WidthHeight-tailDistances[0]);
                    else
                        perceptronsValues[2] = Board1.WidthHeight - foodDistances[0];
                    if (tailDistances[1] < foodDistances[1])
                        perceptronsValues[3] = -(Board1.WidthHeight - tailDistances[1]);
                    else
                        perceptronsValues[3] = Board1.WidthHeight - foodDistances[1];
                    if (tailDistances[3] < foodDistances[3])
                        perceptronsValues[4] = -(Board1.WidthHeight - tailDistances[3]);
                    else
                        perceptronsValues[4] = Board1.WidthHeight - foodDistances[3];
                    break;
                case Board.Direction.up:
                    if (tailDistances[1] < foodDistances[1])
                        perceptronsValues[2] = -(Board1.WidthHeight - tailDistances[1]);
                    else
                        perceptronsValues[2] = Board1.WidthHeight - foodDistances[1];
                    if (tailDistances[2] < foodDistances[2])
                        perceptronsValues[3] = -(Board1.WidthHeight - tailDistances[2]);
                    else
                        perceptronsValues[3] = Board1.WidthHeight - foodDistances[2];
                    if (tailDistances[0] < foodDistances[0])
                        perceptronsValues[4] = -(Board1.WidthHeight - tailDistances[0]);
                    else
                        perceptronsValues[4] = Board1.WidthHeight - foodDistances[0];
                    break;
                case Board.Direction.right:
                    if (tailDistances[2] < foodDistances[2])
                        perceptronsValues[2] = -(Board1.WidthHeight - tailDistances[2]);
                    else
                        perceptronsValues[2] = Board1.WidthHeight - foodDistances[2];
                    if (tailDistances[3] < foodDistances[3])
                        perceptronsValues[3] = -(Board1.WidthHeight - tailDistances[3]);
                    else
                        perceptronsValues[3] = Board1.WidthHeight - foodDistances[3];
                    if (tailDistances[1] < foodDistances[1])
                        perceptronsValues[4] = -(Board1.WidthHeight - tailDistances[1]);
                    else
                        perceptronsValues[4] = Board1.WidthHeight - foodDistances[1];
                    break;
                case Board.Direction.down:
                    if (tailDistances[3] < foodDistances[3])
                        perceptronsValues[2] = -(Board1.WidthHeight - tailDistances[3]);
                    else
                        perceptronsValues[2] = Board1.WidthHeight - foodDistances[3];
                    if (tailDistances[0] < foodDistances[0])
                        perceptronsValues[3] = -(Board1.WidthHeight - tailDistances[0]);
                    else
                        perceptronsValues[3] = Board1.WidthHeight - foodDistances[0];
                    if (tailDistances[2] < foodDistances[2])
                        perceptronsValues[4] = -(Board1.WidthHeight - tailDistances[2]);
                    else
                        perceptronsValues[4] = Board1.WidthHeight - foodDistances[2];
                    break;
            }
            for (int i = 2; i < 5; i++)
            {
                perceptronsValues[i] /= Board1.WidthHeight;
            }

            //calculate what the brain "thinks" that it should output
            double[] brainThoughts = SnakeBrain.InputToOutput(perceptronsValues);

            double maxvalue = brainThoughts.Max();
            for (int i = 0; i < 3; i++)
            {
                if (maxvalue == brainThoughts[i])
                {
                    if (i == 1)
                        Board1.ChangeDirection(Board.Direction.right);
                    else if (i == 2)
                        Board1.ChangeDirection(Board.Direction.left);
                    break;
                }
            }
        }

        private void NewBrain(int hlWidth, int hlHeight)
        {
            SnakeBrain = new Brain(5, hlWidth, hlHeight, 3);
        }
        private void NewBoard()
        {
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

        private double CalculateScore()
        {
            return Math.Pow(Board1.TailLength, 2) / Board1.Tick * 1e3;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    Application.Current.Shutdown();
                    break;
                case Key.P:
                    RedrawField();
                    Pause();
                    break;
                default:
                    break;
            }
        }

        private void ModeNewBrainsBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(ModeNewBrainsTresholdBox.Text, out double dbl))
            {
                MessageBox.Show("Invalid treshold value");
                return;
            }
            ScoreTreshold = dbl;
            if (!int.TryParse(ModeNewBrainsQuantityBox.Text, out int fileMax))
            {
                MessageBox.Show("Invalid desired brains value, input an integer");
                return;
            }
            if (!int.TryParse(ModeNewBrainsHLWidthBox.Text, out int hlWidth))
            {
                MessageBox.Show("Invalid desired HL width value, input an integer");
                return;
            }
            if (!int.TryParse(ModeNewBrainsHLHeightBox.Text, out int hlHeight))
            {
                MessageBox.Show("Invalid desired HL height value, input an integer");
                return;
            }
            savedFilesMax = fileMax;

            int len = MainGrid.Children.Count;
            for (int i = 0; i < len; i++)
                MainGrid.Children.RemoveAt(0);

            ModeGenerateNewBrains(hlWidth, hlHeight);
        }

        private void ModeDemoAIButton_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(ModeDemoAIInterval.Text, out int interval))
            {
                MessageBox.Show("Invalid interval value, please input an integer");
                return;
            }
            TimerInterval = interval;
            OpenFileDialog dlg = new OpenFileDialog
            {
                DefaultExt = ".dat",
                Filter = "Data Files (*.dat)|*.dat"
            };

            bool? result = dlg.ShowDialog();
            if (result != true)
            {
                MessageBox.Show("Couldn't recover file path");
                return;
            }


            if (!File.Exists(dlg.FileName))
            {
                MessageBox.Show("Couldn't find that file");
                return;
            }

            try
            {
                FileStream SnakeBrainInstanceFromFile = new FileStream(dlg.FileName, FileMode.Open, FileAccess.Read);
                SnakeBrain = (Brain)Formatter.Deserialize(SnakeBrainInstanceFromFile);
                SnakeBrainInstanceFromFile.Close();
            }
            catch (Exception err)
            {
                MessageBox.Show("Could not load selected snake brain file for this demo");
                MessageBox.Show(err.Message);
                return;
            }

            int len = MainGrid.Children.Count;
            for (int i = 0; i < len; i++)
                MainGrid.Children.RemoveAt(0);

            ModeDemoAi();
        }

        private void ModeTrainAIButton_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(ModeTrainAIDegreeBox.Text, out double degree))
            {
                MessageBox.Show("Invalid interval value, please input a double (aka. decimal) value");
                return;
            }
            if (!int.TryParse(ModeTrainAIIterationsBox.Text, out int iterations))
            {
                MessageBox.Show("Invalid iterations value, please input an integer");
                return;
            }
            if (!int.TryParse(ModeTrainAIMutationChanceBox.Text, out int mutationChance))
            {
                MessageBox.Show("Invalid iterations value, please input an integer");
                return;
            }
            mutationChance = 100 / mutationChance;
            if (!int.TryParse(ModeTrainAINRRBox.Text, out int nrr))
            {
                MessageBox.Show("Invalid NRR value, please input an integer");
                return;
            }
            if (!double.TryParse(ModeTrainAITresholdBox.Text, out double treshold))
            {
                MessageBox.Show("Invalid iterations value, please input an integer");
                return;
            }

            OpenFileDialog dlg = new OpenFileDialog
            {
                DefaultExt = ".dat",
                Filter = "Data Files (*.dat)|*.dat"
            };

            bool? result = dlg.ShowDialog();
            if (result != true)
            {
                MessageBox.Show("Couldn't recover file path");
                return;
            }


            if (!File.Exists(dlg.FileName))
            {
                MessageBox.Show("Couldn't find that file");
                return;
            }

            try
            {
                FileStream SnakeBrainInstanceFromFile = new FileStream(dlg.FileName, FileMode.Open, FileAccess.Read);
                SnakeBrain = (Brain)Formatter.Deserialize(SnakeBrainInstanceFromFile);
                SnakeBrainInstanceFromFile.Close();
            }
            catch (Exception err)
            {
                MessageBox.Show("Could not load selected snake brain file for this demo");
                MessageBox.Show(err.Message);
                return;
            }

            int len = MainGrid.Children.Count;
            for (int i = 0; i < len; i++)
                MainGrid.Children.RemoveAt(0);

            NewBoard();
            DrawFieldRectangles();

            Brain prevBrain = DeepCopy(SnakeBrain);
            double prevScore = 0;
            for (int i = 0; i < iterations; i++)
            {
                NewBoard();
                while (Board1.Progress1Tick())
                    RunBrain();
                prevScore += CalculateScore();
            }
            prevScore /= iterations;

            int count = 1000000000;
            while (--count > 0)
                {
                double score = Math.Floor(ModeTrainAI(degree, nrr, iterations, mutationChance));
                if (score/treshold > prevScore)
                {
                    try
                    {
                        string filename = "TrainedSnakeBrainFile" + Math.Floor(score).ToString() + "-" + iterations.ToString() + "-" + DateTime.Now.Ticks.ToString() + ".dat";
                        FileStream SnakeBrainFile =
                            new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write);
                        Formatter.Serialize(SnakeBrainFile, SnakeBrain);
                        SnakeBrainFile.Close();
                        savedFilesCount++;
                        //MessageBox.Show("Saved snakebrain as " + filename);
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show("Error saving snake brain.");
                        MessageBox.Show(err.Message);
                    }
                    prevScore = score;
                    prevBrain = DeepCopy(SnakeBrain);
                }
                else
                {
                    SnakeBrain = DeepCopy(prevBrain);
                }
                if (savedFilesCount > savedFilesMax)
                    break;
            }
        }
        /// <summary>
        /// Makes an independent deep copy of a class instance<br/>
        /// https://stackoverflow.com/a/1031062/10055628
        /// </summary>
        /// <typeparam name="T">The Class instance</typeparam>
        /// <param name="other"></param>
        /// <returns></returns>
        public static T DeepCopy<T>(T other)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, other);
                ms.Position = 0;
                return (T)formatter.Deserialize(ms);
            }
        }
    }
}
