using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms.DataVisualization.Charting;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;

namespace Blog.ChartExtensions
{
    class Program
    {
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ChartDemo());
        }
    }

    public static class ChartExtensions
    {
        /// <summary>
        /// Sets Y Axis label on the first chart area
        /// </summary>
        public static Chart YAxisLabel(this Chart chart, string label)
        {
            chart.ChartAreas.First().AxisY.Title = label;
            return chart;
        }

        /// <summary>
        /// Sets X Axis label on the first chart area
        /// </summary>
        public static Chart XAxisLabel(this Chart chart, string label)
        {
            chart.ChartAreas.First().AxisX.Title = label;
            return chart;
        }

        /// <summary>
        /// Hides all grid lines
        /// </summary>
        public static Chart HideGridLines(this Chart chart)
        {
            return chart.HideXGridLines().HideYGridLines();
        }
        /// <summary>
        /// Hides all X axis grid lines
        /// </summary>
        public static Chart HideXGridLines(this Chart chart)
        {
            foreach (var ca in chart.ChartAreas)
            {
                ca.AxisX.MajorGrid.Enabled = false;
                ca.AxisX.MinorGrid.Enabled = false;
                ca.AxisX2.MajorGrid.Enabled = false;
                ca.AxisX2.MinorGrid.Enabled = false;
            }
            return chart;
        }

        /// <summary>
        /// Hides all Y axis grid lines
        /// </summary>
        public static Chart HideYGridLines(this Chart chart)
        {
            foreach (var ca in chart.ChartAreas)
            {
                ca.AxisY.MajorGrid.Enabled = false;
                ca.AxisY.MinorGrid.Enabled = false;
                ca.AxisY2.MajorGrid.Enabled = false;
                ca.AxisY2.MinorGrid.Enabled = false;
            }
            return chart;
        }

        /// <summary>
        /// Appends a title string to your chart
        /// </summary>
        public static Chart AppendTitle(this Chart chart, string title)
        {
            return chart.AppendTitle(new Title(title));
        }

        /// <summary>
        /// Appends a title to your chart
        /// </summary>
        public static Chart AppendTitle(this Chart chart, Title title)
        {
            chart.Titles.Add(title);
            return chart;
        }

        public static Color[] Colors = new[]
        {
        Color.FromArgb(167, 181, 219),
        Color.FromArgb(68, 114, 196),
        Color.FromArgb(55, 93, 161),
        Color.Beige
    };

        private static string[] DataPointProperties = typeof(DataPoint).GetProperties().Select(x => x.Name).ToArray();

        /// <summary>
        /// Builds a chart of the provided data.  All double properties will be plotted as a series 
        /// (if you want something other than bars append _ChartType, i.e. _Spline.)  To specify 
        /// properties on the data points append _Property (i.e. Hours_Color or Hours_ToolTip.)
        /// </summary>
        /// <param name="data">data to chart</param>
        /// <returns>A chart object to Dump in LinqPad</returns>
        public static Chart ToChart<TData>(this IEnumerable<TData> data)
        {
            return data.ToChart(null, null, null);
        }

        /// <summary>
        /// Builds a chart of the provided data.  All double properties will be plotted as a series 
        /// (if you want something other than bars append _ChartType, i.e. _Spline.)  To specify 
        /// properties on the data points append _Property (i.e. Hours_Color or Hours_ToolTip.)
        /// </summary>
        /// <param name="data">data to chart</param>
        /// <param name="xAxis">Customized X Axis</param>
        /// <returns>A chart object to Dump in LinqPad</returns>
        public static Chart ToChart<TData>(this IEnumerable<TData> data, Axis xAxis)
        {
            return data.ToChart(xAxis, null, null);
        }

        /// <summary>
        /// Builds a chart of the provided data.  All double properties will be plotted as a series 
        /// (if you want something other than bars append _ChartType, i.e. _Spline.)  To specify 
        /// properties on the data points append _Property (i.e. Hours_Color or Hours_ToolTip.)
        /// </summary>
        /// <param name="data">data to chart</param>
        /// <param name="xAxis">Customized X Axis</param>
        /// <param name="yAxis">Customized Y Axis</param>
        /// <returns>A chart object to Dump in LinqPad</returns>
        public static Chart ToChart<TData>(this IEnumerable<TData> data, Axis xAxis, Axis yAxis)
        {
            return data.ToChart(xAxis, yAxis, null);
        }

        /// <summary>
        /// Builds a chart of the provided data.  All double properties will be plotted as a series 
        /// (if you want something other than bars append _ChartType, i.e. _Spline.)  To specify 
        /// properties on the data points append _Property (i.e. Hours_Color or Hours_ToolTip.)
        /// </summary>
        /// <param name="data">data to chart</param>
        /// <param name="xAxis">Customized X Axis</param>
        /// <param name="yAxis">Customized Y Axis</param>
        /// <returns>A chart object to Dump in LinqPad</returns>
        public static Chart ToChart<TData>(this IEnumerable<TData> data, Axis xAxis, Axis yAxis, object properties)
        {
            var key = typeof(TData).GetProperties().FirstOrDefault(p => p.Name == "Key");
            if (key == null)
            {
                throw new ArgumentException(string.Format(@"Data Type {0} must have property named ""Key"" for X-Axis", typeof(TData).Name));
            }
            var series = typeof(TData).GetProperties().Where(x => new[] { typeof(double), typeof(float), typeof(decimal), typeof(int), typeof(long), }.Contains(x.PropertyType) && DataPointProperties.Any(dp => x.Name.EndsWith(dp)) == false).Select(x => new { Property = x, Series = new Series() { Name = x.Name } }).ToList();

            var chartTypes = typeof(SeriesChartType).GetFields(BindingFlags.Public | BindingFlags.Static).ToList();
            foreach (var s in series)
            {
                var seriesType = chartTypes.FirstOrDefault(ct => s.Property.Name.EndsWith("_" + ct.Name));
                if (seriesType != null)
                {
                    s.Series.ChartType = (SeriesChartType)seriesType.GetValue(null);
                    s.Series.Name = s.Series.Name.Replace("_" + seriesType.Name, string.Empty);
                    s.Series.LegendText = s.Series.Name.Replace('_', ' ');
                }
            }
            if (series.Any() == false)
            {
                throw new ArgumentException("This function can only chart numeric properties in TData but none where provided.");
            }
            var chart = new Chart();
            var ca = new ChartArea();
            if (xAxis != null) ca.AxisX = xAxis;
            if (yAxis != null) ca.AxisY = yAxis;
            chart.ChartAreas.Add(ca);

            series.ForEach(s => chart.Series.Add(s.Series));
            chart.Legends.Add("default");
            foreach (var d in data)
            {
                foreach (var s in series)
                {
                    var val = Convert.ToDouble(s.Property.GetValue(d));
                    var dp = new DataPoint(0.0, val);
                    dp.AxisLabel = key.GetValue(d).ToString();
                    foreach (var p in DataPointProperties)
                    {
                        var propName = string.Format("{0}_{1}", s.Property.Name, p);
                        var tempProp = typeof(TData).GetProperties().FirstOrDefault(tp => tp.Name == propName);
                        if (tempProp != null && tempProp.PropertyType == typeof(DataPoint).GetProperties().First(x => x.Name == p).PropertyType)
                        {
                            typeof(DataPoint).GetProperties().First(x => x.Name == p).SetValue(dp, tempProp.GetValue(d));
                            if (p == "Color") s.Series.Color = (Color)tempProp.GetValue(d);
                            if (p == "CustomProperties") s.Series.CustomProperties = (string)tempProp.GetValue(d);
                        }
                    }
                    s.Series.Points.Add(dp);
                }
            }
            if (properties != null)
            {
                var chartProps = chart.GetType().GetProperties().ToDictionary(k => k.Name, v => v);
                foreach (var prop in properties.GetType().GetProperties())
                {
                    if (chartProps.ContainsKey(prop.Name) && chartProps[prop.Name].PropertyType == prop.PropertyType)
                    {
                        chartProps[prop.Name].SetValue(chart, prop.GetValue(properties));
                    }
                }
            }
            return chart;
        }
    }
}
