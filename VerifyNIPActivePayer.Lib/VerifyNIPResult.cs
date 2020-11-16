namespace VerifyNIPActivePayer.Lib
{
   /// <summary>
   /// Enum represents possible verification results.
   /// <code>Verify.IsActiveVATPayer</code> is the positive result.
   /// </summary>
    public enum VerifyNIPResult
    {
        /// <summary>
        /// Postive result of verification.
        /// </summary>
        IsActiveVATPayer,
        /// <summary>
        /// Negative result of verification.
        /// </summary>
        NotRegisteredVATPayer,
        /// <summary>
        /// Negative result of verification.
        /// </summary>
        CancelledVATPayer,
        /// <summary>
        /// Error in the query to the webservice.
        /// </summary>
        NIPNotCorrect,
        /// <summary>
        /// Error in the query to the webservice.
        /// </summary>
        DateNotCorrect,
        /// <summary>
        /// The webservice of Ministry is not available.
        /// </summary>
        ServiceIsNotAvailable,
        ServiceRequestError,
        /// <summary>
        /// There was a problem with setting up a connection with the webservice.
        /// </summary>
        ErrorInClientSetUp,
        /// <summary>
        /// Uknown error.
        /// </summary>
        Error,
        
    }

    public static class Extentions
    {
        public static string ToMessage(this VerifyNIPResult verifyNIPResult)
        {
            return VerifyNIPResultMsg.GetMsg(verifyNIPResult);
        }
    }

}