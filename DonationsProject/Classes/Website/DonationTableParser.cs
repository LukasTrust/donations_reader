﻿using DonationsProject.Classes.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DonationsProject.Classes.Website
{
    public class DonationTableParser
    {
        public static DonationTableParser Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new DonationTableParser();

                }
                return _Instance;
            }
        }

        private static DonationTableParser _Instance { get; set; }

        private const string pattern = @"<tr><td>(.*?)<\/td><td>(.*?)<\/td><td>(.*?)<\/td><td>(.*?)<\/td><td colspan=""3"">(.*?)<\/td><\/tr>";
        private const string linkPattern2009 = @"<a\b[^>]*\bhref\s*=\s*[""'](?<url>.*?)[""'][^>]*>";
        private const string patternBefor2014 = @"<tr><td><p>(.*?)<\/p><\/td><td><p>(.*?)<\/p><\/td><td><p>(.*?)<\/p><\/td><td><p>(.*?)<\/p><\/td><td><p>(.*?)<\/p><\/td><\/tr>";
        public async Task ParseHtmlTable(string html, bool befor2015, bool before2009)
        {
            MatchCollection matches = null;

            if (before2009)
            {
                List<string> links = ExtractLinksFromHtml2009(html);
                foreach(string link in links)
                {
                    string test = await WebsiteCrawler.Instance.DownloadPdfContentAsync(link);
                }
            }
            if (befor2015)
            {
                matches = Regex.Matches(html, patternBefor2014);
                foreach (Match match in matches)
                {
                    await Donation.CreateDonationBevor2015(match);
                }
            }else
            {
                matches = Regex.Matches(html, pattern);

                foreach (Match match in matches)
                {
                    await Donation.CreateDonation(match);
                }
            }
        }

        public List<string> ExtractLinksFromHtml2009(string html)
        {
            MatchCollection matches = Regex.Matches(html, linkPattern2009);
            List<string> links = new List<string>();
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    links.Add(match.Groups["url"].Value);
                }
            }
            return links;
        }
    }
}
