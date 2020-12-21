using System;
using VerifyActiveCompany.Lib;
using NUnit.Framework;
using System.Collections.Generic;
using VerifyCompany.Common.Test.Lib;
using VerifyCompany.Common.Lib;
using System.Linq;

namespace VerifyActiveCompany.Lib.Test
{ 
    public class SimpleBiRVerifierTests
    {
        BiRVerifier _verifier;

        [OneTimeSetUp]
        public void CreateVerifier()
        {
            _verifier = new BiRVerifier();
        }

        [OneTimeTearDown]
        public void Close()
        {
            _verifier.Finish();
        }

        [Test]
        public void CorrectNipsPrawna()
        {
            Dictionary<string, Company> correctCompanies = CompanyGenerator.GetCorrectCompanies();
            Dictionary<string, BiRVerifyStatus> verResults = _verifier.AreCompaniesActive(correctCompanies);

            Assert.AreEqual(correctCompanies.Count, verResults.Count);
            Assert.AreEqual(0, correctCompanies.Keys.Except(verResults.Keys).Count());


            foreach(var result in verResults)
            {
                Assert.AreEqual(BiRVerifyStatus.IsActive, result.Value);
            }
        }

        [Test]
        public void CorrectNipsFizyczna()
        {
            Dictionary<string, Company> correctCompanies = CompanyGenerator.GetCorrectPhisicalCompanies();
            Dictionary<string, BiRVerifyStatus> verResults = _verifier.AreCompaniesActive(correctCompanies);

            Assert.AreEqual(correctCompanies.Count, verResults.Count);
            Assert.AreEqual(0, correctCompanies.Keys.Except(verResults.Keys).Count());


            foreach (var result in verResults)
            {
                Assert.AreEqual(BiRVerifyStatus.IsActive, result.Value);
            }
        }

        [Test]
        public void EmptyInput()
        {
            Dictionary<string, BiRVerifyStatus> verResults = _verifier.AreCompaniesActive(new Dictionary<string, Company>());
            Assert.IsNull(verResults);
        }

        [Test]
        public void NullInput()
        {
            Dictionary<string, BiRVerifyStatus> verResults = _verifier.AreCompaniesActive(null);
            Assert.IsNull(verResults);
        }

        [Test]
        public void InCorrectNipInput()
        {
            Dictionary<string, Company> incorrectCompanies = CompanyGenerator.GetInCorrectNipCompanies();
            Dictionary<string, BiRVerifyStatus> verResults = _verifier.AreCompaniesActive(incorrectCompanies);

            Assert.AreEqual(incorrectCompanies.Count, verResults.Count);
            Assert.AreEqual(0, incorrectCompanies.Keys.Except(verResults.Keys).Count());


            foreach (var result in verResults)
            {
                Assert.AreEqual(BiRVerifyStatus.NotFound, result.Value);
            }
        }

        [Test]
        public void NullAndEmptyNipInput()
        {
            Dictionary<string, Company> companies = new Dictionary<string, Company>();
            companies.Add("def", new Company() { NIP = string.Empty });
            companies.Add("ghi", new Company() { NIP = null});
            Dictionary<string, BiRVerifyStatus> verResults = _verifier.AreCompaniesActive(companies);


            foreach (var c in verResults)
            {
                Assert.AreEqual(BiRVerifyStatus.NipIncorrect, c.Value);
            }
        }

        [Test]
        public void NullCompanyInput()
        {
            Dictionary<string, Company> companies = new Dictionary<string, Company>();
            companies.Add("abc", null);
            Dictionary<string, BiRVerifyStatus> verResults = _verifier.AreCompaniesActive(companies);

            foreach (var verResult in verResults)
            {
                Assert.AreEqual(BiRVerifyStatus.CompanyIsNull, verResult.Value);
            }
        }
    }
}
