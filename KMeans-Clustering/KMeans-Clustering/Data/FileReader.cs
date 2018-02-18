using KMeansClustering.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace KMeansClustering.Data
{
    public class FileReader
    {
        private List<Observation> Observations;

        public FileReader()
        {
            Observations = new List<Observation>();
        }

        public List<Observation> GetObservations
        {
            get
            {
                try
                {
                    using (var reader = new StreamReader("Data/WineData.csv"))
                    {
                        var data = new List<List<double>>();
                        var clients = new List<List<double>>();

                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            var properties = line.Split(',');
                            var templist = new List<double>();
                            foreach (var prop in properties)
                            {
                                templist.Add(Convert.ToDouble(prop));
                            }
                            data.Add(templist);
                        }
                        
                        for (int i = 0; i < data[0].Count; i++)
                        {
                            var templist = new List<double>();

                            data.ForEach(list => templist.Add(list[i]));
                            clients.Add(templist);
                        }

                        int id = 1;
                        foreach (var client in clients)
                        {
                            var values = client;
                            var tempValues = new Dictionary<int, double>();
                            for (int i = 0; i < values.Count; i++)
                            {
                                tempValues.Add(i, values[i]);
                            }

                            var observation = new Observation { Id = id, Items = tempValues };
                            Observations.Add(observation);
                            id++;
                        }
                    }
                }
                catch (FileNotFoundException exception)
                {
                    Console.WriteLine(exception.Message);
                }
                return Observations;
            }
        }
    }
}
