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
                var watch = Stopwatch.StartNew();

                try
                {
                    using (var reader = new StreamReader("Data/WineData.csv"))
                    {
                        var data = new List<List<double>>();
                        var clients = new List<List<double>>();

                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            var properties = reader.ReadLine().Split(',');
                            var templist = new List<double>();
                            foreach (var prop in properties)
                            {
                                templist.Add(Convert.ToDouble(prop));
                            }
                            data.Add(templist);
                        }

                        int properties1 = data[0].Count;
                        for (int i = 0; i < properties1; i++)
                        {
                            var templist = new List<double>();
                            foreach (var list in data)
                            {
                                templist.Add(list[i]);
                            }
                            clients.Add(templist);
                        }

                        foreach (var client in clients)
                        {
                            int id = 1;
                            var values = client;
                            var tempValues = new Dictionary<int, double>();
                            for (int i = 0; i < values.Count; i++)
                            {
                                tempValues.Add(i, values[i]);
                            }

                            var observation = new Observation { Id = id, Items = tempValues };
                            Observations.Add(observation);
                        }
                    }
                }
                catch (FileNotFoundException exception)
                {
                    Console.WriteLine(exception.Message);
                }

                watch.Stop();
                Console.WriteLine($"Getting the observations took {watch.Elapsed.Milliseconds} ms to finish.");
                return Observations;
            }
        }
    }
}
