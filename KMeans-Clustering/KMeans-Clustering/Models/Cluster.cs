using System;
using System.Collections.Generic;
using System.Text;

namespace KMeansClustering.Models
{
    public class Cluster
    {
        public int ClusterNumber { get; set; }
        public List<Observation> Observations = new List<Observation>();
        public Observation Centroid { get; set; }

        public Cluster()
        {
        }

        public Cluster(int clusterNumber, Observation observation)
        {
            this.ClusterNumber = clusterNumber;
            Centroid = new Observation { Id = observation.Id, Items = observation.Items };
        }

        public Cluster(List<Observation> observations)
        {
            Observations = observations;
        }

        public Cluster(Observation centroid, List<Observation> observations)
        {
            Observations = observations;
            Centroid = centroid;
        }

        public void RecalculateCenter()
        {
            for (int i = 0; i < Centroid.Items.Count; i++)
            {
                double sum = 0.0;

                foreach (var obs in Observations)
                {
                    sum += obs.Items[i];
                }
                if (sum > 0)
                    Centroid.Items[i] = (sum / Observations.Count);
                else
                    Centroid.Items[i] = 0.0;
            }
        }

        public double SumOfError()
        {
            double sum = 0;
            for (int item = 0; item < Centroid.Items.Keys.Count; item++)
            {
                for (int obs = 0; obs < Observations.Count; obs++)
                {
                    double valueObservation = Observations[obs].Items[item];
                    sum += Math.Pow((Centroid.Items[item] - valueObservation), 2.00);
                }
            }
            return sum;
        }

        public Dictionary<int, int> MostSoldItems()
        {
            Dictionary<int, int> items = new Dictionary<int, int>();
            int total = 0;

            for (int item = 0; item < Centroid.Items.Count; item++)
            {
                int totalSold = 0;
                for (int obs = 0; obs < Observations.Count; obs++)
                {
                    totalSold += (int)Observations[obs].Items[item];
                }
                total += totalSold;
                items.Add(item, totalSold);
            }
            Console.WriteLine(total);
            return items;
        }
    }
}
