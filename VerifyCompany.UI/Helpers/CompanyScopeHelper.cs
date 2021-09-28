using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


            result.Add(string.Format("{0} - {1}", numOfFullScopes * ScopeLimit + 1, numberOfCompanies));

            return result;
        }

        internal static int GetEndScope(string selectedScope)
        {
            return int.Parse(selectedScope.Substring(selectedScope.IndexOf("-") + 1).Trim());
        }

        internal static int GetStartScope(string selectedScope)
        {
            return int.Parse(selectedScope.Substring(0, selectedScope.IndexOf("-")).Trim());
        }
    }
}
