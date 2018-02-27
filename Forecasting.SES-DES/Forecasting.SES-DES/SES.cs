﻿using System;
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
        private List<double> demands = new List<double>();
        Label yLabel, xLabel, chartTitle;
        Chart chart1;

        public SES(Chart chart1, Label yLabel, Label xLabel, Label chartTitle)
        {
            demands = DataImporter.DemandList;
            this.yLabel = yLabel;
            this.xLabel = xLabel;
            this.chartTitle = chartTitle;
            this.chart1 = chart1;
            InitializeSeries();
        }

        private void InitializeSeries()
        {
            Series swordsSerie = new Series { Name = "Swords data", Color = Color.Black, ChartType = SeriesChartType.Line };
            Series smoothingSerie = new Series { Name = "Smoothing", Color = Color.Red, ChartType = SeriesChartType.Line };
            Series forecastSerie = new Series { Name = "Forecast", Color = Color.Blue, ChartType = SeriesChartType.Line };

            double[] alphaAndSse = ComputeAlphaAndSSE();
            List<double> smoothingSequence = ComputeSmoothing(alphaAndSse[0]);
            List<double> forecastingSequence = ComputeForecasting(smoothingSequence, alphaAndSse[0], 12);

            for (int i = 0; i < demands.Count; i++)
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
            chartTitle.Text = string.Format("Sword Forecasting SES/DES, best Alpha {0}, SSE {1}", alphaAndSse[0], alphaAndSse[1]);
            chartTitle.Font = new Font("Verdana", 20);
            chart1.Series.Add(swordsSerie);
            chart1.Series.Add(smoothingSerie);
            chart1.Series.Add(forecastSerie);
        }

        public double InitSmoothValue()
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
            for (int i = 1; i < demands.Count; i++)
            {
                double smoothValue = alpha * demands[i - 1] + (1 - alpha) * smoothing[i - 1];
                smoothing.Add(smoothValue);
            }
            return smoothing;
        }

        public double[] ComputeAlphaAndSSE()
        {
            double smallestSSE = Double.MaxValue;
            double bestAlpha = 0;
            double[] errorAndAlpha = new double[2];

            for (double alpha = 0.001; alpha <= 1; alpha += 0.001)
            {
                double sse = SSE(ComputeSmoothing(alpha));
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

        public List<double> ComputeForecasting(List<double> smoothSeq, double alpha, int timeinMonth)
        {
            //Compute forecast smooth by using last demand and smooth values
            var forecasting = new List<double>();
            double lastSmoothValue = alpha * demands[demands.Count - 1] + (1 - alpha)
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
            for (int i = 0; i < demands.Count; i++)
            {
                sse += Math.Pow((demands[i] - smoothSequence[i]), 2);
            }
            return Math.Sqrt(sse / (smoothSequence.Count - 1));
        }
    }
}
