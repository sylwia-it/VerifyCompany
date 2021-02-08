using NUnit.Framework;
using VerifyWhiteListCompany.Lib.WebServiceModel;
using System.Linq;
using System.Collections.Generic;

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
            Assert.IsNotNull(a.Result.Subjects);

            Entity result = a.Result.Subjects.FirstOrDefault();
            Assert.AreEqual(nipToCheck, result.Nip);
            Assert.AreEqual("Czynny", result.StatusVat);
        }

        [Test]
        public void TestSingleInActiveCompany()
        {
            string nipToCheck = VerifyCompany.Common.Test.Lib.CompanyGenerator.GetZakonczoneUpadaloscioweNIP();
            var a = _client.VerifyCompanies(nipToCheck).GetAwaiter().GetResult();
            Assert.IsNotNull(a.Result.RequestId);
            Assert.IsNotEmpty(a.Result.RequestId);
            Assert.IsNotNull(a.Result.Subjects);

            Entity result = a.Result.Subjects.FirstOrDefault();
            Assert.AreEqual(nipToCheck, result.Nip);
            Assert.AreNotEqual("Czynny", result.StatusVat);
      
        }

        [Test]
        public void TestSingleInCorrectCompany()
        {
            string nipToCheck = "1234567890";

            var a = _client.VerifyCompanies(nipToCheck).GetAwaiter().GetResult();
            Assert.IsNotNull(a.Result.RequestId);
            Assert.IsNotEmpty(a.Result.RequestId);
            Assert.IsNotNull(a.Result.Subjects);

            Entity result = a.Result.Subjects.FirstOrDefault();
            Assert.AreEqual(0, a.Result.Subjects.Count);
            Assert.IsNull(result);

        }

        [Test]
        public void TestSingleEmptyNipCompany()
        {
            string nipToCheck = string.Empty;

            var e = Assert.Throws<WhiteListClientException>(() => _client.VerifyCompanies(nipToCheck).GetAwaiter().GetResult());
            var innerException = e.GetException();
            var code = innerException.Code;
            Assert.AreEqual("WL-112", code);

        }

        [Test]
        public void TestSingleTooShortNipCompany()
        {
            string nipToCheck = "123";

            var e = Assert.Throws<WhiteListClientException>(() => _client.VerifyCompanies(nipToCheck).GetAwaiter().GetResult());
            var innerException = e.GetException();
            var code = innerException.Code;
            Assert.AreEqual("WL-113", code);

        }


        [Test]
        public void TestSingleTooLongNipCompany()
        {
            string nipToCheck = "123456789190";

            var e = Assert.Throws<WhiteListClientException>(() => _client.VerifyCompanies(nipToCheck).GetAwaiter().GetResult());
            var innerException = e.GetException();
            var code = innerException.Code;
            Assert.AreEqual("WL-113", code);

        }

        [Test]
        public void TestSingleWithIncorrectCharsNipCompany()
        {
            List<string> nipsToCheck = new List<string>() { "1w34567890", "1 23456789" };

            foreach (var nipToCheck in nipsToCheck)
            {
                var e = Assert.Throws<WhiteListClientException>(() => _client.VerifyCompanies(nipToCheck).GetAwaiter().GetResult());
                var innerException = e.GetException();
                var code = innerException.Code;
                Assert.AreEqual("WL-114", code);
            }

        }

        /// <summary>
        /// API eliminates the special sings itself
        /// </summary>
        [Test]
        public void TestSingleWithIncorrectCharsButEliminatedByApiNipCompany()
        {
            List<string> nipsToCheck = new List<string>() { "1,34567890", "1%3456789", "1_3456789", "1~3456789" };

            foreach (var nipToCheck in nipsToCheck)
            {
                var e = Assert.Throws<WhiteListClientException>(() => _client.VerifyCompanies(nipToCheck).GetAwaiter().GetResult());
                var innerException = e.GetException();
                var code = innerException.Code;
                Assert.AreEqual("WL-113", code);
            }

        }

    }
}