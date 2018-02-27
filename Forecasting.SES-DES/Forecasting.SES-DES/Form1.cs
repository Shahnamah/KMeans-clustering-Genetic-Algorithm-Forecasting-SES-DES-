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
        public Form1()
        {
            InitializeComponent();
            //SES
            new SES(chart1, yLabel, xLabel, chartTitle);
            //DES
        }
    }
}
