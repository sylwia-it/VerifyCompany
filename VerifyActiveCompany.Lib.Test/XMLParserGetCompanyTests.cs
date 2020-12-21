using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using VerifyActiveCompany.Lib.Test.DataHelpers;

namespace VerifyActiveCompany.Lib.Test
{
    public class XMLParserGetCompanyTests
    {

        [Test]
        public void NullGetCompanyTest()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => BiRResponseXMLParser.GetCompanyFromDaneSzukajPodmiotyResponse(null));
        }

        [Test]
        public void EmptyGetCompanyTest()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => BiRResponseXMLParser.GetCompanyFromDaneSzukajPodmiotyResponse(string.Empty));
        }

        [Test]
        public void CorrectPrawnaGetCompanyTest()
        {
            BiRCompany birCompany1 = XMLResponseGenerator.CorrectPrawnaCompany1;
            BiRCompany birCompany2 = BiRResponseXMLParser.GetCompanyFromDaneSzukajPodmiotyResponse(XMLResponseGenerator.CorrectPrawnaCompany1DaneRaportResponse);

            Assert.AreEqual(birCompany1.Regon, birCompany2.Regon);
            Assert.AreEqual(birCompany1.NIP, birCompany2.NIP);
            Assert.AreEqual(birCompany1.NipStatus, birCompany2.NipStatus);
            Assert.AreEqual(birCompany1.Name, birCompany2.Name);
            Assert.AreEqual(birCompany1.Wojewodztwo, birCompany2.Wojewodztwo);
            Assert.AreEqual(birCompany1.Powiat, birCompany2.Powiat);
            Assert.AreEqual(birCompany1.Gmina, birCompany2.Gmina);
            Assert.AreEqual(birCompany1.Town, birCompany2.Town);
            Assert.AreEqual(birCompany1.PostalCode, birCompany2.PostalCode);
            Assert.AreEqual(birCompany1.Street, birCompany2.Street);
            Assert.AreEqual(birCompany1.BuildingNumber, birCompany2.BuildingNumber);
            Assert.AreEqual(birCompany1.LocalNumber, birCompany2.LocalNumber);
            Assert.AreEqual(birCompany1.CompanyType, birCompany2.CompanyType);
            Assert.AreEqual(birCompany1.Type, birCompany2.Type);
            Assert.AreEqual(birCompany1.SilosID, birCompany2.SilosID);

        }

        [Test]
        public void CorrectPhisicalGetCompanyTest()
        {
            BiRCompany birCompany1 = XMLResponseGenerator.CorrectFizycznaCompany1;
            BiRCompany birCompany2 = BiRResponseXMLParser.GetCompanyFromDaneSzukajPodmiotyResponse(XMLResponseGenerator.CorrectFizicznaCompany1DaneRaportResponse);

            Assert.AreEqual(birCompany1.Regon, birCompany2.Regon);
            Assert.AreEqual(birCompany1.NIP, birCompany2.NIP);
            Assert.AreEqual(birCompany1.NipStatus, birCompany2.NipStatus);
            Assert.AreEqual(birCompany1.Name, birCompany2.Name);
            Assert.AreEqual(birCompany1.Wojewodztwo, birCompany2.Wojewodztwo);
            Assert.AreEqual(birCompany1.Powiat, birCompany2.Powiat);
            Assert.AreEqual(birCompany1.Gmina, birCompany2.Gmina);
            Assert.AreEqual(birCompany1.Town, birCompany2.Town);
            Assert.AreEqual(birCompany1.PostalCode, birCompany2.PostalCode);
            Assert.AreEqual(birCompany1.Street, birCompany2.Street);
            Assert.AreEqual(birCompany1.BuildingNumber, birCompany2.BuildingNumber);
            Assert.AreEqual(birCompany1.LocalNumber, birCompany2.LocalNumber);
            Assert.AreEqual(birCompany1.CompanyType, birCompany2.CompanyType);
            Assert.AreEqual(birCompany1.SilosID, birCompany2.SilosID);

        }

        [Test]
        public void CorrectGetCompanyDetailedReportFizycznaTest()
        {
            BiRCompany birCompany1 = XMLResponseGenerator.FullCorrectPhisicalCompany1;
            BiRCompany birCompany2 = XMLResponseGenerator.FullCorrectPhisicalCompany1;

            BiRResponseXMLParser.AddDaneFizycznaCedig(ref birCompany2, XMLResponseGenerator.CorrectFizycznaCompany1DetailReportResponse);

            Assert.AreEqual(birCompany1.NameShort, birCompany2.NameShort);
            Assert.AreEqual(birCompany1.WpisDoRejestruEwidencjiDate, birCompany2.WpisDoRejestruEwidencjiDate);
            Assert.AreEqual(birCompany1.CreationDate, birCompany2.CreationDate);
            Assert.AreEqual(birCompany1.StartRunDate, birCompany2.StartRunDate);
            Assert.AreEqual(birCompany1.ZaistnieniaZmianyDate, birCompany2.ZaistnieniaZmianyDate);
            Assert.AreEqual(birCompany1.ZakonczeniaDzialalnosciDate, birCompany2.ZakonczeniaDzialalnosciDate);
            Assert.AreEqual(birCompany1.SkresleniaRegonDate, birCompany2.SkresleniaRegonDate);
            Assert.AreEqual(birCompany1.OrzeczenieOUpadlosciDate, birCompany2.OrzeczenieOUpadlosciDate);
            Assert.AreEqual(birCompany1.ZakonczeniePostepowaniaUpadlosiowegoDate, birCompany2.ZakonczeniePostepowaniaUpadlosiowegoDate);
            Assert.AreEqual(birCompany1.NumerTelefonu, birCompany2.NumerTelefonu);
            Assert.AreEqual(birCompany1.NumerFaksu, birCompany2.NumerFaksu);
            Assert.AreEqual(birCompany1.Gmina, birCompany2.Gmina);
            Assert.AreEqual(birCompany1.RodzajRejestruEwidencji, birCompany2.RodzajRejestruEwidencji);
            Assert.AreEqual(birCompany1.NumWRejestrzeEwidencji, birCompany2.NumWRejestrzeEwidencji);
            Assert.AreEqual(birCompany1.OrganZalozycielski, birCompany2.OrganZalozycielski);
            Assert.AreEqual(birCompany1.OrganRejestrowy, birCompany2.OrganRejestrowy);
            Assert.AreEqual(birCompany1.FormaWlasnosci, birCompany2.FormaWlasnosci);
            Assert.AreEqual(birCompany1.FormaFinansowania, birCompany2.FormaFinansowania);
            Assert.AreEqual(birCompany1.SzczegolnaFormaPrawna, birCompany2.SzczegolnaFormaPrawna);
            Assert.AreEqual(birCompany1.PodstawowaFormaPrawna, birCompany2.PodstawowaFormaPrawna);
            Assert.AreEqual(birCompany1.RegonDate, birCompany2.RegonDate);
            Assert.AreEqual(birCompany1.ZawieszeniaDate, birCompany2.ZawieszeniaDate);
            Assert.AreEqual(birCompany1.WziowieniaDate, birCompany2.WziowieniaDate);
            Assert.AreEqual(birCompany1.SiedzibaNietypoweMiejsceLokalizacji, birCompany2.SiedzibaNietypoweMiejsceLokalizacji);
            Assert.AreEqual(birCompany1.Email, birCompany2.Email);
            Assert.AreEqual(birCompany1.WWW, birCompany2.WWW);
            Assert.AreEqual(birCompany1.NumerFaksu, birCompany2.NumerFaksu);
            Assert.AreEqual(birCompany1.NumerTelefonu, birCompany2.NumerTelefonu);
            Assert.AreEqual(birCompany1.NiePodjetoDzialanosci, birCompany2.NiePodjetoDzialanosci);
            Assert.AreEqual(birCompany1.SkresleniaZRejestruEwidencji, birCompany2.SkresleniaZRejestruEwidencji);
        }

        [Test]
        public void CorrectGetCompanyDetailedReportPrawnaTest()
        {
            BiRCompany birCompany1 = XMLResponseGenerator.FullCorrectPrawnaCompany1;
            BiRCompany birCompany2 = XMLResponseGenerator.FullCorrectPrawnaCompany1;

            BiRResponseXMLParser.AddDanePrawna(ref birCompany2, XMLResponseGenerator.CorrectPrawnaCompany1DetailReportResponse);

            Assert.AreEqual(birCompany1.NameShort, birCompany2.NameShort);
            Assert.AreEqual(birCompany1.WpisDoRejestruEwidencjiDate, birCompany2.WpisDoRejestruEwidencjiDate);
            Assert.AreEqual(birCompany1.CreationDate, birCompany2.CreationDate);
            Assert.AreEqual(birCompany1.StartRunDate, birCompany2.StartRunDate);
            Assert.AreEqual(birCompany1.ZaistnieniaZmianyDate, birCompany2.ZaistnieniaZmianyDate);
            Assert.AreEqual(birCompany1.ZakonczeniaDzialalnosciDate, birCompany2.ZakonczeniaDzialalnosciDate);
            Assert.AreEqual(birCompany1.SkresleniaRegonDate, birCompany2.SkresleniaRegonDate);
            Assert.AreEqual(birCompany1.OrzeczenieOUpadlosciDate, birCompany2.OrzeczenieOUpadlosciDate);
            Assert.AreEqual(birCompany1.ZakonczeniePostepowaniaUpadlosiowegoDate, birCompany2.ZakonczeniePostepowaniaUpadlosiowegoDate);
            Assert.AreEqual(birCompany1.NumerTelefonu, birCompany2.NumerTelefonu);
            Assert.AreEqual(birCompany1.NumerFaksu, birCompany2.NumerFaksu);
            Assert.AreEqual(birCompany1.Gmina, birCompany2.Gmina);
            Assert.AreEqual(birCompany1.RodzajRejestruEwidencji, birCompany2.RodzajRejestruEwidencji);
            Assert.AreEqual(birCompany1.NumWRejestrzeEwidencji, birCompany2.NumWRejestrzeEwidencji);
            Assert.AreEqual(birCompany1.OrganZalozycielski, birCompany2.OrganZalozycielski);
            Assert.AreEqual(birCompany1.OrganRejestrowy, birCompany2.OrganRejestrowy);
            Assert.AreEqual(birCompany1.FormaWlasnosci, birCompany2.FormaWlasnosci);
            Assert.AreEqual(birCompany1.FormaFinansowania, birCompany2.FormaFinansowania);
            Assert.AreEqual(birCompany1.SzczegolnaFormaPrawna, birCompany2.SzczegolnaFormaPrawna);
            Assert.AreEqual(birCompany1.PodstawowaFormaPrawna, birCompany2.PodstawowaFormaPrawna);
            Assert.AreEqual(birCompany1.RegonDate, birCompany2.RegonDate);
            Assert.AreEqual(birCompany1.ZawieszeniaDate, birCompany2.ZawieszeniaDate);
            Assert.AreEqual(birCompany1.WziowieniaDate, birCompany2.WziowieniaDate);
            Assert.AreEqual(birCompany1.SiedzibaNietypoweMiejsceLokalizacji, birCompany2.SiedzibaNietypoweMiejsceLokalizacji);
            Assert.AreEqual(birCompany1.Email, birCompany2.Email);
            Assert.AreEqual(birCompany1.WWW, birCompany2.WWW);
            Assert.AreEqual(birCompany1.NumerFaksu, birCompany2.NumerFaksu);
            Assert.AreEqual(birCompany1.NumerTelefonu, birCompany2.NumerTelefonu);
            Assert.AreEqual(birCompany1.NiePodjetoDzialanosci, birCompany2.NiePodjetoDzialanosci);
            Assert.AreEqual(birCompany1.SkresleniaZRejestruEwidencji, birCompany2.SkresleniaZRejestruEwidencji);

        }
    }
}
