using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerifyActiveCompany.Lib
{
    public interface IBiRClient
    {
        void Init();
        bool Close();

        BiRCompany GetCompany(string nip);
        BiRVerifyStatus GetLastErrorStatus();
    }
}
