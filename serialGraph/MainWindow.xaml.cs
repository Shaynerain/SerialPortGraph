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
using System.Threading;
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
            DataBinding._DataBinding.ErrorMessage = ErrorMessage;
            InitConfig();
            InitUI();
            //GreatConfigJsonFile();
            //设置时间，秒数，精确到最接近的毫秒
            Timer.Interval = TimeSpan.FromSeconds(0.02);
            //设置触发时间  
            Timer.Tick += Timer_Tick;
            Timer.Start();

        }

        private void ErrorMessage(object e)
        {
            MessageBox.Show(e.ToString());
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

            if (DataBinding._DataBinding.Configs != null)
            {
                foreach (var item in DataBinding._DataBinding.Configs.BaudRates)
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
                SerialPortComboBox.SelectedItem = DataBinding._DataBinding.Configs.PortName;

                BaudRateComboBox.SelectedItem = DataBinding._DataBinding.Configs.BaudRate;
                DataBitsComboBox.SelectedItem = DataBinding._DataBinding.Configs.DataBits;
                ParityComboBoxItem.SelectedItem = DataBinding._DataBinding.Configs.Parity;
                StopBitsComboBoxItem.SelectedItem = DataBinding._DataBinding.Configs.StopBits;

                DataBinding._DataBinding.InitValue();
            }
        }

        DispatcherTimer Timer = new DispatcherTimer(DispatcherPriority.DataBind);

        void InitConfig()
        {
            try
            {
                string f = File.ReadAllText("config.json", Encoding.Default);
                DataBinding._DataBinding.Configs = JsonConvert.DeserializeObject<ConfigJson>(f);
                f = null;
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
            xList.Clear();
            for (int i = 0; i < DataBinding._DataBinding.Configs.Length; i++)
            {
                xList.Add(i);
            }
            Lines.Children.Clear();
            foreach (var line in DataBinding._DataBinding.Configs.GraphConfigs)
            {
                LineGraph lineGraph = new LineGraph();
                lineGraph.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(line.Color));
                lineGraph.Description = line.Name;
                lineGraph.StrokeThickness = line.Thickness;
                line.Data = new List<double>();
                line.tempData = new List<double>();
                for (int i = 0; i < DataBinding._DataBinding.Configs.Length; i++)
                {
                    line.Data.Add(0);
                }
                lineGraph.Plot(xList, line.Data);
                Lines.Children.Add(lineGraph);
            }

            //图像设置
            D3Chart.PlotOriginX = DataBinding._DataBinding.Configs.OX;
            D3Chart.PlotOriginY = DataBinding._DataBinding.Configs.OY;
            D3Chart.PlotWidth = DataBinding._DataBinding.Configs.Length;
            D3Chart.PlotHeight = DataBinding._DataBinding.Configs.Height;
        }

        private bool Pause = false;
        private void Timer_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < Lines.Children.Count; i++)
            {
                if(Lines.Children[i] is LineGraph lineGraph)
                {
                    lock (DataBinding.locker)
                    {
                        DataBinding._DataBinding.Configs.GraphConfigs[i].Data.AddRange(DataBinding._DataBinding.Configs.GraphConfigs[i].tempData);
                        DataBinding._DataBinding.Configs.GraphConfigs[i].tempData.Clear();
                    }
                    DataBinding._DataBinding.Configs.GraphConfigs[i].Data.RemoveRange(0, DataBinding._DataBinding.Configs.GraphConfigs[i].Data.Count - DataBinding._DataBinding.Configs.Length);
                    if(!DataBinding._DataBinding.GraphPause)
                        lineGraph.Plot(xList, DataBinding._DataBinding.Configs.GraphConfigs[i].Data);
                }
            }
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            if (!DataBinding._DataBinding.SerialPort.IsOpen)
            {
                DataBinding._DataBinding.Configs.PortName = SerialPortComboBox.SelectedItem as string;

                if (UserBaudRateTextBox.Visibility != Visibility.Collapsed)
                {
                    int baudrate;
                    if(int.TryParse(UserBaudRateTextBox.Text, out baudrate))
                    {
                        DataBinding._DataBinding.Configs.BaudRate = baudrate;
                        bool isNew = true;
                        foreach (var item in DataBinding._DataBinding.Configs.BaudRates)
                        {
                            if(item == baudrate)
                            {
                                isNew = false;
                                break;
                            }
                        }
                        if (isNew)
                        {
                            DataBinding._DataBinding.Configs.BaudRates.Add(baudrate);
                            DataBinding._DataBinding.Configs.BaudRates.Sort();
                        }
                    }
                    else
                    {
                        UserBaudRateTextBox.Text = 115200.ToString();
                        DataBinding._DataBinding.Configs.BaudRate = 115200;
                        MessageBox.Show("转换失败, 默认设置为115200");
                    }
                }
                else
                {
                    DataBinding._DataBinding.Configs.BaudRate = (int)BaudRateComboBox.SelectedItem;
                }


                DataBinding._DataBinding.Configs.DataBits = (int)DataBitsComboBox.SelectedItem;
                DataBinding._DataBinding.Configs.Parity = ParityComboBoxItem.SelectedItem as string;
                DataBinding._DataBinding.Configs.StopBits = StopBitsComboBoxItem.SelectedItem as string;

                DataBinding._DataBinding.SerialPort.PortName = DataBinding._DataBinding.Configs.PortName;
                DataBinding._DataBinding.SerialPort.BaudRate = DataBinding._DataBinding.Configs.BaudRate;
                DataBinding._DataBinding.SerialPort.DataBits = DataBinding._DataBinding.Configs.DataBits;
                DataBinding._DataBinding.SerialPort.Parity = (Parity)Enum.Parse(typeof(Parity), DataBinding._DataBinding.Configs.Parity);
                DataBinding._DataBinding.SerialPort.StopBits = (StopBits)Enum.Parse(typeof(StopBits), DataBinding._DataBinding.Configs.StopBits);

                DataBinding._DataBinding.WriteConfig();
            }
        }

        private void ResumeButton_Click(object sender, RoutedEventArgs e)
        {
            D3Chart.PlotOriginX = DataBinding._DataBinding.Configs.OX;
            D3Chart.PlotOriginY = DataBinding._DataBinding.Configs.OY;
            D3Chart.PlotWidth = DataBinding._DataBinding.Configs.Length;
            D3Chart.PlotHeight = DataBinding._DataBinding.Configs.Height;
        }

        private void ReceiveTextChanged(object sender, TextChangedEventArgs e)
        {
            if (ReceiveTextBox.LineCount > 0)
                ReceiveTextBox.ScrollToLine(ReceiveTextBox.LineCount-1);
            if (ReceiveTextBox.LineCount > 1000000000) DataBinding._DataBinding.DataReceive = null;
        }

        private void SettingGraph_Click(object sender, RoutedEventArgs e)
        {
            if(DataBinding._DataBinding.Configs !=null)
            {
                GraphComfigs graphComfigs = new GraphComfigs(DataBinding._DataBinding.Configs);
                if (graphComfigs.ShowDialog() == true)
                {
                    //确定后返回,重新设置图像
                    Timer.Stop();
                    DataBinding._DataBinding.SerialPort.DataReceived -= DataBinding._DataBinding.SerialPort_DataReceived;
                    Thread.Sleep(20);
                    DataBinding._DataBinding.Configs.Length = graphComfigs.NewConfigs.Length;
                    DataBinding._DataBinding.Configs.Height = graphComfigs.NewConfigs.Height;
                    DataBinding._DataBinding.Configs.OX = graphComfigs.NewConfigs.OX;
                    DataBinding._DataBinding.Configs.OY = graphComfigs.NewConfigs.OY;
                    lock (DataBinding.locker)
                    {
                        DataBinding._DataBinding.Configs.GraphConfigs.Clear();
                        foreach (var item in graphComfigs.NewConfigs.GraphConfigs)
                        {
                            DataBinding._DataBinding.Configs.GraphConfigs.Add(item);
                        }
                    }
                    InitLines();
                    DataBinding._DataBinding.WriteConfig();

                    Timer.Start();
                    DataBinding._DataBinding.SerialPort.DataReceived += DataBinding._DataBinding.SerialPort_DataReceived;
                }
            }
        }


        private void UserBaudRate_Click(object sender, RoutedEventArgs e)
        {

            if ((bool)UserBaudRateButton.IsChecked)
            {
                UserBaudRateTextBox.Visibility = Visibility.Visible;
                BaudRateComboBox.Visibility = Visibility.Collapsed;
            }
            else
            {
                UserBaudRateTextBox.Visibility = Visibility.Collapsed;
                BaudRateComboBox.Visibility = Visibility.Visible;
            }
        }
    }

}
