using System;
using System.Collections.Generic;
using System.Data;
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

namespace WTSoundModMaker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataTable table;
        public MainWindow()
        {
            InitializeComponent();
            table = new DataTable();
        }

        private void buttonSelectFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            if (dialog.ShowDialog(this).GetValueOrDefault())
            {
                // The user selected a folder.
                string selectedFolder = dialog.SelectedPath;
                textBoxFolder.Text = selectedFolder;
                table.Rows.Clear();
                table.Columns.Clear();
                dataGridFiles.ItemsSource = null;
                string folderPath = textBoxFolder.Text;
                string[] files = Directory.GetFiles(folderPath);
                table.Columns.Add("Original File");
                foreach (string file in files)
                {
                    table.Rows.Add(System.IO.Path.GetFileName(file));
                }
                table.Columns.Add("Replacement File");
                dataGridFiles.ItemsSource = table.DefaultView;
                dataGridFiles.Columns[0].IsReadOnly = true;
            }
        }

        private void dataGridFiles_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void dataGridFiles_DragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        private void dataGridFiles_Drop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (DataRowView row in dataGridFiles.SelectedItems)
            {
                row["Replacement File"] = System.IO.Path.GetFullPath(files[0]);
            }
        }

        private void buttonReplaceFiles_Click(object sender, RoutedEventArgs e)
        {
            foreach (DataRowView row in dataGridFiles.Items)
            {
                string originalFilePath = textBoxFolder.Text + row["Original File"];
                string replacementFilePath = row["Replacement File"].ToString();

                // Replace the file.
                if (!string.IsNullOrEmpty(replacementFilePath))
                    //MessageBox.Show(replacementFilePath + " is not empty and would replace " + originalFilePath);
                    File.Replace(replacementFilePath, originalFilePath, null);
            }
        }
    }
}
