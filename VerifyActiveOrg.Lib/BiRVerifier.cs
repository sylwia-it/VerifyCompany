using System;
using System.Collections.Generic;
using System.Text;


namespace VerifyActiveOrg.Lib
{
    public class BiRVerifier
    {
        IUslugaBIRzewnPubl client;

        public BiRVerifier(string sid)
        {
            client = BiRClientFactory.GetClient(sid);
        }
          
    }
}
