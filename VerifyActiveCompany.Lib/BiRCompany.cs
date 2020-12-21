using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VerifyCompany.Common.Lib;

namespace VerifyActiveCompany.Lib
{
    public class BiRCompany : Company
    {
        public string NipStatus { get; set; }

        public string Type { get; set; }
        public BiRCompanyType CompanyType { get; set; }

        public DateTime ZakonczeniaDzialalnosciDate { get; set; }

        public string SilosID { get; set; }
        public string NameShort { get;  set; }
        public string PodstawowaFormaPrawna { get;  set; }
        public string SzczegolnaFormaPrawna { get;  set; }
        public string FormaFinansowania { get;  set; }
        public string FormaWlasnosci { get;  set; }
        public string OrganRejestrowy { get;  set; }
        public string OrganZalozycielski { get; internal set; }
        public string NumWRejestrzeEwidencji { get; internal set; }
        public string RodzajRejestruEwidencji { get; internal set; }
        public string WWW { get; internal set; }
        public string Email { get; internal set; }
        public string NumerFaksu { get; internal set; }
        public string NumerTelefonu { get; internal set; }
        public string SiedzibaNietypoweMiejsceLokalizacji { get; internal set; }
        public DateTime ZakonczeniePostepowaniaUpadlosiowegoDate { get; internal set; }
        public DateTime OrzeczenieOUpadlosciDate { get; internal set; }
        public DateTime SkresleniaRegonDate { get; internal set; }
        public DateTime ZaistnieniaZmianyDate { get; internal set; }
        public DateTime ZawieszeniaDate { get; internal set; }
        public DateTime WpisDoRejestruEwidencjiDate { get; internal set; }
        public DateTime RegonDate { get; internal set; }
        public DateTime StartRunDate { get; internal set; }
        public DateTime CreationDate { get; internal set; }
        public DateTime WziowieniaDate { get; internal set; }
        public bool NiePodjetoDzialanosci { get; internal set; }
        public string NrWRejEwidencji { get; internal set; }
        public DateTime SkresleniaZRejestruEwidencji { get; internal set; }
        public DateTime WpisuDoRejestruEwidencji { get; internal set; }
    }
}
