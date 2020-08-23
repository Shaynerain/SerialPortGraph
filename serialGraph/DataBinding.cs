using InteractiveDataDisplay.WPF;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.IconPacks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace serialGraph
{
    public class DataBinding: ObservableObject
    {
        public static DataBinding _DataBinding = new DataBinding();

        public SerialPort SerialPort = new SerialPort();

        public Action<object> ErrorMessage;

        public ConfigJson Configs = null;
        public bool isInit = false;
        public void InitValue()
        {
            IsReceiveHex = Configs.IsReceiveHex;
            IsDataUpdate = Configs.IsDataUpdate;
            IsSendHex = Configs.IsSendHex;
            IsSendNewLine = Configs.IsSendNewLine;
            isInit = true;
        }

        #region 串口设置



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


        #region 串口接收区

        private string _dataReceived = "";

        public string DataReceive
        {
            get { return _dataReceived; }
            set
            {
                _dataReceived = value;
                OnPropertyChanged("DataReceive");
            }
        }

        private bool _isReceiveHex = false;

        public bool IsReceiveHex
        {
            get { return _isReceiveHex; }
            set { _isReceiveHex = value;
                OnPropertyChanged("IsReceiveHex");
            }
        }
        private bool _isDataUpdate = false;

        public bool IsDataUpdate
        {
            get { return _isDataUpdate; }
            set { _isDataUpdate = value;
                OnPropertyChanged("IsDataUpdate");
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
            ReCount = 0;
        }
        #endregion


        #region 串口发送区

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

        private bool _isSendHex = false;

        public bool IsSendHex
        {
            get { return _isSendHex; }
            set { _isSendHex = value;
                OnPropertyChanged("IsSendHex");
            }
        }

        private bool isSendNewLine;

        public bool IsSendNewLine
        {
            get { return isSendNewLine; }
            set { isSendNewLine = value;
                OnPropertyChanged("IsSendNewLine");
            }
        }

        /// <summary>
        /// 发送改变编码
        /// </summary>
        public RelayCommand ChangeEncodingCommand
        {
            get
            {
                return new RelayCommand(OnChangeEncoding);
            }
        }
        private void OnChangeEncoding(object parameter)
        {
            if (SendDataText == null) return;
            //char -> hex
            if (IsSendHex)
            {
                char[] cd = SendDataText.ToArray();
                string sd = null;
                try
                {
                    foreach (var item in cd)
                    {
                        sd += Convert.ToString(item, 16).ToUpper() + ' ';
                    }
                    SendDataText = sd;
                }
                catch (Exception e)
                {
                    ErrorMessage?.Invoke(e);
                }
            }
            //hex -> char
            else
            {
                string[] sdata = SendDataText.Split(' ');
                string data = null;
                try
                {
                    foreach (var item in sdata)
                    {
                        if (item != "")
                            data += Convert.ToChar(Convert.ToByte(item, 16));
                    }
                    SendDataText = data;
                }
                catch (Exception e)
                {
                    ErrorMessage?.Invoke(e);
                }
            }
            OnChanged(parameter);
        }

        /// <summary>
        /// 清空计数
        /// </summary>
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

        private string _sendDataText=null;
        /// <summary>
        /// 发送区域显示
        /// </summary>
        public string SendDataText
        {
            get { return _sendDataText; }
            set { _sendDataText = value;
                OnPropertyChanged("SendDataText");
            }
        }

        /// <summary>
        /// 发送命令
        /// </summary>
        public RelayCommand SendDataComand
        {
            get
            {
                return new RelayCommand(OnSendData);
            }
        }
        private void OnSendData(object parameter)
        {
            string data = SendDataText;
            if (IsSendHex)
            {
                string[] sdata = data.Split(' ');
                List<byte> vs = new List<byte>();
                foreach (var item in sdata)
                {
                    if(item!="")
                        vs.Add(Convert.ToByte(item,16));
                }

                if (isSendNewLine)
                {
                    vs.Add((byte)'\r');
                    vs.Add((byte)'\n');
                }
                if (SerialPort.IsOpen)
                {
                    try
                    {
                        SerialPort.Write(vs.ToArray(),0,vs.Count);
                    }
                    catch (Exception e)
                    {
                        ErrorMessage?.Invoke(e);
                    }
                }
            }
            else
            {
                if (isSendNewLine)
                    data += "\r\n";
                if (SerialPort.IsOpen)
                {
                    try
                    {
                        SerialPort.Write(data);
                    }
                    catch (Exception e)
                    {
                        ErrorMessage?.Invoke(e);
                    }
                }
            }
        }

        #endregion

        #region 串口数据处理

        public static readonly object locker = new object();

        //判断是否波形格式，不能大于512
        string str_temp = null;
        /// <summary>
        /// 数据接收中断
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int count = SerialPort.BytesToRead;
            string str = null;
            if (IsReceiveHex)
            {
                byte[] data = new byte[count];
                try
                {
                    SerialPort.Read(data, 0, count);
                }
                catch (Exception ex)
                {
                    ErrorMessage?.Invoke(ex);
                    return;
                }

                foreach (var item in data)
                {
                    str += Convert.ToString(item, 16).ToUpper() + ' ';
                    str_temp += ((char)item).ToString();
                }
            }
            else
            {
                char[] data = new char[count];
                try
                {
                    SerialPort.Read(data, 0, count);
                }
                catch (Exception ex)
                {
                    ErrorMessage?.Invoke(ex);
                    return;
                }

                foreach (var item in data)
                {
                    str += item.ToString();
                }

                str_temp += str;
            }
            if(!IsDataUpdate)
                DataReceive += str;
            ReCount += str.Length;
            if (str_temp.Contains("\r\n"))
            {
                foreach (var line in Configs.GraphConfigs)
                {
                    if (str_temp.Contains(line.Name))
                    {
                        int index = str_temp.IndexOf('=');
                        int flag = str_temp.IndexOf(';');
                        if (index > -1 && flag > -1)
                        {
                            string value;
                            value = str_temp.Substring(index + 1, flag - index - 1);
                            str_temp = str_temp.Substring(flag + 1);
                            lock (locker)
                            {
                                line.tempData.Add(double.Parse(value));
                            }
                        }
                    }
                }
                int rn = str_temp.IndexOf("\r\n");
                if(rn>-1)
                    str_temp = str_temp.Substring(0, rn + 2);

            }
            if (str_temp.Length > 512) str_temp = null;
        }
        #endregion


        #region 波形显示

        private bool _isPause = false;
        /// <summary>
        /// 波形暂停
        /// </summary>
        public bool GraphPause
        {
            get { return _isPause; }
            set
            {
                _isPause = value;
                OnPropertyChanged("GraphPause");
            }
        }

        /// <summary>
        /// 波形清空指令
        /// </summary>
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
        #endregion



        public RelayCommand ChangedCommand
        {
            get { return new RelayCommand(OnChanged,(p) => { return isInit; }); }
        }
        private void OnChanged(object parameter)
        {
            WriteConfig();
        }

        public void WriteConfig()
        {
            Configs.IsReceiveHex = IsReceiveHex;
            Configs.IsDataUpdate = IsDataUpdate;
            Configs.IsSendHex = IsSendHex;
            Configs.IsSendNewLine = IsSendNewLine;
            string f = JsonConvert.SerializeObject(Configs);
            File.WriteAllText("config.json", f, Encoding.Default);
        }
    }
}
