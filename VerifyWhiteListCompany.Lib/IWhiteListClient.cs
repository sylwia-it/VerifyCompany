using System.Threading.Tasks;
using VerifyWhiteListCompany.Lib.WebServiceModel;

namespace VerifyWhiteListCompany.Lib
{
    interface IWhiteListClient
    {
        void Init(string url);

        Task<EntryListResponse> VerifyCompanies(string nips);
    }
}
