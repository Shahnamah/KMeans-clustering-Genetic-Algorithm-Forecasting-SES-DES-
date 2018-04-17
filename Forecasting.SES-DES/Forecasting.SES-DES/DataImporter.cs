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
    }
}
