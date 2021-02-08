using System;
using System.Collections.Generic;
using System.Text;

namespace VerifyWhiteListCompany.Lib
{
    [Serializable]
    class WhiteListClientException : Exception 
    {
        private VerifyWhiteListCompany.Lib.WebServiceModel.Exception _exception;

        public WhiteListClientException(VerifyWhiteListCompany.Lib.WebServiceModel.Exception e)
        {
            this._exception = e;
        }

        public VerifyWhiteListCompany.Lib.WebServiceModel.Exception GetException()
        {
            return _exception;
        }
    }
}
