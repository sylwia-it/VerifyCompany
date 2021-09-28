using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExcelDataManager.Lib.Import;
using Microsoft.Office.Interop.Excel;

namespace ExcelDataManager.Lib
{
    public static class SpreadSheetHelper
    {
        public  static  bool SheetNameContainsNamesToExclude(string sheetName, List<string> _sheetNamesToExclude)
        {
            foreach (var nameToExclude in _sheetNamesToExclude)
            {
                if (sheetName.Contains(nameToExclude))
                    return true;
            }

            return false;
        }

        public static int FindHeaderRow(Worksheet worksheet, List<ColumnConfig> importColumnsConfig)
        {
            string nipHeaderCaption = importColumnsConfig.FirstOrDefault(c => c.ID.ToLower().Contains(ImportColumnName.NIP.ToString().ToLower())).HeaderText;

            //Recognize the header row by looking for them in the first 75 columns and first 10 rows
            for (int row = 1; row <= 10; row++)
            {
                for (int column = 1; column <= 75; column++)
                {
                    var tempCellContent = ((string)((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[row, column]).Formula).ToLower().Trim();

                    if (tempCellContent.StartsWith(nipHeaderCaption, StringComparison.OrdinalIgnoreCase))
                    {
                        return row;
                    }
                }
            }
            return -1;
        }

        public static string ConvertCellAddresFromNumsToLetterNum(int rowNum, int columnNum)
        {
            int div = columnNum;
            string colLetter = string.Empty;
            int mod = 0;

            while (div > 0)
            {
                mod = (div - 1) % 26; //num of alphabet letters
                colLetter = string.Concat((char)(65 + mod), colLetter); //num of index in UTF
                div = (int)((div - mod) / 26);
            }
            return string.Concat(colLetter, rowNum);
        }
    }
}

