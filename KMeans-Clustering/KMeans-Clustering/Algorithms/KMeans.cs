﻿using KMeansClustering.Data;
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

        private List<Observation> Centroids;
        private List<Cluster> Clusters;
        private Result result;
        private Random random = new Random();

        private List<Observation> Observations;

        public KMeans(int iterations, int totalClusters)
        {
            this.iterations = iterations;
            this.totalClusters = totalClusters;
            Observations = new FileReader().GetObservations;
        }

        /// <summary>
        /// Creates the clusters.
        /// </summary>
        /// <param name="observations"></param>
        public void CreateClusters()
        {
            for (int iteration = 0; iteration <= iterations; iteration++)
            {
                Clusters = new List<Cluster>();
                Centroids = new List<Observation>();
                for (int i = 0; i < totalClusters; i++)
                {
                    int clusterNumber = random.Next(Observations.Count);
                    Observation observation = new Observation { Id = clusterNumber + 50, Items = new Dictionary<int, double>(Observations[clusterNumber].Items) };
                    Centroids.Add(observation);
                    Clusters.Add(new Cluster(i, observation));
                }
                
                Parallel.ForEach(Observations, (observation) =>
                {
                    lock (observation)
                    {
                        CalculateDistance(observation);
                    }
                });

                for (int i = 0; i < 4; i++)
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
                        Parallel.ForEach(Observations, (observation) =>
                        {
                            lock (observation)
                            {
                                CalculateDistance(observation);
                            }
                        });
                    }
                }
                double error = 0;
                for (int i = 0; i < Clusters.Count; i++)
                {
                    error += Clusters[i].SumOfError();
                }

                if (Double.IsNaN(error))
                    continue;

                if (result != null)
                {
                    if (error < result.Error)
                        result = new Result(error, Clusters);
                }
                else
                    result = new Result(error, Clusters);
            }
            PrintResults(result);
        }

        /// <summary>
        /// Computes the distance between the observations.
        /// </summary>
        /// <param name="observation"></param>
        /// <param name="numberOfClusters"></param>
        private void CalculateDistance(Observation observation)
        {
            double distance = 0;
            Dictionary<int, double> centroidDem = new Dictionary<int, double>();
            for (int i = 0; i < Centroids.Count; i++)
            {
                distance = CalculateEuclideanDistance(observation, Centroids[i]);
                centroidDem.Add(i, distance);
            }

            int minimumDistanceKey = centroidDem.OrderBy(x => x.Value).First().Key;
            if (!Clusters[minimumDistanceKey].Observations.Contains(observation))
            {
                Clusters[minimumDistanceKey].Observations.Add(observation);
            }
        }

        /// <summary>
        /// Calculates the Euclidean distance between an observation and the centroid.
        /// </summary>
        /// <param name="observation"></param>
        /// <param name="centroid"></param>
        /// <returns></returns>
        private double CalculateEuclideanDistance(Observation observation, Observation centroid)
        {
            double distance = 0;
            for (int i = 0; i < observation.Items.Count; i++)
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
                List<Observation> cs = result.Clusters[i].Observations;
                Console.WriteLine("");
            }
            Console.WriteLine("Error is: {0}", result.Error);
        }

        public void PrintClusters()
        {
            for (int i = 0; i < Clusters.Count; i++)
            {
                Console.WriteLine("ClusterNumber: {0} has {1} observationpoints", Clusters[i].ClusterNumber, Clusters[i].Observations.Count());
            }
        }
    }
}
