using VerifyNIP.Service;

namespace VerifyNIPActivePayer.Lib
{
    public static class WeryfikacjaVATClientFactory
    {
        private static WeryfikacjaVAT _client;
        public static WeryfikacjaVAT GetWeryfikacjaVATClient()
        {
            if (_client == null)
            {
                _client = new WeryfikacjaVATClient();
            }

            return _client;
        }

        public static void SetWeryfikacjaVATClinet(WeryfikacjaVAT client)
        {
            _client = client;
        }
    }
}
