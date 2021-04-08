using NUnit.Framework;
using VerifyWhiteListCompany.Lib.WebServiceModel;
using System.Linq;
using System.Collections.Generic;
using System;

namespace VerifyWhiteListCompany.Lib.Test.ProductionEnv
{
    public class WhiteListCompanyVerifierPerfomanceTests
    {
        WhiteListClient _client;

        [SetUp]
        public void Setup()
        {
            _client = new WhiteListClient();
            string prodEnvUrl = UrlProvider.ProductionUrl;
            _client.Init(prodEnvUrl);


        }
        /// <summary>
        /// The API has the limit of 10 requests and 30 companies in one
        /// </summary>
        public void TestTooManyRequests()
        {
            Random r = new Random((int)DateTime.Now.ToBinary());
            List<string> nipsToCheck = VerifyCompany.Common.Test.Lib.CompanyGenerator.GetCorrectNIPs();

            List<string> nipsInString = new List<string>();
            int max = nipsToCheck.Count;
            for (int i = 0; i < 11; i++)
            {
                string nipChunkToCheck = string.Empty;
                for (int j = 0; j < 30; j++)
                {
                    nipChunkToCheck += nipsToCheck[r.Next(0, max)] + ",";
                }
                nipChunkToCheck.Remove(nipChunkToCheck.Length - 1);
                nipsInString.Add(nipChunkToCheck);
            }

            for (int i = 0; i < 10; i++)
            {
                var b = _client.VerifyCompanies(nipsInString[i]).GetAwaiter().GetResult();
                Assert.IsNotNull(b.Result.RequestId);
                Assert.IsNotNull(b.Result.RequestDateTime);
                Assert.IsNotNull(b.Result.Entries);
                //Assert.AreEqual(30, b.Result.Subjects.Count);
            }

            _client.VerifyCompanies(nipsInString[10]).GetAwaiter().GetResult();
            _client.VerifyCompanies(nipsInString[9]).GetAwaiter().GetResult();
            _client.VerifyCompanies(nipsInString[8]).GetAwaiter().GetResult();
            _client.VerifyCompanies(nipsInString[7]).GetAwaiter().GetResult();
            _client.VerifyCompanies(nipsInString[6]).GetAwaiter().GetResult();
            _client.VerifyCompanies(nipsInString[5]).GetAwaiter().GetResult();
            _client.VerifyCompanies(nipsInString[4]).GetAwaiter().GetResult();
            _client.VerifyCompanies(nipsInString[3]).GetAwaiter().GetResult();
            _client.VerifyCompanies(nipsInString[2]).GetAwaiter().GetResult();
            _client.VerifyCompanies(nipsInString[1]).GetAwaiter().GetResult();
            _client.VerifyCompanies(nipsInString[0]).GetAwaiter().GetResult();
        }

        /// <summary>
        /// The API has the limit of 10 requests and 30 companies in one
        /// </summary>
        [Test]
        public void TestTooManyCompaniesInRequest()
        {

            Random r = new Random((int)DateTime.Now.ToBinary());
            List<string> nipsToCheck = VerifyCompany.Common.Test.Lib.CompanyGenerator.CorrectNIPs;

            int max = nipsToCheck.Count;
            string nipChunkToCheck = string.Empty;

            for (int i = 0; i < nipsToCheck.Count; i++)
            {
                nipChunkToCheck += nipsToCheck[i] + ",";
            }
            nipChunkToCheck.Remove(nipChunkToCheck.Length - 1);
                       

            var exception = Assert.Throws<WhiteListClientException>( () => _client.VerifyCompanies(nipChunkToCheck).GetAwaiter().GetResult());

            var innerException = exception.GetException();
            Assert.AreEqual("WL-130", innerException.Code);

        }



    }
}