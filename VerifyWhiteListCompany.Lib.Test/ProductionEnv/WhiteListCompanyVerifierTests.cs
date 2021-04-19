using NUnit.Framework;
using VerifyWhiteListCompany.Lib.WebServiceModel;
using System.Linq;
using System.Collections.Generic;
using VerifyCompany.Common.Lib;
using System;
using NLog;
using VerifyCompany.Common.Test.Lib;
using VerifyWhiteListCompany.Lib.Test.Helpers;

namespace VerifyWhiteListCompany.Lib.Test.ProductionEnv
{
    public class WhiteClientCompanyVerifierTests
    {

        private static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        WhiteListCompanyVerifier _verifier;
        
        [SetUp]
        public void Setup()
        {
            WhiteListClient client = new WhiteListClient();
            string prodEnvUrl = UrlProvider.ProductionUrl;
            client.Init(prodEnvUrl);
            WhiteListClientFactory.SetClient(client);
            _verifier = new WhiteListCompanyVerifier();
            var config = new NLog.Config.LoggingConfiguration();
            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = "file.txt" };
            config.AddRule(LogLevel.Info, LogLevel.Fatal, logfile);
            NLog.LogManager.Configuration = config;
            Logger = NLog.LogManager.GetCurrentClassLogger();

        }

        [Test]
        public void TestCorrectCompaniesNoBankAccountTest()
        {
            var companiesToCheck = VerifyCompany.Common.Test.Lib.CompanyGenerator.GetCorrectCompanies();

            Dictionary<string, WhiteListVerResult> verResults = _verifier.VerifyCompanies(companiesToCheck.Values.ToList<Company>(), false);
            Logger.Info("Starts");

            foreach (var companyToCheck in companiesToCheck)
            {
                var companyChecked = verResults.FirstOrDefault(vr => vr.Key == companyToCheck.Key);
                Assert.IsNotNull(companyChecked);
                Assert.AreEqual(WhiteListVerResultStatus.ActiveVATPayerVerSuccessfull, companyChecked.Value.VerificationStatus);
                Assert.AreEqual(true, companyChecked.Value.IsActiveVATPayer);
                Assert.AreEqual(companyToCheck.Value.NIP, companyChecked.Value.Nip);
                Assert.IsNotNull(companyChecked.Value.AccountNumbers);
                Assert.IsTrue(DTHelper.IsItToday(companyChecked.Value.VerificationDate));
                Logger.Info(companyChecked.Key);
            }
        }

        [Test]
        public void TestSingleCorrectCompanyNoBankAccountTest()
        {
            var companiesToCheck = VerifyCompany.Common.Test.Lib.CompanyGenerator.GetCorrectCompanies();

            Dictionary<string, WhiteListVerResult> verResults = _verifier.VerifyCompanies(companiesToCheck.Values.Take(1).ToList<Company>(), false);
            

            foreach (var companyToCheck in companiesToCheck.Take(1))
            {
                var companyChecked = verResults.FirstOrDefault(vr => vr.Key == companyToCheck.Key);
                Assert.IsNotNull(companyChecked);
                Assert.AreEqual(WhiteListVerResultStatus.ActiveVATPayerVerSuccessfull, companyChecked.Value.VerificationStatus);
                Assert.AreEqual(true, companyChecked.Value.IsActiveVATPayer);
                Assert.AreEqual(companyToCheck.Value.NIP, companyChecked.Value.Nip);
                Assert.IsNotNull(companyChecked.Value.AccountNumbers);
                Assert.IsTrue(DTHelper.IsItToday(companyChecked.Value.VerificationDate));
                Logger.Info(companyChecked.Key);
            }
        }

        [Test]
        public void TestNullInputCompaniesTest()
        {
            Assert.Throws<ArgumentNullException>(() => _verifier.VerifyCompanies(null, false));
            Assert.Throws<ArgumentNullException>(() => _verifier.VerifyCompanies(null, true));
        }

        [Test]
        public void TestEmptyInputCompaies()
        {
            Dictionary<string, WhiteListVerResult> verResults = _verifier.VerifyCompanies(new List<Company>(), false);
            Assert.IsNotNull(verResults);
            Assert.AreEqual(0, verResults.Count);

            verResults = _verifier.VerifyCompanies(new List<Company>(), true);
            Assert.IsNotNull(verResults);
            Assert.AreEqual(0, verResults.Count);
        }
        
        [Test]
        public void TestCorrectCompaniesWithCorrectBankAccounts()
        {
            var companiesToCheck = VerifyCompany.Common.Test.Lib.CompanyGenerator.GetCorrectCompanies();

            Dictionary<string, WhiteListVerResult> verResults = _verifier.VerifyCompanies(companiesToCheck.Values.ToList<Company>(), true);


            foreach (var companyToCheck in companiesToCheck)
            {
                var companyChecked = verResults.FirstOrDefault(vr => vr.Key == companyToCheck.Key);
                Assert.IsNotNull(companyChecked);
                Assert.AreEqual(WhiteListVerResultStatus.ActiveVATPayerAccountOKVerSuccessfull, companyChecked.Value.VerificationStatus);
                Assert.IsTrue(companyChecked.Value.IsActiveVATPayer);
                Assert.AreEqual(companyToCheck.Value.NIP, companyChecked.Value.Nip);
                Assert.IsNotNull(companyChecked.Value.AccountNumbers);
                Assert.IsTrue(DTHelper.IsItToday(companyChecked.Value.VerificationDate));
                Assert.IsTrue(companyChecked.Value.IsGivenAccountNumOnWhiteList);
            }
       }

        [Test]
        public void TestCorrectCompaniesWithInCorrectBankAccounts()
        {
            var companiesToCheck = new Dictionary<string, Company>(CompanyGenerator.GetCorrectCompanies());
            //0 - z³y account number, nie na liœcie
            Company tempComp = companiesToCheck.ElementAt(0).Value;
            companiesToCheck.Remove(companiesToCheck.ElementAt(0).Key);
            companiesToCheck.Add(tempComp.ID, new Company() { ID = new string(tempComp.ID), BankAccountNumber = new string(tempComp.BankAccountNumber), NIP = new string(tempComp.NIP)});
            string notOnWhiteListAccountNumberID = tempComp.ID;

            //1- z literk¹ account number
            tempComp = companiesToCheck.ElementAt(1).Value;
            companiesToCheck.Remove(companiesToCheck.ElementAt(1).Key);
            companiesToCheck.Add(tempComp.ID, new Company() { ID = new string(tempComp.ID), BankAccountNumber = new string(tempComp.BankAccountNumber).Remove(5, 2).Insert(4, "ab"), NIP = new string(tempComp.NIP) });
            companiesToCheck[notOnWhiteListAccountNumberID].BankAccountNumber = tempComp.BankAccountNumber;

            //2 - pusty account number
            tempComp = companiesToCheck.ElementAt(2).Value;
            companiesToCheck.Remove(companiesToCheck.ElementAt(2).Key);
            companiesToCheck.Add(tempComp.ID, new Company() { ID = new string(tempComp.ID), BankAccountNumber = string.Empty, NIP = new string(tempComp.NIP) });

            //3 - null account number
            tempComp = companiesToCheck.ElementAt(3).Value;
            companiesToCheck.Remove(companiesToCheck.ElementAt(3).Key);
            companiesToCheck.Add(tempComp.ID, new Company() { ID = new string(tempComp.ID), BankAccountNumber = null, NIP = new string(tempComp.NIP) });

            Dictionary<string, WhiteListVerResult> verResults = _verifier.VerifyCompanies(companiesToCheck.Values.ToList<Company>(), true);

            KeyValuePair<string, WhiteListVerResult> companyChecked = verResults.FirstOrDefault(vr => vr.Key == companiesToCheck.ElementAt(0).Key);
            Assert.IsNotNull(companyChecked);
            Assert.AreEqual(WhiteListVerResultStatus.ActiveVATPayerButGivenAccountNotOnWhiteList, companyChecked.Value.VerificationStatus);
            Assert.IsTrue(companyChecked.Value.IsActiveVATPayer);
            Assert.AreEqual(companiesToCheck.ElementAt(0).Value.NIP, companyChecked.Value.Nip);
            Assert.IsNotNull(companyChecked.Value.AccountNumbers);
            Assert.IsTrue(DTHelper.IsItToday(companyChecked.Value.VerificationDate));
            Assert.IsFalse(companyChecked.Value.IsGivenAccountNumOnWhiteList);

            companyChecked = verResults.FirstOrDefault(vr => vr.Key == companiesToCheck.ElementAt(1).Key);
            Assert.IsNotNull(companyChecked);
            Assert.AreEqual(WhiteListVerResultStatus.ActiveVATPayerButGivenAccountWrong, companyChecked.Value.VerificationStatus);
            Assert.IsTrue(companyChecked.Value.IsActiveVATPayer);
            Assert.AreEqual(companiesToCheck.ElementAt(1).Value.NIP, companyChecked.Value.Nip);
            Assert.IsNotNull(companyChecked.Value.AccountNumbers);
            Assert.IsTrue(DTHelper.IsItToday(companyChecked.Value.VerificationDate));
            Assert.IsFalse(companyChecked.Value.IsGivenAccountNumOnWhiteList);

            
            for (int i = 2; i < 4; i++)
            {
                companyChecked = verResults.FirstOrDefault(vr => vr.Key == companiesToCheck.ElementAt(i).Key);
                Assert.IsNotNull(companyChecked);
                Assert.AreEqual(WhiteListVerResultStatus.ActiveVATPayerVerScuccessButGivenAccountNotVerified, companyChecked.Value.VerificationStatus);
                Assert.IsTrue(companyChecked.Value.IsActiveVATPayer);
                Assert.AreEqual(companiesToCheck.ElementAt(i).Value.NIP, companyChecked.Value.Nip);
                Assert.IsNotNull(companyChecked.Value.AccountNumbers);
                Assert.IsTrue(DTHelper.IsItToday(companyChecked.Value.VerificationDate));
                Assert.IsFalse(companyChecked.Value.IsGivenAccountNumOnWhiteList);
            }


        }

        [Test]
        public void TestOneInCorrectNIPCompany()
        {
            var companiesToCheck = new Dictionary<string, Company>(CompanyGenerator.GetCorrectCompanies());
            var incorrectCompany = VerifyCompany.Common.Test.Lib.CompanyGenerator.GetInCorrectNipCompanies();

            //add incorrect company
            companiesToCheck.Add(incorrectCompany.ElementAt(0).Key, incorrectCompany.ElementAt(0).Value);


            Dictionary<string, WhiteListVerResult> verResults = _verifier.VerifyCompanies(companiesToCheck.Values.ToList<Company>(), true);

            var companyIncorrectNipChecked = verResults.FirstOrDefault(vr => vr.Key == incorrectCompany.ElementAt(0).Key);
            Assert.IsNotNull(companyIncorrectNipChecked);
            Assert.AreEqual(WhiteListVerResultStatus.ErrorNIPError, companyIncorrectNipChecked.Value.VerificationStatus);
            Assert.IsTrue(DTHelper.IsItToday(companyIncorrectNipChecked.Value.VerificationDate));

            foreach (var companyToCheck in companiesToCheck.Where(c=>c.Key != incorrectCompany.ElementAt(0).Key))
            {
                var companyChecked = verResults.FirstOrDefault(vr => vr.Key == companyToCheck.Key);
                Assert.IsNotNull(companyChecked);
                Assert.AreEqual(WhiteListVerResultStatus.ActiveVATPayerAccountOKVerSuccessfull, companyChecked.Value.VerificationStatus);
                Assert.IsTrue(DTHelper.IsItToday(companyChecked.Value.VerificationDate));
            }
        }

        [Test]
        public void TestTwoInCorrectNIPCompany()
        {
            var companiesToCheck = new Dictionary<string, Company>(CompanyGenerator.GetCorrectCompanies());
            var incorrectCompany = VerifyCompany.Common.Test.Lib.CompanyGenerator.GetInCorrectNipCompanies();

            //add incorrect company
            companiesToCheck.Add(incorrectCompany.ElementAt(0).Key, incorrectCompany.ElementAt(0).Value);
            companiesToCheck.Add(incorrectCompany.ElementAt(2).Key, incorrectCompany.ElementAt(2).Value);


            Dictionary<string, WhiteListVerResult> verResults = _verifier.VerifyCompanies(companiesToCheck.Values.ToList<Company>(),  true);

            var companyIncorrectNipChecked = verResults.FirstOrDefault(vr => vr.Key == incorrectCompany.ElementAt(0).Key);
            Assert.IsNotNull(companyIncorrectNipChecked);
            Assert.AreEqual(WhiteListVerResultStatus.ErrorNIPError, companyIncorrectNipChecked.Value.VerificationStatus);
            Assert.IsTrue(DTHelper.IsItToday(companyIncorrectNipChecked.Value.VerificationDate));

            foreach (var companyToCheck in companiesToCheck.Where(c => c.Key != incorrectCompany.ElementAt(0).Key && c.Key != incorrectCompany.ElementAt(2).Key))
            {
                var companyChecked = verResults.FirstOrDefault(vr => vr.Key == companyToCheck.Key);
                Assert.IsNotNull(companyChecked);
                Assert.AreEqual(WhiteListVerResultStatus.ActiveVATPayerAccountOKVerSuccessfull, companyChecked.Value.VerificationStatus);
                Assert.IsTrue(DTHelper.IsItToday(companyChecked.Value.VerificationDate));
            }


        }


        [Test]
        public void TestEmptyNIPCompanyWithCorrectCompanies()
        {
            var companiesToCheck = new Dictionary<string, Company>(VerifyCompany.Common.Test.Lib.CompanyGenerator.GetCorrectCompanies());

            //add incorrect company
            const string emptyNIPCompanyID = "emptyNip";
            companiesToCheck.Add(emptyNIPCompanyID, new Company() { ID = emptyNIPCompanyID, NIP=string.Empty});
            const string nullNipCompanyID = "nullNip";
            companiesToCheck.Add(nullNipCompanyID, new Company() { ID = nullNipCompanyID, NIP = null });

            Dictionary<string, WhiteListVerResult> verResults = _verifier.VerifyCompanies(companiesToCheck.Values.ToList<Company>(), true);

            KeyValuePair<string, WhiteListVerResult> companyIncorrectNipChecked = verResults.FirstOrDefault(vr => vr.Key == emptyNIPCompanyID);
            Assert.IsNotNull(companyIncorrectNipChecked);
            Assert.AreEqual(WhiteListVerResultStatus.ErrorNIPEmpty, companyIncorrectNipChecked.Value.VerificationStatus);
            Assert.IsTrue(DTHelper.IsItToday(companyIncorrectNipChecked.Value.VerificationDate));

            companyIncorrectNipChecked = verResults.FirstOrDefault(vr => vr.Key == nullNipCompanyID);
            Assert.IsNotNull(companyIncorrectNipChecked);
            Assert.AreEqual(WhiteListVerResultStatus.ErrorNIPEmpty, companyIncorrectNipChecked.Value.VerificationStatus);
            Assert.IsTrue(DTHelper.IsItToday(companyIncorrectNipChecked.Value.VerificationDate));

            foreach (var companyToCheck in companiesToCheck.Where(c => c.Key != nullNipCompanyID && c.Key != emptyNIPCompanyID))
            {
                var companyChecked = verResults.FirstOrDefault(vr => vr.Key == companyToCheck.Key);
                Assert.IsNotNull(companyChecked);
                Logger.Info("ID: {0}, Status: {1}, Nip: {2}, Account nr: {3}", companyChecked.Key, companyChecked.Value.VerificationStatus, companyChecked.Value.Nip, companyChecked.Value.GivenAccountNumber);
                Assert.AreEqual(WhiteListVerResultStatus.ActiveVATPayerAccountOKVerSuccessfull, companyChecked.Value.VerificationStatus);
    
                Assert.IsTrue(companyChecked.Value.IsActiveVATPayer);
                Assert.AreEqual(companyToCheck.Value.NIP, companyChecked.Value.Nip);
                Assert.IsNotNull(companyChecked.Value.AccountNumbers);
                Assert.IsTrue(DTHelper.IsItToday(companyChecked.Value.VerificationDate));
                Assert.IsTrue(companyChecked.Value.IsGivenAccountNumOnWhiteList);
            }


        }

        [Test]
        public void TestInActiveAndActiveCompaniesAndAccounts()
        {
            var companiesToCheck = new Dictionary<string, Company>(CompanyGenerator.GetCorrectCompanies());
            string inActiveCompanyNIP = CompanyGenerator.GetZakonczoneUpadaloscioweNIP();
            string inActiveCompanyID = "u1";
            //add inactive company
            companiesToCheck.Add(inActiveCompanyID, new Company() { NIP= inActiveCompanyNIP, ID= inActiveCompanyID });


            Dictionary<string, WhiteListVerResult> verResults = _verifier.VerifyCompanies(companiesToCheck.Values.ToList<Company>(),  true);

            var companyInacticeResultCheck = verResults.FirstOrDefault(vr => vr.Key == inActiveCompanyID);
            Assert.IsNotNull(companyInacticeResultCheck);
            Assert.AreEqual(WhiteListVerResultStatus.NotActiveVATPayer, companyInacticeResultCheck.Value.VerificationStatus);
            Assert.IsTrue(DTHelper.IsItToday(companyInacticeResultCheck.Value.VerificationDate));
            Assert.AreEqual(inActiveCompanyNIP, companyInacticeResultCheck.Value.Nip);

            foreach (var companyToCheck in companiesToCheck.Where(c => c.Key != inActiveCompanyID))
            {
                var companyChecked = verResults.FirstOrDefault(vr => vr.Key == companyToCheck.Key);
                Assert.IsNotNull(companyChecked);
                Assert.AreEqual(WhiteListVerResultStatus.ActiveVATPayerAccountOKVerSuccessfull, companyChecked.Value.VerificationStatus);
                Assert.IsTrue(companyChecked.Value.IsActiveVATPayer);
                Assert.AreEqual(companyToCheck.Value.NIP, companyChecked.Value.Nip);
                Assert.IsNotNull(companyChecked.Value.AccountNumbers);
                Assert.IsTrue(DTHelper.IsItToday(companyChecked.Value.VerificationDate));
                Assert.IsTrue(companyChecked.Value.IsGivenAccountNumOnWhiteList);
            }


        }



    }
}