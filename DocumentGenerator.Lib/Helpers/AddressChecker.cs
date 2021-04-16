using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Authentication.ExtendedProtection;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text;
using System.Text.RegularExpressions;

namespace DocumentGenerator.Lib.Helpers
{
    public class AddressChecker
    {
        private const char _colon = ';';
        private static Dictionary<string, List<PostAddress>> _addresses;
        private static bool _isInited = false;
        public bool Init()
        {
            try
            {
                var reader = new StreamReader(File.OpenRead("input\\spispna.csv"));
                _addresses = new Dictionary<string, List<PostAddress>>();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var address = GetAddress(line);
                    if (!_addresses.ContainsKey(address.PostalCode))
                    {
                        _addresses.Add(address.PostalCode, new List<PostAddress>());
                    }
                       _addresses[address.PostalCode].Add(address);
                    
                }
                return true;

            } catch (Exception e)
            {
                return false;
            }
           
        }

        private PostAddress GetAddress(string line)
        {
            var values = line.Split(_colon);

            var newAddress = new PostAddress();
            newAddress.PostalCode = values[0];
            newAddress.Town = values[1];
            newAddress.Street = values[2];
            newAddress.Numbers = values[3];
            newAddress.Gmina = values[4];
            newAddress.Powiat = values[5];
            newAddress.Wojewodztwo = values[6];
            return newAddress;
        }

        private const string _alTypeOfStreet = "al.";
        private const string _alTypeOfStreetFull = "alej";
        private const string _osTypeOfStreet = "os.";
        private const string _osTypeOfStreetFull = "osiedle";
        private Regex regEx = new Regex("[0-9]{2}-[0-9]{3}", RegexOptions.Compiled);
        private Regex regExNum = new Regex("[0-9]", RegexOptions.Compiled);
        public string GetStart(string addressline)
        {
            
            string startOfAddress = addressline.Substring(0, Math.Min(7, addressline.Length)).ToLower();
            if ((startOfAddress.Contains(_alTypeOfStreet) || startOfAddress.Contains(_osTypeOfStreet) || startOfAddress.Contains(_alTypeOfStreetFull) || startOfAddress.Contains(_osTypeOfStreetFull)))
            {
                //Condition for the street of name "osiedele"
                if (startOfAddress.Contains(_osTypeOfStreetFull))
                {
                    if (addressline.Substring(0, Math.Min(addressline.IndexOf(","), regExNum.Match(addressline).Index)).Trim().ToLower() == _osTypeOfStreetFull)
                    { return "ul."; }
                }
                return string.Empty;
            }

            if (!regEx.IsMatch(addressline))
            {
                return string.Empty;
            }

            string postalCode = regEx.Match(addressline).Value;
            List<PostAddress> codeAddresses = _addresses[postalCode];
            string prefix = string.Empty;
            foreach (var codeAddress in codeAddresses)
            {
                if (codeAddress.Street != string.Empty)
                {
                    prefix = "ul.";
                } else if (!string.IsNullOrEmpty(prefix))
                {
                    prefix = string.Empty;
                }
            }

            return prefix;
        }
    }
}
