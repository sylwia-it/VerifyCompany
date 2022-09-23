using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VerifyCompany.Common.Lib;

namespace VerifyCompany.UI.Helpers
{
    internal static class CompanyScopeHelper
    {
        internal static readonly int ScopeLimit = 300;
        internal static List<string> GetListOfScopes(int numberOfCompanies)
        {
            List<string> result = new List<string>();
            int numOfFullScopes = numberOfCompanies / ScopeLimit; //full scope is 1-300, 301-600 etc.
            int rest = numberOfCompanies % ScopeLimit;

            for (int i = 0; i < numOfFullScopes; i++)
            {
                result.Add(string.Format("{0} - {1}", i * ScopeLimit + 1, (i + 1) * ScopeLimit));
            }

            if (rest > 0)
            {
                result.Add(string.Format("{0} - {1}", numOfFullScopes * ScopeLimit + 1, numberOfCompanies));
            }
            return result;
        }


        internal static List<string> GetListOfScopes(List<InputCompany> inputCompanies )
        {
            List<string> result = new List<string>();
            int numOfFullScopes = inputCompanies.Count / ScopeLimit; //full scope is 1-300, 301-600 etc.
            int rest = inputCompanies.Count % ScopeLimit;

            for (int i = 0; i < numOfFullScopes; i++)
            {
                int startIndex = inputCompanies.ElementAt(i * ScopeLimit).RowNumber;
                int endIndex = inputCompanies.ElementAt((i+1) * ScopeLimit - 1).RowNumber;
                result.Add(string.Format("({2}) Wiersze: {0} - {1}", startIndex, endIndex, i+1));
            }

            if (rest > 0)
            {
                result.Add(string.Format("({2}) Wiersze: {0} - {1}", inputCompanies.ElementAt(numOfFullScopes * ScopeLimit).RowNumber, inputCompanies.Last().RowNumber, numOfFullScopes + 1));
            }
            return result;
        }

        private const string _rowBasedContent = "Wiersze";
        private static Regex _regEx = new Regex(@"\([0-9]*\)");
        internal static int GetEndScope(string selectedScope)
        {
           if (selectedScope.Contains(_rowBasedContent))
            {
                string scopeNum = GetScopeNumFromSelectedScope(selectedScope);
                return int.Parse(scopeNum) * ScopeLimit;
            }

            return int.Parse(selectedScope.Substring(selectedScope.IndexOf("-") + 1).Trim());
        }

        private static string GetScopeNumFromSelectedScope(string selectedScope)
        {
            return _regEx.Match(selectedScope).Value.Split('(', ')')[1];
        }

        internal static int GetStartScope(string selectedScope)
        {
            if (selectedScope.Contains(_rowBasedContent))
            {
                if (selectedScope.Contains(_rowBasedContent))
                {
                    string scopeNum = GetScopeNumFromSelectedScope(selectedScope);
                    return (int.Parse(scopeNum) - 1) * ScopeLimit + 1;
                }
            }

            return int.Parse(selectedScope.Substring(0, selectedScope.IndexOf("-")).Trim());
        }
    }
}
