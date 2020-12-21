using Moq;
using NUnit.Framework;
using VerifyCompany.Common.Test.Lib;
using VerifyNIP.Service;
using VerifyNIPActivePayer.Lib;

namespace VerifyNIPAvtivePayer.Lib.Test
{
    public class SingleNipTestsMock
    {
        private NIPActivePayerVerifier _verifier;
       

        [OneTimeSetUp]
        public void Setup()
        {
            Mock<WeryfikacjaVAT> clientMock = new Mock<WeryfikacjaVAT>();
            clientMock.Setup(c => c.SprawdzNIP(It.Is<SprawdzNIPZapytanie>(z => z.NIP==CompanyGenerator.GetCorrectNIP(1)))).Returns(new SprawdzNIPOdpowiedz() { WynikOperacji = new TWynikWeryfikacjiVAT() { Kod = TKodWeryfikacjiVAT.C } });
            clientMock.Setup(c => c.SprawdzNIP(It.Is<SprawdzNIPZapytanie>(z => z.NIP == CompanyGenerator.GetInCorrectNIP(1)))).Returns(new SprawdzNIPOdpowiedz() { WynikOperacji = new TWynikWeryfikacjiVAT() { Kod = TKodWeryfikacjiVAT.I } });
            


            WeryfikacjaVATClientFactory.SetWeryfikacjaVATClinet(clientMock.Object);
            _verifier = new NIPActivePayerVerifier();
            
        }

        [Test]
        public void OneCorrectNipVerification()
        {
            VerifyNIPResult response = _verifier.VerifyNIP(CompanyGenerator.GetCorrectNIP(1));
            Assert.AreEqual(VerifyNIPResult.IsActiveVATPayer, response);
        }

        [Test]
        public void EmptyNipVerification()
        {
            VerifyNIPResult response = _verifier.VerifyNIP(string.Empty);
            Assert.AreEqual(VerifyNIPResult.NIPNotCorrect, response);
        }
        [Test]
        public void NullNipVerification()
        {
            VerifyNIPResult response = _verifier.VerifyNIP(string.Empty);
            Assert.AreEqual(VerifyNIPResult.NIPNotCorrect, response);
        }

        [Test]
        public void WhiteSpaceNipVerification()
        {
            VerifyNIPResult response = _verifier.VerifyNIP("  ");
            Assert.AreEqual(VerifyNIPResult.NIPNotCorrect, response);
        }

        [Test]
        public void IncorrectNipVerification()
        {
            VerifyNIPResult response = _verifier.VerifyNIP(CompanyGenerator.GetInCorrectNIP(1));
            Assert.AreEqual(VerifyNIPResult.NIPNotCorrect, response);
        }

      
    }
}