using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Forecasting.SES_DES
{
    public class SES
    {
        private const string BestAlpha = "BestAlpha";
        private const string SmallestSse = "SmallestSse";
        private readonly double[] demands;
        private readonly Label yLabel, xLabel, chartTitle;
        private readonly Chart chart1;

        public SES(Chart chart1, Label yLabel, Label xLabel, Label chartTitle)
        {
            demands = DataImporter.DemandList.ToArray();
            this.yLabel = yLabel;
            this.xLabel = xLabel;
            this.chartTitle = chartTitle;
            this.chart1 = chart1;
            InitializeSeries();
        }

        private void InitializeSeries()
        {
            var swordsSerie = new Series
            {
                Name = "Swords data",
                Color = Color.Black,
                ChartType = SeriesChartType.Line,
                MarkerStyle = MarkerStyle.Circle,
                MarkerSize = 6
            };
            var smoothingSerie = new Series
            {
                Name = "Smoothing",
                Color = Color.Red,
                ChartType = SeriesChartType.Line,
                MarkerStyle = MarkerStyle.Circle,
                MarkerSize = 6
            };
            var forecastSerie = new Series
            {
                Name = "Forecast",
                Color = Color.Blue,
                ChartType = SeriesChartType.Line,
                MarkerStyle = MarkerStyle.Circle,
                MarkerSize = 6
            };

            Dictionary<string, double> alphaAndSse = ComputeAlphaAndSse();
            List<double> smoothingSequence = ComputeSmoothing(alphaAndSse[BestAlpha]);
            List<double> forecastingSequence = ComputeForecasting(smoothingSequence, alphaAndSse[BestAlpha], 12);

            for (int i = 0; i < demands.Length; i++)
            {
                swordsSerie.Points.AddXY(i + 1, demands[i]);
            }

            for (int i = 0; i < smoothingSequence.Count; i++)
            {
                smoothingSerie.Points.AddXY(i + 1, smoothingSequence[i]);
            }

            for (int i = 0; i < forecastingSequence.Count; i++)
            {
                forecastSerie.Points.AddXY(smoothingSequence.Count + 1 + i, forecastingSequence[i]);
            }

            xLabel.Text = "Months";
            yLabel.Text = "Demands";
            chartTitle.Text = $"Sword Forecasting SES, best Alpha {alphaAndSse[BestAlpha]}, SSE {alphaAndSse[SmallestSse]}";
            chartTitle.Font = new Font("Verdana", 20);

            if (chart1.Series.Any())
                chart1.Series.Clear();
            
            chart1.Series.Add(swordsSerie);
            chart1.Series.Add(smoothingSerie);
            chart1.Series.Add(forecastSerie);
        }

        private double InitSmoothValue()
        {
            double sumOfDataSeq = 0;
            for (int i = 0; i < 12; i++)
            {
                sumOfDataSeq += demands[i];
            }
            return sumOfDataSeq / 12;
        }

        private List<double> ComputeSmoothing(double alpha)
        {
            //Initialize list
            var smoothing = new List<double>();
            smoothing.Add(InitSmoothValue());
            for (int i = 1; i < demands.Length; i++)
            {
                double smoothValue = alpha * demands[i - 1] + (1 - alpha) * smoothing[i - 1];
                smoothing.Add(smoothValue);
            }
            return smoothing;
        }

        private Dictionary<string, double> ComputeAlphaAndSse()
        {
            double smallestSSE = Double.MaxValue;
            double bestAlpha = 0;
            var errorAndAlpha = new Dictionary<string, double>();

            for (double alpha = 0; alpha <= 1; alpha += 0.001)
            {
                double sse = SSE(ComputeSmoothing(alpha));
                if (sse < smallestSSE)
                {
                    smallestSSE = sse;
                    bestAlpha = alpha;
                }
            }
            errorAndAlpha.Add(BestAlpha, bestAlpha); // first position is for the alpha value
            errorAndAlpha.Add(SmallestSse, smallestSSE);// second position is for the sse value
            return errorAndAlpha;
        }

        private List<double> ComputeForecasting(List<double> smoothSeq, double alpha, int timeinMonth)
        {
            //Compute forecast smooth by using last demand and smooth values
            var forecasting = new List<double>();
            double lastSmoothValue = alpha * demands[demands.Length - 1] + (1 - alpha)
                    * smoothSeq[smoothSeq.Count - 1];
            for (int i = 0; i < timeinMonth; i++)
            {
                forecasting.Add(lastSmoothValue);
            }
            return forecasting;
        }

        private double SSE(List<double> smoothSequence)
        {
            double sse = 0;
            for (int i = 0; i < demands.Length; i++)
            {
                sse += Math.Pow((demands[i] - smoothSequence[i]), 2);
            }
            return Math.Sqrt(sse / (smoothSequence.Count - 1));
        }
    }
}
