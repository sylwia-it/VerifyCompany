using System;

namespace VerifyActiveOrg.Lib
{
    public enum BiRVerifyStatus
    {
        IsActive,

        IsNotActive
    }

    public static class BiRExtentions
    {
        private static readonly string _nonActiveOrg = "Firma NIE jest aktywana";
        private static readonly string _activeOrg = "Firma jest aktywana";

        public static string ToMessage(this BiRVerifyStatus status)
        {
            if (status == BiRVerifyStatus.IsActive)
                return _activeOrg;
            return _nonActiveOrg;
        }

    }
}
