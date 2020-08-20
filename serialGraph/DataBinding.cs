using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.IconPacks;
using System;
using System.Collections.Generic;
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


    }
}
