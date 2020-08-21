using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace serialGraph
{
    public class ConfigJson
    {
        public string PortName { get; set; }
        public int BaudRate { get; set; }
        public int DataBits { get; set; }
        public string Parity { get; set; }
        public string StopBits { get; set; }

        public bool IsReceiveHex { get; set; }
        public bool IsDataUpdate { get; set; }
        public bool IsSendHex { get; set; }
        public bool IsSendNewLine { get; set; }

        public List<int> BaudRates { get; set; }

        /// <summary>
        /// 线的缓存长度
        /// </summary>
        public int Length { get; set; }
        /// <summary>
        /// 波形显示高度
        /// </summary>
        public int Height { get; set; }
        public int OX { get; set; }
        public int OY { get; set; }
        public List<GraphConfig> GraphConfigs { get; set; }
    }
}
