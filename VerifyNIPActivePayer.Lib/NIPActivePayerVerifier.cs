using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;
using VerifyCompany.Common.Lib;
using VerifyNIP.Service;

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

        public VerifyNIPResult VerifyNIP(string nipToVerify, DateTime date)
        {

            if (client == null)
            {
                return VerifyNIPResult.ErrorInClientSetUp;
            }
            if (string.IsNullOrEmpty(nipToVerify.Trim()))
            {
                return VerifyNIPResult.NIPNotCorrect;
            }

            if (DateTime.MinValue == date)
            {
                return VerifyNIPResult.ErrorDateNotCorrect;
            }

            try
            {
                var response = client.SprawdzNIPNaDzien(new SprawdzNIPNaDzienZapytanie(nipToVerify, date));
                return ConvertServiceResponseToResult(response.WynikOperacji);

            }
            
            catch (FaultException)
            {
                return VerifyNIPResult.Error;
            }
        }



        public Dictionary<string, VerifyNIPResult> VerifyNIPs(List<InputCompany> companiesToVerify)
        {
            return VerifyNIPs(companiesToVerify, false);
        }
        public Dictionary<string, VerifyNIPResult> VerifyNIPs(List<InputCompany> companiesToVerify, bool verifyForInvoiceDay)
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
                    throw new NullReferenceException("One element of the companiesToVerify is null.");
                }
                VerifyNIPResult verificationResult;
                if (verifyForInvoiceDay)
                {
                    verificationResult = VerifyNIP(company.NIP, company.InvoiceDate);
                }
                else
                {
                    verificationResult = VerifyNIP(company.NIP);
                }
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
