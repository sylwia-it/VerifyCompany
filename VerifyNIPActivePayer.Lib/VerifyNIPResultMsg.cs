using System;
using System.Collections.Generic;
using System.Text;

namespace VerifyNIPActivePayer.Lib
{
    internal class VerifyNIPResultMsg
    {
        private const string _isActiveVATPayerMsg = "Podatnik jest czynnym podatnikiem VAT.";
        private const string _notRegisteredVATPayer = "Podatnik NIE jest zarejestrowany jako podatnik VAT.";
        private const string _cancelledVATPayerMsg = "Podatnik jest ZWOLNIONYM podatnikiem VAT.";
        private const string _nipNotCorrectMsg = "NIP jest niepoprawny.";
        private const string _dateNotCorrectMsg = "Data w zapytaniu jest niepoprawna.";
        private const string _serviceIsNotAvailableMsg = "Usługa zewnętrzna podatkowa nie jest dostępna.";
        private const string _serviceRequestErrorMsg = "Błąd zapytania do usługi zesnętrznej.";
        private const string _errorInClientSetUp = "Nie ustawiono połączenia z usługą.";
        private const string _errorUnknown = "Wystąpił nieznany błąd.";
    

        public static string GetMsg(VerifyNIPResult responseCode)
        {
            string msg = string.Empty;

            switch (responseCode)
            {
                case VerifyNIPResult.IsActiveVATPayer:
                    msg = _isActiveVATPayerMsg;
                    break;
                case VerifyNIPResult.NotRegisteredVATPayer:
                    msg = _notRegisteredVATPayer;
                    break;
                case VerifyNIPResult.CancelledVATPayer:
                    msg = _cancelledVATPayerMsg;
                    break;
                case VerifyNIPResult.NIPNotCorrect:
                    msg = _nipNotCorrectMsg;
                    break;
                case VerifyNIPResult.DateNotCorrect:
                    msg = _dateNotCorrectMsg;
                    break;
                case VerifyNIPResult.ServiceIsNotAvailable:
                    msg = _serviceIsNotAvailableMsg;
                    break;
                case VerifyNIPResult.ServiceRequestError:
                    msg = _serviceRequestErrorMsg;
                    break;
                case VerifyNIPResult.ErrorInClientSetUp:
                    msg = _errorInClientSetUp;
                    break;
                case VerifyNIPResult.Error:
                    msg = _errorUnknown;
                    break;
              
                default:
                    break;
            }

            return msg;
        }
    }
}
