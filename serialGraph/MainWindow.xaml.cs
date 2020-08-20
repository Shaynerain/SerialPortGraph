using InteractiveDataDisplay.WPF;
using MahApps.Metro.Controls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = DataBinding._DataBinding;

            InitConfig();
            InitUI();
            //GreatConfigJsonFile();

            //设置时间，秒数，精确到最接近的毫秒
            Timer.Interval = TimeSpan.FromSeconds(0.01);
            //设置触发时间  
            Timer.Tick += Timer_Tick;
            Timer.Start();

            DataBinding._DataBinding.ReCount = 10;
        }

        private void InitUI()
        { 

            foreach (var item in SerialPort.GetPortNames())
            {
                SerialPortComboBox.Items.Add(item);
            }
            for (int i = 5; i <= 8; i++)
            {
                DataBitsComboBox.Items.Add(i);
            }

            if (Configs != null)
            {
                foreach (var item in Configs.BaudRates)
                {
                    BaudRateComboBox.Items.Add(item);
                }
                foreach (var item in Enum.GetNames(typeof(Parity)))
                {
                    ParityComboBoxItem.Items.Add(item);
                }
                foreach (var item in Enum.GetNames(typeof(StopBits)))
                {
                    StopBitsComboBoxItem.Items.Add(item);
                }

                InitLines();
                SerialPortComboBox.SelectedItem = Configs.PortName;

                BaudRateComboBox.SelectedItem = Configs.BaudRate;
                DataBitsComboBox.SelectedItem = Configs.DataBits;
                ParityComboBoxItem.SelectedItem = Configs.Parity;
                StopBitsComboBoxItem.SelectedItem = Configs.StopBits;

            }
        }

        DispatcherTimer Timer = new DispatcherTimer(DispatcherPriority.DataBind);


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
                //str = serialPort.ReadLine();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
            Dispatcher.Invoke(new Action(() => { 
                //textBlock.Text = str; 
            }), DispatcherPriority.DataBind);
            
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
                        lock (locker)
                        {
                            line.tempData.Add(double.Parse(value));
                        }
                    }
                }
            }
        }

        private static readonly object locker = new object();
        private bool Pause = false;
        private void Timer_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < Lines.Children.Count; i++)
            {
                if(Lines.Children[i] is LineGraph lineGraph)
                {
                    lock (locker)
                    {
                        Configs.GraphConfigs[i].Data.AddRange(Configs.GraphConfigs[i].tempData);
                        Configs.GraphConfigs[i].tempData.Clear();
                    }
                    Configs.GraphConfigs[i].Data.RemoveRange(0, Configs.GraphConfigs[i].Data.Count - Configs.Length);
                    if(!Pause)
                        lineGraph.Plot(xList, Configs.GraphConfigs[i].Data);
                }
            }
        }
    }

}
