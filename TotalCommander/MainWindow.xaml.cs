using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using TotalCommander.MainViews;
using TotalCommander.Classes;
using TotalCommander.AdditionalElements;
using System.Windows.Markup;
using Microsoft.Win32;

namespace TotalCommander
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window

    {
        SideView sideLeft;
        SideView sideRight;
        public Operations operation { get; set;}

        private SolidColorBrush fontColor;
        private SolidColorBrush backgroundColor;
        private int fontSize;
        private enum Colors
        {
            Default,
            Red,
            Black,
            White
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            sideLeft = new SideView();
            sideRight = new SideView();
            
            operation = new Operations(sideLeft, sideRight);
            operation.ShowAfterDeleted += RefreshAllList;
           
            leftBorder.Child = sideLeft;
            rightBorder.Child = sideRight;
            Up.Child = operation;
        }

       public MainWindow() :base()
        {
            InitializeComponent();

            var items = stylesMenu.Items;

            var z = Enum.GetValues(typeof(Colors));
            foreach (Colors color in z)
            {
                var newItem = new MenuItem();
                newItem.Header = color;
                newItem.Click += someColorFill_Click;
                items.Add(newItem);
            }
        }

        private void someColorFill_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menu = (MenuItem)sender;
            switch (menu.Header)
            {
                case Colors.Red:
                    fontColor = Brushes.White;
                    backgroundColor = Brushes.Red;
                    break;
                case Colors.Black:
                    fontColor = Brushes.White;
                    backgroundColor = Brushes.Black;
                    break;
                case Colors.White:
                    fontColor = Brushes.Black;
                    backgroundColor = Brushes.White;
                    break;
                case Colors.Default:
                    fontColor = Brushes.Blue;
                    backgroundColor = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFE3F8FF");
                    break;
            }
        }

        public void RefreshAllList()
        {
            this.sideLeft.RefreshList();
            this.sideRight.RefreshList();
        }

        private void applyFontSize_Click(object sender, RoutedEventArgs e)
        {
            bool fuck = false;
            for (int i = 0; i < fontSizeBox.Text.Length; i++)
            {
                if (fontSizeBox.Text[i] < '0' || fontSizeBox.Text[i] > '9')
                {
                    fuck = true;
                    break;
                }
            }

            if (!fuck)
            {
                int size = int.Parse(fontSizeBox.Text);

                if (size < 10 || size > 20)
                {
                    fontSize = 15;
                    sideLeft.changeFont(fontSize);
                    sideRight.changeFont(fontSize);
                    return;
                }

                fontSize = size;
                sideLeft.changeFont(size);
                sideRight.changeFont(size);
            }
        }

        private void applyStyle_Click(object sender, RoutedEventArgs e)
        {
            if (fontColor != null && backgroundColor != null)
            {
                sideLeft.chageColor(fontColor, backgroundColor);
                sideRight.chageColor(fontColor, backgroundColor);
            }
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "dat files (*.dat)|*.dat";
            dialog.ShowDialog();

            string path = dialog.FileName;

            if (path.Equals(""))
            {
                return;
            }

            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                MyStyle style = new MyStyle();
                if (fontColor == null || backgroundColor == null)
                {
                    style.fontColor = Brushes.Blue;
                    style.backgroundColor = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFE3F8FF");
                }
                else
                {
                    style.fontColor = fontColor;
                    style.backgroundColor = backgroundColor;
                }

                if (fontSize < 10 || fontSize > 20)
                {
                    style.fontSize = 15;
                }
                else
                {
                    style.fontSize = fontSize;
                }
                
                string styleStr = XamlWriter.Save(style);
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(styleStr);
                sw.Close();
                fs.Close();
            }
        }

        private void load_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "dat files (*.dat)|*.dat";
            dialog.ShowDialog();

            string path = dialog.FileName;

            if (path.Equals(""))
            {
                return;
            }

            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                StreamReader sr = new StreamReader(fs);
                MyStyle style = (MyStyle)XamlReader.Parse(sr.ReadToEnd());

                sideLeft.chageColor(style.fontColor, style.backgroundColor);
                sideRight.chageColor(style.fontColor, style.backgroundColor);
                sideLeft.changeFont(style.fontSize);
                sideRight.changeFont(style.fontSize);
            }
        }
    }
}
