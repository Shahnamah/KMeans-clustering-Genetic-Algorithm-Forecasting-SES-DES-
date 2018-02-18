using System;
using System.Collections.Generic;
using System.Text;

namespace KMeansClustering.Models
{
    public class Observation
    {
        public Dictionary<int, double> Items { get; set; }
        public int Id { get; set; }
        
        public Observation()
        {
            Items = new Dictionary<int, double>();
        }

        public void UpdateItems(int key, double value)
        {
            Items[key] = value;
        }
    }
}
