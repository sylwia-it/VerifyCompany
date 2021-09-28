using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerifyCompany.Common.Lib
{
    public enum InputCompanyFormatError
    {
        NIPFormatError,
        BankAccountFormatError,
        PaymentDateError,
        LPFormatError,
        InvoiceDateError
    }

    public static class InputCompanyFormatErrorExtenstions
    {
        public static string ToMessage(this InputCompanyFormatError error)
        {
            switch (error)
            {
                case InputCompanyFormatError.NIPFormatError:
                    return "Błędny format NIPu";
                case InputCompanyFormatError.BankAccountFormatError:
                    return "Błędny format konta bankowego";
                case InputCompanyFormatError.PaymentDateError:
                    return "Błędny format daty płatności";
                case InputCompanyFormatError.LPFormatError:
                    return "Błędny format LP";
                case InputCompanyFormatError.InvoiceDateError:
                    return "Błędny format daty faktury";
                default:
                    return "Błąd";
            }
        }
    }
}
