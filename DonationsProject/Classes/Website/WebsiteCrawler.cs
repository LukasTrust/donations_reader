using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Shapes;
using DonationsProject.Classes.Objects;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace DonationsProject.Classes.Website
{
    public class WebsiteCrawler
    {
        private const string Base_URL = "https://www.bundestag.de";
        private const string WEBSITE_URL = "/parlament/praesidium/parteienfinanzierung/fundstellen50000";
        private const string searchString = "Parteispenden über 50.000 € - Jahr ";
        private const string linkPattern = @"href=""([^""]*)""";

        private IWebDriver Crawler = new ChromeDriver();
        private string content { get; set;}
        private string[] lines { get; set; }

        public static WebsiteCrawler Instance
        {
            get {
                if (_Instance == null)
                {
                    _Instance = new WebsiteCrawler();
                    
                }
                return _Instance;
            }
        }

        private static WebsiteCrawler _Instance { get; set;}


        public void ConnectToWebsite()
        {
            Crawler.Navigate().GoToUrl(Base_URL + WEBSITE_URL);

            System.Threading.Thread.Sleep(5000);

            content = Crawler.PageSource;
            lines = content.Split('\n');
            string[] specificLines = lines.Where(x => x.Contains(searchString)).ToArray();

            string[] links = new string[specificLines.Length];
            for(int i = 0; i < specificLines.Length; i++)
            {
                ConnectToSubWebsite(ExtractHrefValue(specificLines[i]));
            }
            Donation.Donations.Clear();
        }

        public static string ExtractHrefValue(string input)
        {
            Match match = Regex.Match(input, linkPattern);

            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            else
            {
                return null;
            }
        }

        public void ConnectToSubWebsite(string link)
        {
            Crawler.Navigate().GoToUrl(Base_URL + link);

            System.Threading.Thread.Sleep(1000);

            content = Crawler.PageSource;
            lines = content.Split('\n');
            string specificLines = lines.Where(x => x.Contains("Spender")).FirstOrDefault();
            if (specificLines != null)
            {
                DonationTableParser.ParseHtmlTable(specificLines);
            }
        }
    }
}
