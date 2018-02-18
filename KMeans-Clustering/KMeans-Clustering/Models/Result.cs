using System;
using System.Collections.Generic;
using System.Text;

namespace KMeansClustering.Models
{
    public class Result
    {
        public double Error { get; set; }
        public List<Cluster> Clusters;

        public Result(double error, List<Cluster> clusters)
        {
            Error = error;
            Clusters = clusters;
        }
    }
}
