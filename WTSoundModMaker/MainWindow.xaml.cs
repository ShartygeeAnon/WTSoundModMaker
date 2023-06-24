using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
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
            var dialog = new VistaFolderBrowserDialog();
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
            try
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
                MessageBox.Show("Replacement Complete!", "Success!");
            }
            catch
            {
                MessageBox.Show("Replacement Failed!", "Error!");
            }
        }

        private void buttonSaveReplacements_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new VistaSaveFileDialog();
                dialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
                dialog.DefaultExt = "txt";
                dialog.AddExtension = true;
                if (dialog.ShowDialog(this).GetValueOrDefault())
                {
                    using (StreamWriter writer = new StreamWriter(dialog.FileName))
                    {
                        foreach (DataRow row in table.Rows)
                        {
                            writer.WriteLine(row["Replacement File"]);
                        }
                    }
                }
                MessageBox.Show("Replacements saved!", "Success!");
            }
            catch
            {
                MessageBox.Show("Couldn't save replacements!", "Error!");
            }
        }

        private void buttonLoadReplacements_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new VistaOpenFileDialog();
                dialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
                dialog.DefaultExt = "txt";
                dialog.AddExtension = true;
                if (dialog.ShowDialog(this).GetValueOrDefault())
                {
                    using (StreamReader reader = new StreamReader(dialog.FileName))
                    {
                        foreach (DataRow row in table.Rows)
                        {
                            row["Replacement File"] = reader.ReadLine();
                        }
                    }
                }
                MessageBox.Show("Replacements loaded!", "Success!");
            }
            catch
            {
                MessageBox.Show("Couldn't load replacements!", "Error!");
            }
        }

        private void buttonFindAndReplace_Click(object sender, RoutedEventArgs e)
        {
            foreach (DataRow row in table.Rows)
            {
                row["Replacement File"] = row["Replacement File"].ToString().Replace(textBoxFind.Text, textBoxReplace.Text);
            }
        }
    }
}
