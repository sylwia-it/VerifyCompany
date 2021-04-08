using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerifyWhiteListCompany.Lib.WebServiceModel
{
    public class EntryError : EntryResult
    {
      
        public Exception Error { get; set; }
    }
}
