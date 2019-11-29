using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace TotalCommander.Classes
{
    [Serializable]
    public class MyStyle
    {
        public MyStyle() { }

        public SolidColorBrush fontColor { get; set; }
        public SolidColorBrush backgroundColor { get; set; }
        public int fontSize { get; set; }
    }
}
