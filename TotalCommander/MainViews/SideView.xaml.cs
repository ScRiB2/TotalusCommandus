﻿using System;
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
using TotalCommander.Classes;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.ComponentModel;
using TotalCommander.AdditionalElements;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace TotalCommander.MainViews
{
    /// <summary>
    /// Interaction logic for SideView.xaml
    /// </summary>
    public partial class SideView : UserControl
    {
        public delegate void RefreshAllListEventHandler();
        public event RefreshAllListEventHandler RefreshAllList;

        protected virtual void onRefreshAllList()
        {
            if (RefreshAllList != null)
            {
                RefreshAllList.Invoke();
            }
        }

        public SideView()
        {
            InitializeComponent();

        }

        public void chageColor(SolidColorBrush fontColor, SolidColorBrush backgroundColor)
        {
            this.listView.Foreground = fontColor;
            this.listView.Background = backgroundColor;
        }

        public void changeFont(int fontSize)
        {
            this.listView.FontSize = fontSize;
        }

        public bool isActive { get; set; }
        private void Side_Loaded(object sender, RoutedEventArgs e)
        {
            isActive = false;
        }

        Stack myStack = new Stack();
        public DiscElement SelectedElement
        {
            get
            {
                Contr selectedItem = ((Contr)listView.SelectedItem);
                if (selectedItem != null)
                {
                    return selectedItem.Ele;
                }
                else return null;

            }
        }

        GridViewColumnHeader _lastHeaderClicked = null;
        ListSortDirection _lastDirection = ListSortDirection.Ascending;

        void sortHandler(object sender, RoutedEventArgs e)
        {
            var headerClicked = e.OriginalSource as GridViewColumnHeader;
            ListSortDirection direction;
            if (headerClicked != null)
            {
                if (headerClicked.Role != GridViewColumnHeaderRole.Padding)
                {
                    if (headerClicked != _lastHeaderClicked)
                    {
                        direction = ListSortDirection.Ascending;
                    }
                    else
                    {
                        if (_lastDirection == ListSortDirection.Ascending)
                        {
                            direction = ListSortDirection.Descending;
                        }
                        else
                        {
                            direction = ListSortDirection.Ascending;
                        }
                    }

                    string columnName = headerClicked.Column.Header as string;
                    switch (columnName)
                    {
                        case "Название":
                            RefreshList();
                            sortBy("Name", direction);
                            break;
                        case "Дата создания":
                            RefreshList();
                            sortBy("CreationDate", direction);
                            break;
                        case "Тип":
                            RefreshList();
                            sortBy("Type", direction);
                            break;
                        case "Размер":
                            RefreshList();
                            sortBy("IntSize", direction);
                            break;
                    }
                    _lastHeaderClicked = headerClicked;
                    _lastDirection = direction;
                }
            }
        }

        public void sortBy(string property, ListSortDirection direction)
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listView.ItemsSource);
            view.SortDescriptions.Add(new SortDescription(property, direction));
        }
        
        private void Side_loaded(object sender, RoutedEventArgs e)
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (var dysk in allDrives)
            {
                if (dysk.IsReady)
                    Disc.Items.Add(dysk.Name);
            }
            Disc.SelectedIndex = 0;
            mainPath.Text = allDrives[0].Name;
            myStack.Push(mainPath.Text);
        }

        public bool RefreshList()
        {
            listView.ItemsSource = "";
            bool isThrow = false;
            List<Contr> controller = new List<Contr>();
            MyDirectory dirs = new MyDirectory(mainPath.Text);
            List<DiscElement> elements = null;
            try
            {
                elements = dirs.GetSubElements();
            }
            catch
            {
                MessageBox.Show("Нет доступа");
                elements = dirs.GetSubElements();
                isThrow = true;
            }

            foreach (var item in elements)
            {
                controller.Add(new Contr(item));
            }
            listView.ItemsSource = controller;

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listView.ItemsSource);
            view.Filter = UserFilter;
            return isThrow;
        }

        private bool UserFilter(object item)
        {
            if (String.IsNullOrEmpty(txtFilter.Text))
                return true;
            else
                return ((item as Contr).Name.IndexOf(txtFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private void Disc_SelectedIndexChanged(object sender, EventArgs e)
        {
            string[] name;
            name = Directory.GetLogicalDrives();
            mainPath.Text = name[Disc.SelectedIndex];
            myStack.Clear();
            myStack.Push(mainPath.Text);
            RefreshList();
            isActive = true;

        }

        private void OpenDirectory(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                txtFilter.Text = "";
                isActive = true;

                Contr selectedItem = ((Contr)listView.SelectedItem);
                if (selectedItem != null)
                {
                    if (selectedItem.isFile)
                    {
                        if (selectedItem.Type == "TXT")
                        {
                            RichText textWindow = new RichText(selectedItem.Path, selectedItem.Name);
                            textWindow.SaveTextFile += onRefreshAllList;
                            textWindow.Show();
                        }
                        else
                            Process.Start(selectedItem.Path);
                    }

                    else
                    {
                        string oldText = mainPath.Text;
                        mainPath.Text = selectedItem.Path;

                        bool isThrow = RefreshList();
                        if (isThrow)
                        {
                            mainPath.Text = oldText;
                            return;
                        }
                        myStack.Push(mainPath.Text);
                    }

                }
            }
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            if (myStack.Count > 1)
            {
                myStack.Pop();
                mainPath.Text = myStack.Peek().ToString();
                RefreshList();
            }

        }

        private void listView_GotFocus(object sender, RoutedEventArgs e)
        {
            isActive = true;
        }

        private void txtFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(listView.ItemsSource).Refresh();
        }

    }
}
