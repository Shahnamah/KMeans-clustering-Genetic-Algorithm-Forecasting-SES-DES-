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
        private static char csvSeparator = ';';

        public static List<double> DemandList
        {
            get
            {
                List<double> demandList = new List<double>();
                try
                {
                    using (StreamReader reader = new StreamReader("SwordForecasting.csv"))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            var demand = line.Split(csvSeparator)[1];
                            demandList.Add(Convert.ToDouble(demand));
                        }
                    }
                }
                catch (FileNotFoundException fileNotFoundException)
                {
                    throw fileNotFoundException;
                }
                catch (Exception exception)
                {
                    throw exception;
                }
                return demandList;
            }
        }
    }
}
