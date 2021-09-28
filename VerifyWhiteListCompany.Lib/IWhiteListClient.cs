using System.Threading.Tasks;
using VerifyWhiteListCompany.Lib.WebServiceModel;
using System;

namespace VerifyWhiteListCompany.Lib
{
    interface IWhiteListClient
    {
        void Init(string url);

        Task<EntryListResponse> VerifyCompanies(string nips, DateTime date);
    }
}
