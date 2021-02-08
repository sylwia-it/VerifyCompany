using System;
using System.Collections.Generic;
using System.Text;

namespace VerifyWhiteListCompany.Lib
{
    static class WhiteListClientFactory
    {
        private static IWhiteListClient _client;
        private static readonly string _baseUrl = "https://wl-api.mf.gov.pl/";
        public static IWhiteListClient GetClient()
        {
            if (_client == null)
            {
                _client = new WhiteListClient();
                _client.Init(_baseUrl);
                return _client;
            }

            return _client;
        }

        public static void SetClient(IWhiteListClient client)
        {
            _client = client;
        }
    }
}
