using System;

namespace TotalCommander.Classes
{
    public abstract class DiscElement
    {

        public string Path { get; set; }
        public DateTime creationDate { get; set; }
        public DiscElement(string path)
        {
            this.Path = path;
        }

        public abstract bool isFile();
        public abstract string getName();
    }
}
