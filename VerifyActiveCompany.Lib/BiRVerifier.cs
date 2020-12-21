using System;
using System.Collections.Generic;
using System.Text;
using VerifyActiveCompany.Lib.Service;
using VerifyCompany.Common.Lib;

namespace VerifyActiveCompany.Lib
{
    public class BiRVerifier
    {
        private const string _dash = "-";
        private const string _space = " ";
        IBiRClient _client;

        public BiRVerifier()
        {
            _client = BiRClientFactory.GetClient();
        }

        private BiRVerifyStatus IsCompanyActive(string nip)
        {
            if (string.IsNullOrEmpty(nip))
            {
                return BiRVerifyStatus.NipIncorrect;
            }

            string nipPure = nip.Replace(_dash, string.Empty).Replace(_space, string.Empty).Trim();
            BiRCompany biRCompany = _client.GetCompany(nipPure);
            
            if (biRCompany == null)
                return _client.GetLastErrorStatus();

            if (IsActive(biRCompany) ?? true)
                return BiRVerifyStatus.IsActive;
            else
                return BiRVerifyStatus.IsNotActive;
        }

        private bool? IsActive(BiRCompany biRCompany)
        {
            if (biRCompany.CompanyType == BiRCompanyType.Prawna)
            {
                if (biRCompany.ZawieszeniaDate == DateTime.MinValue && biRCompany.ZakonczeniaDzialalnosciDate == DateTime.MinValue && biRCompany.SkresleniaRegonDate == DateTime.MinValue && biRCompany.OrzeczenieOUpadlosciDate == DateTime.MinValue && biRCompany.ZakonczeniePostepowaniaUpadlosiowegoDate == DateTime.MinValue)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            else // Os. Fizyczna
            {
                if (biRCompany.ZawieszeniaDate == DateTime.MinValue && biRCompany.ZakonczeniaDzialalnosciDate == DateTime.MinValue && biRCompany.SkresleniaRegonDate == DateTime.MinValue && biRCompany.OrzeczenieOUpadlosciDate == DateTime.MinValue && biRCompany.ZakonczeniePostepowaniaUpadlosiowegoDate == DateTime.MinValue)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public Dictionary<string, BiRVerifyStatus> AreCompaniesActive(Dictionary<string, Company> inputCompanies)
        {
            if (inputCompanies == null || inputCompanies.Count == 0)
                return null;

            Dictionary<string, BiRVerifyStatus> result = new Dictionary<string, BiRVerifyStatus>();

            foreach (var company in inputCompanies)
            {
                if (company.Value != null)
                {
                    BiRVerifyStatus verResult = IsCompanyActive(company.Value.NIP);
                    result.Add(company.Key, verResult);
                }
                else
                {
                    result.Add(company.Key, BiRVerifyStatus.CompanyIsNull);
                }
            }
            
            
            return result;
        }

        public void Finish()
        {
            _client.Close(); 
        }
        
        
    }
}
