using InteractiveDataDisplay.WPF;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.IconPacks;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace serialGraph
{
    public class DataBinding: ObservableObject
    {
        public static DataBinding _DataBinding = new DataBinding();

        #region 串口设置

        public SerialPort SerialPort = new SerialPort();


        public ConfigJson Configs = null;


        private PackIconModernKind _openSerialIcon = PackIconModernKind.Connect;

        public PackIconModernKind OpenSerialIcon
        {
            get { return _openSerialIcon; }
            set
            {
                _openSerialIcon = value;
                OnPropertyChanged("OpenSerialIcon");
            }
        }

        private string _openSerialString = "打开串口";

        public string OpenSerialString
        {
            get { return _openSerialString; }
            set
            {
                _openSerialString = value;
                OnPropertyChanged("OpenSerialString");
            }
        }


        public RelayCommand OpenSerialPort
        {
            get
            {
                return new RelayCommand(OnOpenSerialPort);
            }
        }
        Action<object> ErrorMessage;
        private void OnOpenSerialPort(object parameter)
        {
            //已经打开，现在关闭
            if (SerialPort.IsOpen)
            {
                SerialPort.Close();
            }
            else
            {
                try
                {
                    SerialPort.DataReceived -= SerialPort_DataReceived;
                    SerialPort.DataReceived += SerialPort_DataReceived;
                    SerialPort.Open();
                }
                catch(Exception e)
                {
                    ErrorMessage?.Invoke(e);
                }
            }

            if (SerialPort.IsOpen)
            {
                OpenSerialString = "关闭串口";
                OpenSerialIcon = PackIconModernKind.Disconnect;
            }
            else
            {
                OpenSerialIcon = PackIconModernKind.Connect;
                OpenSerialString = "打开串口";
            }
        }

        #endregion

        private string _dataReceived;

        public string DataReceive
        {
            get { return _dataReceived; }
            set { _dataReceived = value;
                OnPropertyChanged("DataReceive");
            }
        }


        public static readonly object locker = new object();
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string str = null;
            try
            {
                str = SerialPort.ReadLine();
            }
            catch (Exception ex)
            {
                ErrorMessage?.Invoke(ex);
                return;
            }
            DataReceive += str;
            ReCount += str.Length;

            foreach (var line in Configs.GraphConfigs)
            {
                if (str.Contains(line.Name))
                {
                    int index = str.IndexOf('=');
                    int flag = str.IndexOf(';');
                    if (index > -1 && flag > -1)
                    {
                        string value;
                        value = str.Substring(index + 1, flag - index - 1);
                        str = str.Substring(flag + 1);
                        lock (locker)
                        {
                            line.tempData.Add(double.Parse(value));
                        }
                    }
                }
            }
        }


        private int _sendCount = 0;

        public int SendCount
        {
            get { return _sendCount; }
            set
            {
                _sendCount = value;
                OnPropertyChanged("SendCount");
            }
        }
        private int _reCount = 0;

        public int ReCount
        {
            get { return _reCount; }
            set
            {
                _reCount = value;
                OnPropertyChanged("ReCount");
            }
        }

        private bool _isPause = false;

        public bool GraphPause
        {
            get { return _isPause; }
            set { _isPause = value;
                OnPropertyChanged("GraphPause");
            }
        }

        public RelayCommand DeleteCommand
        {
            get
            {
                return new RelayCommand(OnDelete);
            }
        }
        private void OnDelete(object parameter)
        {
            List<double> temp = new List<double>();
            for (int i = 0; i < Configs.Length; i++)
            {
                temp.Add(0);
            }
            foreach (var line in Configs.GraphConfigs)
            {
                lock (locker)
                {
                    line.tempData.AddRange(temp);
                }
            }
        }

        public RelayCommand ClearTextCommand
        {
            get
            {
                return new RelayCommand(OnClearText);
            }
        }
        private void OnClearText(object parameter)
        {
            DataReceive = null;
        }

        public RelayCommand ClearCountCommand
        {
            get
            {
                return new RelayCommand(OnClearCount);
            }
        }
        private void OnClearCount(object parameter)
        {
            SendCount = 0;
            ReCount = 0;
        }

    }
}
