using System;
using System.Collections.Generic;
using System.Text;

namespace KMeansClustering.Models
{
    public class Cluster
    {
        public int ClusterNumber { get; set; }
        public List<Observation> Observations { get; set; }
        public Observation Centroid { get; set; }

        public Cluster()
        {
            Observations = new List<Observation>();
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


    }
}
