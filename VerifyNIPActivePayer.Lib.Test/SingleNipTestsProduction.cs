using NUnit.Framework;
using VerifyCompany.Common.Test.Lib;
using VerifyNIPActivePayer.Lib;

namespace VerifyNIPAvtivePayer.Lib.Test
{
    public class SingleNipTestsProduction
    {
        private NIPActivePayerVerifier verifier;
        [OneTimeSetUp]
        public void Setup()
        {
            verifier = new NIPActivePayerVerifier();
            
        }

        [Test]
        public void OneCorrectNipVerification()
        {
            VerifyNIPResult response = verifier.VerifyNIP(CompanyGenerator.GetCorrectNIP(1));
            Assert.AreEqual(VerifyNIPResult.IsActiveVATPayer, response);
        }

        [Test]
        public void EmptyNipVerification()
        {
            VerifyNIPResult response = verifier.VerifyNIP(string.Empty);
            Assert.AreEqual(VerifyNIPResult.NIPNotCorrect, response);
        }

        [Test]
        public void WhiteSpaceNipVerification()
        {
            VerifyNIPResult response = verifier.VerifyNIP("  ");
            Assert.AreEqual(VerifyNIPResult.NIPNotCorrect, response);
        }

        [Test]
        public void IncorrectNipVerification()
        {
            VerifyNIPResult response = verifier.VerifyNIP(CompanyGenerator.GetInCorrectNIP(1));
            Assert.AreEqual(VerifyNIPResult.NotRegisteredVATPayer, response);
        }

      
    }
}