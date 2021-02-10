using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace DocumentGenerator.Lib.Helpers
{
    public static class FormatHelper
    {
        public static readonly string SPACE = " ";
        public static string GetAccountNumberInString(string accountNumber)
        {
            if (!string.IsNullOrEmpty(accountNumber))
            {
                string accountWithSpaces = Regex.Replace(accountNumber.Substring(2), ".{4}", "$0 ");
                accountWithSpaces = string.Concat(accountNumber.Substring(0, 2), SPACE, accountWithSpaces);
                return accountWithSpaces.Trim();
            }
            return string.Empty;
        }

        private const string _spzooLong = "SPÓŁKA Z OGRANICZONĄ ODPOWIEDZIALNOŚCIĄ";
        private const string _spzooShort = "SP. Z O. O.";
        private const string _spjLong = "SPÓŁKA JAWNA";
        private const string _spjShort = "SP. J.";
        public static string AbbreviateFullNameOfCompany(string fullName)
        {
            string result = fullName;

            result = result.Replace(_spzooLong, _spzooShort);
            result = result.Replace(_spjLong, _spjShort);

            return result;
        }
    }
}
