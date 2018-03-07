using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forecasting.SES_DES
{
    public static class DataImporter
    {
        private const char csvSeparator = ',';

        public static IEnumerable<double> DemandList
        {
            get
            {
                using (StreamReader reader = new StreamReader("SwordForecasting.csv"))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var demand = line.Split(csvSeparator)[1];
                        yield return Convert.ToDouble(demand);
                    }
                }
            }
        }

        public static IEnumerable<double> WalmartData
        {
            get
            {
                using (StreamReader reader = new StreamReader("forecastingWalmart.csv"))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string storeNumber = line.Split(';')[0];
                        string deptNumber = line.Split(';')[1];
                        string demand = line.Split(';')[3];
                        if (storeNumber.Equals("1"))
                            if (deptNumber.Equals("2"))
                                yield return Convert.ToDouble(demand);
                    }
                }
            }
        }
    }
}
