using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DocumentGenerator.Lib.Helpers
{
    /// <summary>
    /// The source code of NumberToWordsConverter is insipred greatly by the algorithm at http://algorytm.org
    /// </summary>
    public class NumberToWordsConverter
    {
        private static readonly string[] _units =
             {
            "zero", "jeden", "dwa", "trzy", "cztery", "pięć",
            "sześć", "siedem", "osiem", "dziewięć", "dziesięć",
            "jedenaście", "dwanaście", "trzynaście", "czternaście", "piętnaście",
            "szesnaście", "siedemnaście", "osiemnaście", "dziewiętnaście"
        };

        private static readonly string[] _tens =
          {
            "dwadzieścia", "trzydzieści", "czterdzieści", "pięćdziesiąt",
            "sześćdziesiąt", "siedemdziesiąt", "osiemdziesiąt", "dziewięćdziesiąt"
        };

        static readonly string[] _hundreds =
        {
            "", "sto", "dwieście", "trzysta", "czterysta", "pięćset",
            "sześćset", "siedemset", "osiemset", "dziewięćset"
        };

        static readonly string[,] _orderUnits =
        {
            { "tysiąc", "tysiące", "tysięcy"     },
            { "milion", "miliony", "milionów"    }
        };

        static string HundredsPartToWords(int n)
        {
            if (n == 0)
            {
                return String.Empty;
            }

            StringBuilder valueInWords = new StringBuilder();

            int temp = n / 100;
            if (temp > 0)
            {
                valueInWords.Append(_hundreds[temp]);
                n -= temp * 100;
            }

            if (n > 0)
            {
                if (valueInWords.Length > 0)
                {
                    valueInWords.Append(" ");
                }

                if (n < 20)
                {
                    valueInWords.Append(_units[n]);
                }
                else
                {
                    valueInWords.Append(_tens[(n / 10) - 2]); //minus 2 bo tablica od 0 = dwadziescia
                    int lastDigit = n % 10;

                    if (lastDigit > 0)
                    {
                        valueInWords.Append(" ");
                        valueInWords.Append(_units[lastDigit]);
                    }
                }
            }

            return valueInWords.ToString();
        }

        static int GetOrderUnitEndingIndex(long n)
        {
            int lastDigit = (int)n % 10;

            if ((n >= 10 && (n <= 20 || lastDigit == 0)) ||
                (lastDigit > 4) || (n > 1 && lastDigit == 1))
            {
                return 2;
            }

            return (lastDigit == 1) ? 0 : 1;
        }

        static int ToWords(ref StringBuilder valueInWords, int number,
    int level)
        {
            int smallValue = 0;
            int divisor = (int)Math.Pow(1000, level + 1);

            if (divisor <= number)
            {
                number = ToWords(ref valueInWords, number, level + 1);
                smallValue = (int)(number / divisor);

                if (valueInWords.Length > 0)
                {
                    valueInWords.Append(" ");
                }

                if (smallValue > 0)
                {
                    valueInWords.Append(HundredsPartToWords(smallValue));
                    valueInWords.Append(" ");
                }

                valueInWords.Append(
                    _orderUnits[level, GetOrderUnitEndingIndex(smallValue)]);
            }

            return number - smallValue * divisor;
        }

        public static string ConvertToWords(int number)
        {

            //int decimalIndex = number.Contains(".") ? number.IndexOf(".") : number.IndexOf(",");
            //int beforeDecimal = int.Parse(number.Substring(0, decimalIndex));

            if (number < 20)
            {
                return _units[number];
            }

            StringBuilder numberInWords = new StringBuilder();

            int hundredsPart = (int)ToWords(ref numberInWords, number, 0);

            if (hundredsPart > 0)
            {
                if (numberInWords.Length > 0)
                {
                    numberInWords.Append(" ");
                }

                numberInWords.Append(HundredsPartToWords(hundredsPart));
            }

            return numberInWords.ToString();
        }

        private const string _dot = ".";
        private const string _colon = ",";
        private const string _grEnding = "/100";
        private const string _pln = " zł ";
        private const string _oneAtTheBegining = "jeden ";
        private const string _zerosGr = "00";
        public static string ConvertNumberToAmountPln(string numInString)
        {
            int decimalIndex = numInString.Contains(_dot) ? numInString.IndexOf(_dot) : numInString.Length;
            decimalIndex = numInString.Contains(_colon) ? numInString.IndexOf(_colon) : decimalIndex;

            string beforDecimalStr = numInString.Substring(0, decimalIndex);
            int beforeDecimal = int.Parse(beforDecimalStr);

            StringBuilder numberInWords = new StringBuilder();
            if (beforeDecimal / 1000000 == 1)
            {
                numberInWords.Append(_oneAtTheBegining);
            }
            
            numberInWords.Append(NumberToWordsConverter.ConvertToWords(beforeDecimal));
            numberInWords.Append(FormatHelper.SPACE);
            numberInWords.Append(GetZlotyEnding(beforDecimalStr));
            numberInWords.Append(FormatHelper.SPACE);
            string grPart = numInString.Length == decimalIndex ? _zerosGr : numInString.Substring(decimalIndex + 1);
            if (grPart.Length == 1)
                grPart = string.Concat(grPart, "0");
            numberInWords.Append(grPart);
            numberInWords.Append(_grEnding);

            return numberInWords.ToString();
        }

        private const string _zloty = "złoty";
        private static string[] _zloteDigit = new string[] { "2", "3", "4"};
        private const string _zlote = "złote";
        private const string _zlotych = "złotych";
        private const string _jeden = "1";
        public static string GetZlotyEnding(string numberInStr)
        {
            if (string.IsNullOrEmpty(numberInStr))
                throw new ArgumentOutOfRangeException("Argument nie może być pusty.");

            if (numberInStr == _jeden)
                return _zloty;
            string lastDigit = numberInStr.Substring(numberInStr.Length - 1);
            string lastTwoDigit = numberInStr.Length > 1 ? numberInStr.Substring(numberInStr.Length - 2, 1) : string.Empty;
            int number = int.Parse(numberInStr);
            if (_zloteDigit.Contains(lastDigit) && (lastTwoDigit == string.Empty || lastTwoDigit !=_jeden))
                return _zlote;
            return _zlotych;

        }
    }
}




