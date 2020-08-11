using InteractiveDataDisplay.WPF;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace serialGraph
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            InitConfig();

            //GreatConfigJsonFile();

            //设置时间，秒数，精确到最接近的毫秒
            Timer.Interval = TimeSpan.FromSeconds(0.01);
            //设置触发时间  
            Timer.Tick += Timer_Tick;
            Timer.Start();
        }

        DispatcherTimer Timer = new DispatcherTimer();
        SerialPort serialPort = new SerialPort();

        ConfigJson Configs = null;

        void InitConfig()
        {
            try
            {
                string f = File.ReadAllText("config.json", Encoding.Default);
                Configs = JsonConvert.DeserializeObject<ConfigJson>(f);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                Close();
            }
            if (Configs != null)
            {
                InitLines();

                serialPort.PortName = Configs.PortName;
                serialPort.BaudRate = Configs.BaudRate;
                serialPort.ReadTimeout = 500;
                serialPort.DataReceived -= SerialPort_DataReceived;
                serialPort.DataReceived += SerialPort_DataReceived;
                try
                {
                    serialPort.Open();
                }
                catch(Exception e)
                {
                    MessageBox.Show(e.ToString());
                    Close();
                }

                //if (serialPort.IsOpen())
                //{
                //}
            }
        }
        private List<int> xList = new List<int>();
        private void InitLines()
        {
            for (int i = 0; i < Configs.Length; i++)
            {
                xList.Add(i);
            }
            foreach (var line in Configs.GraphConfigs)
            {
                LineGraph lineGraph = new LineGraph();
                lineGraph.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(line.Color));
                lineGraph.Description = line.Name;
                lineGraph.StrokeThickness = line.Thickness;
                if (line.Visibility)
                    lineGraph.Visibility = Visibility.Visible;
                else
                    lineGraph.Visibility = Visibility.Hidden;
                line.Data = new List<double>();
                line.tempData = new List<double>();
                for (int i = 0; i < Configs.Length; i++)
                {
                    line.Data.Add(0);
                }
                lineGraph.Plot(xList, line.Data);
                Lines.Children.Add(lineGraph);
            }
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string str = null;
            try
            {
                str = serialPort.ReadLine();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
            Dispatcher.Invoke(new Action(() => { 
                textBlock.Text = str; 
            }));
            
            foreach (var line in Configs.GraphConfigs)
            {
                if (str.Contains(line.Name))
                {
                    int index = str.IndexOf('=');
                    int flag = str.IndexOf(';');
                    if (index > -1 && flag > -1)
                    {
                        string value;
                        value = str.Substring(index + 1,flag - index - 1);
                        str = str.Substring(flag + 1);
                        line.tempData.Add(double.Parse(value));
                    }
                }
            }

        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < Lines.Children.Count; i++)
            {
                if(Lines.Children[i] is LineGraph lineGraph)
                {
                    Configs.GraphConfigs[i].Data.AddRange(Configs.GraphConfigs[i].tempData);
                    Configs.GraphConfigs[i].tempData.Clear();
                    Configs.GraphConfigs[i].Data.RemoveRange(0, Configs.GraphConfigs[i].Data.Count - Configs.Length);
                    lineGraph.Plot(xList, Configs.GraphConfigs[i].Data);
                }
            }
        }

        private void D3Chart_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //自动填充切换
            D3Chart.IsAutoFitEnabled = !D3Chart.IsAutoFitEnabled;

            if(!D3Chart.IsAutoFitEnabled)
            {
                D3Chart.PlotHeight = 1000;
                D3Chart.PlotWidth = 1000;
            }

        }
    }

    public class ConfigJson
    {
        public string PortName { get; set; }
        public int BaudRate { get; set; }

        /// <summary>
        /// 线的缓存长度
        /// </summary>
        public int Length { get; set; }
        public List<GraphConfig> GraphConfigs { get; set; }
    }

    public class GraphConfig
    {
        //线的名字
        public string Name { get; set; }
        //public int Address { get; set; }

        public string Color { get; set; }
        public int Thickness { get; set; }
        public int Factory { get; set; }
        public int OffSet { get; set; }
        public bool Visibility { get; set; }
        public List<double> Data { get; set; }
        public List<double> tempData { get; set; }
    }

    public class VisibilityToCheckedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((Visibility)value) == Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((bool)value) ? Visibility.Visible : Visibility.Collapsed;
        }
    }

}
