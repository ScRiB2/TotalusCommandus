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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TotalCommander.Classes;
using TotalCommander.AdditionalElements;
using Microsoft.VisualBasic.FileIO;
using System.ComponentModel;

namespace TotalCommander.MainViews
{
    /// <summary>
    /// Interaction logic for Operations.xaml
    /// </summary>
    public partial class Operations : UserControl
    {
        SelectedSide selectedSite;

        public SideView sideLeft { get; set; }
        public SideView sideRight { get; set; }

        public Operations(SideView sideLeft, SideView sideRight)
        {
            InitializeComponent();
            this.sideLeft = sideLeft;
            this.sideRight = sideRight;
            this.sideLeft.RefreshAllList += RefreshAllList;
            this.sideRight.RefreshAllList += RefreshAllList;
        }

        public void RefreshAllList()
        {
            sideLeft.RefreshList();
            sideRight.RefreshList();
        }

        public delegate void deletedEventHandler();
        public event deletedEventHandler ShowAfterDeleted;

        FocusCommunication communication = new FocusCommunication();


        protected virtual void onShowAfterDeleted()
        {
            if (ShowAfterDeleted != null)
            {
                ShowAfterDeleted.Invoke();
            }
        }

        private void asyncCopy(object sender, DoWorkEventArgs e)
        {
            var path = (PathToMove)e.Argument;
            if (path.isFile)
                try
                {
                    FileSystem.CopyFile(path.From, path.To, UIOption.AllDialogs);
                }
                catch { }
            else
                try
                {
                    FileSystem.CopyDirectory(path.From, path.To, UIOption.AllDialogs);
                }
                catch { }
        }

        private void asyncMove(object sender, DoWorkEventArgs e)
        {
            var path = (PathToMove)e.Argument;
            if (path.isFile)
                try
                {
                    FileSystem.MoveFile(path.From, path.To, UIOption.AllDialogs);
                }
                catch { }
            else
                try
                {
                    FileSystem.MoveDirectory(path.From, path.To, UIOption.AllDialogs);
                }
                catch { }
        }

        private void asyncDelete(object sender, DoWorkEventArgs e)
        {
            var path = (PathToMove)e.Argument;
            if (path.isFile)
                try
                {
                    FileSystem.DeleteFile(path.From);
                }
                catch { }
            else
                try
                {
                    FileSystem.DeleteDirectory(path.From, DeleteDirectoryOption.DeleteAllContents);
                }
                catch { }
        }

        private void afterAsyncDelete(object sender, RunWorkerCompletedEventArgs e)
        {
            ShowAfterDeleted();
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            if (sideRight.SelectedElement != null)
            {
                selectedSite = SelectedSide.right;
            }
            else selectedSite = SelectedSide.left;

            if (selectedSite == SelectedSide.left)
            {

                try
                {
                    var paths = new PathToMove(sideLeft.SelectedElement.Path, "", sideLeft.SelectedElement.isFile());
                    var bW = new BackgroundWorker();
                    bW.DoWork += asyncDelete;
                    bW.RunWorkerCompleted += afterAsyncDelete;
                    bW.RunWorkerAsync(paths);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            else
            {
                try
                {
                    var paths = new PathToMove(sideRight.SelectedElement.Path, "", sideRight.SelectedElement.isFile());
                    var bW = new BackgroundWorker();
                    bW.DoWork += asyncDelete;
                    bW.RunWorkerCompleted += afterAsyncDelete;
                    bW.RunWorkerAsync(paths);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void rename_Click(object sender, RoutedEventArgs e)
        {
            if (sideRight.SelectedElement != null)
            {
                selectedSite = SelectedSide.right;
            }
            else selectedSite = SelectedSide.left;
            DiscElement presentElement = selectedSite == SelectedSide.left ? sideLeft.SelectedElement : sideRight.SelectedElement;
            string path = selectedSite == SelectedSide.left ? sideLeft.SelectedElement.Path : sideRight.SelectedElement.Path;
            string sourcePath = selectedSite == SelectedSide.left ? sideLeft.mainPath.Text : sideRight.mainPath.Text;

            string fileName = selectedSite == SelectedSide.left ? sideLeft.SelectedElement.getName() : sideRight.SelectedElement.getName();
            var dialog = new RenamePanel(path, sourcePath, presentElement.isFile(), fileName);
            dialog.Show();
            dialog.RenameObject += RefreshAllList;
        }



        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            onShowAfterDeleted();
        }

        private void copy_Click(object sender, RoutedEventArgs e)
        {
            if (sideRight.SelectedElement != null)
            {
                selectedSite = SelectedSide.right;
            }
            else selectedSite = SelectedSide.left;

            DiscElement presentElement = selectedSite == SelectedSide.left ? sideLeft.SelectedElement : sideRight.SelectedElement;
            string dirName = selectedSite == SelectedSide.left ? sideLeft.SelectedElement.Path : sideRight.SelectedElement.Path;
            string fileName = selectedSite == SelectedSide.left ? sideLeft.SelectedElement.getName() : sideRight.SelectedElement.getName();
            string sourcePath = selectedSite == SelectedSide.left ? sideLeft.mainPath.Text : sideRight.mainPath.Text;
            string targetPath = selectedSite == SelectedSide.left ? sideRight.mainPath.Text : sideLeft.mainPath.Text;
            string sourceFile = System.IO.Path.Combine(sourcePath, fileName);
            string destFile = System.IO.Path.Combine(targetPath, fileName);

            if (sourcePath == targetPath)
                return;

            if (presentElement.isFile())
            {
                if (!Directory.Exists(targetPath))
                {
                    Directory.CreateDirectory(targetPath);
                }
                var paths = new PathToMove(sourceFile, destFile, presentElement.isFile());
                var bW = new BackgroundWorker();
                bW.DoWork += asyncCopy;
                bW.RunWorkerCompleted += backgroundWorker_RunWorkerCompleted;
                bW.RunWorkerAsync(paths);
            }

            else
            {
                var paths = new PathToMove(dirName, destFile, presentElement.isFile());
                var bW = new BackgroundWorker();
                bW.DoWork += asyncCopy;
                bW.RunWorkerCompleted += backgroundWorker_RunWorkerCompleted;
                bW.RunWorkerAsync(paths);
            }
        }

        private void Move_Click(object sender, RoutedEventArgs e)
        {
            if (sideRight.SelectedElement != null)
            {
                selectedSite = SelectedSide.right;
            }
            else selectedSite = SelectedSide.left;

            DiscElement presentElement = selectedSite == SelectedSide.left ? sideLeft.SelectedElement : sideRight.SelectedElement;
            string dirName = selectedSite == SelectedSide.left ? sideLeft.SelectedElement.Path : sideRight.SelectedElement.Path;
            string fileName = selectedSite == SelectedSide.left ? sideLeft.SelectedElement.getName() : sideRight.SelectedElement.getName();
            string sourcePath = selectedSite == SelectedSide.left ? sideLeft.mainPath.Text : sideRight.mainPath.Text;
            string targetPath = selectedSite == SelectedSide.left ? sideRight.mainPath.Text : sideLeft.mainPath.Text;
            string sourceFile = System.IO.Path.Combine(sourcePath, fileName);
            string destFile = System.IO.Path.Combine(targetPath, fileName);

            if (sourcePath == targetPath)
                return;

            if (presentElement.isFile())
            {
                if (!Directory.Exists(targetPath))
                {
                    Directory.CreateDirectory(targetPath);
                }

                var paths = new PathToMove(sourceFile, destFile, presentElement.isFile());
                var bW = new BackgroundWorker();
                bW.DoWork += asyncMove;
                bW.RunWorkerCompleted += backgroundWorker_RunWorkerCompleted;
                bW.RunWorkerAsync(paths);
            }

            else
            {
                if (!Directory.Exists(destFile))
                {
                    var paths = new PathToMove(dirName, destFile, presentElement.isFile());
                    var bW = new BackgroundWorker();
                    bW.DoWork += asyncMove;
                    bW.RunWorkerCompleted += backgroundWorker_RunWorkerCompleted;
                    bW.RunWorkerAsync(paths);
                }
                else
                {
                    MessageBox.Show("Папка с указанным именем уже существует");
                    return;
                }
            }
        }


        private void СreateDirectoryHandler(object sender, RoutedEventArgs e)
        {
            string side = communication.CorrectSide(sideLeft, sideRight);
            sideLeft.isActive = false;
            sideRight.isActive = false;
            string sourcePath = side == "left" ? sideLeft.mainPath.Text : sideRight.mainPath.Text;
            var dialog = new CreateDirectory(sourcePath);
            dialog.Show();
            dialog.CreatedDirectory += RefreshAllList;
        }

        private void refresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshAllList();
        }


    }
}
