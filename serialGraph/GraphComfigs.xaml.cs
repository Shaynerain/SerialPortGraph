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
    /// GraphComfigs.xaml 的交互逻辑
    /// </summary>
    public partial class GraphComfigs : MetroWindow
    {
        ConfigJson ConfigJson;
        public GraphComfigs(ConfigJson configJson)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ConfigJson = configJson;
            InitUI();
        }

        private void InitUI()
        {
            if(ConfigJson != null)
            {
                XWidthTextBox.Text = ConfigJson.Length.ToString();
                YWidthTextBox.Text = ConfigJson.Height.ToString();
                OYTextBox.Text = ConfigJson.OY.ToString();
                OXTextBox.Text = ConfigJson.OX.ToString();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            new SelecteColor(SelecteButton).ShowDialog();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
