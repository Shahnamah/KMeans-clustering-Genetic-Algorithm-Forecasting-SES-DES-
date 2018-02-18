using KMeansClustering.Data;
using KMeansClustering.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace KMeansClustering.Algorithms
{
    public class KMeans
    {
        private readonly int iterations;
        private readonly int totalClusters;

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

                }
            }
        }
    }
}
