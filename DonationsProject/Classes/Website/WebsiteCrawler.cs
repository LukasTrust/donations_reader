using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Shapes;
using DonationsProject.Classes.Objects;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
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

        private WebClient client = new WebClient();
        private IWebDriver Crawler = new ChromeDriver();
        private string content { get; set; }
        private string[] lines { get; set; }

        public static WebsiteCrawler Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new WebsiteCrawler();

                }
                return _Instance;
            }
        }

        private static WebsiteCrawler _Instance { get; set; }

        public void NavigateToWebsite(string link)
        {
            Crawler.Navigate().GoToUrl(link);

            if (link == Base_URL + WEBSITE_URL)
            {
                System.Threading.Thread.Sleep(5000);
            }
            content = Crawler.PageSource;
            lines = content.Split('\n');
        }

        public async Task<string> DownloadPdfContentAsync(string pdfUrl)
        {
            StringWriter output = new StringWriter();
            await Task.Run(() =>
            {
                byte[] pdfBytes = client.DownloadData(pdfUrl);

                PdfReader reader = new PdfReader(pdfBytes);

                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    output.WriteLine(PdfTextExtractor.GetTextFromPage(reader, i));
                }
            });

            return output.ToString();
        }

        public async Task ConnectToWebsite()
        {
            NavigateToWebsite(Base_URL + WEBSITE_URL);

            string[] specificLines = lines.Where(x => x.Contains(searchString)).ToArray();
            bool befor2015 = false;
            bool befor2009 = false;
            for (int i = 0; i < specificLines.Length; i++)
            {
                if (specificLines[i].Contains("Jahr 2014"))
                {
                    befor2015 = true;
                }
                if (specificLines[i].Contains("Jahr 2008"))
                {
                    befor2009 = true;
                }
                await ConnectToSubWebsite(ExtractLinksFromHtml(specificLines[i]), befor2015, befor2009);
            }
            Donation.Donations.Add(new Donation()
            {
                Party = "CDU",
                Amount = 75000,
                Donor = "Allfinanz Deutsche Vermögensberatung AG",
                ReceiptDate = new DateTime(2010, 02, 10),
            });
            Donation.Donations.Clear();
        }

        public async Task ConnectToSubWebsite(string link, bool befor2015, bool befor2009)
        {
            NavigateToWebsite(Base_URL + link);
            string specificLines = null;
            if (befor2009)
            {
                specificLines = lines.Where(x => x.Contains("Drucksachennummer")).LastOrDefault();
            }
            else
            {
                specificLines = lines.Where(x => x.Contains("Spender")).FirstOrDefault();
            }

            if (specificLines != null)
            {
                await DonationTableParser.Instance.ParseHtmlTable(specificLines, befor2015, befor2009);
            }
        }

        public string ExtractLinksFromHtml(string input)
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
    }
}
