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

namespace TotalCommander.AdditionalElements
{
    /// <summary>
    /// Логика взаимодействия для RenamePanel.xaml
    /// </summary>
    public partial class RenamePanel : Window
    {
        public delegate void RenameEventHandler();
        public event RenameEventHandler RenameObject;

        protected virtual void onRename()
        {
            if (RenameObject != null)
            {
                RenameObject.Invoke();
            }
        }

        public string path { get; }
        public string sourcePath { get; }
        public string fileName { get; }
        public bool isFile { get; }
        public RenamePanel(string path, string sourcePath, bool isFile, string fileName)
        {
            InitializeComponent();
            this.path = path;
            this.sourcePath = sourcePath;
            this.fileName = fileName;
            this.isFile = isFile;
            if (isFile)
                newNameBox.Text = fileName.Substring(0, fileName.LastIndexOf("."));
            else newNameBox.Text = fileName;
        }

        private void Rename(object sender, RoutedEventArgs e)
        {

            string newFileName = newNameBox.Text;

            if (newFileName.Length < 1)
            {
                MessageBox.Show("Имя не может быть пустым");
                return;
            }
            if (isFile)
            {
                string[] nameWithType = fileName.Split('.');
                var type = nameWithType[nameWithType.Length - 1];

                if (newFileName + "." + type == fileName)
                {
                    this.Close();
                    return;
                }

                string newPath = sourcePath + "\\" + newFileName + "." + type;
                if (!File.Exists(newPath))
                {
                    File.Move(path, newPath);
                    onRename();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Файл с указанным именем уже существует");
                }
            }
            else
            {
                if (newFileName == fileName)
                {
                    this.Close();
                    return;
                }

                string newPath = sourcePath + "\\" + newFileName;
                if (!Directory.Exists(newPath))
                {
                    Directory.Move(path, newPath);
                    onRename();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Папка с указанным именем уже существует");
                }
            }
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
