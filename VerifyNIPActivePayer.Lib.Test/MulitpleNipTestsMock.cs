using System;
using Moq;
using NUnit.Framework;
using VerifyCompany.Common.Test.Lib;
using VerifyNIP.Service;
using VerifyNIPActivePayer.Lib;
using System.Collections.Generic;
using System.Linq;

namespace VerifyNIPAvtivePayer.Lib.Test
{
    public class MulitpleNipTestsMock
    {
        private NIPActivePayerVerifier _verifier;
        private List<string> _nipsToCheck = new List<string>();
        
        [OneTimeSetUp]
        public void Setup()
        {
          
            Mock<WeryfikacjaVAT> clientMock = new Mock<WeryfikacjaVAT>();
            clientMock.Setup(c => c.SprawdzNIP(It.Is<SprawdzNIPZapytanie>(z => z.NIP == CompanyGenerator.GetCorrectNIP(1)))).Returns(new SprawdzNIPOdpowiedz() { WynikOperacji = new TWynikWeryfikacjiVAT() { Kod = TKodWeryfikacjiVAT.C } });
            _nipsToCheck.Add(CompanyGenerator.GetCorrectNIP(1));
            clientMock.Setup(c => c.SprawdzNIP(It.Is<SprawdzNIPZapytanie>(z => z.NIP == CompanyGenerator.GetCorrectNIP(2)))).Returns(new SprawdzNIPOdpowiedz() { WynikOperacji = new TWynikWeryfikacjiVAT() { Kod = TKodWeryfikacjiVAT.C } });
            _nipsToCheck.Add(CompanyGenerator.GetCorrectNIP(2));
            clientMock.Setup(c => c.SprawdzNIP(It.Is<SprawdzNIPZapytanie>(z => z.NIP == CompanyGenerator.GetCorrectNIP(3)))).Returns(new SprawdzNIPOdpowiedz() { WynikOperacji = new TWynikWeryfikacjiVAT() { Kod = TKodWeryfikacjiVAT.C } });
            _nipsToCheck.Add(CompanyGenerator.GetCorrectNIP(3));
            clientMock.Setup(c => c.SprawdzNIP(It.Is<SprawdzNIPZapytanie>(z => z.NIP == CompanyGenerator.GetCorrectNIP(4)))).Returns(new SprawdzNIPOdpowiedz() { WynikOperacji = new TWynikWeryfikacjiVAT() { Kod = TKodWeryfikacjiVAT.C } });
            _nipsToCheck.Add(CompanyGenerator.GetCorrectNIP(4));
            clientMock.Setup(c => c.SprawdzNIP(It.Is<SprawdzNIPZapytanie>(z => z.NIP == CompanyGenerator.GetCorrectNIP(0)))).Returns(new SprawdzNIPOdpowiedz() { WynikOperacji = new TWynikWeryfikacjiVAT() { Kod = TKodWeryfikacjiVAT.C } });
            _nipsToCheck.Add(CompanyGenerator.GetCorrectNIP(0))
                ;
            clientMock.Setup(c => c.SprawdzNIP(It.Is<SprawdzNIPZapytanie>(z => z.NIP == CompanyGenerator.GetInCorrectNIP(1)))).Returns(new SprawdzNIPOdpowiedz() { WynikOperacji = new TWynikWeryfikacjiVAT() { Kod = TKodWeryfikacjiVAT.I } });
            _nipsToCheck.Add(CompanyGenerator.GetInCorrectNIP(1));
            clientMock.Setup(c => c.SprawdzNIP(It.Is<SprawdzNIPZapytanie>(z => z.NIP == CompanyGenerator.GetInCorrectNIP(2)))).Returns(new SprawdzNIPOdpowiedz() { WynikOperacji = new TWynikWeryfikacjiVAT() { Kod = TKodWeryfikacjiVAT.I } });
            _nipsToCheck.Add(CompanyGenerator.GetInCorrectNIP(2));
            clientMock.Setup(c => c.SprawdzNIP(It.Is<SprawdzNIPZapytanie>(z => z.NIP == CompanyGenerator.GetInCorrectNIP(3)))).Returns(new SprawdzNIPOdpowiedz() { WynikOperacji = new TWynikWeryfikacjiVAT() { Kod = TKodWeryfikacjiVAT.I } });
            _nipsToCheck.Add(CompanyGenerator.GetInCorrectNIP(3));
            clientMock.Setup(c => c.SprawdzNIP(It.Is<SprawdzNIPZapytanie>(z => z.NIP == CompanyGenerator.GetInCorrectNIP(4)))).Returns(new SprawdzNIPOdpowiedz() { WynikOperacji = new TWynikWeryfikacjiVAT() { Kod = TKodWeryfikacjiVAT.I } });
            _nipsToCheck.Add(CompanyGenerator.GetInCorrectNIP(4));
            clientMock.Setup(c => c.SprawdzNIP(It.Is<SprawdzNIPZapytanie>(z => z.NIP == CompanyGenerator.GetInCorrectNIP(0)))).Returns(new SprawdzNIPOdpowiedz() { WynikOperacji = new TWynikWeryfikacjiVAT() { Kod = TKodWeryfikacjiVAT.I } });
            _nipsToCheck.Add(CompanyGenerator.GetInCorrectNIP(0));


            WeryfikacjaVATClientFactory.SetWeryfikacjaVATClinet(clientMock.Object);
            _verifier = new NIPActivePayerVerifier();

        }

        [Test]
        public void FullListNipVerification()
        {
            var verificationResults = _verifier.VerifyNIPs(CompanyGenerator.GetAllCompanies().Where(c => _nipsToCheck.Contains(c.NIP)).ToList());

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
            _verifier.VerifyNIPs(CompanyGenerator.GetAllCompanies().Where(c => _nipsToCheck.Contains(c.NIP)).ToList());
            DateTime stopTime = DateTime.Now;

            Assert.GreaterOrEqual((stopTime - startTime).TotalMilliseconds, 1000);
        }



    }
}
