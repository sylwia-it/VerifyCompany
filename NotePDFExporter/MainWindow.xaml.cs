﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace NotePDFExporter
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void fileExportBtn_Click(object sender, RoutedEventArgs e)
        {
            DisableControls();
            try
            {

                var dialog = new OpenFileDialog()
                {
                    Multiselect = true,
                    DefaultExt = ".xls",
                    Filter = "Excel dokumenty (.xls,.xlsx, .xlsm)|*.xls;*.xlsx;*.xlsm",
                    CheckPathExists = true,
                    CheckFileExists = true
                };
                if (dialog.ShowDialog() == true)
                {
                    ExportFiles(dialog.FileNames);
                }

                FinishApplication();


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void FinishApplication()
        {
            if (MessageBox.Show("Pomyślnie wyeksportowano noty", "Zakończono pracę", MessageBoxButton.OK, MessageBoxImage.Information) == MessageBoxResult.OK)
            {
                this.Close();
            }

        }

        private void ExportFiles(string[] fileNames)
        {
            Microsoft.Office.Interop.Excel.Application app = null;
            try
            {
                string dirToExport = CreatePDFFolderForExportedDocs(fileNames[0]);
                app = new Microsoft.Office.Interop.Excel.Application();
                foreach (var filePath in fileNames)
                {
                    ExportToPDF(app, filePath, dirToExport);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Problem eksportu plików.  Msg: {0}", ex.Message));
            }
            finally
            {
                if (app != null)
                {
                    app.Quit();
                }
            }
        }

        private string CreatePDFFolderForExportedDocs(string fullFilePathToExport)
        {
            string dirOfFile = System.IO.Path.GetDirectoryName(fullFilePathToExport);
            DirectoryInfo pdfExportDir = new DirectoryInfo(string.Format("{0}\\PDF", dirOfFile));

            if (!pdfExportDir.Exists)
            {
                pdfExportDir.Create();
            }

            return pdfExportDir.FullName;
        }

        private Regex xlsFileExtension = new Regex(@"\.xls(x)?");
        private void ExportToPDF(Microsoft.Office.Interop.Excel.Application app, string filePath, string dirToExport)
        {


            string pdfFileName = xlsFileExtension.Replace(System.IO.Path.GetFileName(filePath), @".pdf");


            try
            {
                Workbook theWorkbook = app.Workbooks.Open(
                    filePath, true, false,
                            Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                string fullPDFFilePath = System.IO.Path.Combine(dirToExport, pdfFileName);

                theWorkbook.ExportAsFixedFormat(XlFixedFormatType.xlTypePDF, fullPDFFilePath, IgnorePrintAreas: false);

            }
            catch (Exception ex)
            {
                throw new PdfExportException(string.Format("Problem z plikiem {0}. Msg: {1}", filePath, ex.Message));
            }

        }

        private void dirExportBtn_Click(object sender, RoutedEventArgs e)
        {
            DisableControls();

            try
            {

                var dialog = new CommonOpenFileDialog()
                {
                    IsFolderPicker = true,
                    Title = "Wybierz folder"
                };
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    string directoryPath = dialog.FileName;
                    string[] filesInDirectory = Directory.GetFiles(directoryPath);

                    List<string> filesToExport = new List<string>();

                    foreach (string file in filesInDirectory)
                    {
                        if (File.Exists(file) && xlsFileExtension.IsMatch(file))
                        {
                            filesToExport.Add(file);
                        }
                    }

                    ExportFiles(filesToExport.ToArray());
                }

                FinishApplication();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DisableControls()
        {
            fileExportBtn.IsEnabled = false;
            dirExportBtn.IsEnabled = false;
        }
    }
}