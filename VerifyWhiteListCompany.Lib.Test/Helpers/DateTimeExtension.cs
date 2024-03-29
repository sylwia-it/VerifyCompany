﻿using System;
using System.Collections.Generic;
using System.Text;

namespace VerifyWhiteListCompany.Lib.Test.Helpers
{
    public static class DTHelper
    {
        private static readonly string _dateFormat = "dd-MM-yyyy";
        private static readonly string _dateFormat2 = "yyyy-MM-dd";
        private static readonly string _dateFormat3 = "dd.MM.yyyy";
        public static bool IsItToday(string givenDate)
        {
            if (givenDate.Contains(DateTime.Now.Date.ToString(_dateFormat)) || givenDate.Contains(DateTime.Now.Date.ToString(_dateFormat2)) || givenDate.Contains(DateTime.Now.Date.ToString(_dateFormat3)))
                return true;
            return false;
        }
    }
}
