using NUnit.Framework;
using VerifyActiveOrg.Lib;
using System.Threading.Tasks;

namespace VerifyActive.Org.Lib.Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public   void Test1Async()
        {
            var c = BiRClientFactory.GetClient("");
            Assert.Pass();
           
        }
    }
}