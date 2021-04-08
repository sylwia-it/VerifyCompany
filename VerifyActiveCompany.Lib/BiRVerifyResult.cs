using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerifyActiveCompany.Lib
{
    public class BiRVerifyResult
    {
        private string _message;
        public BiRVerifyResult(BiRVerifyStatus status)
        {
            BiRVerifyStatus = status;
            _message = BiRVerifyStatus.ToMessage();
        }

        public BiRVerifyStatus BiRVerifyStatus { get; private set; }
        public string Message { get { return _message; } set { _message = value; } } 
    }
}
