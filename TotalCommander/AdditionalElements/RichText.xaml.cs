using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;
using TotalCommander.Classes;

namespace TotalCommander.AdditionalElements
{
    /// <summary>
    /// Interaction logic for RichText.xaml
    // / </summary>
    public partial class RichText : Window
    {
        private string path;

        public delegate void SaveTextFileEventHandler();
        public event SaveTextFileEventHandler SaveTextFile;

        protected virtual void onSaveTextFile()
        {
            if (SaveTextFile != null)
            {
                SaveTextFile.Invoke();
            }
        }

        public RichText(string txtFilePath, string name)
        {
            InitializeComponent();
            path = txtFilePath;
            string text = System.IO.File.ReadAllText(txtFilePath);
            richTextBox.AppendText(text);
            nameFileTxt.Content = name;
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            string richText = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd).Text;
            if (richText.Length > 2)
            {
                int x1 = 0;
                x1 = richText.Length - 2;
                richText = richText.Substring(0, x1);
            }

            StreamWriter sw = File.CreateText(path);
            sw.Write(richText);
            sw.Close();
            onSaveTextFile();
            this.Close();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}