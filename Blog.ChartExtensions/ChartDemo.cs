using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Blog.ChartExtensions
{
    public partial class ChartDemo : Form
    {
        public ChartDemo()
        {
            InitializeComponent();
            BuildChart();
        }

        private void BuildChart()
        {
            var rnd = new Random();


            var chart = (from i in Enumerable.Range(0, 30)
                         select new
                         {
                             Key = i.ToString(),
                             Series_One = rnd.NextDouble() * (i / 10),
                             Series_One_ToolTip = "Just a Random Number",
                             Series_Two_Spline = rnd.NextDouble() * (i / 10),
                             Series_Two_Spline_ToolTip = "A Spline",
                             Series_Two_Spline_BorderWidth = 3,
                         }).ToChart();
            chart.Width = this.Width;
            chart.Height = this.Height;
            this.Controls.Add(chart);
        }
    }
}
