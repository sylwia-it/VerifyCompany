using System;

namespace VerifyActiveCompany.Lib
{
    public enum BiRVerifyStatus
    {
        IsActive,
        IsNotActive,
        NipIncorrect,
        NoSession,
        Error,
        TooManyIds,
        NotFound,
        ErroneusOrEmptyReportName,
        NoSearchYet,
        CompanyIsNull
    }

    public static class BiRExtentions
    {
        private static readonly string _nonActiveOrg = "Firma NIE jest aktywana";
        private static readonly string _activeOrg = "Firma jest aktywana";
        private static readonly string _nipNotCorrect = "NIP firmy jest niepoprawny";
        private static readonly string _noSession = "Brak sesji. Sesja wygasła lub przekazano nieprawidłową wartość nagłówka sid.";
        private static readonly string _error = "Wystąpił nieznany błąd.";
        private static readonly string _tooManyIds = "Do metody DaneSzukaj przekazano zbyt wiele identyfikatorów.";
        private static readonly string _notFound = "Nie znaleziono podmiotów. (Częsta przyczyna: pNazwaRaportu dla P zamiast dla F i na odwrót).";
        private static readonly string _erroneusOrEmptyReportName = "Nieprawidłowa lub pusta nazwa raportu.";
        private static readonly string _noSearchYet = "Nie wykonano zapytania.";
        private static readonly string _companyIsNull = "Firma jest pusta (null).";

        public static string ToMessage(this BiRVerifyStatus status)
        {
            switch (status)
            {
                case BiRVerifyStatus.IsActive:
                    return _activeOrg;
                case BiRVerifyStatus.IsNotActive:
                    return _nonActiveOrg;
                case BiRVerifyStatus.NipIncorrect:
                    return _nipNotCorrect;
                case BiRVerifyStatus.NoSession:
                    return _noSession;
                case BiRVerifyStatus.Error:
                    return _error;
                case BiRVerifyStatus.TooManyIds:
                    return _tooManyIds;
                case BiRVerifyStatus.NotFound:
                    return _notFound;
                case BiRVerifyStatus.ErroneusOrEmptyReportName:
                    return _erroneusOrEmptyReportName;
                case BiRVerifyStatus.NoSearchYet:
                    return _noSearchYet;
                case BiRVerifyStatus.CompanyIsNull:
                    return _companyIsNull;
                default:
                    return string.Empty;
                    
            }
            
        }

    }
}
