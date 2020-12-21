using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerifyActiveCompany.Lib.Test.DataHelpers
{
    internal static class XMLResponseGenerator
    {
        internal static readonly string Error4Response = "<root>\r\n  <dane>\r\n    <ErrorCode>4</ErrorCode>\r\n    <ErrorMessagePl>Nie znaleziono podmiotu dla podanych kryteriów wyszukiwania.</ErrorMessagePl>\r\n    <ErrorMessageEn>No data found for the specified search criteria.</ErrorMessageEn>\r\n    <Nip>1234</Nip>\r\n  </dane>\r\n</root>";

        internal static readonly string Error4ErrMessage = "Nie znaleziono podmiotu dla podanych kryteriów wyszukiwania.";

        internal static readonly string CorrectPrawnaCompany1DaneRaportResponse = "<root>\r\n<dane>\r\n<Regon>000001554</Regon>\r\n<Nip>5250005834</Nip>\r\n<StatusNip />\r\n<Nazwa> POLITECHNIKA WARSZAWSKA</Nazwa>\r\n<Wojewodztwo> MAZOWIECKIE</Wojewodztwo>\r\n<Powiat> Warszawa</Powiat>\r\n<Gmina> Śródmieście</Gmina>\r\n<Miejscowosc> Warszawa</Miejscowosc>\r\n<KodPocztowy>00-661</KodPocztowy>\r\n<Ulica> Plac Politechniki</Ulica>\r\n<NrNieruchomosci>1</NrNieruchomosci>\r\n<NrLokalu />\r\n<Typ> P</Typ>\r\n<SilosID>6</SilosID>\r\n<DataZakonczeniaDzialalnosci />\r\n<MiejscowoscPoczty> Warszawa</MiejscowoscPoczty>\r\n</dane>\r\n</root>";

        internal static readonly string CorrectFizicznaCompany1DaneRaportResponse = "<root >\r\n<dane>\r\n<Regon>472925442</Regon>\r\n<Nip>9471862705</Nip>\r\n<StatusNip />\r\n<Nazwa> Mikołaj Gauer</Nazwa>\r\n<Wojewodztwo> ŁÓDZKIE</Wojewodztwo>\r\n<Powiat> Łódź</Powiat>\r\n<Gmina> Łódź-Śródmieście</Gmina>\r\n<Miejscowosc> Łódź</Miejscowosc>\r\n<KodPocztowy>90-224</KodPocztowy>\r\n<Ulica> ul. Pomorska</Ulica>\r\n<NrNieruchomosci>77</NrNieruchomosci>\r\n<NrLokalu />\r\n<Typ> F</Typ>\r\n<SilosID>1</SilosID>\r\n<DataZakonczeniaDzialalnosci />\r\n<MiejscowoscPoczty> Łódź</MiejscowoscPoczty>\r\n</dane>\r\n</root>";

        internal static BiRCompany CorrectPrawnaCompany1 { 
            get 
            {
                return new BiRCompany()
                {
                    Regon = "000001554",
                    NIP = "5250005834",
                    NipStatus = string.Empty,
                    Name = "POLITECHNIKA WARSZAWSKA",
                    Wojewodztwo = "MAZOWIECKIE",
                    Powiat = "Warszawa",
                    Gmina = "Śródmieście",
                    Town = "Warszawa",
                    PostalCode = "00-661",
                    Street = "Plac Politechniki",
                    BuildingNumber = "1",
                    LocalNumber = string.Empty,
                    Type = "P",
                    CompanyType = BiRCompanyType.Prawna,
                    SilosID = "6",
                    ZakonczeniaDzialalnosciDate = DateTime.MinValue
                };
            } 
        }

        internal static BiRCompany CorrectFizycznaCompany1
        {
            get
            {
                return new BiRCompany()
                {
                    Regon = "472925442",
                    NIP = "9471862705",
                    NipStatus = string.Empty,
                    Name = "Mikołaj Gauer",
                    Wojewodztwo = "ŁÓDZKIE",
                    Powiat = "Łódź",
                    Gmina = "Łódź-Śródmieście",
                    Town = "Łódź",
                    PostalCode = "90-224",
                    Street = "ul. Pomorska",
                    BuildingNumber = "77",
                    LocalNumber = string.Empty,
                    Type = "F",
                    CompanyType = BiRCompanyType.FizycznaProwadzacaDzialalnoscGosp,
                    SilosID = "1",
                    ZakonczeniaDzialalnosciDate = DateTime.MinValue
                };
            }
        }

        internal static BiRCompany FullCorrectPrawnaCompany1 
            { get {

                BiRCompany birCompany1 = CorrectPrawnaCompany1;
                birCompany1.NameShort = "PW";
                birCompany1.WpisDoRejestruEwidencjiDate = new DateTime(1975, 12, 15);
                birCompany1.CreationDate = new DateTime(1975, 12, 15);
                birCompany1.StartRunDate = new DateTime(1975, 12, 15);
                birCompany1.ZaistnieniaZmianyDate = new DateTime(2017, 12, 30);
                birCompany1.ZakonczeniaDzialalnosciDate = DateTime.MinValue;
                birCompany1.SkresleniaRegonDate = DateTime.MinValue;
                birCompany1.OrzeczenieOUpadlosciDate = DateTime.MinValue;
                birCompany1.ZakonczeniePostepowaniaUpadlosiowegoDate = DateTime.MinValue;
                birCompany1.NumerTelefonu = "0226281625";
                birCompany1.NumerFaksu = "0222346290";
                birCompany1.Gmina = "Śródmieście";

                birCompany1.RodzajRejestruEwidencji = "PODMIOTY UTWORZONE Z MOCY USTAWY";
                birCompany1.NumWRejestrzeEwidencji = string.Empty;
                birCompany1.OrganZalozycielski = "MINISTER EDUKACJI NARODOWEJ I SPORTU";
                birCompany1.OrganRejestrowy = string.Empty;

                birCompany1.FormaWlasnosci = "WŁASNOŚĆ PAŃSTWOWYCH OSÓB PRAWNYCH";
                birCompany1.FormaFinansowania = "JEDNOSTKA SAMOFINANSUJĄCA NIE BĘDĄCA JEDNOSTKĄ BUDŻETOWĄ LUB SAMORZĄDOWYM ZAKŁADEM BUDŻETOWYM";

                birCompany1.SzczegolnaFormaPrawna = "UCZELNIE";
                birCompany1.PodstawowaFormaPrawna = "OSOBA PRAWNA";

                birCompany1.RegonDate = DateTime.MinValue;
                birCompany1.ZawieszeniaDate = DateTime.MinValue;
                birCompany1.WziowieniaDate = DateTime.MinValue;
                birCompany1.SiedzibaNietypoweMiejsceLokalizacji = string.Empty;
                birCompany1.Email = string.Empty;
                birCompany1.WWW = string.Empty;

                return birCompany1;

            } }

          
        internal static readonly string CorrectPrawnaCompany1DetailReportResponse = "<root>\r\n  <dane>\r\n    <praw_regon9>000001554</praw_regon9>\r\n    <praw_nip>5250005834</praw_nip>\r\n    <praw_statusNip />\r\n    <praw_nazwa>POLITECHNIKA WARSZAWSKA</praw_nazwa>\r\n    <praw_nazwaSkrocona>PW</praw_nazwaSkrocona>\r\n    <praw_numerWRejestrzeEwidencji />\r\n    <praw_dataWpisuDoRejestruEwidencji>1975-12-15</praw_dataWpisuDoRejestruEwidencji>\r\n    <praw_dataPowstania>1975-12-15</praw_dataPowstania>\r\n    <praw_dataRozpoczeciaDzialalnosci>1975-12-15</praw_dataRozpoczeciaDzialalnosci>\r\n    <praw_dataWpisuDoRegon />\r\n    <praw_dataZawieszeniaDzialalnosci />\r\n    <praw_dataWznowieniaDzialalnosci />\r\n    <praw_dataZaistnieniaZmiany>2017-12-30</praw_dataZaistnieniaZmiany>\r\n    <praw_dataZakonczeniaDzialalnosci />\r\n    <praw_dataSkresleniaZRegon />\r\n    <praw_dataOrzeczeniaOUpadlosci />\r\n    <praw_dataZakonczeniaPostepowaniaUpadlosciowego />\r\n    <praw_adSiedzKraj_Symbol>PL</praw_adSiedzKraj_Symbol>\r\n    <praw_adSiedzWojewodztwo_Symbol>14</praw_adSiedzWojewodztwo_Symbol>\r\n    <praw_adSiedzPowiat_Symbol>65</praw_adSiedzPowiat_Symbol>\r\n    <praw_adSiedzGmina_Symbol>108</praw_adSiedzGmina_Symbol>\r\n    <praw_adSiedzKodPocztowy>00661</praw_adSiedzKodPocztowy>\r\n    <praw_adSiedzMiejscowoscPoczty_Symbol>0919810</praw_adSiedzMiejscowoscPoczty_Symbol>\r\n    <praw_adSiedzMiejscowosc_Symbol>0919810</praw_adSiedzMiejscowosc_Symbol>\r\n    <praw_adSiedzUlica_Symbol>45376</praw_adSiedzUlica_Symbol>\r\n    <praw_adSiedzNumerNieruchomosci>1</praw_adSiedzNumerNieruchomosci>\r\n    <praw_adSiedzNumerLokalu />\r\n    <praw_adSiedzNietypoweMiejsceLokalizacji />\r\n    <praw_numerTelefonu>0226281625</praw_numerTelefonu>\r\n    <praw_numerWewnetrznyTelefonu />\r\n    <praw_numerFaksu>0222346290</praw_numerFaksu>\r\n    <praw_adresEmail />\r\n    <praw_adresStronyinternetowej />\r\n    <praw_adSiedzKraj_Nazwa>POLSKA</praw_adSiedzKraj_Nazwa>\r\n    <praw_adSiedzWojewodztwo_Nazwa>MAZOWIECKIE</praw_adSiedzWojewodztwo_Nazwa>\r\n    <praw_adSiedzPowiat_Nazwa>Warszawa</praw_adSiedzPowiat_Nazwa>\r\n    <praw_adSiedzGmina_Nazwa>Śródmieście</praw_adSiedzGmina_Nazwa>\r\n    <praw_adSiedzMiejscowosc_Nazwa>Warszawa</praw_adSiedzMiejscowosc_Nazwa>\r\n    <praw_adSiedzMiejscowoscPoczty_Nazwa>Warszawa</praw_adSiedzMiejscowoscPoczty_Nazwa>\r\n    <praw_adSiedzUlica_Nazwa>Plac Politechniki</praw_adSiedzUlica_Nazwa>\r\n    <praw_podstawowaFormaPrawna_Symbol>1</praw_podstawowaFormaPrawna_Symbol>\r\n    <praw_szczegolnaFormaPrawna_Symbol>044</praw_szczegolnaFormaPrawna_Symbol>\r\n    <praw_formaFinansowania_Symbol>1</praw_formaFinansowania_Symbol>\r\n    <praw_formaWlasnosci_Symbol>112</praw_formaWlasnosci_Symbol>\r\n    <praw_organZalozycielski_Symbol>033000000</praw_organZalozycielski_Symbol>\r\n    <praw_organRejestrowy_Symbol />\r\n    <praw_rodzajRejestruEwidencji_Symbol>000</praw_rodzajRejestruEwidencji_Symbol>\r\n    <praw_podstawowaFormaPrawna_Nazwa>OSOBA PRAWNA</praw_podstawowaFormaPrawna_Nazwa>\r\n    <praw_szczegolnaFormaPrawna_Nazwa>UCZELNIE</praw_szczegolnaFormaPrawna_Nazwa>\r\n    <praw_formaFinansowania_Nazwa>JEDNOSTKA SAMOFINANSUJĄCA NIE BĘDĄCA JEDNOSTKĄ BUDŻETOWĄ LUB SAMORZĄDOWYM ZAKŁADEM BUDŻETOWYM</praw_formaFinansowania_Nazwa>\r\n    <praw_formaWlasnosci_Nazwa>WŁASNOŚĆ PAŃSTWOWYCH OSÓB PRAWNYCH</praw_formaWlasnosci_Nazwa>\r\n    <praw_organZalozycielski_Nazwa>MINISTER EDUKACJI NARODOWEJ I SPORTU</praw_organZalozycielski_Nazwa>\r\n    <praw_organRejestrowy_Nazwa />\r\n    <praw_rodzajRejestruEwidencji_Nazwa>PODMIOTY UTWORZONE Z MOCY USTAWY</praw_rodzajRejestruEwidencji_Nazwa>\r\n    <praw_liczbaJednLokalnych>2</praw_liczbaJednLokalnych>\r\n  </dane>\r\n</root>";


        internal static BiRCompany FullCorrectPhisicalCompany1
        {
            get
            {

                BiRCompany birCompany1 = CorrectFizycznaCompany1;
                birCompany1.NameShort = "Mikołaj Gauer";
                birCompany1.WpisDoRejestruEwidencjiDate = new DateTime(2019, 09, 16);
                birCompany1.CreationDate = new DateTime(2019, 09, 16);
                birCompany1.StartRunDate = new DateTime(2019, 09, 16);
                birCompany1.ZaistnieniaZmianyDate = new DateTime(2020, 09, 24);
                birCompany1.ZakonczeniaDzialalnosciDate = DateTime.MinValue;
                birCompany1.SkresleniaRegonDate = DateTime.MinValue;
                birCompany1.OrzeczenieOUpadlosciDate = DateTime.MinValue;
                birCompany1.ZakonczeniePostepowaniaUpadlosiowegoDate = DateTime.MinValue;
                birCompany1.NumerTelefonu = string.Empty;
                birCompany1.NumerFaksu = string.Empty;
                
                birCompany1.RodzajRejestruEwidencji = "CENTRALNA EWIDENCJA I INFORMACJA O DZIAŁALNOŚCI GOSPODARCZEJ";
                birCompany1.NumWRejestrzeEwidencji = "001465897/2019";
                birCompany1.OrganZalozycielski = string.Empty;
                birCompany1.OrganRejestrowy = "MINISTER ROZWOJU";
                birCompany1.NiePodjetoDzialanosci = false;

                birCompany1.FormaWlasnosci = string.Empty;
                birCompany1.FormaFinansowania = string.Empty;

             
                birCompany1.RegonDate = new DateTime(2019, 09, 16);
                birCompany1.ZawieszeniaDate = DateTime.MinValue;
                birCompany1.WziowieniaDate = DateTime.MinValue;
                birCompany1.SiedzibaNietypoweMiejsceLokalizacji = string.Empty;
                birCompany1.Email = string.Empty;
                birCompany1.WWW = string.Empty;

                return birCompany1;

            }
        }


        internal static readonly string CorrectFizycznaCompany1DetailReportResponse = "<root>\r\n  <dane>\r\n    <fiz_regon9>472925442</fiz_regon9>\r\n    <fiz_nazwa>Mikołaj Gauer</fiz_nazwa>\r\n    <fiz_nazwaSkrocona>Mikołaj Gauer</fiz_nazwaSkrocona>\r\n    <fiz_dataPowstania>2019-09-16</fiz_dataPowstania>\r\n    <fiz_dataRozpoczeciaDzialalnosci>2019-09-16</fiz_dataRozpoczeciaDzialalnosci>\r\n    <fiz_dataWpisuDzialalnosciDoRegon>2019-09-16</fiz_dataWpisuDzialalnosciDoRegon>\r\n    <fiz_dataZawieszeniaDzialalnosci />\r\n    <fiz_dataWznowieniaDzialalnosci />\r\n    <fiz_dataZaistnieniaZmianyDzialalnosci>2020-09-24</fiz_dataZaistnieniaZmianyDzialalnosci>\r\n    <fiz_dataZakonczeniaDzialalnosci />\r\n    <fiz_dataSkresleniaDzialalnosciZRegon />\r\n    <fiz_dataOrzeczeniaOUpadlosci />\r\n    <fiz_dataZakonczeniaPostepowaniaUpadlosciowego />\r\n    <fiz_adSiedzKraj_Symbol>PL</fiz_adSiedzKraj_Symbol>\r\n    <fiz_adSiedzWojewodztwo_Symbol>10</fiz_adSiedzWojewodztwo_Symbol>\r\n    <fiz_adSiedzPowiat_Symbol>61</fiz_adSiedzPowiat_Symbol>\r\n    <fiz_adSiedzGmina_Symbol>059</fiz_adSiedzGmina_Symbol>\r\n    <fiz_adSiedzKodPocztowy>90224</fiz_adSiedzKodPocztowy>\r\n    <fiz_adSiedzMiejscowoscPoczty_Symbol>0958430</fiz_adSiedzMiejscowoscPoczty_Symbol>\r\n    <fiz_adSiedzMiejscowosc_Symbol>0958430</fiz_adSiedzMiejscowosc_Symbol>\r\n    <fiz_adSiedzUlica_Symbol>17088</fiz_adSiedzUlica_Symbol>\r\n    <fiz_adSiedzNumerNieruchomosci>77</fiz_adSiedzNumerNieruchomosci>\r\n    <fiz_adSiedzNumerLokalu />\r\n    <fiz_adSiedzNietypoweMiejsceLokalizacji />\r\n    <fiz_numerTelefonu />\r\n    <fiz_numerWewnetrznyTelefonu />\r\n    <fiz_numerFaksu />\r\n    <fiz_adresEmail />\r\n    <fiz_adresStronyinternetowej />\r\n    <fiz_adSiedzKraj_Nazwa>POLSKA</fiz_adSiedzKraj_Nazwa>\r\n    <fiz_adSiedzWojewodztwo_Nazwa>ŁÓDZKIE</fiz_adSiedzWojewodztwo_Nazwa>\r\n    <fiz_adSiedzPowiat_Nazwa>Łódź</fiz_adSiedzPowiat_Nazwa>\r\n    <fiz_adSiedzGmina_Nazwa>Łódź-Śródmieście</fiz_adSiedzGmina_Nazwa>\r\n    <fiz_adSiedzMiejscowosc_Nazwa>Łódź</fiz_adSiedzMiejscowosc_Nazwa>\r\n    <fiz_adSiedzMiejscowoscPoczty_Nazwa>Łódź</fiz_adSiedzMiejscowoscPoczty_Nazwa>\r\n    <fiz_adSiedzUlica_Nazwa>ul. Pomorska</fiz_adSiedzUlica_Nazwa>\r\n    <fizC_dataWpisuDoRejestruEwidencji>2019-09-16</fizC_dataWpisuDoRejestruEwidencji>\r\n    <fizC_dataSkresleniaZRejestruEwidencji />\r\n    <fizC_numerWRejestrzeEwidencji>001465897/2019</fizC_numerWRejestrzeEwidencji>\r\n    <fizC_OrganRejestrowy_Symbol>120191115</fizC_OrganRejestrowy_Symbol>\r\n    <fizC_OrganRejestrowy_Nazwa>MINISTER ROZWOJU</fizC_OrganRejestrowy_Nazwa>\r\n    <fizC_RodzajRejestru_Symbol>151</fizC_RodzajRejestru_Symbol>\r\n    <fizC_RodzajRejestru_Nazwa>CENTRALNA EWIDENCJA I INFORMACJA O DZIAŁALNOŚCI GOSPODARCZEJ</fizC_RodzajRejestru_Nazwa>\r\n    <fizC_NiePodjetoDzialalnosci>false</fizC_NiePodjetoDzialalnosci>\r\n  </dane>\r\n</root>";
    }
}
