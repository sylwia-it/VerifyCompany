using System;
using System.Collections.Generic;
using System.Security.Authentication.ExtendedProtection;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text;
using ServiceReference1;

namespace DocumentGenerator.Lib.Helpers
{
    public class AddressChecker
    {
        public void Init()
        {
            

            System.ServiceModel.WSHttpBinding binding = new WSHttpBinding();
            binding.Security.Mode = SecurityMode.TransportWithMessageCredential;
            binding.Security.Message.ClientCredentialType = MessageCredentialType.UserName;
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            //binding.Security.Transport.ExtendedProtectionPolicy = ExtendedProtectionPolicy.
            binding.MaxReceivedMessageSize = 2147483647;
            binding.MaxBufferPoolSize = 2147483647;
            
            //binding.MessageVersion = new System.ServiceModel.Channels.MessageVersion()
                //= MessageSecurityVersion.WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10
            binding.AllowCookies = true;


            var endPoint = new EndpointAddress("https://uslugaterytws1.stat.gov.pl/TerytWs1.svc");


            try
            {
                
                var proxy = new ChannelFactory<ServiceReference1.ITerytWs1>(binding, endPoint);
                proxy.Credentials.UserName.UserName = "TestPubliczny";
                proxy.Credentials.UserName.Password = "1234abcd";
                var result = proxy.CreateChannel();
                var test = result.CzyZalogowany();
            }
            catch (Exception e)
            {

            }
        }
    }
}
