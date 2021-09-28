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

        private BiRVerifyResult IsCompanyActive(string nip)
        {
            if (string.IsNullOrEmpty(nip))
            {
                return new BiRVerifyResult(BiRVerifyStatus.NipIncorrect);
            }

            string nipPure = nip.Replace(_dash, string.Empty).Replace(_space, string.Empty).Trim();
            BiRCompany biRCompany = _client.GetCompany(nipPure);
            
            if (biRCompany == null)
                return new BiRVerifyResult(_client.GetLastErrorStatus());

            if (IsActive(biRCompany) ?? true)
            {
                return new BiRVerifyResult(BiRVerifyStatus.IsActive);
            }
            else
            {
                BiRVerifyResult result = new BiRVerifyResult(BiRVerifyStatus.IsNotActive);
                result.Message = GetMessageWithDetailedData(result, biRCompany);
                return result;
            }
        }

        private string GetMessageWithDetailedData(BiRVerifyResult result, BiRCompany biRCompany)
        {
            string fullMessage = result.Message;
            if (biRCompany.ZawieszeniaDate != DateTime.MinValue)
            {
                fullMessage = string.Concat(fullMessage, $" Data zawieszenia działaności: {biRCompany.ZawieszeniaDate.ToShortDateString()}.");
            }
            if (biRCompany.ZakonczeniaDzialalnosciDate != DateTime.MinValue)
            {
                fullMessage = string.Concat(fullMessage, $" Data zakończenia: {biRCompany.ZakonczeniaDzialalnosciDate.ToShortDateString()}.");
            }
            if (biRCompany.SkresleniaRegonDate != DateTime.MinValue)
            {
                fullMessage = string.Concat(fullMessage, $" Data skreślenia z Regon: {biRCompany.SkresleniaRegonDate.ToShortDateString()}.");
            }
            if (biRCompany.OrzeczenieOUpadlosciDate != DateTime.MinValue)
            {
                fullMessage = string.Concat(fullMessage, $" Data orzeczenia o upadłości: {biRCompany.OrzeczenieOUpadlosciDate.ToShortDateString()}.");
            }
            if (biRCompany.ZakonczeniePostepowaniaUpadlosiowegoDate != DateTime.MinValue)
            {
                fullMessage = string.Concat(fullMessage, $" Data zakończenia postępowania upadłościowego: {biRCompany.ZakonczeniePostepowaniaUpadlosiowegoDate.ToShortDateString()}.");
            }

            return fullMessage;
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

        public Dictionary<string, BiRVerifyResult> AreCompaniesActive(List<InputCompany> inputCompanies)
        {
            if (inputCompanies == null || inputCompanies.Count == 0)
                return null;

            Dictionary<string, BiRVerifyResult> result = new Dictionary<string, BiRVerifyResult>();

            foreach (var company in inputCompanies)
            {
                if (company != null)
                {
                    BiRVerifyResult verResult = IsCompanyActive(company.NIP);
                    result.Add(company.ID, verResult);
                    
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
