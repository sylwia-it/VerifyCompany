using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VerifyActiveCompany.Lib;
using VerifyCompany.Common.Lib;
using VerifyCompany.UI.Data;
using VerifyNIPActivePayer.Lib;

namespace VerifyCompany.UI.Helpers
{
    internal class TxtResultExporter
    {
        private StringBuilder errorCompanies;
        private StringBuilder allResults;

        internal StringBuilder Errors { get { return errorCompanies; } }


        internal void StoreToFile(VerificationResult verificationResult, SearchSettings searchSettings)
        {
            errorCompanies = new StringBuilder();
            allResults = new StringBuilder();

            if (verificationResult.ErroredWhileReadingInputFileCompanies != null && verificationResult.ErroredWhileReadingInputFileCompanies.Count > 0)
            {
                errorCompanies.AppendLine("Błędy w formacie danych w pliku wejściowym (firmy nie były lub były częściowo sprawdzane w systemach zewnętrznych");
                allResults.AppendLine("Błędy w formacie danych w pliku wejściowym (firmy nie były lub były częściowo sprawdzane w systemach zewnętrznych");
                StringBuilder sB = new StringBuilder();
                foreach (var erroredCompany in verificationResult.ErroredWhileReadingInputFileCompanies)
                {
                    sB.Append($"Wiersz:{erroredCompany.RowNumber} NIP:{erroredCompany.NIP} - ");
                    foreach (var error in erroredCompany.FormatErrors)
                    {
                        sB.Append($"{error.ToMessage()} ");
                    }
                    sB.Append("\n");
                }
                errorCompanies.AppendLine(sB.ToString());
                allResults.AppendLine(sB.ToString());
            }

            if (searchSettings.VerifyCompaniesInVATSystem)
            {
                errorCompanies.AppendLine("BŁĘDY w bazie NIP (podatki, czy jest aktywnym płatnikiem VAT):");
                allResults.AppendLine("Wyniki sprawdzenia w bazie NIP (aktywny płatnik VAT):");
                if (verificationResult.VatSystemVerResultForInvoiceDate != null)
                {
                    foreach (var vatSystemCompVerResult in verificationResult.VatSystemVerResultForInvoiceDate)
                    {
                        var verifiedNipResult = InputCompany.GetNIPFromID(vatSystemCompVerResult.Key);

                        if (vatSystemCompVerResult.Value != VerifyNIPResult.IsActiveVATPayer)
                        {
                            errorCompanies.Append($"{InputCompany.GetFormattedStringFromID(vatSystemCompVerResult.Key)} -- {vatSystemCompVerResult.Value.ToMessage()}\n");

                        }
                        allResults.Append($"{InputCompany.GetFormattedStringFromID(vatSystemCompVerResult.Key)} -- {vatSystemCompVerResult.Value.ToMessage()}\n");
                    }
                }
                else
                {
                    errorCompanies.AppendLine("Wystąpił BŁĄD podczas sprawdzania danych w bazie NIP. Brak wyników. Skontaktuj się z administratorem.");
                    allResults.AppendLine("Wystąpił BŁĄD podczas sprawdzania danych w bazie NIP. Brak wyników. Skontaktuj się z administratorem.");
                }
            }

            if (searchSettings.VerifyCompaniesInBiRSystem)
            {
                errorCompanies.AppendLine("\nBŁĘDY w bazie REGON (czy jest aktywnym podmiotem):");
                allResults.AppendLine("\nWyniki sprawdzenia w bazie REGON (aktywny podmiot):");

                if (verificationResult.BiRSystemVerResult != null)
                {
                    foreach (var birSystemCompResult in verificationResult.BiRSystemVerResult)
                    {
                        if (birSystemCompResult.Value.BiRVerifyStatus != BiRVerifyStatus.IsActive)
                        {
                            errorCompanies.Append($"{InputCompany.GetFormattedStringFromID(birSystemCompResult.Key)} -- {birSystemCompResult.Value.Message}\n");
                        }
                        allResults.Append($"{InputCompany.GetFormattedStringFromID(birSystemCompResult.Key)} -- {birSystemCompResult.Value.Message}\n");
                    }
                }
                else
                {
                    errorCompanies.AppendLine("Wystąpił BŁĄD podczas sprawdzania danych w bazie REGON (BiR. Brak wyników. Skontaktuj się z administratorem.");
                    allResults.AppendLine("Wystąpił BŁĄD podczas sprawdzania danych w bazie REGON (BiR). Brak wyników. Skontaktuj się z administratorem.");
                }
            }

            if (searchSettings.VerifyCompaniesInWhiteListSystem)
            {
                errorCompanies.AppendLine("\nBŁĘDY w bazie LISTY BIAŁYCH FIRM (czy jest czynnym płatnikiem VAT i ma przypisane dobre konto bankowe):");
                allResults.AppendLine("\nWyniki sprawdzenia w bazie LISTY BIAŁYCH FIRM (czy jest czynnym płatnikiem VAT i ma przypisane dobre konto bankowe):");

                if (verificationResult.WhiteListCompVerResult != null)
                {
                    foreach (var whiteListCompVerResult in verificationResult.WhiteListCompVerResult)
                    {
                        if (whiteListCompVerResult.Value.VerificationStatus != VerifyWhiteListCompany.Lib.WhiteListVerResultStatus.ActiveVATPayerAccountOKVerSuccessfull &&
                            whiteListCompVerResult.Value.VerificationStatus != VerifyWhiteListCompany.Lib.WhiteListVerResultStatus.ActiveVATPayerVerSuccessfull)
                        {
                            errorCompanies.Append($"{InputCompany.GetFormattedStringFromID(whiteListCompVerResult.Key)} -- {whiteListCompVerResult.Value.ToMessage()} Ciąg id zapytania w systemie: {whiteListCompVerResult.Value.ConfirmationResponseString}\n");
                        }

                        allResults.Append(string.Format($"{InputCompany.GetFormattedStringFromID(whiteListCompVerResult.Key)} -- {whiteListCompVerResult.Value.ToMessage()} Ciąg id zapytania w systemie: {whiteListCompVerResult.Value.ConfirmationResponseString}\n"));
                    }
                }
                else
                {
                    {
                        errorCompanies.AppendLine("Wystąpił BŁĄD podczas sprawdzania danych w bazie Biała Lista Przedsiębiorców. Brak wyników. Skontaktuj się z administratorem.");
                        allResults.AppendLine("Wystąpił BŁĄD podczas sprawdzania danych w bazie Biała Lista Przedsiębiorców. Brak wyników. Skontaktuj się z administratorem.");
                    }
                }
            }

            if (searchSettings.VerifyAlsoForInvoiceDate)
            {
                errorCompanies.AppendLine("\nBŁĘDY w bazie LISTY BIAŁYCH FIRM na *DATĘ FAKTURY* (czy jest czynnym płatnikiem VAT i ma przypisane dobre konto bankowe):");
                allResults.AppendLine("\nWyniki sprawdzenia w bazie LISTY BIAŁYCH FIRM na *DATĘ FAKTURY*  (czy jest czynnym płatnikiem VAT i ma przypisane dobre konto bankowe):");

                if (verificationResult.WhiteListCompVerResultForInvoiceData != null)
                {
                    foreach (var whiteListCompVerResult in verificationResult.WhiteListCompVerResultForInvoiceData)
                    {
                        if (whiteListCompVerResult.Value.VerificationStatus != VerifyWhiteListCompany.Lib.WhiteListVerResultStatus.ActiveVATPayerAccountOKVerSuccessfull &&
                            whiteListCompVerResult.Value.VerificationStatus != VerifyWhiteListCompany.Lib.WhiteListVerResultStatus.ActiveVATPayerVerSuccessfull)
                        {
                            errorCompanies.Append($"{InputCompany.GetFormattedStringFromID(whiteListCompVerResult.Key)} -- {whiteListCompVerResult.Value.ToMessage()} Ciąg id zapytania w systemie: {whiteListCompVerResult.Value.ConfirmationResponseString}\n");
                        }

                        allResults.Append(string.Format($"{InputCompany.GetFormattedStringFromID(whiteListCompVerResult.Key)} -- {whiteListCompVerResult.Value.ToMessage()} Ciąg id zapytania w systemie: {whiteListCompVerResult.Value.ConfirmationResponseString}\n"));
                    }
                }
                else
                {
                    {
                        errorCompanies.AppendLine("Wystąpił BŁĄD podczas sprawdzania danych w bazie Biała Lista Przedsiębiorców na *datę faktury*. Brak wyników. Skontaktuj się z administratorem.");
                        allResults.AppendLine("Wystąpił BŁĄD podczas sprawdzania danych w bazie Biała Lista Przedsiębiorców *datę faktury*. Brak wyników. Skontaktuj się z administratorem.");
                    }
                }
            }
            SaveToFile("result.txt", searchSettings, errorCompanies, allResults);

            string fileNameInSrcDir = string.Format(@"{0}\result-{1}-{2}.txt", searchSettings.InputFileDir, searchSettings.InputFileName, DateTime.Now.ToString("yyyyMMdd-hhmm"));
            SaveToFile(fileNameInSrcDir, searchSettings, errorCompanies, allResults);

        }

        private static void SaveToFile(string filePath, SearchSettings searchSettings, StringBuilder errorComponies, StringBuilder allResults)
        {
            using (System.IO.StreamWriter file =
          new System.IO.StreamWriter(filePath))
            {
                file.WriteLine("Weryfikacja firm");
                file.WriteLine(string.Format("Analiza pliku: {0}", searchSettings.InputFileName));
                file.WriteLine(string.Format("Stan na dzień: {0}", DateTime.Now.ToString()));
                file.WriteLine("Błędy:");
                file.WriteLine(errorComponies.ToString());
                file.WriteLine("Wszyskie:");
                file.WriteLine(allResults.ToString());
            }
        }
    }
}
