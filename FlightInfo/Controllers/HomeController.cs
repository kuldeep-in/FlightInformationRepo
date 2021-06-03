using FlightInfo.Models;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace FlightInfo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var resultdataList = new List<RawDataModel>();
            System.Net.WebClient webClient = new System.Net.WebClient
            {
                Encoding = System.Text.Encoding.UTF8
            };
            string TodaysDate = DateTime.Now.AddMinutes(330).ToString("dd MMM yyyy");
            string YesterdaysDate = DateTime.Now.AddMinutes(330).AddDays(-1).ToString("dd MMM yyyy");
            string beforeyesterday = DateTime.Now.AddMinutes(330).AddDays(-2).ToString("dd MMM yyyy");
            string TomorrowsDate = DateTime.Now.AddMinutes(330).AddDays(1).ToString("dd MMM yyyy");

            resultdataList.AddRange(GetData("6e1215", YesterdaysDate, beforeyesterday, "P1", "Indigo", "6E 1215", 480));
            resultdataList.AddRange(GetData("sg471", YesterdaysDate, TomorrowsDate, "P2", "Spicejet", "SG 471", 330));
            resultdataList.AddRange(GetData("uk860", YesterdaysDate, TomorrowsDate, "P3", "Vistara", "UK 860", 330));
            resultdataList.AddRange(GetData("ai120", YesterdaysDate, beforeyesterday, "P4", "Air India", "AI 120", 60));
            resultdataList.AddRange(GetData("ai112", YesterdaysDate, beforeyesterday, "P5", "Air India", "AI 112", 0));
            resultdataList.AddRange(GetData("sg8937", YesterdaysDate, TomorrowsDate, "P6", "Spicejet", "SG 8937", 330));
            resultdataList.AddRange(GetData("uk954", YesterdaysDate, TomorrowsDate, "P7", "Vistara", "UK 954", 330));

            return View(resultdataList.OrderBy(x => x.STA));
        }

        public static List<RawDataModel> GetData(string numberurl, string day1, string day2, string name, string airline, string flightnumber, long timediff)
        {
            var dataList = new List<RawDataModel>();
            System.Net.WebClient webClient = new System.Net.WebClient
            {
                Encoding = System.Text.Encoding.UTF8
            };
            string TodaysDate = DateTime.Now.AddMinutes(330).ToString("dd MMM yyyy");
            string statusstring = "";
            string statusstring1 = "";
            string statusstring2 = "";
            string atdstring = "";
            string url = string.Format("https://www.flightradar24.com/data/flights/{0}", numberurl);

            //Mohit
            string result = webClient.DownloadString(url);

            var _doc = new HtmlDocument();
            _doc.LoadHtml(result);

            var tab2 = _doc.DocumentNode.SelectNodes("//section//div//table");
            if (tab2 != null)
            {
                IEnumerable<HtmlNode> tab = tab2.Where(x => x.Id == "tbl-datatable").FirstOrDefault().SelectNodes("tbody//tr");

                foreach (HtmlNode row in tab)
                {
                    var nodes = row.SelectNodes("td");
                    if ((nodes[2].InnerText.Trim() == day1 || nodes[2].InnerText.Trim() == day2 || nodes[2].InnerText.Trim() == TodaysDate) && nodes[4].InnerText.ToString().Contains("Delhi"))
                    {
                        statusstring = nodes[11].InnerText;
                        if (nodes[11].Attributes["data-timestamp"].Value != "" && (nodes[11].InnerText.Contains("Landed") || nodes[11].InnerText.Contains("Delayed") || nodes[11].InnerText.Contains("Estimated")))
                        {
                            statusstring1 = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(nodes[11].Attributes["data-timestamp"].Value)).ToString("hh:mm");
                            statusstring2 = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(nodes[11].Attributes["data-timestamp"].Value)).AddMinutes(330).ToString("hh:mm tt");
                            statusstring = statusstring.Replace(statusstring1, statusstring2);
                        }

                        if (nodes[8].InnerText.Trim() != "&mdash;")
                            atdstring = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(nodes[8].Attributes["data-timestamp"].Value)).AddMinutes(timediff).ToString("hh:mm tt");

                        dataList.Add(new RawDataModel
                        {
                            Name = name,
                            Airline = airline,
                            FlightNumber = flightnumber,
                            Date = nodes[2].InnerText,
                            From = nodes[3].InnerText,
                            To = nodes[4].InnerText,
                            STD = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(nodes[7].Attributes["data-timestamp"].Value)).AddMinutes(timediff),
                            ATD = atdstring,
                            STA = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(nodes[9].Attributes["data-timestamp"].Value)).AddMinutes(330),
                            Status = statusstring
                        });
                    }
                }
            }

            return dataList;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
