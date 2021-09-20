using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using CITechnicalTest.Models;


namespace CITechnicalTest.Helpers
{
    public class ReadCSVFile
    {
        public static List<CSVData> getCSVData()
        {
            List<CSVData> csvData = new List<CSVData>();

            for (int i = 1; i <= 3; i++)
            {
                //read csv file through stream reader
                var filePath = string.Format(@"D:\Capital Index\data\data{0}.csv", i);
                if (File.Exists(filePath))
                {
                    using (var reader = new StreamReader(filePath))
                    {
                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();
                            if (!line.Contains("date")) { //do not include column header
                                var values = line.Split(',');
                                if (values.Any())
                                {
                                    //check if record already exist
                                    var exist = csvData.Where(x => x.Date == Convert.ToDateTime(values[0]) && x.country == values[1] && x.currency == values[2] && x.amount == Convert.ToDecimal(values[3])).FirstOrDefault();
                                    if (exist == null)
                                    {
                                        CSVData data = new CSVData()
                                        {
                                            Date = Convert.ToDateTime(values[0]),
                                            country = values[1],
                                            currency = values[2],
                                            amount = Convert.ToDecimal(values[3])
                                        };

                                        csvData.Add(data);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return csvData;
        }
    }
}
