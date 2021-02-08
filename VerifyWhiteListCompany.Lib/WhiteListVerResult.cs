using System;
using System.Collections.Generic;
using System.Text;

namespace VerifyWhiteListCompany.Lib
{
    public class WhiteListVerResult
    {
        public bool IsActiveVATPayer { get; internal set; }
        public bool IsGivenAccountNumOnWhiteList { get; internal set; }

        /// <summary>
        /// Provided by web serrice the string to verify in case
        /// </summary>
        public string ConfirmationResponseString { get; internal set; }

        /// <summary>
        /// Date when the verification was run
        /// </summary>
        public DateTime VerificationDate { get; internal set; }

        /// <summary>
        /// Correct account numbers
        /// </summary>
        public List<string> AccountNumbers { get; set; }
        
 
        public WhiteListVerResultStatus VerificationStatus { get; set; }

        private string _verStatusMsg = string.Empty;

        internal void SetVerStatusMessage(string msg)
        {
            this._verStatusMsg = msg;
        }
        public string ToMessage()
        {
            return _verStatusMsg;
        }

        /// <summary>
        /// Nip of the organization
        /// </summary>
        public string Nip { get; set; }


        public string FullResidenceAddress { get; set; }

        public string FullWorkingAddress { get; set; }
        
        /// <summary>
        /// The account number that shall be given as the input for checking
        /// </summary>
        public string GivenAccountNumber { get; set; }

        public string FullName { get; set; }

       
    }
}
