using System;
using System.Text;
using System.Collections.Generic;
using VerifyCompany.Common.Lib;
using NUnit.Framework;
using VerifyCompany.Common.Test.Lib;
using VerifyActiveCompany.Lib;
using System.Linq;
using VerifyActiveCompany.Lib.Test.DataHelpers;

namespace VerifyActiveCompany.Lib.Test.TestEnv
{
   
    public class SimpleBiRVerifierTestTests
    {
        BiRVerifier _verifier;

        [OneTimeSetUp]
        public void CreateVerifier()
        {
            IBiRClient client = new BiRClient(TestBiRClientHelper.Url, TestBiRClientHelper.Url);
            client.Init();
            BiRClientFactory.SetClient(client);
            _verifier = new BiRVerifier();
        }
        
        [OneTimeTearDown]
        public void DeleteTestClient()
        {
            _verifier.Finish();

            BiRClientFactory.SetClient(null);
            
        }


        [Test]
        public void CorrectNips()
        {
            Dictionary<string, Company> correctCompanies = CompanyGenerator.GetCorrectCompanies();
            Dictionary<string, BiRVerifyResult> verResults = _verifier.AreCompaniesActive(correctCompanies);

            Assert.AreEqual(correctCompanies.Count, verResults.Count);
            Assert.AreEqual(0, correctCompanies.Keys.Except(verResults.Keys).Count());


            foreach (var result in verResults)
            {
                Assert.AreEqual(BiRVerifyStatus.IsActive, result.Value.BiRVerifyStatus);
            }
        }

  

        [Test]
        public void EmptyInput()
        {
            Dictionary<string, BiRVerifyResult> verResults = _verifier.AreCompaniesActive(new Dictionary<string, Company>());
            Assert.IsNull(verResults);
        }

        [Test]
        public void NullInput()
        {
            Dictionary<string, BiRVerifyResult> verResults = _verifier.AreCompaniesActive(null);
            Assert.IsNull(verResults);
        }

        [Test]
        public void InCorrectNipInput()
        {
            Dictionary<string, Company> incorrectCompanies = CompanyGenerator.GetInCorrectNipCompanies();
            Dictionary<string, BiRVerifyResult> verResults = _verifier.AreCompaniesActive(incorrectCompanies);

            Assert.AreEqual(incorrectCompanies.Count, verResults.Count);
            Assert.AreEqual(0, incorrectCompanies.Keys.Except(verResults.Keys).Count());


            foreach (var result in verResults)
            {
                Assert.AreEqual(BiRVerifyStatus.NotFound, result.Value.BiRVerifyStatus);
            }
        }

        [Test]
        public void NullAndEmptyNipInput()
        {
            Dictionary<string, Company> companies = new Dictionary<string, Company>();
            companies.Add("def", new Company() { NIP = string.Empty });
            companies.Add("ghi", new Company() { NIP = null });
            Dictionary<string, BiRVerifyResult> verResults = _verifier.AreCompaniesActive(companies);


            foreach (var c in verResults)
            {
                Assert.AreEqual(BiRVerifyStatus.NipIncorrect, c.Value.BiRVerifyStatus);
            }
        }

        [Test]
        public void NullCompanyInput()
        {
            Dictionary<string, Company> companies = new Dictionary<string, Company>();
            companies.Add("abc", null);
            Dictionary<string, BiRVerifyResult> verResults = _verifier.AreCompaniesActive(companies);

            foreach (var verResult in verResults)
            {
                Assert.AreEqual(BiRVerifyStatus.CompanyIsNull, verResult.Value.BiRVerifyStatus);
            }
        }

    }
}
