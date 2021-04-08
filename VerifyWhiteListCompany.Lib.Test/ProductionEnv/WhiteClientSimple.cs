using NUnit.Framework;
using VerifyWhiteListCompany.Lib.WebServiceModel;
using System.Linq;
using System.Collections.Generic;
using VerifyWhiteListCompany.Lib.Test.Helpers;
using System;

namespace VerifyWhiteListCompany.Lib.Test.ProductionEnv
{
    public class WhiteClientSimpleProd
    {
        WhiteListClient _client;
        
        [SetUp]
        public void Setup()
        {
            _client = new WhiteListClient();
            string prodEnvUrl = UrlProvider.ProductionUrl;
            _client.Init(prodEnvUrl);
           
            
        }

        [Test]
        public void TestSingleCorrectCompany()
        {
            string nipToCheck = VerifyCompany.Common.Test.Lib.CompanyGenerator.GetCorrectNIP();
            var a = _client.VerifyCompanies(nipToCheck).GetAwaiter().GetResult();
            Assert.IsNotNull(a.Result.RequestId);
            Assert.IsNotNull(a.Result.RequestDateTime);
            Assert.IsNotNull(a.Result.Entries);

            Entry result = a.Result.Entries.FirstOrDefault();
            Assert.AreEqual(nipToCheck, result.Identifier);
            Assert.AreEqual(nipToCheck, result.Subjects[0].Nip);
            Assert.AreEqual("Czynny", result.Subjects[0].StatusVat);
        }

        [Test]
        public void TestSingleInActiveCompany()
        {
            string nipToCheck = VerifyCompany.Common.Test.Lib.CompanyGenerator.GetZakonczoneUpadaloscioweNIP();
            var a = _client.VerifyCompanies(nipToCheck).GetAwaiter().GetResult();
            Assert.IsNotNull(a.Result.RequestId);
            Assert.IsNotEmpty(a.Result.RequestId);
            Assert.IsNotNull(a.Result.RequestDateTime);
            Assert.IsNotEmpty(a.Result.RequestDateTime);
            Assert.IsNotNull(a.Result.Entries);

            Entry result = a.Result.Entries.FirstOrDefault();
            Assert.AreEqual(nipToCheck, result.Identifier);
            Assert.AreEqual(nipToCheck, result.Subjects[0].Nip);
            Assert.AreNotEqual("Czynny", result.Subjects[0].StatusVat);
      
        }

        [Test]
        public void TestSingleInCorrectCompany()
        {
            string nipToCheck = "1234567890";

            var a = _client.VerifyCompanies(nipToCheck).GetAwaiter().GetResult();
            Assert.IsNotNull(a.Result.RequestId);
            Assert.IsNotEmpty(a.Result.RequestId);
            Assert.IsNotNull(a.Result.RequestDateTime);
            Assert.IsNotEmpty(a.Result.RequestDateTime);
            Assert.IsNotNull(a.Result.Entries);

            Entry result = a.Result.Entries.FirstOrDefault();
            Assert.AreEqual(0, result.Subjects.Count);
            //Assert.IsNull(result);

        }

        [Test]
        public void TestSingleEmptyNipCompany()
        {
            string nipToCheck = string.Empty;

            EntryListResponse eLR = _client.VerifyCompanies(nipToCheck).GetAwaiter().GetResult();
            Assert.IsTrue(DTHelper.IsItToday(eLR.Result.RequestDateTime));
            Assert.IsNotNull(eLR.Result.RequestId);
            Assert.IsNotEmpty(eLR.Result.RequestId);

            Assert.AreEqual(nipToCheck, eLR.Result.Entries[0].Identifier);
            Assert.AreEqual("WL-112", eLR.Result.Entries[0].Error.Code);
        }

        [Test]
        public void TestSingleTooShortNipCompany()
        {
            string nipToCheck = "123";

            EntryListResponse eLR = _client.VerifyCompanies(nipToCheck).GetAwaiter().GetResult();
            Assert.IsTrue(DTHelper.IsItToday(eLR.Result.RequestDateTime));
            Assert.IsNotNull(eLR.Result.RequestId);
            Assert.IsNotEmpty(eLR.Result.RequestId);

            Assert.AreEqual(nipToCheck, eLR.Result.Entries[0].Identifier);
            Assert.AreEqual("WL-113", eLR.Result.Entries[0].Error.Code);

        }


        [Test]
        public void TestSingleTooLongNipCompany()
        {
            string nipToCheck = "123456789190";

            EntryListResponse eLR = _client.VerifyCompanies(nipToCheck).GetAwaiter().GetResult();
            Assert.IsTrue(DTHelper.IsItToday(eLR.Result.RequestDateTime));
            Assert.IsNotNull(eLR.Result.RequestId);
            Assert.IsNotEmpty(eLR.Result.RequestId);

            Assert.AreEqual(nipToCheck, eLR.Result.Entries[0].Identifier);
            Assert.AreEqual("WL-113", eLR.Result.Entries[0].Error.Code);

        }

        [Test]
        public void TestSingleWithIncorrectCharsNipCompany()
        {
            List<string> nipsToCheck = new List<string>() { "1w34567890", "1 23456789" };

            foreach (var nipToCheck in nipsToCheck)
            {
                EntryListResponse eLR = _client.VerifyCompanies(nipToCheck).GetAwaiter().GetResult();
                Assert.IsTrue(DTHelper.IsItToday(eLR.Result.RequestDateTime));
                Assert.IsNotNull(eLR.Result.RequestId);
                Assert.IsNotEmpty(eLR.Result.RequestId);

                Assert.AreEqual(nipToCheck, eLR.Result.Entries[0].Identifier);
                Assert.AreEqual("WL-114", eLR.Result.Entries[0].Error.Code);
            }

        }

        /// <summary>
        /// API eliminates the special sings itself
        /// </summary>
        [Test]
        public void TestSingleWithIncorrectCharsButEliminatedByApiNipCompany()
        {
            List<string> nipsToCheck = new List<string>() { "1.34567890", "1+3456789", "1_3456789", "1~3456789" };

            foreach (var nipToCheck in nipsToCheck)
            {
                EntryListResponse eLR = _client.VerifyCompanies(nipToCheck).GetAwaiter().GetResult();
                Assert.IsTrue(DTHelper.IsItToday(eLR.Result.RequestDateTime));
                Assert.IsNotNull(eLR.Result.RequestId);
                Assert.IsNotEmpty(eLR.Result.RequestId);

                Assert.AreEqual(nipToCheck, eLR.Result.Entries[0].Identifier);
                Assert.IsTrue(eLR.Result.Entries[0].Error.Code.Equals("WL-113") || eLR.Result.Entries[0].Error.Code.Equals("WL-114"));
                Console.WriteLine(eLR.Result.Entries[0].Error.Code);
            }

        }

    }
}