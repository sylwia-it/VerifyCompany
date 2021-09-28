using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VerifyCompany.Common.Lib;

namespace VerifyActiveCompany.Lib.Test.DataHelpers
{
    static class InActiveCompanyGeneratorForProdDB
    {
        private static Dictionary<string, InputCompany> _inactiveCompanies;

        internal static Dictionary<string, InputCompany> GetInActiveCompanies()
        {
            if (_inactiveCompanies != null)
                return _inactiveCompanies;

            _inactiveCompanies = new Dictionary<string, InputCompany>();

            _inactiveCompanies = _inactiveCompanies.Concat(GetDataZakoczeniaPostepowaniaUpadlosiowego())
               .GroupBy(kv => kv.Key)
               .ToDictionary(g => g.Key, g => g.First().Value);

            _inactiveCompanies = _inactiveCompanies.Concat(GetDataOrzeczeniaOUpadlosci())
            .GroupBy(kv => kv.Key)
            .ToDictionary(g => g.Key, g => g.First().Value);

            _inactiveCompanies = _inactiveCompanies.Concat(GetDataZakonczeniaDzialanosci())
            .GroupBy(kv => kv.Key)
            .ToDictionary(g => g.Key, g => g.First().Value);

            _inactiveCompanies = _inactiveCompanies.Concat(GetDataZawieszeniaDzialanosci())
            .GroupBy(kv => kv.Key)
            .ToDictionary(g => g.Key, g => g.First().Value);

            return _inactiveCompanies;
        }

        private static IEnumerable<KeyValuePair<string, InputCompany>> GetDataZawieszeniaDzialanosci()
        {
            var result = new Dictionary<string, InputCompany>();
            InputCompany tempComp;

            //Prawna
            tempComp = new InputCompany() { NIP = "8442344628", LP = "3948", RowNumber = 1 };
            result.Add(tempComp.ID, tempComp);

            //Fizyczna
            tempComp = new InputCompany() { NIP = "5631802823", LP = "3948", RowNumber = 1 };
            result.Add(tempComp.ID, tempComp);

            return result;
        }

        private static IEnumerable<KeyValuePair<string, InputCompany>> GetDataZakonczeniaDzialanosci()
        {
            var result = new Dictionary<string, InputCompany>();
            InputCompany tempComp;

            //Prawna
            tempComp = new InputCompany() { NIP = "6750001308", LP = "3948", RowNumber = 1 };
            result.Add(tempComp.ID, tempComp);

            //Fizyczna silos=4
            // result.Add("7311680354", new Company() { NIP = "7311680354" });
            //Fizyczna silos=4
            //result.Add("5252044755", new Company() { NIP = "5252044755" });

            //Fizyczna
            tempComp = new InputCompany() { NIP = "8281014020", LP = "3948", RowNumber = 1 };
            result.Add(tempComp.ID, tempComp);

            return result;
        }

        static Dictionary<string, InputCompany> GetDataZakoczeniaPostepowaniaUpadlosiowego()
        {
            var result = new Dictionary<string, InputCompany>();
            InputCompany tempComp;

            //Prawna
            tempComp = new InputCompany() { NIP = "5992973948", LP="3948", RowNumber=1};
            result.Add(tempComp.ID, tempComp);
            //Fizyczna
            tempComp = new InputCompany() { NIP = "1132317562", LP = "3948", RowNumber = 1 };
            result.Add(tempComp.ID, tempComp);

            return result;
        }


        static Dictionary<string, InputCompany> GetDataOrzeczeniaOUpadlosci()
        {
            var result = new Dictionary<string, InputCompany>();
            InputCompany tempComp;

            //Prawna
            tempComp = new InputCompany() { NIP = "6861573829", LP = "3948", RowNumber = 1 };
            result.Add(tempComp.ID, tempComp);

            //Fizyczna
            tempComp = new InputCompany() { NIP = "7581384594", LP = "3948", RowNumber = 1 };
            result.Add(tempComp.ID, tempComp);

            return result;
        }
    }
}
