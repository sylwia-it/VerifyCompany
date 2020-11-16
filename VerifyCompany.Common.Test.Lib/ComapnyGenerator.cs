using System;
using System.Collections.Generic;
using System.Linq;
using VerifyCompany.Common.Lib;

namespace VerifyCompany.Common.Test.Lib
{
    public class CompanyGenerator
    {
        private static readonly string _correctNIP1 = "5250005834";
        private static readonly string _correctNIP2 = "7792348141";
        private static readonly string _correctNIP3 = "7811767696";
        private static readonly string _correctNIP4 = "7781454968";
        private static readonly string _correctNIP5 = "7781424849";

        private static readonly string _correctID1 = "c1";
        private static readonly string _correctID2 = "c2";
        private static readonly string _correctID3 = "c3";
        private static readonly string _correctID4 = "c4";
        private static readonly string _correctID5 = "c5";

        private static readonly string _incorrectNIP1 = "1234";
        private static readonly string _incorrectNIP2 = "779-234-81-41";
        private static readonly string _incorrectNIP3 = "789009";
        private static readonly string _incorrectNIP4 = "34k3444";
        private static readonly string _incorrectNIP5 = "14555";

        private static readonly string _incorrectID1 = "ic1";
        private static readonly string _incorrectID2 = "ic2";
        private static readonly string _incorrectID3 = "ic3";
        private static readonly string _incorrectID4 = "ic4";
        private static readonly string _incorrectID5 = "ic5";

        private static List<string> _correctNIPs;
        private static List<string> _incorrectNIPs;

        private static List<Company> _correctCompanies;
        private static List<Company> _incorrectCompanies;

        public static Dictionary<string, string> Companies { get; private set; }

        private static bool _isSetUp = false;

        protected CompanyGenerator() { }

        private static Random _random;

        public static string GetCorrectNIP()
        {
            if (!_isSetUp)
            { SetUp(); }
            return _correctNIPs[_random.Next(0, _correctNIPs.Count)];
        }

        public static string GetCorrectNIP(int index)
        {
            if (!_isSetUp)
            { SetUp(); }

            return _correctNIPs[index];
        }

        public static string GetInCorrectNIP(int index)
        {
            if (!_isSetUp)
            { SetUp(); }

            return _incorrectNIPs[index];
        }

        public static List<Company> GetAllCompanies()
        {
            if (!_isSetUp)
            { SetUp(); }

            return _correctCompanies.Concat(_incorrectCompanies).ToList();
        }


        private static void SetUp()
        {
            _random = new Random((int)DateTime.Now.Ticks);

            _correctNIPs = new List<string>() { _correctNIP1, _correctNIP2, _correctNIP3, _correctNIP4, _correctNIP5 };
            _incorrectNIPs = new List<string>() { _incorrectNIP1, _incorrectNIP2, _incorrectNIP3, _incorrectNIP4, _incorrectNIP5 };

            _correctCompanies = new List<Company>()
            {   new Company(){ NIP = _correctNIP1, ID = _correctID1},
                new Company(){ NIP = _correctNIP2,  ID = _correctID2},
                new Company(){ NIP = _correctNIP3,  ID = _correctID3},
                new Company(){ NIP = _correctNIP4, ID = _correctID4},
                new Company(){ NIP = _correctNIP5, ID = _correctID5}
            };

            _incorrectCompanies = new List<Company>()
            { 
                new Company(){ NIP = _incorrectNIP1, ID = _incorrectID1},
                new Company(){ NIP = _incorrectNIP2, ID = _incorrectID2},
                new Company(){ NIP = _incorrectNIP3, ID = _incorrectID3},
                new Company(){ NIP = _incorrectNIP4, ID = _incorrectID4},
                new Company(){ NIP = _incorrectNIP5, ID = _incorrectID5}
            };

            Companies = new Dictionary<string, string>
            {
                { _correctNIP1, _correctID1 },
                { _correctNIP2, _correctID2 },
                { _correctNIP3, _correctID3 },
                { _correctNIP4, _correctID4 },
                { _correctNIP5, _correctID5 },

                { _incorrectNIP1, _incorrectID1 },
                { _incorrectNIP2, _incorrectID2 },
                { _incorrectNIP3, _incorrectID3 },
                { _incorrectNIP4, _incorrectID4 },
                { _incorrectNIP5, _incorrectID5 }
            };

            _isSetUp = true;
        }
    }
}
