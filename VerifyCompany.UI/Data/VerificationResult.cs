using System.Collections.Generic;
using VerifyActiveCompany.Lib;
using VerifyCompany.Common.Lib;
using VerifyNIPActivePayer.Lib;
using VerifyWhiteListCompany.Lib;

namespace VerifyCompany.UI
{
    internal class VerificationResult
    {
        internal Dictionary<string, VerifyNIPResult> VatSystemVerResultForInvoiceDate { get; set; }
        internal Dictionary<string, BiRVerifyResult> BiRSystemVerResult { get; set; }

        internal Dictionary<string, WhiteListVerResult> WhiteListCompVerResult { get; set; }
        internal Dictionary<string, WhiteListVerResult> WhiteListCompVerResultForInvoiceData { get; set; }
        internal List<InputCompany> ErroredWhileReadingInputFileCompanies { get; set; }
    }
}
