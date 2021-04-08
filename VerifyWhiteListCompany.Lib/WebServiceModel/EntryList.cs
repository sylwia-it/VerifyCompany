using System.Collections.Generic;

namespace VerifyWhiteListCompany.Lib.WebServiceModel
{
    public class EntryList
    {
        public string RequestDateTime { get; set; }

        public string RequestId { get; set; }

        public List<Entry> Entries { get; set; }

    }
}