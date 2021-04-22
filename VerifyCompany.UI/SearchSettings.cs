namespace VerifyCompany.UI
{
    internal class SearchSettings
    {
        public bool GenerateNotes { get; internal set; }
        public bool AddAccountsInSeparateColumns { get; internal set; }
        public bool ImportCompaniesOnlyWithPaymentDateInColumn { get; internal set; }
        public bool VerifyCompaniesInVATSystem { get; internal set; }
        public bool VerifyCompaniesInBiRSystem { get; internal set; }
        public bool? VerifyCompaniesInWhiteListSystem { get; internal set; }
    }
}