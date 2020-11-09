using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using VerifyCompany.Common.Lib;
using VerifyNIP.Service;
using VerifyNIPActivePayer.Lib;

namespace VerifyNIPAvtivePayer.Lib.Test
{
    public class MulitpleNipTestsMock
    {
        private NIPActivePayerVerifier _verifier;
        private string _correctNIP1 = "5250005834";
        private string _correctNIP2 = "7792348141";
        private string _correctNIP3 = "7811767696";
        private string _correctNIP4 = "7781454968";
        private string _correctNIP5 = "7781424849";
        private string _correctNIP6 = "7822261960";


        private string _correctID1 = "c1";
        private string _correctID2 = "c2";
        private string _correctID3 = "c3";
        private string _correctID4 = "c4";
        private string _correctID5 = "c5";
        private string _correctID6 = "c6";

        private string _incorrectNIP1 = "1234";
        private string _incorrectNIP2 = "779-234-81-41";
        private string _incorrectNIP3 = "789009";
        private string _incorrectNIP4 = "34k3444";
        private string _incorrectNIP5 = "14555";
        private string _incorrectNIP6 = "11111110";

        private string _incorrectID1 = "ic1";
        private string _incorrectID2 = "ic2";
        private string _incorrectID3 = "ic3";
        private string _incorrectID4 = "ic4";
        private string _incorrectID5 = "ic5";
        private string _incorrectID6 = "ic6";

        private List<Company> _companiesToCheck;

        [OneTimeSetUp]
        public void Setup()
        {
            _companiesToCheck = new List<Company>()
            {   new Company(){ NIP = _correctNIP1, ID = _correctID1}, 
                new Company(){ NIP = _correctNIP2,  ID = _correctID2}, 
                new Company(){ NIP = _correctNIP3,  ID = _correctID3}, 
                new Company(){ NIP = _correctNIP4, ID = _correctID4}, 
                new Company(){ NIP = _correctNIP5, ID = _correctID5}, 
                new Company(){ NIP = _incorrectNIP1, ID = _incorrectID1},
                new Company(){ NIP = _incorrectNIP2, ID = _incorrectID2},
                new Company(){ NIP = _incorrectNIP3, ID = _incorrectID3},
                new Company(){ NIP = _incorrectNIP4, ID = _incorrectID4},
                new Company(){ NIP = _incorrectNIP5, ID = _incorrectID5}
            };
            Mock<WeryfikacjaVAT> clientMock = new Mock<WeryfikacjaVAT>();
            clientMock.Setup(c => c.SprawdzNIP(It.Is<SprawdzNIPZapytanie>(z => z.NIP == _correctNIP1))).Returns(new SprawdzNIPOdpowiedz() { WynikOperacji = new TWynikWeryfikacjiVAT() { Kod = TKodWeryfikacjiVAT.C } });
            clientMock.Setup(c => c.SprawdzNIP(It.Is<SprawdzNIPZapytanie>(z => z.NIP == _correctNIP2))).Returns(new SprawdzNIPOdpowiedz() { WynikOperacji = new TWynikWeryfikacjiVAT() { Kod = TKodWeryfikacjiVAT.C } });
            clientMock.Setup(c => c.SprawdzNIP(It.Is<SprawdzNIPZapytanie>(z => z.NIP == _correctNIP3))).Returns(new SprawdzNIPOdpowiedz() { WynikOperacji = new TWynikWeryfikacjiVAT() { Kod = TKodWeryfikacjiVAT.C } });
            clientMock.Setup(c => c.SprawdzNIP(It.Is<SprawdzNIPZapytanie>(z => z.NIP == _correctNIP4))).Returns(new SprawdzNIPOdpowiedz() { WynikOperacji = new TWynikWeryfikacjiVAT() { Kod = TKodWeryfikacjiVAT.C } });
            clientMock.Setup(c => c.SprawdzNIP(It.Is<SprawdzNIPZapytanie>(z => z.NIP == _correctNIP5))).Returns(new SprawdzNIPOdpowiedz() { WynikOperacji = new TWynikWeryfikacjiVAT() { Kod = TKodWeryfikacjiVAT.C } });

            clientMock.Setup(c => c.SprawdzNIP(It.Is<SprawdzNIPZapytanie>(z => z.NIP == _incorrectNIP1))).Returns(new SprawdzNIPOdpowiedz() { WynikOperacji = new TWynikWeryfikacjiVAT() { Kod = TKodWeryfikacjiVAT.I } });
            clientMock.Setup(c => c.SprawdzNIP(It.Is<SprawdzNIPZapytanie>(z => z.NIP == _incorrectNIP2))).Returns(new SprawdzNIPOdpowiedz() { WynikOperacji = new TWynikWeryfikacjiVAT() { Kod = TKodWeryfikacjiVAT.I } });
            clientMock.Setup(c => c.SprawdzNIP(It.Is<SprawdzNIPZapytanie>(z => z.NIP == _incorrectNIP3))).Returns(new SprawdzNIPOdpowiedz() { WynikOperacji = new TWynikWeryfikacjiVAT() { Kod = TKodWeryfikacjiVAT.I } });
            clientMock.Setup(c => c.SprawdzNIP(It.Is<SprawdzNIPZapytanie>(z => z.NIP == _incorrectNIP4))).Returns(new SprawdzNIPOdpowiedz() { WynikOperacji = new TWynikWeryfikacjiVAT() { Kod = TKodWeryfikacjiVAT.I } });
            clientMock.Setup(c => c.SprawdzNIP(It.Is<SprawdzNIPZapytanie>(z => z.NIP == _incorrectNIP5))).Returns(new SprawdzNIPOdpowiedz() { WynikOperacji = new TWynikWeryfikacjiVAT() { Kod = TKodWeryfikacjiVAT.I } });



            WeryfikacjaVATClientFactory.SetWeryfikacjaVATClinet(clientMock.Object);
            _verifier = new NIPActivePayerVerifier();

        }

        [Test]
        public void FullListNipVerification()
        {
            var verificationResults = _verifier.VerifyNIPs(_companiesToCheck);

            Assert.AreEqual(VerifyNIPResult.IsActiveVATPayer, verificationResults[_correctID1]);
            Assert.AreEqual(VerifyNIPResult.IsActiveVATPayer, verificationResults[_correctID2]);
            Assert.AreEqual(VerifyNIPResult.IsActiveVATPayer, verificationResults[_correctID3]);
            Assert.AreEqual(VerifyNIPResult.IsActiveVATPayer, verificationResults[_correctID4]);
            Assert.AreEqual(VerifyNIPResult.IsActiveVATPayer, verificationResults[_correctID5]);

            Assert.AreEqual(VerifyNIPResult.NIPNotCorrect, verificationResults[_incorrectID1]);
            Assert.AreEqual(VerifyNIPResult.NIPNotCorrect, verificationResults[_incorrectID2]);
            Assert.AreEqual(VerifyNIPResult.NIPNotCorrect, verificationResults[_incorrectID3]);
            Assert.AreEqual(VerifyNIPResult.NIPNotCorrect, verificationResults[_incorrectID4]);
            Assert.AreEqual(VerifyNIPResult.NIPNotCorrect, verificationResults[_incorrectID5]);

        }

        [Test]
        public void TimeForFullListNipVerification()
        {
            DateTime startTime = DateTime.Now;
            var verificationResults = _verifier.VerifyNIPs(_companiesToCheck);
            DateTime stopTime = DateTime.Now;
            
            Assert.GreaterOrEqual((stopTime - startTime).TotalMilliseconds, 1000);
        }



    }
}
