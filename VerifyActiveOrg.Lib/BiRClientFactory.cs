using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using VerifyCompany.Common.Lib;

namespace VerifyActiveOrg.Lib
{
    public  static class BiRClientFactory
    {
        private static UslugaBIRzewnPublClient _client =null;
        public static  UslugaBIRzewnPublClient GetClient(string sid)
        {
            if (_client == null)
            {
                 CreateClientAsync(sid);
            }

            return _client;
        }

        private static UslugaBIRzewnPublClient CreateClientAsync(string sid)
        {
            UslugaBIRzewnPublClient client = null;
            try
            {
                WSHttpBinding binding = new WSHttpBinding();
                binding.Security.Mode = SecurityMode.Transport;
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
                //binding.
                //binding.MessageEncoding = WSMessageEncoding.Mtom;
                binding.AllowCookies = true;

                var endPoint =
                    new EndpointAddress("https://wyszukiwarkaregon.stat.gov.pl/wsBIR/UslugaBIRzewnPubl.svc");
                //new EndpointAddress("https://wyszukiwarkaregontest.stat.gov.pl/wsBIR/UslugaBIRzewnPubl.svc");

                client = new UslugaBIRzewnPublClient(binding, endPoint);
               
                client.OpenAsync().GetAwaiter().GetResult();
                
                string sidLogin = "f2661e11eac6486993cd";
                // sidLogin = "abcde12345abcde12345";
               sid = client.Zaloguj(sidLogin);




                if (sid == string.Empty)
                {
                 //   IsInitialized = false;
                   // LastSearchMsg = "SID Login jest pusty";
                   // return false;
                }

                OperationContextScope scope = new OperationContextScope(client.InnerChannel);
                HttpRequestMessageProperty reqProps = new HttpRequestMessageProperty();
                reqProps.Headers.Add("sid", sid);
                reqProps.Headers.Add("user-agent", "CompanyVerifierbot/1.2 ()");
                reqProps.Headers.Add(System.Net.HttpRequestHeader.Cookie, "security=true");

                OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = reqProps;

                var paramse = new ParametryWyszukiwania();
                //string nipPure = nip.Replace("-", string.Empty).Replace(" ", string.Empty).Trim();
                paramse.Nip = "7830002670";
                string errorCode = string.Empty;
                string result = string.Empty;

                result = _client.DaneSzukajPodmioty(paramse);


            }
            catch
            { }

            return client;
        }

        public static Company GetCompany(string nip)
        {

           
          
            return null;


        }

        public static void SetClient(UslugaBIRzewnPublClient client)
        {
            _client = client;
        }
    }
}