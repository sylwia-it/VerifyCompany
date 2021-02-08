using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using VerifyWhiteListCompany.Lib.WebServiceModel;

namespace VerifyWhiteListCompany.Lib.Test
{
    public class WhiteListClientFactoryTests
    {
        [Test]
        public void DefaultClientTest()
        {
            IWhiteListClient client = WhiteListClientFactory.GetClient();

            Assert.IsInstanceOf(typeof(WhiteListClient), client);
        }

        [Test]
        public void SetClientTest()
        {
            IWhiteListClient client = new TestWhiteLictClient();
            IWhiteListClient defaultClient = WhiteListClientFactory.GetClient();
            Assert.IsInstanceOf(typeof(WhiteListClient), defaultClient);
            Assert.IsNotInstanceOf(typeof(TestWhiteLictClient), defaultClient);
            
            WhiteListClientFactory.SetClient(client);
            IWhiteListClient clientToTest = WhiteListClientFactory.GetClient();
            Assert.IsNotInstanceOf(typeof(WhiteListClient), clientToTest);
            Assert.IsInstanceOf(typeof(TestWhiteLictClient), clientToTest);

        }
    }

    public class TestWhiteLictClient : IWhiteListClient
    {
        public void Init(string url)
        {
            throw new NotImplementedException();
        }

        public Task<EntityListResponse> VerifyCompanies(string nips)
        {
            throw new NotImplementedException();
        }
    }

}
