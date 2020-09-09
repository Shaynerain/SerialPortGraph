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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace serialGraph
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        public UserControl1()
        {
            InitializeComponent();
            ShowText.Text =
                "串口波形显示工具\r\n\r\n" +
                "写该项目初衷：串口发送上来的数据进行波形显示，windows下有个串口助手带有这个功能，然而他收费，所以有了该项目\r\n\r\n\r\n" +
                "项目地址：https://github.com/Shaynerain/SerialPortGraph\r\n\r\n" +
                "下载地址：https://github.com/Shaynerain/SerialPortGraph/releases\r\n\r\n" +
                "使用帮助：https://github.com/Shaynerain/SerialPortGraph/blob/master/README.md";
        }
    }
}
