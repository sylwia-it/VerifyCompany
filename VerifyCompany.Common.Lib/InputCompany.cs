using System;
using System.Collections.Generic;
using System.Text;

namespace VerifyCompany.Common.Lib
{
    public class InputCompany
    {
        private string _id = string.Empty;
        private static readonly string _sep = "-";
        private static readonly string _rowSep = "+";
        private static readonly string _space = " ";
        public InputCompany()
        {
            this.IsToBeVerified = true;
            
        }

        public static string GetNIPFromID(string id)
        {
            return id.Substring(id.IndexOf(_sep) + 1);
        }

        public static string GetID(int rowNumber, string lp, string nip)
        {
            string nipS = nip.Replace(_space, string.Empty);
            nipS = nipS.Replace(_sep, string.Empty);
            return string.Concat(rowNumber, _rowSep, lp, _sep, nipS);
        }

        public static string GetFormattedStringFromID(string id)
        {
            return $"Wiersz:{id.Substring(0, id.IndexOf(_rowSep))} LP:{id.Substring(id.IndexOf(_rowSep) + 1, id.IndexOf(_sep) - id.IndexOf(_rowSep) - 1)} NIP:{id.Substring(id.IndexOf(_sep) + 1)}";
        }

        public string ID
        {
            get
            {
                if (string.IsNullOrEmpty(_id))
                {
                    _id = GetID(RowNumber, LP, NIP);
                }
                return _id;
            }

            private set
            {
                this._id = value;
            }
        }
        private string _nip = string.Empty;
        public string NIP 
        { get { return _nip; } 
          set 
            {
                if (value is null)
                    throw new ArgumentOutOfRangeException("NIP nie może być null");
                this._nip = value;
                _id = GetID(RowNumber, LP, NIP);
            }
        }
        private string _lp = string.Empty;
        public string LP
        {
            get
            {
                return _lp;
            }
            set
            {
                this._lp = value;
                _id = GetID(RowNumber, LP, NIP);
            }
        }
        
        public string NoteID { get; set; }
        public string NoteTitle { get; set; }
        public string NoteNettoAmount { get; set; }

        public string NoteDate { get; set; }
        public string BankAccountNumber { get; set; }

        public DateTime InvoiceDate { get; set; }
        public string PaymentDate { get; set; }
        public bool IsToBeVerified { get; set; }
        private int _rowNumber = 0;
        public int RowNumber
        {
            get
            {
                return this._rowNumber;
            }
            set
            {
                this._rowNumber = value;
                _id = GetID(RowNumber, LP, NIP);
            }
        }

        private List<InputCompanyFormatError> _formatErrors;
        public List<InputCompanyFormatError> FormatErrors
        {
            get
            {
                if (_formatErrors == null)
                    _formatErrors = new List<InputCompanyFormatError>();
                return _formatErrors;
            }
        }
    }
}


