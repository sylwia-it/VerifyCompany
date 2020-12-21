using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using VerifyActiveCompany.Lib.Test.DataHelpers;

namespace VerifyActiveCompany.Lib.Test.TestEnv
{
    class BiRClientFactoryTests
    {
        [Test]
        public void SetTestClientTest()
        {
            IBiRClient clientSet = new BiRClient(TestBiRClientHelper.Url, TestBiRClientHelper.Url);
       
            BiRClientFactory.SetClient(clientSet);
            IBiRClient clientGet = BiRClientFactory.GetClient();

            Assert.AreEqual(clientSet, clientGet);
            Assert.AreSame(clientSet, clientGet);

            
        }
    }
}
