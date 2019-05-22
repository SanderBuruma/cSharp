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

namespace DataGridEditableTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            List<DataItem> listA = new List<DataItem> {
                new DataItem("Mary fed her lambs"),
                new DataItem("And feed them she did"),
                new DataItem("All day long she fed her lambs"),
                new DataItem("And at night she sang to the sheep"),
            };
            DataGridXML.ItemsSource = listA;
            listA.Add(new DataItem("And then she fell asleep"));
        }
    }

    internal class DataItem
    {
        public string PropertyA { get; set; }
        public DataItem(string pa)
        {
            PropertyA = pa;
        }
    }
}
