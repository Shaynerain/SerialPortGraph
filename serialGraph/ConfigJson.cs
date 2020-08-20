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


        public List<int> BaudRates { get; set; }

        /// <summary>
        /// 线的缓存长度
        /// </summary>
        public int Length { get; set; }
        public int Height { get; set; }

        public List<GraphConfig> GraphConfigs { get; set; }
    }
}
