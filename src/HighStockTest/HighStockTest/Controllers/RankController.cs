using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web.Mvc;
using Dapper;
using DotNet.Highstock.Enums;
using DotNet.Highstock.Helpers;
using DotNet.Highstock.Options;

namespace HighStockTest.Controllers
{
    public class RankController : Controller
    {
        // GET: Rank
        public ActionResult Index()
        {
            var sqlConn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\SampleData.mdf;Integrated Security=True");
            var trends = sqlConn.Query("Select * From dbo.udft_TrafficRankings()").ToList();


            var gData = new object[trends.Count(), 2]; // global ranking data
            var iData = new object[trends.Count(), 2];  // iran ranking data

            for (var i = 0; i < trends.Count(); i++)
            {
                // The x value of the point. For datetime axes, the X value is the timestamp in milliseconds since 1970.
                gData[i, 0] = new TimeSpan(((DateTime)trends[i].ModifyDate).Subtract(new DateTime(1970, 1, 1)).Ticks).TotalMilliseconds;
                iData[i, 0] = gData[i, 0];

                // The y value of the point. Defaults to null.
                gData[i, 1] = (double)trends[i].GlobalRanking;
                iData[i, 1] = (double)trends[i].IranRanking;
            }

            var rangeSelect = new RangeSelector()
            {
                Buttons = new[] {
                    //new ButtonsItems() { Type = "hour", Count = 12, Text = "12h" },
                    new ButtonsItems() {Type = "day", Count = 3, Text = "3d"},
                    new ButtonsItems() {Type = "week", Count = 1, Text = "1w"},
                    new ButtonsItems() {Type = "month", Count = 1, Text = "1m"},
                    new ButtonsItems() {Type = "month", Count = 6, Text = "6m"},
                    new ButtonsItems() {Type = "year", Count = 1, Text = "1y"},
                    new ButtonsItems() {Type = "all", Text = "All"}
                },
                Selected = 5
            };

            DotNet.Highstock.Highstock gChart = new DotNet.Highstock.Highstock("gStockChart");
            DotNet.Highstock.Highstock iChart = new DotNet.Highstock.Highstock("iStockChart");

            gChart.SetTitle(new Title() { Text = "Global Historical Traffic Trends" });
            iChart.SetTitle(new Title() { Text = "Iran Historical Traffic Trends" });

            gChart.SetYAxis(new[] { new YAxis { Title = new YAxisTitle() { Text = "Ranking Score" }, LineWidth = 2 } });
            iChart.SetYAxis(new[] { new YAxis { Title = new YAxisTitle() { Text = "Ranking Score" }, LineWidth = 2 } });

            gChart.SetSeries(
                new Series()
                {
                    Data = new Data(gData),
                    Color = Color.DeepSkyBlue,
                    DashStyle = DashStyles.Solid,
                    Type = ChartTypes.Area,
                    Name = "Global"
                });

            iChart.SetSeries(
                new Series()
                {
                    Data = new Data(iData),
                    Color = Color.LawnGreen,
                    DashStyle = DashStyles.Solid,
                    Type = ChartTypes.Areaspline,
                    Name = "Iran"
                });

            gChart.SetRangeSelector(rangeSelect);
            iChart.SetRangeSelector(rangeSelect);

            ViewData.Add("GlobalChart", gChart);
            ViewData.Add("IranChart", iChart);

            return View();
        }
    }
}