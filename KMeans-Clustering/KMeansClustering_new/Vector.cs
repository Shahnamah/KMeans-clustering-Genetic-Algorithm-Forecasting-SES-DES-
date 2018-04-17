using System;
using System.Collections.Generic;
using System.Text;

namespace KMeansClustering_new
{
    public class Vector
    {
        public Vector(double[] coordinates)
        {
            Coordinates = coordinates;
        }

        public double[] Coordinates;
        public double ShortestDistanceToCentroid;
        public int ClusterId;
    }
}
