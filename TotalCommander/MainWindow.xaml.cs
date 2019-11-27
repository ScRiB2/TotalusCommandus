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
            operation.ShowAfterNameSorted += RefreshAfterNameSort;
            operation.ShowAfterDateSorted += RefreshAfterDateSort;
           
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

         public void RefreshAfterNameSort(string a)
        {
            if (a == "left")
            {
                this.sideLeft.RefreshList();
                sideLeft.sortByName();

            }
            else {
                this.sideRight.RefreshList();
                sideRight.sortByName(); }
        }
        public void RefreshAfterDateSort(string a)
        {
         
            if (a == "left")
            {
                this.sideLeft.RefreshList();
                sideLeft.sortByDate();

            }
            else
            {
                this.sideRight.RefreshList();
                sideRight.sortByDate();
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
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
                    sideLeft.changeFont(15);
                    sideRight.changeFont(15);
                }

                sideLeft.changeFont(size);
                sideRight.changeFont(size);
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (fontColor != null && backgroundColor != null)
            {
                sideLeft.chageColor(fontColor, backgroundColor);
                sideRight.chageColor(fontColor, backgroundColor);
            }
        }
    }
}
