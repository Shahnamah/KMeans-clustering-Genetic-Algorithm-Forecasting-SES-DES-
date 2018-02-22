using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Forecasting.SES_DES
{
    public partial class Form1 : Form
    {
        private List<double> demands = new List<double>();

        public Form1()
        {
            InitializeComponent();
            demands = DataImporter.DemandList;
            InitializeSeries();
        }

        private void InitializeSeries()
        {
            chart1.Titles.Add("Sword Forecasting SES/DES, best Alpha {0}, SSE {1}");

            //
            Series serie1 = new Series { Name = "Swords data", Color = Color.Black, ChartType = SeriesChartType.Line };
            Series serie2 = new Series { Name = "Smoothing", Color = Color.Red, ChartType = SeriesChartType.Line };
            Series serie3 = new Series { Name = "Forecast", Color = Color.Blue, ChartType = SeriesChartType.Line };
            for (int i = 0; i < demands.Count; i++)
            {
                serie1.Points.AddXY(i + 1, demands[i]);
            }

            for (int i = 0; i < demands.Count; i++)
            {
                serie1.Points.AddXY(i + 1, demands[i]);
            }

            for (int i = 0; i < demands.Count; i++)
            {
                serie1.Points.AddXY(i + 1, demands[i]);
            }
            //serie1.Points.Add(new DataPoint { x})

            chart1.Series.Add(serie1);
        }

        private List<double> ComputeSmoothing(double alpha)
        {
            
        }

        public double[] computeAlphaAndSSE()
        {
            double smallestSSE = Double.MaxValue;
            double bestAlpha = 0;
            double[] errorAndAlpha = new double[2];
            for (double alpha = 0.001; alpha <= 1; alpha += 0.001)
            {
                List<Double> smoothingSeq = ComputeSmoothing(alpha);
                double sse = SSE(smoothingSeq);
                if (sse < smallestSSE)
                {
                    smallestSSE = sse;
                    bestAlpha = alpha;
                }
            }
            errorAndAlpha[0] = bestAlpha; // first position is for the alpha value
            errorAndAlpha[1] = smallestSSE;// second position is for the sse value
            return errorAndAlpha;
        }

        private double SSE(List<double> smoothSequence)
        {
            double sse = 0;
            for (int i = 0; i < demands.Count; i++)
            {
                sse += Math.Pow((demands[i] - smoothSequence[i]), 2);
            }
            return Math.Sqrt(sse / (smoothSequence.Count - 1));
        }

        private void chart1_Click(object sender, EventArgs e)
        {
            
        }
    }
}
