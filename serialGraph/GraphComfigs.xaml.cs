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
        public ConfigJson NewConfigs;
        public GraphComfigs(ConfigJson configJson)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ConfigJson = configJson;
            if (ConfigJson != null)
            {
                InitConfig();
                InitUI();
            }
            else DialogResult = false;
        }
        
        private void InitConfig()
        {
            NewConfigs = new ConfigJson();
            NewConfigs.GraphConfigs = new List<GraphConfig>();

            NewConfigs.Height = ConfigJson.Height;
            NewConfigs.Length = ConfigJson.Length;
            NewConfigs.OX = ConfigJson.OX;
            NewConfigs.OY = ConfigJson.OY;

            foreach (var item in ConfigJson.GraphConfigs)
            {
                NewConfigs.GraphConfigs.Add(item);
            }
            //ConfigJson.GraphConfigs.Clear();
        }

        private void InitUI()
        {
            //坐标
            XWidthTextBox.Text = ConfigJson.Length.ToString();
            YWidthTextBox.Text = ConfigJson.Height.ToString();
            OYTextBox.Text = ConfigJson.OY.ToString();
            OXTextBox.Text = ConfigJson.OX.ToString();

            //波形
            GraphListBox.Items.Clear();
            foreach (var item in ConfigJson.GraphConfigs)
            {
                AddGraph(item);
            }
            
        }

        private void AddGraph(GraphConfig graphConfig)
        {
            if (graphConfig != null)
            {
                Grid grid = new Grid();
                int[] width = { 1, 1, 1, 1, 2 };
                foreach (var item in width)
                {
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(item, GridUnitType.Star) });
                }
                grid.Children.Add(new Label() { Margin = new Thickness(1), Content = graphConfig.Name, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center });
                grid.Children.Add(new Label() { Margin = new Thickness(1), Content = graphConfig.OffSet, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center });
                grid.Children.Add(new Label() { Margin = new Thickness(1), Content = graphConfig.Factory, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center });
                grid.Children.Add(new Label() { Margin = new Thickness(1), Content = graphConfig.Thickness, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center });
                grid.Children.Add(new Grid() { Margin = new Thickness(5), Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(graphConfig.Color)) });
                for (int i = 0; i < grid.Children.Count; i++)
                {
                    Grid.SetColumn(grid.Children[i], i);
                }
                GraphListBox.Items.Add(grid);
            }
        }

        private void ColorButton_Click(object sender, RoutedEventArgs e)
        {
            new SelecteColor(ColorButton).ShowDialog();
        }

        private void AddLine()
        {
            foreach (var item in NewConfigs.GraphConfigs)
            {
                if(item.Name == NameTextBox.Text)
                {
                    MessageBox.Show("该名称已经存在");
                    return;
                }
            }
            GraphConfig graphConfig = new GraphConfig();
            double offSet = 0, factory=0, thickness=0;
            graphConfig.Name = NameTextBox.Text;
            if (double.TryParse(OffSetTextBox.Text, out offSet))
                graphConfig.OffSet = offSet;
            else 
                graphConfig.OffSet = 0;
            if (double.TryParse(FactoryTextBox.Text, out factory))
                graphConfig.Factory = factory;
            else
                graphConfig.Factory = 1;
            if (double.TryParse(ThicknessTextBox.Text, out thickness))
                graphConfig.Thickness = thickness;
            else
                graphConfig.Thickness = 1;
            graphConfig.Color = ColorButton.Background.ToString();

            NewConfigs.GraphConfigs.Add(graphConfig);
            AddGraph(graphConfig);
        }
        private void DeleteLine()
        {
            var item = GraphListBox.SelectedItem;
            if(item is Grid grid)
            {
                Label label = grid.Children[0] as Label;
                foreach (var line in NewConfigs.GraphConfigs)
                {
                    if(line.Name == label.Content as string)
                    {
                        NewConfigs.GraphConfigs.Remove(line);
                        break;
                    }
                }
                GraphListBox.Items.Remove(item);
            }

        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddLine();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteLine();
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            int Length, Height, OX, OY;

            if (int.TryParse(XWidthTextBox.Text, out Length))
                NewConfigs.Length = Length;
            else
                NewConfigs.Length = 1000;

            if (int.TryParse(YWidthTextBox.Text, out Height))
                NewConfigs.Height = Height;
            else
                NewConfigs.Height = 1000;

            if (int.TryParse(OXTextBox.Text, out OX))
                NewConfigs.OX = OX;
            else
                NewConfigs.OX = 1000;

            if (int.TryParse(OYTextBox.Text, out OY))
                NewConfigs.OY = OY;
            else
                NewConfigs.OY = 1000;

            DialogResult = true;
        }

    }
}
