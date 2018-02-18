using KMeansClustering.Algorithms;
using KMeansClustering.Data;
using System;
using System.Diagnostics;

namespace KMeans_Clustering
{
    class Program
    {
        static void Main(string[] args)
        {
            var stopwatch = Stopwatch.StartNew();

            KMeans kMeans = new KMeans(40, 5);

            kMeans.CreateClusters();
            kMeans.PrintClusters();

            stopwatch.Stop();
            Console.WriteLine($"Elapsed time: {stopwatch.Elapsed.Milliseconds} ms.");

            Console.Read();
        }
    }
}
