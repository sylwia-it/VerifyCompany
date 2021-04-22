using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using Microsoft.Win32;
using ExcelDataManager.Lib.Import;

namespace VerifyCompany.UI
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void VerifyNIPsFromSpreadSheet()
        {
            try
            {
                var dialog = new OpenFileDialog()
                {
                    DefaultExt = ".xls",
                    Filter = "Excel dokumenty (.xls,.xlsx, .xlsm)|*.xls;*.xlsx;*.xlsm",
                    CheckPathExists = true,
                    CheckFileExists = true
                };

                if (dialog.ShowDialog() == true)
                {
                    DisableControls();

                    BackUpTheFile(dialog.FileName);

                    var searchSettings = GetSearchSettingsFromUI();


                    var progress = new Progress<string>(report =>
                    {
                        resultTBl.Text += report;
                    });



                    await Task.Factory.StartNew(() => ReadInputFile(dialog.FileName, searchSettings, progress));

                    //    if (onlyWithPaymentDateInColumn == true)
                    //    {
                    //        var dates = from c in companiesReadFromFile
                    //                    where !string.IsNullOrEmpty(c.PaymentDate)
                    //                    select c.PaymentDate;

                    //        dates = dates.Distinct();

                    //        if (dates.Count() > 1)
                    //        {
                    //            AskAboutPaymentDate askAboutPaymentDateWindow;
                    //            askAboutPaymentDateWindow = new AskAboutPaymentDate(dates.ToList());
                    //            if (askAboutPaymentDateWindow.ShowDialog() == true)
                    //            {
                    //                foreach (var company in companiesReadFromFile)
                    //                {
                    //                    string dateToVerify = askAboutPaymentDateWindow.SelectedScope;
                    //                    company.IsToBeVerified = company.PaymentDate == dateToVerify;
                    //                }
                    //            }
                    //            else
                    //            {
                    //                this.Close();
                    //            }
                    //        }
                    //        else
                    //        {
                    //            foreach (var company in companiesReadFromFile)
                    //            {
                    //                company.IsToBeVerified = !string.IsNullOrEmpty(company.PaymentDate);
                    //            }

                    //        }
                    //        companiesReadFromFile = companiesReadFromFile.Where(c => c.IsToBeVerified == true).ToList();
                    //    }

                    //    scopeStart = 1;
                    //    scopeEnd = companiesReadFromFile.Count;

                    //    if (companiesReadFromFile.Count > scopeLimit)
                    //    {
                    //        AskAboutNIPLimits askAboutNIPLimitsWindow;
                    //        List<string> scopesToAnalyze = GetScopesToAnalyze(companiesReadFromFile.Count);
                    //        askAboutNIPLimitsWindow = new AskAboutNIPLimits(scopesToAnalyze);
                    //        if (askAboutNIPLimitsWindow.ShowDialog() == true)
                    //        {
                    //            scopeStart = GetStartScope(askAboutNIPLimitsWindow.SelectedScope);
                    //            scopeEnd = GetEndScope(askAboutNIPLimitsWindow.SelectedScope);
                    //        }
                    //        else
                    //        {
                    //            this.Close();
                    //        }
                    //    }

                    //    companiesReadFromFile = companiesReadFromFile.Skip(scopeStart - 1).Take(scopeEnd - scopeStart + 1).ToList();

                    //    await Task.Factory.StartNew(() => ReadAndVerify(dialog.FileName, generateNotes, addAccountsInSeparateColumns, progress));
                }
            }
            catch (System.Exception e)
            {
                resultTBl.Text += string.Concat("Wystąpił błąd. Skontaktuj się z administratorem.\n \n", e.Message, "\n", e.StackTrace);
            }
        }

        private void ReadInputFile(string fileName, SearchSettings searchSettings, IProgress<string> progress)
        {
            
                DateTime startTime = DateTime.Now;

            
                progress.Report(string.Format("{0}: Rozpoczęto wczytywanie danych z pliku.\n", DateTime.Now.ToLongTimeString()));

                //SpreadSheetReader ssReader = new SpreadSheetReader(filePath, generateNotes);
                //ssReader.ReadDataFromFile();
                //companiesReadFromFile = ssReader.CompaniesReadFromFile;

            
        }

        private SearchSettings GetSearchSettingsFromUI()
        {
            SearchSettings settings = new SearchSettings();
            settings.GenerateNotes = generateNotesCB.IsChecked ?? false;
            settings.AddAccountsInSeparateColumns = addAccountsInSeparateColumnsCB.IsChecked ?? false;
            settings.ImportCompaniesOnlyWithPaymentDateInColumn = importCompaniesOnlyWithPaymentDateCB.IsChecked ?? false;
            settings.VerifyCompaniesInVATSystem = verifyCompaniesInNipSystemCB.IsChecked ?? false;
            settings.VerifyCompaniesInBiRSystem = verifyCompaniesInBiRSystemCB.IsChecked ?? false;
            settings.VerifyCompaniesInWhiteListSystem = verifyCompaniesInWhiteListSystemCB.IsChecked ?? false;

            return settings;
        }

        private static void BackUpTheFile(string fullPath)
        {
            int endOfPathIndex = fullPath.LastIndexOf("\\");
            string inputPath = fullPath.Substring(0, endOfPathIndex);
            string inputFileName = fullPath.Substring(endOfPathIndex + 1);
            inputFileName = inputFileName.Replace(" ", "_");


            string backUpFileName = string.Format(@"{0}\KopiaPrzed-{1}-{2}", inputPath, DateTime.Now.ToString("MMdd-hhmmss"), inputFileName);
            File.Copy(fullPath, backUpFileName);
        }

        private void DisableControls()
        {
            verifyCompaniesInNipSystemCB.IsEnabled = false;
            verifyCompaniesInBiRSystemCB.IsEnabled = false;
            verifyCompaniesInWhiteListSystemCB.IsEnabled = false;

            importCompaniesOnlyWithPaymentDateCB.IsEnabled = false;
            generateNotesCB.IsEnabled = false;
            addAccountsInSeparateColumnsCB.IsEnabled = false;

            selectFileBtn.IsEnabled = false;
        }

        private void selectFileBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
