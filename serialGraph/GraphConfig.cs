using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace serialGraph
{
    public class GraphConfig
    {
        //线的名字
        public string Name { get; set; }
        public string Color { get; set; }
        public double Thickness { get; set; }
        public double Factory { get; set; }
        public double OffSet { get; set; }
        public List<double> Data { get; set; }
        public List<double> tempData { get; set; }
    }
}
