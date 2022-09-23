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
using VerifyCompany.Common.Lib;
using VerifyCompany.UI.Helpers;
using VerifyNIPActivePayer.Lib;
using VerifyActiveCompany.Lib;
using VerifyWhiteListCompany.Lib;
using VerifyCompany.UI.Data;
using ExcelDataManager.Lib.Export;
using DocumentGenerator.Lib;

namespace VerifyCompany.UI
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string _errorsResult = "BŁĘDY:\n";
        private bool _scroll = true;
        private List<InputCompany> _companiesReadFromFile;
        private VerificationResult _verificationResult;
        private SearchSettings _searchSettings;

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

                    _searchSettings = GetSearchSettingsFromUI();
                    _searchSettings.InputFileDir = dialog.FileName.Substring(0, dialog.FileName.LastIndexOf("\\"));
                    _searchSettings.InputFileName = dialog.FileName.Substring(dialog.FileName.LastIndexOf("\\") + 1);

                    IProgress<string> progress = new Progress<string>(report =>
                    {
                        resultTBl.Text += report;
                        if (_scroll)
                            resultTBl.ScrollToEnd();
                    });

                    await Task.Factory.StartNew(() => ReadInputFile(dialog.FileName, _searchSettings, progress));

                    if (_searchSettings.ImportCompaniesOnlyWithPaymentDateInColumn == true)
                    {
                        AskToSelectOnePaymentDate();
                    }

                    _verificationResult = new VerificationResult();
                    _verificationResult.ErroredWhileReadingInputFileCompanies = _companiesReadFromFile.Where(c =>  c.FormatErrors != null && c.FormatErrors.Count > 0).ToList();
                    _companiesReadFromFile = _companiesReadFromFile.Where(c => c.FormatErrors == null || c.FormatErrors.Count == 0 || (c.FormatErrors.Count == 1 && c.FormatErrors.Contains(InputCompanyFormatError.BankAccountFormatError))).ToList();

                    DateTime startTime = DateTime.Now;

                    _searchSettings.ScopeStart = 1;
                    _searchSettings.ScopeEnd = _companiesReadFromFile.Count;

                    if (_companiesReadFromFile.Count > CompanyScopeHelper.ScopeLimit)
                    {
                        AskToSelectScope(ref _searchSettings);
                    }

                    var scopeToTake =  _companiesReadFromFile.Skip(_searchSettings.ScopeStart - 1).Take(_searchSettings.ScopeEnd - _searchSettings.ScopeStart + 1);
                    int orderOfTheLastElement = scopeToTake.Last().Order;
                    int orderOfTheFirstElement = scopeToTake.First().Order;
                    _companiesReadFromFile = scopeToTake.ToList();
                    _verificationResult.ErroredWhileReadingInputFileCompanies = _verificationResult.ErroredWhileReadingInputFileCompanies.Where(c => c.Order <= orderOfTheLastElement && c.Order >= orderOfTheFirstElement ).ToList();
                    

                    progress.Report(string.Format("{0}: Wczytano dane z pliku. Czas trwania operacji: {1}s.\n", DateTime.Now.ToLongTimeString(), Math.Round((DateTime.Now - startTime).TotalSeconds, 0)));

                    await Task.Factory.StartNew(() => VerifyCompanies(_searchSettings, progress));

                    if (_searchSettings.GenerateNotes)
                    {
                        await Task.Factory.StartNew(() => GenerateNotes(_searchSettings, progress));
                    }

                    string task = await Task.Factory.StartNew(() => StoreResultsToFilesAndShowToUser(_searchSettings, progress));

                    printBtn.DataContext = task;
                    printBtn.IsEnabled = true;
                    printBtn.Visibility = Visibility.Visible;

                }

            }
            catch (System.Exception e)
            {
                resultTBl.Text += string.Concat("Wystąpił błąd. Skontaktuj się z administratorem.\n \n", e.Message, "\n\n\nInformacja dla administratora:", e.StackTrace);
            }
        }
        private void GenerateNotes(SearchSettings searchSettings, IProgress<string> progress)
        {
            progress.Report(string.Format("{0}: Rozpoczęto generowanie not.\n", DateTime.Now.ToLongTimeString()));

            NoteGenerator noteGenerator = new NoteGenerator();
            string outputPath = searchSettings.InputFileDir;
            noteGenerator.GenerateNotes(outputPath, _companiesReadFromFile, _verificationResult.WhiteListCompVerResult, searchSettings.ExportToPdf);

            progress.Report(string.Format("{0}: Zakończono generowanie not.\n", DateTime.Now.ToLongTimeString()));

        }
        /// <summary>
        /// Saves to txt  files all the results and errors
        /// </summary>
        /// <param name="searchSettings"></param>
        /// <param name="progress"></param>
        /// <returns>errors that were detected</returns>
        private string StoreResultsToFilesAndShowToUser(SearchSettings searchSettings, IProgress<string> progress)
        {
            progress.Report(string.Format("{0}: Rozpoczęto zapis do pliku tekstowego.\n", DateTime.Now.ToLongTimeString()));

            TxtResultExporter txtResultFileWriter = new TxtResultExporter();
            txtResultFileWriter.StoreToFile(_verificationResult, searchSettings);

            progress.Report(string.Format("{0}: Zakończono zapis do pliku tekstowego.\n", DateTime.Now.ToLongTimeString()));

            progress.Report(string.Format("{0}: Rozpoczęto zapis do pliku Excel.\n", DateTime.Now.ToLongTimeString()));

            string fullPathToInputExportFile = string.Format($"{searchSettings.InputFileDir}\\{searchSettings.InputFileName}");
            SpreadSheetWriter ssWriter = new SpreadSheetWriter(fullPathToInputExportFile);
            ssWriter.WriteResultsToFile(_companiesReadFromFile, _verificationResult.ErroredWhileReadingInputFileCompanies, _verificationResult.VatSystemVerResultForInvoiceDate, _verificationResult.BiRSystemVerResult, _verificationResult.WhiteListCompVerResult, _verificationResult.WhiteListCompVerResultForInvoiceData, searchSettings.AddAccountsInSeparateColumns);

            progress.Report(string.Format("{0}: Zakończono zapis do pliku Excel.\n", DateTime.Now.ToLongTimeString()));

            progress.Report("-----------------------------------------------\n");
            progress.Report(_errorsResult);
            _scroll = false;
            progress.Report(txtResultFileWriter.Errors.ToString());

            return txtResultFileWriter.Errors.ToString();

        }

        private void VerifyCompanies(SearchSettings searchSettings, IProgress<string> progress)
        {
            progress.Report(string.Format("{0}: Rozpoczęto weryfikację firm w wybranach bazach.\n", DateTime.Now.ToLongTimeString()));

         
            if (searchSettings.VerifyCompaniesInVATSystem)
            {
                VerifyCompaniesInVATSystem(searchSettings, progress);
            }

            if (searchSettings.VerifyCompaniesInBiRSystem)
            {
                VerifyCompaniesInBiRSystem(searchSettings, progress);
            }

            if (searchSettings.VerifyCompaniesInWhiteListSystem)
            {
                VerifyCompaniesInWhiteListSystem(searchSettings, progress);
            }
        }

        private void VerifyCompaniesInVATSystem(SearchSettings searchSettings, IProgress<string> progress)
        {
            DateTime startTime = DateTime.Now;
            progress.Report(string.Format("{0}: Rozpoczęto weryfikację firm w bazie VAT (NIP).\n", DateTime.Now.ToLongTimeString()));


            NIPActivePayerVerifier verifier = new NIPActivePayerVerifier();
            _verificationResult.VatSystemVerResultForInvoiceDate = verifier.VerifyNIPs(_companiesReadFromFile);

            //if (_searchSettings.VerifyAlsoForInvoiceDate)
           // {
            //    _verificationResult.VatSystemVerResultForInvoiceDate = verifier.VerifyNIPs(_companiesReadFromFile, true);
           // }


            progress.Report(string.Format("{0}: Zakończono sprawdzanie NIPów. Czas trwania operacji: {1}s.\n", DateTime.Now.ToLongTimeString(), Math.Round((DateTime.Now - startTime).TotalSeconds, 0)));

            
        }

        private void VerifyCompaniesInBiRSystem(SearchSettings searchSettings, IProgress<string> progress)
        {
            DateTime startTime = DateTime.Now;
            progress.Report(string.Format("{0}: Rozpoczęto sprawdzenie w systemie BiR (REGON).\n", DateTime.Now.ToLongTimeString()));

            BiRVerifier biRVerifier = new BiRVerifier();

            _verificationResult.BiRSystemVerResult = biRVerifier.AreCompaniesActive(_companiesReadFromFile);
            biRVerifier.Finish();

            progress.Report(string.Format("{0}: Zakończono sprawdzanie w systemie BiR (REGON). Czas trwania operacji: {1}s.\n", DateTime.Now.ToLongTimeString(), Math.Round((DateTime.Now - startTime).TotalSeconds, 0)));

            
        }
        private void VerifyCompaniesInWhiteListSystem(SearchSettings searchSettings, IProgress<string> progress)
        {
            DateTime startTime = DateTime.Now;
            progress.Report(string.Format("{0}: Rozpoczęto sprawdzenie na Liście Białych Firm.\n", DateTime.Now.ToLongTimeString(), Math.Round((DateTime.Now - startTime).TotalSeconds, 0)));

            WhiteListCompanyVerifier whiteListCompVerifier = new WhiteListCompanyVerifier();
            _verificationResult.WhiteListCompVerResult = whiteListCompVerifier.VerifyCompanies(_companiesReadFromFile, true, false);

            if (_searchSettings.VerifyAlsoForInvoiceDate)
            {
                _verificationResult.WhiteListCompVerResultForInvoiceData = whiteListCompVerifier.VerifyCompanies(_companiesReadFromFile, true, true);
            }

            progress.Report(string.Format("{0}: Zakończono sprawdzanie na Liście Białych Firm. Czas trwania operacji: {1}s.\n", DateTime.Now.ToLongTimeString(), Math.Round((DateTime.Now - startTime).TotalSeconds, 0)));
            
        }

        private void AskToSelectScope(ref SearchSettings searchSettings)
        {
            AskToSelectScope askToSelectScopeWindow;
            //List<string> scopesToAnalyze = CompanyScopeHelper.GetListOfScopes(_companiesReadFromFile.Count);
            List<string> scopesToAnalyze = CompanyScopeHelper.GetListOfScopes(_companiesReadFromFile);
            askToSelectScopeWindow = new AskToSelectScope(scopesToAnalyze);
            askToSelectScopeWindow.Owner = this;
            if (askToSelectScopeWindow.ShowDialog() == true)
            {
                
                searchSettings.ScopeStart = CompanyScopeHelper.GetStartScope(askToSelectScopeWindow.SelectedScope);
                searchSettings.ScopeEnd = CompanyScopeHelper.GetEndScope(askToSelectScopeWindow.SelectedScope);
            }
            else
            {
                this.Close();
            }
        }

        private void AskToSelectOnePaymentDate()
        {
            var dates = from c in _companiesReadFromFile
                             where !string.IsNullOrEmpty(c.PaymentDate)
                              select c.PaymentDate;

            dates = dates.Distinct();

            if (dates.Count() > 1)
            {
                AskAboutPaymentDate askAboutPaymentDateWindow;
                askAboutPaymentDateWindow = new AskAboutPaymentDate(dates.ToList());
                askAboutPaymentDateWindow.Owner = this;
                if (askAboutPaymentDateWindow.ShowDialog() == true)
                {
                    string dateToBeVerified = askAboutPaymentDateWindow.SelectedPaymentDate;

                    foreach (var company in _companiesReadFromFile)
                    {
                        company.IsToBeVerified = company.PaymentDate == dateToBeVerified;
                    }
                }
                else
                {
                    this.Close();
                }
            }
            else if (dates.Count() == 1)
            {
                string msg = string.Format($"Jest tylko jedna data płatności {dates.FirstOrDefault()} Firmy z tą datą zostaną sprawdzone.");
                MessageBox.Show(msg, "Tylko jedna data", MessageBoxButton.OK, MessageBoxImage.Information);

                //The companies withour payment date are NOT to be verified
                foreach (var company in _companiesReadFromFile)
                {
                    company.IsToBeVerified = !string.IsNullOrEmpty(company.PaymentDate);
                }

            } else
            {
                MessageBox.Show("Brak firm z datą zapłaty. Naciśnij OK, aby zamknąć program.", "Brak firm z datą zapłaty", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
            
            _companiesReadFromFile = _companiesReadFromFile.Where(c => c.IsToBeVerified == true).ToList();
        }

        private void ReadInputFile(string filePath, SearchSettings searchSettings, IProgress<string> progress)
        {
            DateTime startTime = DateTime.Now;

            progress.Report(string.Format("{0}: Rozpoczęto wczytywanie danych z pliku.\n", DateTime.Now.ToLongTimeString()));
            progress.Report(string.Format("{0}: Nazwa pliku: {1}.\n", DateTime.Now.ToLongTimeString(), searchSettings.InputFileName));
            progress.Report(string.Format("{0}: Pełna ścieżka pliku: {1}\\{2}.\n", DateTime.Now.ToLongTimeString(), searchSettings.InputFileDir, searchSettings.InputFileName));

            SpreadSheetReader ssReader = new SpreadSheetReader(filePath, searchSettings.GenerateNotes, searchSettings.VerifyAlsoForInvoiceDate);

            _companiesReadFromFile = ssReader.ReadDataFromFile();

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
            settings.ExportToPdf = exportToPDFCB.IsChecked ?? false;
            settings.VerifyAlsoForInvoiceDate = checkForInvoiceDateCB.IsChecked ?? false;

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
            exportToPDFCB.IsEnabled = false;
            checkForInvoiceDateCB.IsEnabled = false;

            selectFileBtn.IsEnabled = false;

        }

        private void SelectFileBtn_Click(object sender, RoutedEventArgs e)
        {
          

            VerifyNIPsFromSpreadSheet();

        }

        private void PrintBtn_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).DataContext != null)
            {
                string sB = (sender as Button).DataContext as string;

                if (sB != null)
                {
                    
                    PrintDialog printDlg = new PrintDialog();
                      
                    FlowDocument doc = CreateFlowDocument(sB);
                    doc.Name = "Errors";
                    IDocumentPaginatorSource idpSource = doc;
                    printDlg.PrintDocument(idpSource.DocumentPaginator, "Drukowanie błędów.");
                  
                }
                else 
                {
                    MessageBox.Show( "Wystąpił błąd. Nie można eksportować danych.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
            else
            {
                MessageBox.Show( "Wystąpił błąd. Nie można eksportować danych.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private FlowDocument CreateFlowDocument(string sB)
        {
            FlowDocument doc = new FlowDocument();
            doc.PageWidth = 793;// a4 width in px
            doc.ColumnWidth = doc.PageWidth - 30;

            Section sec = new Section();
            
            Paragraph paragrph = new Paragraph();
            Bold bld = new Bold();
            bld.Inlines.Add(new Run($"Sprawdzanie firm"));
            paragrph.Inlines.Add(bld);
            paragrph.Inlines.Add(new LineBreak());
            paragrph.Inlines.Add($"Data: {DateTime.Now}");
            paragrph.Inlines.Add(new LineBreak());
            paragrph.Inlines.Add($"Plik: {_searchSettings.InputFileName}");
            sec.Blocks.Add(paragrph);

            paragrph = new Paragraph();
            bld = new Bold();
            bld.Inlines.Add(new Run("BŁĘDY:"));
            paragrph.Inlines.Add(bld);
            sec.Blocks.Add(paragrph);

            paragrph = new Paragraph();
            string errors = sB.ToString();
            errors = errors.Replace("Wiersz", "-- Wiersz");
            paragrph.Inlines.Add(new Run(errors));
            sec.Blocks.Add(paragrph);

            doc.Blocks.Add(sec);
            return doc;
        }
    }
}
