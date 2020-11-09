using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;
using VerifyCompany.Common.Lib;
using VerifyNIP.Service;
using static VerifyNIP.Service.WeryfikacjaVATClient;

namespace VerifyNIPActivePayer.Lib
{
    /// <summary>
    /// Verifier is responsible for verificaion if a given organization is an active tax payer.
    /// according to the Polish Ministry of Finance.
    /// </summary>
    public class NIPActivePayerVerifier
    {
        private readonly WeryfikacjaVAT client;
        
        public NIPActivePayerVerifier()
        {
            client = WeryfikacjaVATClientFactory.GetWeryfikacjaVATClient();
        }
       
        public VerifyNIPResult VerifyNIP(string nipToVerify)
        {

            if (client == null)
            {
                return VerifyNIPResult.ErrorInClientSetUp;
            }   
            if (string.IsNullOrEmpty(nipToVerify.Trim()))
            {
                return VerifyNIPResult.NIPNotCorrect;
            }

            try
            {
                var response = client.SprawdzNIP(new SprawdzNIPZapytanie(nipToVerify));
                return ConvertServiceResponseToResult(response.WynikOperacji);

            }
            catch (FaultException)
            {
                return VerifyNIPResult.Error;
            }
        }

        public Dictionary<string, VerifyNIPResult> VerifyNIPs(List<Company> companiesToVerify)
        {
            if (companiesToVerify == null)
            {
                throw new ArgumentNullException("companiesToVerify", "companiesToVerify is null.");
            }

           
            Dictionary<string, VerifyNIPResult> result = new Dictionary<string, VerifyNIPResult>();
                       

            foreach (var company in companiesToVerify)
            {
                if (company == null)
                {
                    throw new ArgumentNullException("element of companiesToVerify", "One of the companiesToVerify is null.");
                }
                var verificationResult = VerifyNIP(company.NIP);
                result.Add(company.ID, verificationResult);
                Thread.Sleep(100);
            }

            return result;
        }

        private VerifyNIPResult ConvertServiceResponseToResult(TWynikWeryfikacjiVAT response)
        {
            if (response.Kod == TKodWeryfikacjiVAT.C)
            {
                return VerifyNIPResult.IsActiveVATPayer;
            }
            if (response.Kod == TKodWeryfikacjiVAT.D)
            {
                return VerifyNIPResult.ServiceRequestError;
            }
            if (response.Kod == TKodWeryfikacjiVAT.I)
            {
                return VerifyNIPResult.NIPNotCorrect;
            }
            if (response.Kod == TKodWeryfikacjiVAT.N)
            {
                return VerifyNIPResult.NotRegisteredVATPayer;
            }
            if (response.Kod == TKodWeryfikacjiVAT.X)
            {
                return VerifyNIPResult.ServiceIsNotAvailable;
            }

            if (response.Kod == TKodWeryfikacjiVAT.Z)
            {
                return VerifyNIPResult.NotRegisteredVATPayer;
            }

            return VerifyNIPResult.Error;
        }
    }
}
