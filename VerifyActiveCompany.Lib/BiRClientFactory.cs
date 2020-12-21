using System;
using System.Configuration;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using VerifyActiveCompany.Lib.Service;

namespace VerifyActiveCompany.Lib
{
    public  static class BiRClientFactory
    {
        private static IBiRClient _client = null;
        public static  IBiRClient GetClient()
        {
           
            if (_client == null)
            {
                 CreateClient();
            }

            return _client;
        }

        private static IBiRClient CreateClient()
        {
            string sidLogin, url;
            try
            {
                sidLogin = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location).AppSettings.Settings["birLogin"].Value;
                url = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location).AppSettings.Settings["url"].Value;

            } catch (Exception e)
            {
                throw new BiRClientSetUpException("Błąd poczas odczytywania parametrów logowania i ustawiania z pliku konfiguracyjnego.", e);
            }
            if (string.IsNullOrEmpty(sidLogin) || string.IsNullOrEmpty(url))
            {
                throw new BiRClientSetUpException("Błąd wartości parametrów logowania i ustawiania z pliku konfiguracyjnego - są puste.");
            }

            _client = new BiRClient(url, sidLogin);
            _client.Init();

            return _client;
        }

        public static void SetClient(IBiRClient client)
        {
            _client = client;
        }
    }
}