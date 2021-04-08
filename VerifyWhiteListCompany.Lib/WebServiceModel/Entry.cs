using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VerifyWhiteListCompany.Lib.WebServiceModel;

namespace VerifyWhiteListCompany.Lib.WebServiceModel
{
    public class Entry
    {
        public string Identifier { get; set; }

        public List<Entity> Subjects { get; set; }

        public VerifyWhiteListCompany.Lib.WebServiceModel.Exception Error { get; set; }


    }
}
