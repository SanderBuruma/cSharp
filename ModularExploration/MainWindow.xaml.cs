using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Numerics;
using System.Timers;

namespace ModularExploration
{
    /// <summary>
    /// This program is supposed to count how soon a modular pattern repeats when a = a * a mod i is repeated.
    /// </summary>
    public partial class MainWindow : Window
    {
        List<ModularNumber> ModNrList = new List<ModularNumber>() { };
        System.Timers.Timer MyTimer;
        Task MainTask;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            MainListView.Items.Add(new ModularNumber { X = 2, Modular = 25, RepeatLength = 13 });
            MainListView.Items.Refresh();

            MyTimer = new System.Timers.Timer();
            MyTimer.Elapsed += new ElapsedEventHandler(RefreshListEvent);
            MyTimer.Interval = 50;
            MyTimer.Enabled = true;

            MainTask = Task.Run(MoreModularNumbers);
        }
        private void RefreshListEvent(object source, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                RefreshListView();
            });
        }
        public void RefreshListView()
        {
            MainListView.Items.Clear();
            for (int i = 0; i < ModNrList.Count; i++)
            {
                var item = ModNrList[i];
                MainListView.Items.Add(new ModularNumber { X = item.X, Modular = item.Modular, RepeatLength = item.RepeatLength });
            }
            MainListView.Items.Refresh();
        }
        private void MoreModularNumbers()
        {
            BigInteger i = 9999999;
            BigInteger count, b;
            while (true)
            {
                i+=2;
                for (BigInteger a = 2; a < 3; a++)
                {
                    count = 0;
                    b = a;
                    for (int j = 0; j < 25; j++)
                    {
                        b *= b;
                        b %= i;
                    }
                    BigInteger startingNr = b;

                    while (true)
                    {
                        b *= b;
                        b %= i;
                        count++;
                        if (startingNr == b) break;
                        if (b == 1) break;
                    }
                    if (b == 1) continue;

                    if (ModNrList.Count >= 50 && ModNrList[0].RepeatLength < count)
                    {
                        var mnr = new ModularNumber
                        {
                            RepeatLength = count,
                            X = a,
                            Modular = i
                        };
                        ModNrList.Add(mnr);
                        ModNrList = ModNrList.OrderBy(n => n.RepeatLength).ToList();
                        ModNrList.RemoveAt(0);
                    }
                    else if (ModNrList.Count < 50)
                    {
                        var mnr = new ModularNumber
                        {
                            RepeatLength = count,
                            X = a,
                            Modular = i
                        };
                        ModNrList.Add(mnr);
                        ModNrList.OrderBy(n => n.RepeatLength);
                    }
                }
            }
        }
    }
}
