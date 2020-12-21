using System;
using Moq;
using NUnit.Framework;
using VerifyCompany.Common.Test.Lib;
using VerifyNIP.Service;
using VerifyNIPActivePayer.Lib;

namespace VerifyNIPAvtivePayer.Lib.Test
{
    public class MulitpleNipTestsMock
    {
        private NIPActivePayerVerifier _verifier;
        
        [OneTimeSetUp]
        public void Setup()
        {
          
            Mock<WeryfikacjaVAT> clientMock = new Mock<WeryfikacjaVAT>();
            clientMock.Setup(c => c.SprawdzNIP(It.Is<SprawdzNIPZapytanie>(z => z.NIP == CompanyGenerator.GetCorrectNIP(1)))).Returns(new SprawdzNIPOdpowiedz() { WynikOperacji = new TWynikWeryfikacjiVAT() { Kod = TKodWeryfikacjiVAT.C } });
            clientMock.Setup(c => c.SprawdzNIP(It.Is<SprawdzNIPZapytanie>(z => z.NIP == CompanyGenerator.GetCorrectNIP(2)))).Returns(new SprawdzNIPOdpowiedz() { WynikOperacji = new TWynikWeryfikacjiVAT() { Kod = TKodWeryfikacjiVAT.C } });
            clientMock.Setup(c => c.SprawdzNIP(It.Is<SprawdzNIPZapytanie>(z => z.NIP == CompanyGenerator.GetCorrectNIP(3)))).Returns(new SprawdzNIPOdpowiedz() { WynikOperacji = new TWynikWeryfikacjiVAT() { Kod = TKodWeryfikacjiVAT.C } });
            clientMock.Setup(c => c.SprawdzNIP(It.Is<SprawdzNIPZapytanie>(z => z.NIP == CompanyGenerator.GetCorrectNIP(4)))).Returns(new SprawdzNIPOdpowiedz() { WynikOperacji = new TWynikWeryfikacjiVAT() { Kod = TKodWeryfikacjiVAT.C } });
            clientMock.Setup(c => c.SprawdzNIP(It.Is<SprawdzNIPZapytanie>(z => z.NIP == CompanyGenerator.GetCorrectNIP(0)))).Returns(new SprawdzNIPOdpowiedz() { WynikOperacji = new TWynikWeryfikacjiVAT() { Kod = TKodWeryfikacjiVAT.C } });

            clientMock.Setup(c => c.SprawdzNIP(It.Is<SprawdzNIPZapytanie>(z => z.NIP == CompanyGenerator.GetInCorrectNIP(1)))).Returns(new SprawdzNIPOdpowiedz() { WynikOperacji = new TWynikWeryfikacjiVAT() { Kod = TKodWeryfikacjiVAT.I } });
            clientMock.Setup(c => c.SprawdzNIP(It.Is<SprawdzNIPZapytanie>(z => z.NIP == CompanyGenerator.GetInCorrectNIP(2)))).Returns(new SprawdzNIPOdpowiedz() { WynikOperacji = new TWynikWeryfikacjiVAT() { Kod = TKodWeryfikacjiVAT.I } });
            clientMock.Setup(c => c.SprawdzNIP(It.Is<SprawdzNIPZapytanie>(z => z.NIP == CompanyGenerator.GetInCorrectNIP(3)))).Returns(new SprawdzNIPOdpowiedz() { WynikOperacji = new TWynikWeryfikacjiVAT() { Kod = TKodWeryfikacjiVAT.I } });
            clientMock.Setup(c => c.SprawdzNIP(It.Is<SprawdzNIPZapytanie>(z => z.NIP == CompanyGenerator.GetInCorrectNIP(4)))).Returns(new SprawdzNIPOdpowiedz() { WynikOperacji = new TWynikWeryfikacjiVAT() { Kod = TKodWeryfikacjiVAT.I } });
            clientMock.Setup(c => c.SprawdzNIP(It.Is<SprawdzNIPZapytanie>(z => z.NIP == CompanyGenerator.GetInCorrectNIP(0)))).Returns(new SprawdzNIPOdpowiedz() { WynikOperacji = new TWynikWeryfikacjiVAT() { Kod = TKodWeryfikacjiVAT.I } });



            WeryfikacjaVATClientFactory.SetWeryfikacjaVATClinet(clientMock.Object);
            _verifier = new NIPActivePayerVerifier();

        }

        [Test]
        public void FullListNipVerification()
        {
            var verificationResults = _verifier.VerifyNIPs(CompanyGenerator.GetAllCompanies());

            Assert.AreEqual(VerifyNIPResult.IsActiveVATPayer, verificationResults[CompanyGenerator.CompaniesNIPIDDic[CompanyGenerator.GetCorrectNIP(1)]]);
            Assert.AreEqual(VerifyNIPResult.IsActiveVATPayer, verificationResults[CompanyGenerator.CompaniesNIPIDDic[CompanyGenerator.GetCorrectNIP(2)]]);
            Assert.AreEqual(VerifyNIPResult.IsActiveVATPayer, verificationResults[CompanyGenerator.CompaniesNIPIDDic[CompanyGenerator.GetCorrectNIP(3)]]);
            Assert.AreEqual(VerifyNIPResult.IsActiveVATPayer, verificationResults[CompanyGenerator.CompaniesNIPIDDic[CompanyGenerator.GetCorrectNIP(4)]]);
            Assert.AreEqual(VerifyNIPResult.IsActiveVATPayer, verificationResults[CompanyGenerator.CompaniesNIPIDDic[CompanyGenerator.GetCorrectNIP(0)]]);

            Assert.AreEqual(VerifyNIPResult.NIPNotCorrect, verificationResults[CompanyGenerator.CompaniesNIPIDDic[CompanyGenerator.GetInCorrectNIP(1)]]);
            Assert.AreEqual(VerifyNIPResult.NIPNotCorrect, verificationResults[CompanyGenerator.CompaniesNIPIDDic[CompanyGenerator.GetInCorrectNIP(2)]]);
            Assert.AreEqual(VerifyNIPResult.NIPNotCorrect, verificationResults[CompanyGenerator.CompaniesNIPIDDic[CompanyGenerator.GetInCorrectNIP(3)]]);
            Assert.AreEqual(VerifyNIPResult.NIPNotCorrect, verificationResults[CompanyGenerator.CompaniesNIPIDDic[CompanyGenerator.GetInCorrectNIP(4)]]);
            Assert.AreEqual(VerifyNIPResult.NIPNotCorrect, verificationResults[CompanyGenerator.CompaniesNIPIDDic[CompanyGenerator.GetInCorrectNIP(0)]]);

        }

        [Test]
        public void TimeForFullListNipVerification()
        {
            DateTime startTime = DateTime.Now;
            _verifier.VerifyNIPs(CompanyGenerator.GetAllCompanies());
            DateTime stopTime = DateTime.Now;

            Assert.GreaterOrEqual((stopTime - startTime).TotalMilliseconds, 1000);
        }



    }
}
