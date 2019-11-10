using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TotalCommander.Classes
{
    class MyDirectory :DiscElement
    {
        public bool isBack = false;
        public MyDirectory(string path) : base(path)
        {
            creationDate = Directory.GetCreationTime(path);
        }

        public MyDirectory(string path, bool isBack) : base(path)
        {
            creationDate = Directory.GetCreationTime(path);
            this.isBack = true;
        }

        public override string getName()
        {
            return Path.Substring(Path.LastIndexOf(@"\") + 1);
        }

        public override bool isFile()
        {
            return false;
        }

        public List<DiscElement> GetSubElements()
        {
            List<DiscElement> elements = new List<DiscElement>();
            string[] dirs = null;
            string[] files = null;
            var backPath = Path.Substring(0, Path.LastIndexOf(@"\"));
            if (backPath == "C:") backPath = "C:\\";

            try
            {
                dirs = Directory.GetDirectories(Path);
                files = Directory.GetFiles(Path);
            }
            catch (System.UnauthorizedAccessException)
            {
                Path = backPath;
                throw new Exception("");
            }

            var backDir = new MyDirectory(backPath, true);
            if (backDir.Path != Path)
                elements.Add(backDir);

            foreach (var item in dirs)
            {
                elements.Add(new MyDirectory(item));
            }

            foreach (var item in files)
            {
                elements.Add(new MyFile(item));
            }
            return elements;

        }
        //public List<DiscElement> getGetAllSubDirectories()
        //{
        //    List<DiscElement> elements = new List<DiscElement>();
        //    string[] dirs = Directory.GetDirectories(Path);
        //    foreach (var i in dirs)
        //    {
        //        elements.Add(new MyDirectory(i));

        //    }

        //    return elements;
        //}
    }
}

