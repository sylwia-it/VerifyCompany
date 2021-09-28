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
        private const string _spjShort = "SP.J.";
        private const string _spkLong = "SPÓŁKA KOMANDYTOWA";
        private const string _spkShort = "SP.K.";
        private const string _spaLong = "SPÓŁKA AKCYJNA";
        private const string _spaShort = "S.A.";
        public static string AbbreviateFullNameOfCompany(string fullName)
        {
            string result = fullName;

            result = result.Replace(_spzooLong, _spzooShort);
            result = result.Replace(_spjLong, _spjShort);
            result = result.Replace(_spaLong, _spaShort);
            result = result.Replace(_spkLong, _spkShort);

            return result;
        }
    }
}
