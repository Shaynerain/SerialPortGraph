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

            //InitJsonFile();


            //设置时间，秒数，精确到最接近的毫秒，0.5就是500ms  
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
            }
            if (Configs != null)
            {
                serialPort.PortName = Configs.PortName;
                serialPort.BaudRate = Configs.BaudRate;
                serialPort.DataReceived -= SerialPort_DataReceived;
                serialPort.DataReceived += SerialPort_DataReceived;
                try
                {
                    serialPort.Open();
                }
                catch(Exception e)
                {
                    MessageBox.Show(e.ToString());
                }


                //if (serialPort.IsOpen())
                //{
                //}
            }
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int count = serialPort.BytesToRead;
            byte[] rbuf = new byte[count];
            serialPort.Read(rbuf, 0, count);
        }

        void InitJsonFile()
        {
            Configs = new ConfigJson();
            Configs.BaudRate = 1000000;
            Configs.PortName = "COM1";
            Configs.GraphConfigs = new List<GraphConfig>();
            Configs.GraphConfigs.Add(new GraphConfig() { Color = "#FF3333", Factory = 1, OffSet = 0, Thickness = 1, Visibility = true });
            Configs.GraphConfigs.Add(new GraphConfig() { Color = "#FF3333", Factory = 1, OffSet = 0, Thickness = 1, Visibility = true });
            string f = JsonConvert.SerializeObject(Configs);
            File.WriteAllText("config.json", f, Encoding.Default);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {


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
        //public string Name { get; set; }
        //public int Address { get; set; }
        public string Color { get; set; }
        public int Thickness { get; set; }
        public int Factory { get; set; }
        public int OffSet { get; set; }
        public bool Visibility { get; set; }
        public List<double> Data { get; set; }
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
