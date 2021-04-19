using System;
using System.Collections.Generic;
using System.Text;

namespace VerifyWhiteListCompany.Lib
{
    public enum WhiteListVerResultStatus
    {
        ErrorNIPEmpty,
        ErrorEmptyResponse,
        ErrorNIPError,
        Error,
        ErrorVerProcessFailed,

        ActiveVATPayerAccountOKVerSuccessfull,
        ActiveVATPayerVerSuccessfull,
        ActiveVATPayerVerScuccessButGivenAccountNotVerified,

        ActiveVATPayerButHasNoAccounts,
        ActiveVATPayerButGivenAccountNotOnWhiteList,
        ActiveVATPayerButGivenAccountWrong,
        
        NotActiveVATPayer,
        NotInVATRegisterCompany,
        ErrorOtherNIPError,
        MultipleEntriesError,
        
    }

  
}
