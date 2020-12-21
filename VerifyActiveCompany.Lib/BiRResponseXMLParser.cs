using System;
using System.Linq;
using System.Xml.Linq;

namespace VerifyActiveCompany.Lib
{
    internal class BiRResponseXMLParser
    {
        private const string _errorMsgAttrName = "ErrorMessagePl";
        private const string _regonElName = "Regon";
        private const string _statusNipElName = "StatusNip";
        private const string _nameElName = "Nazwa";
        private const string _wojewodztwoElName = "Wojewodztwo";
        private const string _powiatElName = "Powiat";
        private const string _gminaElName = "Gmina";
        private const string _townElName = "Miejscowosc";
        private const string _postalCodeElName = "KodPocztowy";
        private const string _streetElName = "Ulica";
        private const string _buildingNumElName = "NrNieruchomosci";
        private const string _apartmentNumElName = "NrLokalu";
        private const string _typeElName = "Typ";
        private const string _silosIDElName = "SilosID";
        private const string _finishDateElName = "DataZakonczeniaDzialalnosci";
        private const string _errorCodeElName = "ErrorCode";
        private const string _nipElName = "Nip";
       

        internal static bool IsResponseEmpty(string result)
        {
            if (result == null)
            {
                throw new ArgumentNullException(result, "Result is null.");
            }
            if (result == string.Empty)
            {
                return true;
            }

            XElement root = XElement.Parse(result);
            if (root.Descendants().Count() == 0)
            {
                return true;
            }
            return false;
        }

        internal static string GetErrorMsg(string result)
        {
            if (string.IsNullOrEmpty(result))
            {
                throw new ArgumentNullException(result, "Result is null or empty.");
            }

            XElement root = XElement.Parse(result);

            if (root.Descendants().Count(d => d.Name == _errorMsgAttrName) == 1)
            {
                return root.Descendants().First(d => d.Name == _errorMsgAttrName).Value;
            }
            else
            {
                throw new ArgumentOutOfRangeException(result, "Input parameter does not contain one element with error.");
            }
        }

        internal static bool ContainsError(string result)
        {
            if (string.IsNullOrEmpty(result))
            {
                throw new ArgumentNullException(result, "Result is null or empty.");
            }

            XElement root = XElement.Parse(result);

            if (root.Descendants().Count(d => d.Name == _errorCodeElName) == 1)
            { return true; }
            return false;
        }

        internal static BiRCompany GetCompanyFromDaneSzukajPodmiotyResponse(string response)
        {
            if (string.IsNullOrEmpty(response))
                throw new ArgumentOutOfRangeException(response, "Odpowiedź jest null lub pusta od klienta BiR.");

            BiRCompany company = new BiRCompany();
            XElement root = XElement.Parse(response);

            company.Regon = root.Descendants().First(d => d.Name == _regonElName).Value.Trim();
            company.NIP = root.Descendants().First(d => d.Name == _nipElName).Value.Trim();
            company.NipStatus = root.Descendants().First(d => d.Name == _statusNipElName).Value.Trim();
            company.Name = root.Descendants().First(d => d.Name == _nameElName).Value.Trim();
            company.Wojewodztwo = root.Descendants().First(d => d.Name == _wojewodztwoElName).Value.Trim();
            company.Powiat = root.Descendants().First(d => d.Name == _powiatElName).Value.Trim();
            company.Gmina = root.Descendants().First(d => d.Name == _gminaElName).Value.Trim();
            company.Town = root.Descendants().First(d => d.Name == _townElName).Value.Trim();
            company.PostalCode = root.Descendants().First(d => d.Name == _postalCodeElName).Value.Trim();
            company.Street = GetValueFrom(root, _streetElName);
            company.BuildingNumber = root.Descendants().First(d => d.Name == _buildingNumElName).Value.Trim();
            company.LocalNumber = root.Descendants().First(d => d.Name == _apartmentNumElName).Value.Trim();

            company.Type = root.Descendants().First(d => d.Name == _typeElName).Value.Trim();
            company.SilosID = root.Descendants().First(d => d.Name == _silosIDElName).Value.Trim();
            string dateInStr = root.Descendants().First(d => d.Name == _finishDateElName).Value.Trim();
            company.ZakonczeniaDzialalnosciDate = string.IsNullOrEmpty(dateInStr) ? DateTime.MinValue : DateTime.Parse(dateInStr);

            company.CompanyType = (company.Type == "P") ? BiRCompanyType.Prawna : BiRCompanyType.FizycznaProwadzacaDzialalnoscGosp;

            return company;
        }

        private static string GetValueFrom(XElement root, string nameOfElement)
        {
            string result = string.Empty;
            if (root.Descendants().Any(d => d.Name == nameOfElement))
            {
                result = root.Descendants().First(d => d.Name == nameOfElement).Value.Trim();
            }
            return result;
        }

        private const string _praw_RodzajRejEwidNazwaElName = "praw_rodzajRejestruEwidencji_Nazwa";
        private const string _praw_NumerWRejEwidElName = "praw_numerWRejestrzeEwidencji";
        private const string _praw_OrganZalozyNazwaElName = "praw_organZalozycielski_Nazwa";
        private const string _praw_OrganRejestrowyNazwaElName = "praw_organRejestrowy_Nazwa";
        private const string _praw_FormaWlanosciNazwaElName = "praw_formaWlasnosci_Nazwa";
        private const string _praw_FormaFinansowaniaNazwaElName = "praw_formaFinansowania_Nazwa";
        private const string _praw_SzczegolnaFormaPrawnaNazwaElName = "praw_szczegolnaFormaPrawna_Nazwa";
        private const string _praw_PodstaowaFormaPrawnaNazwaElName = "praw_podstawowaFormaPrawna_Nazwa";
        private const string _praw_NazwaSkroconaElName = "praw_nazwaSkrocona";
        private const string _praw_DataPowstaniaElName = "praw_dataPowstania";
        private const string _praw_DataRozpoczeciaDzialElName = "praw_dataRozpoczeciaDzialalnosci";
        private const string _praw_DataWpisuDoRegonElName = "praw_dataWpisuDoRegon";
        private const string _praw_DataWpisuDoRejestruEwidencjiElName = "praw_dataWpisuDoRejestruEwidencji";
        private const string _praw_DataZawieszeniaDzialanosciElName = "praw_dataZawieszeniaDzialalnosci";
        private const string _praw_DataWaznosiDzialnosciElName = "praw_dataWznowieniaDzialalnosci";
        private const string _praw_DataZaistnieniaZmianyElName = "praw_dataZaistnieniaZmiany";
        private const string _praw_DataZakonczeniaDzialalnosciElName = "praw_dataZakonczeniaDzialalnosci";
        private const string _praw_DataSkresleniaZRegonElName = "praw_dataSkresleniaZRegon";
        private const string _praw_DataOrzeczeniaOUpadlosciElName = "praw_dataOrzeczeniaOUpadlosci";
        private const string _praw_DataZakoczeniaPostepUpadlElName = "praw_dataZakonczeniaPostepowaniaUpadlosciowego";
        private const string _praw_AdresNietypowejLokalizajiElName = "praw_adSiedzNietypoweMiejsceLokalizacji";
        private const string _praw_NumerTelefonuElName = "praw_numerTelefonu";
        private const string _praw_NumerFaksuElName = "praw_numerFaksu";
        private const string _praw_AdresEmailElName = "praw_adresEmail";
        private const string _praw_AdresStronyInterElName = "praw_adresStronyinternetowej";


        public static void AddDanePrawna(ref BiRCompany company, string result)
        {
            XElement root = XElement.Parse(result);

            company.RodzajRejestruEwidencji = root.Descendants().First(d => d.Name == _praw_RodzajRejEwidNazwaElName).Value.Trim();
            company.NumWRejestrzeEwidencji = root.Descendants().First(d => d.Name == _praw_NumerWRejEwidElName).Value.Trim();
            company.OrganZalozycielski = root.Descendants().First(d => d.Name == _praw_OrganZalozyNazwaElName).Value.Trim();
            company.OrganRejestrowy = root.Descendants().First(d => d.Name == _praw_OrganRejestrowyNazwaElName).Value.Trim();
            company.FormaWlasnosci = root.Descendants().First(d => d.Name == _praw_FormaWlanosciNazwaElName).Value.Trim();
            company.FormaFinansowania = root.Descendants().First(d => d.Name == _praw_FormaFinansowaniaNazwaElName).Value.Trim();
            company.SzczegolnaFormaPrawna = root.Descendants().First(d => d.Name == _praw_SzczegolnaFormaPrawnaNazwaElName).Value.Trim();
            company.PodstawowaFormaPrawna = root.Descendants().First(d => d.Name == _praw_PodstaowaFormaPrawnaNazwaElName).Value.Trim();
            company.NameShort = root.Descendants().First(d => d.Name == _praw_NazwaSkroconaElName).Value.Trim();


            string dateInStr = string.Empty;
            dateInStr = root.Descendants().First(d => d.Name == _praw_DataPowstaniaElName).Value.Trim();
            company.CreationDate = string.IsNullOrEmpty(dateInStr) ? DateTime.MinValue : DateTime.Parse(dateInStr);
            dateInStr = root.Descendants().First(d => d.Name == _praw_DataRozpoczeciaDzialElName).Value.Trim();
            company.StartRunDate = string.IsNullOrEmpty(dateInStr) ? DateTime.MinValue : DateTime.Parse(dateInStr);

            dateInStr = root.Descendants().First(d => d.Name == _praw_DataWpisuDoRegonElName).Value.Trim();
            company.RegonDate = string.IsNullOrEmpty(dateInStr) ? DateTime.MinValue : DateTime.Parse(dateInStr);

            dateInStr = root.Descendants().First(d => d.Name == _praw_DataWpisuDoRejestruEwidencjiElName).Value.Trim();
            company.WpisDoRejestruEwidencjiDate = string.IsNullOrEmpty(dateInStr) ? DateTime.MinValue : DateTime.Parse(dateInStr);

            dateInStr = root.Descendants().First(d => d.Name == _praw_DataZawieszeniaDzialanosciElName).Value.Trim();
            company.ZawieszeniaDate = string.IsNullOrEmpty(dateInStr) ? DateTime.MinValue : DateTime.Parse(dateInStr);
            dateInStr = root.Descendants().First(d => d.Name == _praw_DataWaznosiDzialnosciElName).Value.Trim();
            company.WziowieniaDate = string.IsNullOrEmpty(dateInStr) ? DateTime.MinValue : DateTime.Parse(dateInStr);
            dateInStr = root.Descendants().First(d => d.Name == _praw_DataZaistnieniaZmianyElName).Value.Trim();
            company.ZaistnieniaZmianyDate = string.IsNullOrEmpty(dateInStr) ? DateTime.MinValue : DateTime.Parse(dateInStr);
            dateInStr = root.Descendants().First(d => d.Name == _praw_DataZakonczeniaDzialalnosciElName).Value.Trim();
            company.ZakonczeniaDzialalnosciDate = string.IsNullOrEmpty(dateInStr) ? DateTime.MinValue : DateTime.Parse(dateInStr);
            dateInStr = root.Descendants().First(d => d.Name == _praw_DataSkresleniaZRegonElName).Value.Trim();
            company.SkresleniaRegonDate = string.IsNullOrEmpty(dateInStr) ? DateTime.MinValue : DateTime.Parse(dateInStr);

            dateInStr = root.Descendants().First(d => d.Name == _praw_DataOrzeczeniaOUpadlosciElName).Value.Trim();
            company.OrzeczenieOUpadlosciDate = string.IsNullOrEmpty(dateInStr) ? DateTime.MinValue : DateTime.Parse(dateInStr);

            dateInStr = root.Descendants().First(d => d.Name == _praw_DataZakoczeniaPostepUpadlElName).Value.Trim();
            company.ZakonczeniePostepowaniaUpadlosiowegoDate = string.IsNullOrEmpty(dateInStr) ? DateTime.MinValue : DateTime.Parse(dateInStr);

            company.SiedzibaNietypoweMiejsceLokalizacji = root.Descendants().First(d => d.Name == _praw_AdresNietypowejLokalizajiElName).Value.Trim();
            company.NumerTelefonu = root.Descendants().First(d => d.Name == _praw_NumerTelefonuElName).Value.Trim();
            company.NumerFaksu = root.Descendants().First(d => d.Name == _praw_NumerFaksuElName).Value.Trim();
            company.Email = root.Descendants().First(d => d.Name == _praw_AdresEmailElName).Value.Trim();
            company.WWW = root.Descendants().First(d => d.Name == _praw_AdresStronyInterElName).Value.Trim();
        }


        

        private const string _fiz_RodzajRejestruNazwaElName = "fizC_RodzajRejestru_Nazwa";
        private const string _fiz_NumerWRejEwidencjiElName = "fizC_numerWRejestrzeEwidencji";
        private const string _fiz_OrganRejestrowyNazwaElName = "fizC_OrganRejestrowy_Nazwa";
        private const string _fiz_NazwaSkroconaElName = "fiz_nazwaSkrocona";
        private const string _fiz_DataPowstaniaElName = "fiz_dataPowstania";
        private const string _fiz_DataRozpDzialanElName = "fiz_dataRozpoczeciaDzialalnosci";
        private const string _fiz_DataWpisuDzialnDoRegonElName = "fiz_dataWpisuDzialalnosciDoRegon";
        private const string _fiz_DataZawieszeniaDzialElName = "fiz_dataZawieszeniaDzialalnosci";
        private const string _fiz_DataWznowieniaDzialElName = "fiz_dataWznowieniaDzialalnosci";
        private const string _fiz_DataZaistnieniaZmianyDzialElName = "fiz_dataZaistnieniaZmianyDzialalnosci";
        private const string _fiz_DataZakonczeniaDzialElName = "fiz_dataZakonczeniaDzialalnosci";
        private const string _fiz_DataSkreleniaDzialnosciZRegonElName = "fiz_dataSkresleniaDzialalnosciZRegon";
        private const string _fiz_DataWpisuDoRejestruEwidencjiElName = "fizC_dataWpisuDoRejestruEwidencji";
        private const string _fiz_DataSkresleniaZRejestruEwidencjiElName = "fizC_dataSkresleniaZRejestruEwidencji";
        private const string _fiz_DataOrzeczeniaoOUpadlosciElName = "fiz_dataOrzeczeniaOUpadlosci";
        private const string _fiz_DataZakonczeniaPstepowaniaUpadlElName = "fiz_dataZakonczeniaPostepowaniaUpadlosciowego";
        private const string _fiz_NiePodjetoDzialansociElName = "fizC_NiePodjetoDzialalnosci";
        private const string _fiz_AdSiedzNietypoweMiejsceLokazlizacjiElName = "fiz_adSiedzNietypoweMiejsceLokalizacji";
        private const string _fiz_NumerTelefonuElName = "fiz_numerTelefonu";
        private const string _fiz_NumberFaksuElName = "fiz_numerFaksu";
        private const string _fiz_AdresEmailElName = "fiz_adresEmail";
        private const string _fiz_AdresStronyInternetowejElName = "fiz_adresStronyinternetowej";
        private const string _fiz_NumerWRejestrzeEwidencjiElName = "fizC_numerWRejestrzeEwidencji";
        

        public static void AddDaneFizycznaCedig(ref BiRCompany company, string result)
        {
            XElement root = XElement.Parse(result);

            company.RodzajRejestruEwidencji = root.Descendants().First(d => d.Name == _fiz_RodzajRejestruNazwaElName).Value.Trim();
            company.NumWRejestrzeEwidencji = root.Descendants().First(d => d.Name == _fiz_NumerWRejEwidencjiElName).Value.Trim();
            company.OrganRejestrowy = root.Descendants().First(d => d.Name == _fiz_OrganRejestrowyNazwaElName).Value.Trim();

            company.NameShort = root.Descendants().First(d => d.Name == _fiz_NazwaSkroconaElName).Value.Trim();

            string dateInString = string.Empty;
            dateInString = root.Descendants().First(d => d.Name == _fiz_DataPowstaniaElName).Value.Trim();
            company.CreationDate = string.IsNullOrEmpty(dateInString) ? DateTime.MinValue : DateTime.Parse(dateInString);
            dateInString = root.Descendants().First(d => d.Name == _fiz_DataRozpDzialanElName).Value.Trim();
            company.StartRunDate = string.IsNullOrEmpty(dateInString) ? DateTime.MinValue : DateTime.Parse(dateInString);
            dateInString = root.Descendants().First(d => d.Name == _fiz_DataWpisuDzialnDoRegonElName).Value.Trim();
            company.RegonDate = string.IsNullOrEmpty(dateInString) ? DateTime.MinValue : DateTime.Parse(dateInString);
            dateInString = root.Descendants().First(d => d.Name == _fiz_DataZawieszeniaDzialElName).Value.Trim();
            company.ZawieszeniaDate = string.IsNullOrEmpty(dateInString) ? DateTime.MinValue : DateTime.Parse(dateInString);
            dateInString = root.Descendants().First(d => d.Name == _fiz_DataWznowieniaDzialElName).Value.Trim();
            company.WziowieniaDate = string.IsNullOrEmpty(dateInString) ? DateTime.MinValue : DateTime.Parse(dateInString);
            dateInString = root.Descendants().First(d => d.Name == _fiz_DataZaistnieniaZmianyDzialElName).Value.Trim();
            company.ZaistnieniaZmianyDate = string.IsNullOrEmpty(dateInString) ? DateTime.MinValue : DateTime.Parse(dateInString);
            dateInString = root.Descendants().First(d => d.Name == _fiz_DataZakonczeniaDzialElName).Value.Trim();
            company.ZakonczeniaDzialalnosciDate = string.IsNullOrEmpty(dateInString) ? DateTime.MinValue : DateTime.Parse(dateInString);
            dateInString = root.Descendants().First(d => d.Name == _fiz_DataSkreleniaDzialnosciZRegonElName).Value.Trim();
            company.SkresleniaRegonDate = string.IsNullOrEmpty(dateInString) ? DateTime.MinValue : DateTime.Parse(dateInString);
            dateInString = root.Descendants().First(d => d.Name == _fiz_DataWpisuDoRejestruEwidencjiElName).Value.Trim();
            company.WpisuDoRejestruEwidencji = string.IsNullOrEmpty(dateInString) ? DateTime.MinValue : DateTime.Parse(dateInString);
            dateInString = root.Descendants().First(d => d.Name == _fiz_DataSkresleniaZRejestruEwidencjiElName).Value.Trim();
            company.SkresleniaZRejestruEwidencji = string.IsNullOrEmpty(dateInString) ? DateTime.MinValue : DateTime.Parse(dateInString);

            dateInString = root.Descendants().First(d => d.Name == _fiz_DataOrzeczeniaoOUpadlosciElName).Value.Trim();
            company.OrzeczenieOUpadlosciDate = string.IsNullOrEmpty(dateInString) ? DateTime.MinValue : DateTime.Parse(dateInString);

            dateInString = root.Descendants().First(d => d.Name == _fiz_DataZakonczeniaPstepowaniaUpadlElName).Value.Trim();
            company.ZakonczeniePostepowaniaUpadlosiowegoDate = string.IsNullOrEmpty(dateInString) ? DateTime.MinValue : DateTime.Parse(dateInString);

            string boolStr = root.Descendants().First(d => d.Name == _fiz_NiePodjetoDzialansociElName).Value;
            company.NiePodjetoDzialanosci = string.IsNullOrEmpty(boolStr) ? false : bool.Parse(boolStr);


            company.SiedzibaNietypoweMiejsceLokalizacji = root.Descendants().First(d => d.Name == _fiz_AdSiedzNietypoweMiejsceLokazlizacjiElName).Value;
            company.NumerTelefonu = root.Descendants().First(d => d.Name == _fiz_NumerTelefonuElName).Value;
            company.NumerFaksu = root.Descendants().First(d => d.Name == _fiz_NumberFaksuElName).Value;
            company.Email = root.Descendants().First(d => d.Name == _fiz_AdresEmailElName).Value;
            company.WWW = root.Descendants().First(d => d.Name == _fiz_AdresStronyInternetowejElName).Value;
            
         

        }
    }
}