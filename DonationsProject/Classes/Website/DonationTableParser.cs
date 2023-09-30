using DonationsProject.Classes.Objects;
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
        public const string pattern = @"<tr><td>(.*?)<\/td><td>(.*?)<\/td><td>(.*?)<\/td><td>(.*?)<\/td><td colspan=""3"">(.*?)<\/td><\/tr>";

        public static void ParseHtmlTable(string html)
        {

            // Use a regular expression to match rows of the table

            Regex regex = new Regex(pattern);

            MatchCollection matches = Regex.Matches(html, pattern);

            foreach (Match match in matches)
            {
                Donation.CreateDonation(match);
            }
        }
    }
}
