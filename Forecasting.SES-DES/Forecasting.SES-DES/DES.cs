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
    public class DES
    {
        private const string BEST_ALPHA = "BestAlpha";
        private const string BEST_BETA = "BestBeta";
        private const string SMALLEST_SSE = "SmallestSse";
        private const string TREND_LIST = "TrendList";
        private const string SMOOTHING_LIST = "SmoothingList";
        private readonly double[] demands;
        private readonly int forecastingTimeInMonths;
        private readonly Label yLabel, xLabel, chartTitle;
        private readonly Chart chart1;

        public DES(Chart chart1, Label yLabel, Label xLabel, Label chartTitle, int forecastingTimeInMonths)
        {
            demands = DataImporter.DemandList.ToArray();
            this.yLabel = yLabel;
            this.xLabel = xLabel;
            this.chartTitle = chartTitle;
            this.chart1 = chart1;
            this.forecastingTimeInMonths = forecastingTimeInMonths;
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
            
            var alphaBetaSse = ComputeAlphaBetaSSE();
            var smoothSeq = ComputeTrendSmoothing(alphaBetaSse[BEST_ALPHA], alphaBetaSse[BEST_BETA]);
            var finalForecastSeq = ComputeFinalForecast(smoothSeq[SMOOTHING_LIST], smoothSeq[TREND_LIST], forecastingTimeInMonths).ToArray();

            for (int i = 0; i < demands.Length; i++)
            {
                swordsSerie.Points.AddXY(i + 1, demands[i]);
            }
            for (int i = 0; i < smoothSeq[SMOOTHING_LIST].Length; i++)
            {
                smoothingSerie.Points.AddXY(i + 1, smoothSeq[SMOOTHING_LIST][i]);
            }
            for (int i = 0; i < finalForecastSeq.Length; i++)
            {
                forecastSerie.Points.AddXY(smoothSeq[SMOOTHING_LIST].Length + 1 + i, finalForecastSeq[i]);
            }

            xLabel.Text = "Months";
            yLabel.Text = "Demands";
            chartTitle.Text = $"Sword Forecasting DES, Alpha {alphaBetaSse[BEST_ALPHA]}, Beta {alphaBetaSse[BEST_BETA]} and SSE {alphaBetaSse[SMALLEST_SSE]}";
            chartTitle.Font = new Font("Verdana", 20);

            if (chart1.Series.Any())
                chart1.Series.Clear();
            
            chart1.Series.Add(swordsSerie);
            chart1.Series.Add(smoothingSerie);
            chart1.Series.Add(forecastSerie);
        }

        private Dictionary<string, double[]> ComputeTrendSmoothing(double alpha, double beta)
        {
            var smoothingList = new List<double>();
            var trendList = new List<double>();
            var trendAndSmoothingList = new Dictionary<string, double[]>();
            //Initialize lists
            smoothingList.Add(demands[0]);
            trendList.Add(demands[1] - demands[0]);
            int indexTrendSmooth = 1;
            for (int i = 1; i < demands.Length; i++)
            {
                double smoothValue = alpha * demands[indexTrendSmooth] + (1 - alpha) 
                                     * (smoothingList[indexTrendSmooth - 1]
                                     + trendList[indexTrendSmooth - 1]);
                smoothingList.Add(smoothValue);
                double trendValue = beta * (smoothingList[indexTrendSmooth] - smoothingList[indexTrendSmooth - 1])
                        + (1 - beta) * trendList[indexTrendSmooth - 1];
                trendList.Add(trendValue);
                indexTrendSmooth++;
            }
            trendAndSmoothingList.Add(TREND_LIST, trendList.ToArray()); //first position is for the trendlist
            trendAndSmoothingList.Add(SMOOTHING_LIST, smoothingList.ToArray()); //second position is for the smoothinglist
            return trendAndSmoothingList;
        }

        private IEnumerable<double> ComputeInitialForecasting(double[] smoothSeq, double[] trendSeq)
        {
            //The initial Forecasting begins at t = 3 so in order to compute that
            //we need to begin at i = 1 which is t = 2 for the dataSeq (see week5.pdf slide 21)
            for (int i = 1; i < smoothSeq.Length; i++)
            {
                yield return smoothSeq[i] + trendSeq[i];
            }
        }

        private double ComputeSse(double[] forecastSeq)
        {
            //Sum Of Squared Error
            double sse = 0;
            for (int i = 2; i < demands.Length; i++)
            {
                sse += Math.Pow(demands[i] - forecastSeq[i - 2], 2);
            }
            return Math.Sqrt(sse / (forecastSeq.Length - 2));
        }
        
        private IEnumerable<double> ComputeFinalForecast(double[] smoothSeq, double[] trendSeq, int timeInMonth)
        {
            double s = smoothSeq[smoothSeq.Length - 1];
            double b = trendSeq[trendSeq.Length - 1];
            for (int i = 1; i <= timeInMonth; i++)
            {
                yield return s + (i * b);
            }
        }

        private Dictionary<string, double> ComputeAlphaBetaSSE()
        {
            var alphaBetaSSE = new Dictionary<string, double>();
            double smallestSSE = Double.MaxValue;
            double bestAlpha = 0;
            double bestBeta = 0;
            for (double alpha = 0.001; alpha < 1; alpha += 0.001)
            {
                for (double beta = 0.001; beta < 1; beta += 0.001)
                {
                    Dictionary<string, double[]> trendAndSmoothSeq = ComputeTrendSmoothing(alpha, beta);//pos 0 is trend/ pos 1 is smooth
                    double[] initialForecastSeq = ComputeInitialForecasting(trendAndSmoothSeq[SMOOTHING_LIST], trendAndSmoothSeq[TREND_LIST]).ToArray();
                    double sse = ComputeSse(initialForecastSeq);
                    if (sse < smallestSSE)
                    {
                        smallestSSE = sse;
                        bestAlpha = alpha;
                        bestBeta = beta;
                    }
                }
            }
            alphaBetaSSE.Add(BEST_ALPHA, bestAlpha); // first position is for the alpha value
            alphaBetaSSE.Add(BEST_BETA, bestBeta); // second position is for the beta value
            alphaBetaSSE.Add(SMALLEST_SSE, smallestSSE);// third position is for the sse value
            return alphaBetaSSE;
        }
    }
}
