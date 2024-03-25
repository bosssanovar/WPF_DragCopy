using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    public enum Type
    {
        Type1,
        Type2,
        Type3,
        Type4,
    }

    public class MyItems
    {
        public int Col1 { get; set; }
        public Type Col2 { get; set; }
        public int Col3 { get; set; }
        public int Col4 { get; set; }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // create a source for the datagrid
        public List<MyItems> DataList { get; set; }

        // somewhere to hold the selected cells
        IList<DataGridCellInfo> DataGridSelectedCells { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            DataList = new List<MyItems>()
                {
                    new MyItems() { Col1=1, Col2=Type.Type2, Col3=3, Col4=4},
                    new MyItems() { Col1=5, Col2=Type.Type4, Col3=7, Col4=8},
                    new MyItems() { Col1=9, Col2=Type.Type1, Col3=11, Col4=12},
                    new MyItems() { Col1=13, Col2=Type.Type3, Col3=15, Col4=16},
                };

            MyGrid.ItemsSource = DataList;

        }

        private void SelectionChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            DataGridSelectedCells = MyGrid.SelectedCells;

            var firstCell = DataGridSelectedCells[0].Column.GetCellContent(DataGridSelectedCells[0].Item);

            Type content;
            if (firstCell != null)
            {  // if it's not null try to get the content
                DataGridCell dc = (DataGridCell)firstCell.Parent;
                if (dc.Content is ComboBox cb)
                {
                    content = (Type)cb.SelectedValue;

                    // Check your key here (Ctrl D, Ctrl R etc)                  
                    // then loop around your data looking at what is selected
                    // chosing the direction based on what key was pressed       
                    foreach (DataGridCellInfo d in DataGridSelectedCells)
                    {   // get the content of the cell           
                        var cellContent = d.Column.GetCellContent(d.Item);
                        if (cellContent != null)
                        {  // if it's not null try to get the content
                            DataGridCell dc2 = (DataGridCell)cellContent.Parent;

                            if (dc2.Content is ComboBox cb2)
                            {
                                cb2.SelectedValue = content;
                            }
                        }
                    }
                }
            }
        }
    }
}