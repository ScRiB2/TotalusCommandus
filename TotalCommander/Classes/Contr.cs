using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TotalCommander.Classes
{
    
    class Contr
    {
        DiscElement el;
        public Contr(DiscElement el)
        {
            this.el = el;
            Name = el.getName();
            Path = el.Path;
            isFile = el.isFile();
            CreationDate = el.creationDate;
            if (el is MyDirectory)
            {
                MyDirectory temp = (MyDirectory)el;
                Type = "<DIR>";
                Size = "";
                if (temp.isBack)
                {
                    Name = "..";
                    Type = "";
                    Size = "";
                    CreationDate = null;
                }
            }

            else
            {
                MyFile temp = (MyFile)el;
                Type = temp.extension;
                Size = temp.size.ToString();
                IntSize = temp.size;
            }
        }


        public string Name { get; set; }
        public string Type { get; set; }
        public Nullable<DateTime> CreationDate { get; set; }
        public string Size { get; set; }
        public double IntSize { get; set; }
        public string Path { get; set; }
        public bool isFile { get; set; }
        public string Extension { get; set; }
        public DiscElement Ele
        {
            get
            {
                return el;
            }
        }
    }
}
