﻿using Reactive.Bindings;

using System.Diagnostics;
using System.Reactive.Linq;
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
        public ReactivePropertySlim<int> Col1 { get; } = new(0);
        public ReactivePropertySlim<Type> Col2 { get; } = new(Type.Type1);
        public ReadOnlyReactivePropertySlim<string?> Col2Disp { get; }
        public ReactivePropertySlim<int> Col3 { get; } = new(0);
        public ReactivePropertySlim<int> Col4 { get; } = new(0);

        public Dictionary<Type, string> TypeList { get; set; }

        public MyItems()
        {

            TypeList = new()
            {
                { Type.Type1, "Type 1" },
                { Type.Type2, "Type 2" },
                { Type.Type3, "Type 3" },
                { Type.Type4, "Type 4" },
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
        private MyItems copySourceItem;

        public MainWindow()
        {

            DataList = new List<MyItems>();
            for(int index = 0; index < 30; index++)
            {
                DataList.Add(new());
            }

            InitializeComponent();
        }

        private void SelectionChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var items = MyGrid.SelectedCells;

            foreach(var item in items)
            {
                if(item.Item is MyItems myItem)
                {
                    myItem.Col2.Value = copySourceItem.Col2.Value;
                }
            }
        }

        private void MyGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isRowDragging = true;
            //クリック位置取得
            var row = FindVisualParent<DataGridRow>(e.OriginalSource as DependencyObject);
            if (row != null)
            {
                copySourceItem = row.Item as MyItems;
            }
        }

        private void MyGrid_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (isRowDragging && e.LeftButton == MouseButtonState.Pressed)
            {
                Cursor = Cursors.Hand;
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
        }
    }
}