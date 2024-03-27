using Reactive.Bindings;

using System.Diagnostics;
using System.Dynamic;
using System.Reactive.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
        public int Number { get; init; }
        public ReactivePropertySlim<int> Col1 { get; } = new(0);
        public ReactivePropertySlim<Type> Col2 { get; } = new(Type.Type1);
        public ReadOnlyReactivePropertySlim<string?> Col2Disp { get; }
        public ReactivePropertySlim<int> Col3 { get; } = new(0);
        public ReactivePropertySlim<int> Col4 { get; } = new(0);
        public ReactivePropertySlim<bool> Col5 { get; } = new(false);

        public Dictionary<Type, string> TypeList { get; set; }

        public MyItems()
        {

            TypeList = new()
            {
                { Type.Type1, "AAAAAAAAAA" },
                { Type.Type2, "BBBBBBB" },
                { Type.Type3, "CCCC" },
                { Type.Type4, "D" },
            };

            Col2Disp =
                Col2.Select(x => TypeList[x])
                .ToReadOnlyReactivePropertySlim();
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // create a source for the datagrid
        public List<MyItems> DataList { get; set; }

        private bool isRowDragging;
        private MyItems? copySourceItem;

        public MainWindow()
        {

            DataList = new List<MyItems>();
            for(int index = 0; index < 30; index++)
            {
                DataList.Add(new() { Number = index + 1 });
            }

            InitializeComponent();
        }

        private void SelectionChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var dataGrid = sender as DataGrid;
            if (dataGrid == null) return;

            if (copySourceItem is not null)
            {
                var items = dataGrid.SelectedCells;

                foreach (var item in items)
                {
                    if (item.Item is MyItems myItem)
                    {
                        myItem.Col2.Value = copySourceItem.Col2.Value;
                    }
                }
            }
        }

        private void MyGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var dataGrid = sender as DataGrid;
            if (dataGrid == null) return;

            var point = e.GetPosition(dataGrid);
            var cell = GetDataGridCell<DataGridCell>(dataGrid, point);
            if (cell != null)
            {
                var columnIndex = cell.Column.DisplayIndex;

                if (columnIndex == 2)
                {
                    Cursor = Cursors.Hand;

                    isRowDragging = true;
                    //クリック位置取得
                    var row = FindVisualParent<DataGridRow>(e.OriginalSource as DependencyObject);
                    if (row != null)
                    {
                        copySourceItem = row.Item as MyItems;
                    }
                }
            }
        }

        private T GetDataGridCell<T>(DataGrid dataGrid, Point point)
        {
            T result = default(T);
            var hitResultTest = VisualTreeHelper.HitTest(dataGrid, point);
            if (hitResultTest != null)
            {
                var visualHit = hitResultTest.VisualHit;
                while (visualHit != null)
                {
                    if (visualHit is T)
                    {
                        result = (T)(object)visualHit;
                        break;
                    }
                    visualHit = VisualTreeHelper.GetParent(visualHit);
                }
            }
            return result;
        }


        private void MyGrid_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (isRowDragging && e.LeftButton == MouseButtonState.Pressed)
            {
            }
        }

        private T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            if (parentObject == null) return null;

            if (parentObject is T parent)
            {
                return parent;
            }
            else
            {
                return FindVisualParent<T>(parentObject);
            }
        }

        private void MyGrid_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Cursor = null;
            isRowDragging = false;
            copySourceItem = null;
        }

        private void MyGrid_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var dataGrid = sender as DataGrid;
            if (dataGrid == null) return;

            var selectedCells = dataGrid.SelectedCells;
            int startIndex = DataGridRow.GetRowContainingElement(GetCell(selectedCells[0])).GetIndex();
            int endIndex = DataGridRow.GetRowContainingElement(GetCell(selectedCells[selectedCells.Count-1])).GetIndex();

            ContextMenu contextMenu = new ContextMenu();

            MenuItem menuItem = new MenuItem();
            menuItem.Header = $"{startIndex+1}～{endIndex+1}行目の＊＊＊＊を設定する...";
            menuItem.Click += new RoutedEventHandler(ShowSettingDialog);
            contextMenu.Items.Add(menuItem);

            contextMenu.IsOpen = true;
        }

        private static DataGridCell? GetCell(DataGridCellInfo info)
        {
            FrameworkElement cellElement = info.Column.GetCellContent(info.Item);
            if (cellElement != null)
            {
                //DataGridTextColumnの場合はSystem.Windows.Controls.TextBlockが取得される。
                //dataGridCell.Content = TextBlock
                //dataGridCell.DataContent = Bindingされたデータインスタンス
                var dataGridCell = cellElement.Parent as DataGridCell;

                return dataGridCell;
            }

            return null;
        }

        private void ShowSettingDialog(object sender, RoutedEventArgs e)
        {
            var setting = new Setting();
            setting.Owner = this;
            setting.ShowDialog();
        }

        private void MyGrid_Selected(object sender, RoutedEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                // Lookup for the source to be DataGridCell
                if (e.OriginalSource.GetType() == typeof(DataGridCell))
                {
                    // Starts the Edit on the row;
                    DataGrid grd = (DataGrid)sender;
                    grd.BeginEdit(e);
                }
            }
        }

        private void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender != null)
            {
                ComboBox cmb = sender as ComboBox;
                cmb.IsDropDownOpen = true;
            }
        }

        private void ComboBox_DropDownClosed(object sender, EventArgs e)
        {
            MyGrid.CommitEdit();
        }

        private void TextBox_Loaded(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            textBox.Focus();
            textBox.SelectAll();
        }

        private void CheckBox_Loaded(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            checkBox.IsChecked = !checkBox.IsChecked;
        }
    }
}