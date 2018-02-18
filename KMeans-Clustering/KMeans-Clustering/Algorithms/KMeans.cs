using KMeansClustering.Data;
using KMeansClustering.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMeansClustering.Algorithms
{
    public class KMeans
    {
        private readonly int iterations;
        private readonly int totalClusters;
        private Result result;

        private List<Observation> Centroids{ get; set; }
        private List<Cluster> Clusters{ get; set; }

        private Observation[] Observations { get; }

        public KMeans(int iterations, int totalClusters)
        {
            this.iterations = iterations;
            this.totalClusters = totalClusters;

            Centroids = new List<Observation>();
            Clusters = new List<Cluster>();

            Observations = new FileReader().GetObservations.ToArray();
        }

        public void CreateClusters()
        {
            for (int iteration = 0; iteration <= this.iterations; iteration++)
            {
                Clusters = new List<Cluster>();
                Centroids = new List<Observation>();

                for (int i = 0; i < totalClusters; i++)
                {
                    int clusterNumber = new Random().Next(Observations.Length);
                    var observation = new Observation() { Id = clusterNumber + 50, Items = Observations[clusterNumber].Items };
                    Centroids.Add(observation);
                    Clusters.Add(new Cluster(i, observation));
                }

                //Parallel.ForEach(Observations, (observation) =>
                //{
                //    lock (observation)
                //    {
                //        CalculateDistance(observation);
                //    }
                //});

                foreach (var observation in Observations)
                {

                    CalculateDistance(observation);
                }

                for (int i = 0; i < 10; i++)
                {
                    int counter = 0;
                    for (int j = 0; j < Clusters.Count; j++)
                    {
                        Clusters[j].RecalculateCenter();
                        Clusters[j].Observations.Clear();
                        Clusters[counter] = Clusters[j];
                        counter++;
                    }

                    for (int x = 0; x < Clusters.Count; x++)
                    {
                        for (int y = 0; y < Observations.Length; y++)
                        {
                            CalculateDistance(Observations[y]);
                        }

                        //Parallel.ForEach(Observations, (observation) =>
                        //{
                        //    lock (observation)
                        //    {
                        //        CalculateDistance(observation);
                        //    }
                        //});
                    }
                }

                double error = 0;
                for (int i = 0; i < Clusters.Count; i++)
                {
                    error += Clusters[i].SumOfError();
                }

                if (result != null)
                {
                    if (error < result.Error)
                        result = new Result(error, Clusters);
                }
                else
                {
                    result = new Result(error, Clusters);
                }
            }
            PrintResults(result);
        }

        private void CalculateDistance(Observation observation)
        {
            double distance = 0;
            var centroidDem = new Dictionary<int, double>();
            for (int i = 0; i < Centroids.Count; i++)
            {
                distance = CalculateEuclideanDistance(observation, Centroids[i]);
                centroidDem.Add(i, distance);
            }

            int minimumDistanceKey = centroidDem.OrderBy(c => c.Value).First().Key;
            if (!Clusters[minimumDistanceKey].Observations.Contains(observation))
            {
                Clusters[minimumDistanceKey].Observations.Add(observation);
            }
        }

        private double CalculateEuclideanDistance(Observation observation, Observation centroid)
        {
            double distance = 0;
            for(int i = 0; i < observation.Items.Count; i++)
            {
                distance += Math.Pow(observation.Items[i] - centroid.Items[i], 2.00);
            }
            distance = Math.Sqrt(distance);
            return distance;
        }

        private void PrintResults(Result result)
        {
            for (int i = 0; i < result.Clusters.Count; i++)
            {
                Dictionary<int, int> map = result.Clusters[i].MostSoldItems();
                Console.WriteLine("ClusterID {0}", result.Clusters[i].ClusterNumber);
                foreach (var item in map)
                {
                    Console.WriteLine("Item number {0} sold {1} times", item.Key, item.Value);
                }
                Console.WriteLine("");
            }
            Console.WriteLine("Error is: {0}", result.Error);
        }

        public void PrintClusters()
        {
            for (int i = 0; i < Clusters.Count; i++)
            {
                Console.WriteLine("ClusterNumber: {0} has {1} observationpoints", Clusters[i].ClusterNumber, Clusters[i].Observations.Count);
            }
        }
    }
}
