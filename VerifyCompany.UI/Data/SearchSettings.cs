namespace VerifyCompany.UI.Data
{
    internal class SearchSettings
    {
        public bool GenerateNotes { get; internal set; }
        public bool AddAccountsInSeparateColumns { get; internal set; }
        public bool ImportCompaniesOnlyWithPaymentDateInColumn { get; internal set; }
        public bool VerifyCompaniesInVATSystem { get; internal set; }
        public bool VerifyCompaniesInBiRSystem { get; internal set; }
        public bool VerifyCompaniesInWhiteListSystem { get; internal set; }
        public int ScopeStart { get; internal set; }
        public int ScopeEnd { get; internal set; }
        public string InputFileDir { get; internal set; }
        public string InputFileName { get; internal set; }
        public bool ExportToPdf { get; internal set; }
        public bool VerifyAlsoForInvoiceDate { get; internal set; }
    }
}