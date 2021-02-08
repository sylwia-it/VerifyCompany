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
        private static readonly string _correctNIP6 = "5252678750";
        private static readonly string _correctNIP7 = "7010363179";
        private static readonly string _correctNIP8 = "5262493733";
        private static readonly string _correctNIP9 = "6340128015";
        private static readonly string _correctNIP10 = "5270103391";
        private static readonly string _correctNIP11 = "6770044350";



        public static readonly List<string> CorrectNIPs = new List<string>
            {_correctNIP1, _correctNIP2, _correctNIP3, _correctNIP4, _correctNIP5, _correctNIP6, _correctNIP7, _correctNIP8, _correctNIP9,  _correctNIP10,  _correctNIP11, "9721305036", "2090001440", "7851732729", "7770000755", "7811642368", "7770003274", "7770005132", "7822622168", "7790021358", "5260002151", "7820021602", "7811781236", "7820069744", "7811846734", "7791002618", "7792433421", "7781118503", "5223071241", "7770000991", "5260021533", "7792446406", "5252289623", "7822131157", "777001479", "7810013469"};

        private static readonly string _correctPhisicalCompanyNIP1 = "9471862705";
        

        private static readonly string _correctID1 = "c1";
        private static readonly string _correctID2 = "c2";
        private static readonly string _correctID3 = "c3";
        private static readonly string _correctID4 = "c4";
        private static readonly string _correctID5 = "c5";
        private static readonly string _correctID6 = "c6";
        private static readonly string _correctID7 = "c7";
        private static readonly string _correctID8 = "c8";
        private static readonly string _correctID9 = "c9";
        private static readonly string _correctID10 = "c10";
        private static readonly string _correctID11 = "c11";
        private static readonly string _correctPhisicalCompanyID1 = "phc1";

        private static readonly string _incorrectNIP1 = "1234";
        private static readonly string _incorrectNIP2 = "9999999999";
        private static readonly string _incorrectNIP3 = "78 9009";
        private static readonly string _incorrectNIP4 = "34k3444";
        private static readonly string _incorrectNIP5 = "14-555";

        private static readonly string _incorrectID1 = "ic1";
        private static readonly string _incorrectID2 = "ic2";
        private static readonly string _incorrectID3 = "ic3";
        private static readonly string _incorrectID4 = "ic4";
        private static readonly string _incorrectID5 = "ic5";

        private static readonly string _zakUpadloscioweNIP1 = "5992973948";
        private static readonly string _zakUpadloscioweNIPID1 = "u1";

        private static List<string> _correctNIPs;
        private static List<string> _incorrectNIPs;

        private static List<Company> _correctCompanies;
        private static List<Company> _incorrectCompanies;

        public static Dictionary<string, string> CompaniesNIPIDDic { get; private set; }
        private static Dictionary<string, Company> _correctPhisicalCompanyDic;
        private static Dictionary<string, Company> _correctCompaniesDic;

        private static bool _isSetUp = false;

        protected CompanyGenerator() { }

        private static Random _random;
        private static Dictionary<string, Company> _incorrectNipCompaniesDic;

        public static Dictionary<string, Company> GetCorrectPhisicalCompanies()
        {
            if (!_isSetUp)
            { SetUp(); }
            return _correctPhisicalCompanyDic;
        }
        public static string GetCorrectNIP()
        {
            if (!_isSetUp)
            { SetUp(); }
            return _correctNIPs[_random.Next(0, _correctNIPs.Count)];
        }

        public static string GetZakonczoneUpadaloscioweNIP()
        {
            return _zakUpadloscioweNIP1;
        }

        public static string GetCorrectNIP(int index)
        {
            if (!_isSetUp)
            { SetUp(); }

            return _correctNIPs[index];
        }

        public static List<string> GetCorrectNIPs()
        {
            if (!_isSetUp)
            { SetUp(); }

            return _correctNIPs.ToList<string>();
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



        public static Dictionary<string, Company> GetCorrectCompanies()
        {
            if (!_isSetUp)
            { SetUp(); }

            return _correctCompaniesDic;
        }

        public static Dictionary<string, Company> GetInCorrectNipCompanies()
        {
            if (!_isSetUp)
            { SetUp(); }

            return _incorrectNipCompaniesDic;
        }



        private static void SetUp()
        {
            _random = new Random((int)DateTime.Now.Ticks);

            _correctNIPs = new List<string>() { _correctNIP1, _correctNIP2, _correctNIP3, _correctNIP4, _correctNIP5, _correctNIP6, _correctNIP7, _correctNIP8, _correctNIP9, _correctNIP10, _correctNIP11};
            _incorrectNIPs = new List<string>() { _incorrectNIP1, _incorrectNIP2, _incorrectNIP3, _incorrectNIP4, _incorrectNIP5 };

            _correctCompanies = new List<Company>()
            {   new Company(){ NIP = _correctNIP1, ID = _correctID1, BankAccountNumber = "03124061751111001095041381" }, 
                new Company(){ NIP = _correctNIP2, ID = _correctID2, BankAccountNumber = "24114011240000582620001003" },
                new Company(){ NIP = _correctNIP3, ID = _correctID3, BankAccountNumber = "68160010841844744520000009" },
                new Company(){ NIP = _correctNIP4, ID = _correctID4, BankAccountNumber = "27116022020000000109081086" },
                new Company(){ NIP = _correctNIP5, ID = _correctID5, BankAccountNumber = "11114011240000584072001027" },
                new Company(){ NIP = _correctNIP6, ID = _correctID6, BankAccountNumber = "64114020040000300278432279" },
                new Company(){ NIP = _correctNIP7, ID = _correctID7, BankAccountNumber = "17124060031111000049409738" },
                new Company(){ NIP = _correctNIP8, ID = _correctID8, BankAccountNumber = "67160011270003012765973021" },
                new Company(){ NIP = _correctNIP9, ID = _correctID9, BankAccountNumber = "51124013301111001028688812" },
                new Company(){ NIP = _correctNIP10, ID = _correctID10, BankAccountNumber = "31103015080000000500217065" },
                new Company(){ NIP = _correctNIP11, ID = _correctID11, BankAccountNumber = "77160010130002001222057198" }
            };

            _incorrectCompanies = new List<Company>()
            { 
                new Company(){ NIP = _incorrectNIP1, ID = _incorrectID1},
                new Company(){ NIP = _incorrectNIP2, ID = _incorrectID2},
                new Company(){ NIP = _incorrectNIP3, ID = _incorrectID3},
                new Company(){ NIP = _incorrectNIP4, ID = _incorrectID4},
                new Company(){ NIP = _incorrectNIP5, ID = _incorrectID5}
            };

            CompaniesNIPIDDic = new Dictionary<string, string>
            {
                { _correctNIP1, _correctID1 },
                { _correctNIP2, _correctID2 },
                { _correctNIP3, _correctID3 },
                { _correctNIP4, _correctID4 },
                { _correctNIP5, _correctID5 },
                { _correctNIP6, _correctID6 },
                { _correctNIP7, _correctID7 },
                { _correctNIP8, _correctID8 },
                { _correctNIP9, _correctID9 },
                { _correctNIP10, _correctID10 },
                { _correctNIP11, _correctID11 },

                { _incorrectNIP1, _incorrectID1 },
                { _incorrectNIP2, _incorrectID2 },
                { _incorrectNIP3, _incorrectID3 },
                { _incorrectNIP4, _incorrectID4 },
                { _incorrectNIP5, _incorrectID5 }
            };

            _correctCompaniesDic = new Dictionary<string, Company>()
            {
                { _correctCompanies[0].ID, _correctCompanies[0] },
                { _correctCompanies[1].ID, _correctCompanies[1] },
                { _correctCompanies[2].ID, _correctCompanies[2] },
                { _correctCompanies[3].ID, _correctCompanies[3] },
                { _correctCompanies[4].ID, _correctCompanies[4] },
                { _correctCompanies[5].ID, _correctCompanies[5] },
                { _correctCompanies[6].ID, _correctCompanies[6] },
                { _correctCompanies[7].ID, _correctCompanies[7] },
                { _correctCompanies[8].ID, _correctCompanies[8] },
                { _correctCompanies[9].ID, _correctCompanies[9] },
                { _correctCompanies[10].ID, _correctCompanies[10] },


            };

            _incorrectNipCompaniesDic = new Dictionary<string, Company>()
            {
                { _incorrectCompanies[0].ID, _incorrectCompanies[0] },
                { _incorrectCompanies[1].ID, _incorrectCompanies[1] },
                { _incorrectCompanies[2].ID, _incorrectCompanies[2] },
                { _incorrectCompanies[3].ID, _incorrectCompanies[3] },
                { _incorrectCompanies[4].ID, _incorrectCompanies[4] },


            };

            _correctPhisicalCompanyDic = new Dictionary<string, Company>()
            {
                {_correctPhisicalCompanyID1, new Company(){NIP = _correctPhisicalCompanyNIP1} }
            
            };

            _isSetUp = true;
        }
    }
}
