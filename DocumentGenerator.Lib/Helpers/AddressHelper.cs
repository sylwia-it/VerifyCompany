using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentGenerator.Lib.Helpers
{
    public static class AddressHelper
    {
        private static AddressChecker _addressChecker = null;
        
        public static string GetStreetPrefix(string lineWithAddress)
        {
            if (_addressChecker == null)
            {
                _addressChecker = new AddressChecker();
                _addressChecker.Init();
            }

            return _addressChecker.GetStart(lineWithAddress);
        }
    }
}
