using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace InventarServer
{
    class ExcelHelper
    {
        public static void Setup()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            if (!Directory.Exists("excel"))
                Directory.CreateDirectory("excel");
        }

        public static List<Item> LoadExcel(string _name, string _filename)
        {
            List<Item> items = new List<Item>();
            using (var stream = File.Open(_filename, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet();
                    var table = result.Tables[0];

                    int rows = 0;
                    for(int y = 0; y < table.Rows.Count; y++)
                    {
                        var row = table.Rows[y].ItemArray;
                        string firstCol = row[0].ToString();

                        if (string.IsNullOrWhiteSpace(firstCol))
                            continue;

                        if (!char.IsDigit(firstCol[0]))
                            continue;

                        rows++;

                        Item i = new Item(row);
                        i.ItemCollectionName = _name;
                        items.Add(i);
                    }
                }
            }
            return items;
        }

    }
}
