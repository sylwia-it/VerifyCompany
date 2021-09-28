using System;
using VerifyActiveCompany.Lib;
using NUnit.Framework;
using System.Collections.Generic;
using VerifyCompany.Common.Test.Lib;
using VerifyCompany.Common.Lib;
using System.Linq;
using VerifyActiveCompany.Lib.Test.DataHelpers;

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
            Dictionary<string, InputCompany> correctCompanies = CompanyGenerator.GetCorrectCompanies();
            Dictionary<string, BiRVerifyResult> verResults = _verifier.AreCompaniesActive(correctCompanies.Values.ToList<InputCompany>());

            Assert.AreEqual(correctCompanies.Count, verResults.Count);
            Assert.AreEqual(0, correctCompanies.Keys.Except(verResults.Keys).Count());


            foreach(var result in verResults)
            {
                Assert.AreEqual(BiRVerifyStatus.IsActive, result.Value.BiRVerifyStatus);
            }
        }

        [Test]
        public void CorrectNipsPrawnaMulitiple()
        {
            Dictionary<string, InputCompany> correctCompanies = CompanyGenerator.GetCorrectCompanies();
            var correctC = correctCompanies.Values.ToList();
            for (int i = 0; i < correctC.Count(); i++)
            {
                var c = correctC[i];
                InputCompany tempCompany = new InputCompany() { RowNumber= c.RowNumber+20, NIP = c.NIP, LP = c.LP};
                correctCompanies.Add(tempCompany.ID, tempCompany);
                tempCompany = new InputCompany() { RowNumber = c.RowNumber + 120, NIP = c.NIP, LP = c.LP };
                correctCompanies.Add(tempCompany.ID, tempCompany);
            }

            Dictionary<string, BiRVerifyResult> verResults = _verifier.AreCompaniesActive(correctCompanies.Values.ToList<InputCompany>());

            Assert.AreEqual(correctCompanies.Count, verResults.Count);
            Assert.AreEqual(0, correctCompanies.Keys.Except(verResults.Keys).Count());


            foreach (var result in verResults)
            {
                Assert.AreEqual(BiRVerifyStatus.IsActive, result.Value.BiRVerifyStatus);
            }
        }

        [Test]
        public void CorrectNipsFizyczna()
        {
            Dictionary<string, InputCompany> correctCompanies = CompanyGenerator.GetCorrectPhisicalCompanies();
            Dictionary<string, BiRVerifyResult> verResults = _verifier.AreCompaniesActive(correctCompanies.Values.ToList<InputCompany>());

            Assert.AreEqual(correctCompanies.Count, verResults.Count);
            Assert.AreEqual(0, correctCompanies.Keys.Except(verResults.Keys).Count());


            foreach (var result in verResults)
            {
                Assert.AreEqual(BiRVerifyStatus.IsActive, result.Value.BiRVerifyStatus);
            }
        }

        [Test]
        public void InActiveCompanyTest()
        {
            Dictionary<string, InputCompany> inActiveCompanies = InActiveCompanyGeneratorForProdDB.GetInActiveCompanies();
            Dictionary<string, BiRVerifyResult> verResults = _verifier.AreCompaniesActive(inActiveCompanies.Values.ToList<InputCompany>());

            Assert.AreEqual(inActiveCompanies.Count, verResults.Count);
            Assert.AreEqual(0, inActiveCompanies.Keys.Except(verResults.Keys).Count());

            foreach (var result in verResults)
            {
                Assert.AreEqual(BiRVerifyStatus.IsNotActive, result.Value.BiRVerifyStatus);
            }


        }

        [Test]
        public void EmptyInput()
        {
            Dictionary<string, BiRVerifyResult> verResults = _verifier.AreCompaniesActive(new List<InputCompany>());
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
            Dictionary<string, InputCompany> incorrectCompanies = CompanyGenerator.GetInCorrectNipCompanies();
            Dictionary<string, BiRVerifyResult> verResults = _verifier.AreCompaniesActive(incorrectCompanies.Values.ToList<InputCompany>());

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
            List<InputCompany> companies = new List<InputCompany>();
            companies.Add( new InputCompany() { NIP = string.Empty });
            Dictionary<string, BiRVerifyResult> verResults = _verifier.AreCompaniesActive(companies);


            foreach (var c in verResults)
            {
                Assert.AreEqual(BiRVerifyStatus.NipIncorrect, c.Value.BiRVerifyStatus);
            }
        }

        [Test]
        public void NullCompanyInput()
        {
            List<InputCompany> companies = new List<InputCompany>();
            companies.Add(null);
            Dictionary<string, BiRVerifyResult> verResults = _verifier.AreCompaniesActive(companies);

           Assert.AreEqual(0, verResults.Count());
            
        }
    }
}
