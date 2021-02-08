using System;
using System.Collections.Generic;
using System.Text;

namespace VerifyCompany.Common.Lib
{
    public class InputCompany
    {
        private string _id = string.Empty;
        private static readonly string _sep = "-";

        public static string GetID(string lp, string nip)
        {
            string nipS = nip.Replace(" ", string.Empty);
            nipS = nipS.Replace("-", string.Empty);
            return string.Concat(lp, _sep, nipS);
        }
        public string ID
        {
            get
            {
                if (string.IsNullOrEmpty(_id))
                {
                    _id = GetID(LP, NIP);
                }
                return _id;
            }

            private set
            {
                this._id = value;
            }
        }
        public string NIP { get; set; }
        public string LP { get; set; }
        public string NoteID { get; set; }
        public string NoteTitle { get; set; }
        public string NoteNettoAmount { get; set; }

        public string NoteDate { get; set; }
        public string BankAccountNumber { get; set; }

        public string PaymentDate { get; set; }
        public bool IsToBeVerified { get; set; }
        public int RowNumber { get; set; }
    }
}


