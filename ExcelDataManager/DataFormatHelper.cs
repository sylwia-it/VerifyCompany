using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ExcelDataManager.Lib
{
    static class DataFormatHelper
    {
        public static bool IsNipValid(string nip)
        {
            nip = nip.Replace("-", string.Empty);

            if (nip.Length != 10 || nip.Any(chr => !Char.IsDigit(chr)))
                return false;

            int[] weights = { 6, 5, 7, 2, 3, 4, 5, 6, 7, 0 };
            int sum = nip.Zip(weights, (digit, weight) => (digit - '0') * weight).Sum();

            return (sum % 11) == (nip[9] - '0');
        }


        internal static string GetAccountInString(string account)
        {
            if (!string.IsNullOrEmpty(account))
            {
                string accountWithSpaces = Regex.Replace(account.Substring(2), ".{4}", "$0 ");
                accountWithSpaces = string.Concat(account.Substring(0, 2), " ", accountWithSpaces);
                return accountWithSpaces.Trim();
            }
            return string.Empty;
        }

        public static string GetAccountsInString(List<string> accounts)
        {
            if (accounts is null)
                return string.Empty;

            StringBuilder sB = new StringBuilder();

            foreach (var account in accounts)
            {
                if (!string.IsNullOrEmpty(account))
                {
                    sB.AppendFormat("{0} ,", GetAccountInString(account));
                }
            }

            return sB.ToString();
        }
    }
}
