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
        private static Dictionary<string, Company> _inactiveCompanies;

        internal static Dictionary<string, Company> GetInActiveCompanies()
        {
            if (_inactiveCompanies != null)
                return _inactiveCompanies;

            _inactiveCompanies = new Dictionary<string, Company>();

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

        private static IEnumerable<KeyValuePair<string, Company>> GetDataZawieszeniaDzialanosci()
        {
            var result = new Dictionary<string, Company>();

            //Prawna
            result.Add("8442344628", new Company() { NIP = "8442344628" });

            //Fizyczna
            result.Add("5631802823", new Company() { NIP = "5631802823" });
           

            return result;
        }

        private static IEnumerable<KeyValuePair<string, Company>> GetDataZakonczeniaDzialanosci()
        {
            var result = new Dictionary<string, Company>();

            //Prawna
            result.Add("6750001308", new Company() { NIP = "6750001308" });

            //Fizyczna silos=4
           // result.Add("7311680354", new Company() { NIP = "7311680354" });
            //Fizyczna silos=4
            //result.Add("5252044755", new Company() { NIP = "5252044755" });

            //Fizyczna
            result.Add("8281014020", new Company() { NIP = "8281014020" });

            return result;
        }

        static Dictionary<string, Company> GetDataZakoczeniaPostepowaniaUpadlosiowego()
        {
            var result = new Dictionary<string, Company>();

            //Prawna
            result.Add("5992973948", new Company() { NIP = "5992973948"});

            //Fizyczna
            result.Add("1132317562", new Company() { NIP = "1132317562" });


            return result;
        }


        static Dictionary<string, Company> GetDataOrzeczeniaOUpadlosci()
        {
            var result = new Dictionary<string, Company>();

            //Prawna
            result.Add("6861573829", new Company() { NIP = "6861573829" });

            //Fizyczna
            result.Add("7581384594", new Company() { NIP = "7581384594" });


            return result;
        }
    }
}
