using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TotalCommander.Classes
{
    class PathToMove
    {
        public string To
        { get; set; }

        public string From
        { get; set; }

        public bool isFile
        { get; set; }
        public PathToMove(string from, string to, bool isFile)
        {
            To = to;
            From = from;
            this.isFile = isFile;
        }
    }
}
