using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace serialGraph
{
    public class CommandBinding
    {
        public RelayCommand OpenSerialPort
        {
            get
            {
                return new RelayCommand(OnOpenSerialPort);
            }
        }
        private void OnOpenSerialPort(object parameter)
        {
            //if (OpenSerialIcon == PackIconModernKind.Connect)
            //{
            //    OpenSerialIcon = PackIconModernKind.Disconnect;
            //}
            //else
            //    OpenSerialIcon = PackIconModernKind.Connect;
        }
    }
}
