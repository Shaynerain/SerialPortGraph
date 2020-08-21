using MahApps.Metro.Controls;
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
using System.Windows.Shapes;

namespace serialGraph
{
    /// <summary>
    /// SelecteColor.xaml 的交互逻辑
    /// </summary>
    public partial class SelecteColor : MetroWindow
    {
        private Button _button;
        public SelecteColor(Button button)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            _button = button;
            InitColors();
        }

        private void InitColors()
        {
            ColorsListBox.Items.Clear();
            var v = typeof(Brushes).GetProperties();
            foreach (var item in v)
            {
                Grid grid = new Grid();
                grid.Margin = new Thickness(1);
                Label label = new Label();
                label.Content = item.Name;
                HorizontalAlignment = HorizontalAlignment.Left;
                Grid grid1 = new Grid();
                grid1.Width = 150;
                grid1.HorizontalAlignment = HorizontalAlignment.Right;
                grid1.Background = (SolidColorBrush)item.GetValue(item);
                grid.Children.Add(label);
                grid.Children.Add(grid1);
                ColorsListBox.Items.Add(grid);
            }
        }
        private void ColorsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var v = typeof(Brushes).GetProperties();
            _button.Background = (SolidColorBrush)v[ColorsListBox.SelectedIndex].GetValue(v[ColorsListBox.SelectedIndex]);
            DialogResult = true;
        }
    }
}
