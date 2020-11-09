﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Ten kod został wygenerowany przez narzędzie.
//
//     Zmiany w tym pliku mogą spowodować niewłaściwe zachowanie i zostaną utracone
//     w przypadku ponownego wygenerowania kodu.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VerifyNIP.Service
{
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://www.mf.gov.pl/uslugiBiznesowe/uslugiDomenowe/AP/WeryfikacjaVAT/2018/03/01", ConfigurationName="VerifyNIP.Service.WeryfikacjaVAT")]
    public interface WeryfikacjaVAT
    {
        
        // CODEGEN: Trwa generowanie kontraktu komunikatu, ponieważ operacja SprawdzNIP nie jest ani zdalnym wywołaniem procedury, ani opakowanym dokumentem.
        [System.ServiceModel.OperationContractAttribute(Action="http://www.mf.gov.pl/uslugiBiznesowe/uslugiDomenowe/AP/WeryfikacjaVAT/2018/03/01/" +
            "WeryfikacjaVAT/SprawdzNIP", ReplyAction="http://www.mf.gov.pl/uslugiBiznesowe/uslugiDomenowe/AP/WeryfikacjaVAT/2018/03/01/" +
            "WeryfikacjaVAT/SprawdzNIPResponse")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        VerifyNIP.Service.SprawdzNIPOdpowiedz SprawdzNIP(VerifyNIP.Service.SprawdzNIPZapytanie request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.mf.gov.pl/uslugiBiznesowe/uslugiDomenowe/AP/WeryfikacjaVAT/2018/03/01/" +
            "WeryfikacjaVAT/SprawdzNIP", ReplyAction="http://www.mf.gov.pl/uslugiBiznesowe/uslugiDomenowe/AP/WeryfikacjaVAT/2018/03/01/" +
            "WeryfikacjaVAT/SprawdzNIPResponse")]
        System.Threading.Tasks.Task<VerifyNIP.Service.SprawdzNIPOdpowiedz> SprawdzNIPAsync(VerifyNIP.Service.SprawdzNIPZapytanie request);
        
        // CODEGEN: Trwa generowanie kontraktu komunikatu, ponieważ operacja SprawdzNIPNaDzien nie jest ani zdalnym wywołaniem procedury, ani opakowanym dokumentem.
        [System.ServiceModel.OperationContractAttribute(Action="http://www.mf.gov.pl/uslugiBiznesowe/uslugiDomenowe/AP/WeryfikacjaVAT/2018/03/01/" +
            "WeryfikacjaVAT/SprawdzNIPNaDzien", ReplyAction="http://www.mf.gov.pl/uslugiBiznesowe/uslugiDomenowe/AP/WeryfikacjaVAT/2018/03/01/" +
            "WeryfikacjaVAT/SprawdzNIPNaDzienResponse")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        VerifyNIP.Service.SprawdzNIPNaDzienOdpowiedz SprawdzNIPNaDzien(VerifyNIP.Service.SprawdzNIPNaDzienZapytanie request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.mf.gov.pl/uslugiBiznesowe/uslugiDomenowe/AP/WeryfikacjaVAT/2018/03/01/" +
            "WeryfikacjaVAT/SprawdzNIPNaDzien", ReplyAction="http://www.mf.gov.pl/uslugiBiznesowe/uslugiDomenowe/AP/WeryfikacjaVAT/2018/03/01/" +
            "WeryfikacjaVAT/SprawdzNIPNaDzienResponse")]
        System.Threading.Tasks.Task<VerifyNIP.Service.SprawdzNIPNaDzienOdpowiedz> SprawdzNIPNaDzienAsync(VerifyNIP.Service.SprawdzNIPNaDzienZapytanie request);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.mf.gov.pl/uslugiBiznesowe/uslugiDomenowe/AP/WeryfikacjaVAT/2018/03/01")]
    public partial class TWynikWeryfikacjiVAT
    {
        
        private TKodWeryfikacjiVAT kodField;
        
        private string komunikatField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public TKodWeryfikacjiVAT Kod
        {
            get
            {
                return this.kodField;
            }
            set
            {
                this.kodField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public string Komunikat
        {
            get
            {
                return this.komunikatField;
            }
            set
            {
                this.komunikatField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.mf.gov.pl/uslugiBiznesowe/uslugiDomenowe/AP/WeryfikacjaVAT/2018/03/01")]
    public enum TKodWeryfikacjiVAT
    {
        
        /// <remarks/>
        N,
        
        /// <remarks/>
        C,
        
        /// <remarks/>
        Z,
        
        /// <remarks/>
        I,
        
        /// <remarks/>
        D,
        
        /// <remarks/>
        X,
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class SprawdzNIPZapytanie
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.mf.gov.pl/uslugiBiznesowe/uslugiDomenowe/AP/WeryfikacjaVAT/2018/03/01", Order=0)]
        public string NIP;
        
        public SprawdzNIPZapytanie()
        {
        }
        
        public SprawdzNIPZapytanie(string NIP)
        {
            this.NIP = NIP;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class SprawdzNIPOdpowiedz
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.mf.gov.pl/uslugiBiznesowe/uslugiDomenowe/AP/WeryfikacjaVAT/2018/03/01", Order=0)]
        public VerifyNIP.Service.TWynikWeryfikacjiVAT WynikOperacji;
        
        public SprawdzNIPOdpowiedz()
        {
        }
        
        public SprawdzNIPOdpowiedz(VerifyNIP.Service.TWynikWeryfikacjiVAT WynikOperacji)
        {
            this.WynikOperacji = WynikOperacji;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class SprawdzNIPNaDzienZapytanie
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.mf.gov.pl/uslugiBiznesowe/uslugiDomenowe/AP/WeryfikacjaVAT/2018/03/01", Order=0)]
        public string NIP;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.mf.gov.pl/uslugiBiznesowe/uslugiDomenowe/AP/WeryfikacjaVAT/2018/03/01", Order=1)]
        public System.DateTime Data;
        
        public SprawdzNIPNaDzienZapytanie()
        {
        }
        
        public SprawdzNIPNaDzienZapytanie(string NIP, System.DateTime Data)
        {
            this.NIP = NIP;
            this.Data = Data;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class SprawdzNIPNaDzienOdpowiedz
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.mf.gov.pl/uslugiBiznesowe/uslugiDomenowe/AP/WeryfikacjaVAT/2018/03/01", Order=0)]
        public VerifyNIP.Service.TWynikWeryfikacjiVAT WynikOperacji;
        
        public SprawdzNIPNaDzienOdpowiedz()
        {
        }
        
        public SprawdzNIPNaDzienOdpowiedz(VerifyNIP.Service.TWynikWeryfikacjiVAT WynikOperacji)
        {
            this.WynikOperacji = WynikOperacji;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    public interface WeryfikacjaVATChannel : VerifyNIP.Service.WeryfikacjaVAT, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    public partial class WeryfikacjaVATClient : System.ServiceModel.ClientBase<VerifyNIP.Service.WeryfikacjaVAT>, VerifyNIP.Service.WeryfikacjaVAT
    {
        
        /// <summary>
        /// Wdróż tę metodę częściową, aby skonfigurować punkt końcowy usługi.
        /// </summary>
        /// <param name="serviceEndpoint">Punkt końcowy do skonfigurowania</param>
        /// <param name="clientCredentials">Poświadczenia klienta</param>
        static partial void ConfigureEndpoint(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Description.ClientCredentials clientCredentials);
        
        public WeryfikacjaVATClient() : 
                base(WeryfikacjaVATClient.GetDefaultBinding(), WeryfikacjaVATClient.GetDefaultEndpointAddress())
        {
            this.Endpoint.Name = EndpointConfiguration.BasicHttpBinding_WeryfikacjaVAT.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public WeryfikacjaVATClient(EndpointConfiguration endpointConfiguration) : 
                base(WeryfikacjaVATClient.GetBindingForEndpoint(endpointConfiguration), WeryfikacjaVATClient.GetEndpointAddress(endpointConfiguration))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public WeryfikacjaVATClient(EndpointConfiguration endpointConfiguration, string remoteAddress) : 
                base(WeryfikacjaVATClient.GetBindingForEndpoint(endpointConfiguration), new System.ServiceModel.EndpointAddress(remoteAddress))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public WeryfikacjaVATClient(EndpointConfiguration endpointConfiguration, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(WeryfikacjaVATClient.GetBindingForEndpoint(endpointConfiguration), remoteAddress)
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public WeryfikacjaVATClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        VerifyNIP.Service.SprawdzNIPOdpowiedz VerifyNIP.Service.WeryfikacjaVAT.SprawdzNIP(VerifyNIP.Service.SprawdzNIPZapytanie request)
        {
            return base.Channel.SprawdzNIP(request);
        }
        
        public VerifyNIP.Service.TWynikWeryfikacjiVAT SprawdzNIP(string NIP)
        {
            VerifyNIP.Service.SprawdzNIPZapytanie inValue = new VerifyNIP.Service.SprawdzNIPZapytanie();
            inValue.NIP = NIP;
            VerifyNIP.Service.SprawdzNIPOdpowiedz retVal = ((VerifyNIP.Service.WeryfikacjaVAT)(this)).SprawdzNIP(inValue);
            return retVal.WynikOperacji;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<VerifyNIP.Service.SprawdzNIPOdpowiedz> VerifyNIP.Service.WeryfikacjaVAT.SprawdzNIPAsync(VerifyNIP.Service.SprawdzNIPZapytanie request)
        {
            return base.Channel.SprawdzNIPAsync(request);
        }
        
        public System.Threading.Tasks.Task<VerifyNIP.Service.SprawdzNIPOdpowiedz> SprawdzNIPAsync(string NIP)
        {
            VerifyNIP.Service.SprawdzNIPZapytanie inValue = new VerifyNIP.Service.SprawdzNIPZapytanie();
            inValue.NIP = NIP;
            return ((VerifyNIP.Service.WeryfikacjaVAT)(this)).SprawdzNIPAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        VerifyNIP.Service.SprawdzNIPNaDzienOdpowiedz VerifyNIP.Service.WeryfikacjaVAT.SprawdzNIPNaDzien(VerifyNIP.Service.SprawdzNIPNaDzienZapytanie request)
        {
            return base.Channel.SprawdzNIPNaDzien(request);
        }
        
        public VerifyNIP.Service.TWynikWeryfikacjiVAT SprawdzNIPNaDzien(string NIP, System.DateTime Data)
        {
            VerifyNIP.Service.SprawdzNIPNaDzienZapytanie inValue = new VerifyNIP.Service.SprawdzNIPNaDzienZapytanie();
            inValue.NIP = NIP;
            inValue.Data = Data;
            VerifyNIP.Service.SprawdzNIPNaDzienOdpowiedz retVal = ((VerifyNIP.Service.WeryfikacjaVAT)(this)).SprawdzNIPNaDzien(inValue);
            return retVal.WynikOperacji;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<VerifyNIP.Service.SprawdzNIPNaDzienOdpowiedz> VerifyNIP.Service.WeryfikacjaVAT.SprawdzNIPNaDzienAsync(VerifyNIP.Service.SprawdzNIPNaDzienZapytanie request)
        {
            return base.Channel.SprawdzNIPNaDzienAsync(request);
        }
        
        public System.Threading.Tasks.Task<VerifyNIP.Service.SprawdzNIPNaDzienOdpowiedz> SprawdzNIPNaDzienAsync(string NIP, System.DateTime Data)
        {
            VerifyNIP.Service.SprawdzNIPNaDzienZapytanie inValue = new VerifyNIP.Service.SprawdzNIPNaDzienZapytanie();
            inValue.NIP = NIP;
            inValue.Data = Data;
            return ((VerifyNIP.Service.WeryfikacjaVAT)(this)).SprawdzNIPNaDzienAsync(inValue);
        }
        
        public virtual System.Threading.Tasks.Task OpenAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndOpen));
        }
        
        public virtual System.Threading.Tasks.Task CloseAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginClose(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndClose));
        }
        
        private static System.ServiceModel.Channels.Binding GetBindingForEndpoint(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.BasicHttpBinding_WeryfikacjaVAT))
            {
                System.ServiceModel.BasicHttpBinding result = new System.ServiceModel.BasicHttpBinding();
                result.MaxBufferSize = int.MaxValue;
                result.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
                result.MaxReceivedMessageSize = int.MaxValue;
                result.AllowCookies = true;
                result.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
                return result;
            }
            throw new System.InvalidOperationException(string.Format("Nie można znaleźć punktu końcowego o nazwie „{0}”.", endpointConfiguration));
        }
        
        private static System.ServiceModel.EndpointAddress GetEndpointAddress(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.BasicHttpBinding_WeryfikacjaVAT))
            {
                return new System.ServiceModel.EndpointAddress("https://sprawdz-status-vat.mf.gov.pl/");
            }
            throw new System.InvalidOperationException(string.Format("Nie można znaleźć punktu końcowego o nazwie „{0}”.", endpointConfiguration));
        }
        
        private static System.ServiceModel.Channels.Binding GetDefaultBinding()
        {
            return WeryfikacjaVATClient.GetBindingForEndpoint(EndpointConfiguration.BasicHttpBinding_WeryfikacjaVAT);
        }
        
        private static System.ServiceModel.EndpointAddress GetDefaultEndpointAddress()
        {
            return WeryfikacjaVATClient.GetEndpointAddress(EndpointConfiguration.BasicHttpBinding_WeryfikacjaVAT);
        }
        
        public enum EndpointConfiguration
        {
            
            BasicHttpBinding_WeryfikacjaVAT,
        }
    }
}
