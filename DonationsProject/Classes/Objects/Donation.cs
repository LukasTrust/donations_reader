using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DonationsProject.Classes.Objects
{
    public class Donation
    {
        public static List<Donation> Donations { get; set; } = new List<Donation>();

        public string Party { get; set; }
        public double Amount { get; set; }
        public string Donor { get; set; }
        public DateTime ReceiptDate { get; set; }
        public DateTime ReportLink { get; set; }

        public static void CreateDonation(Match match)
        {
            Donation donation = new Donation();
            int beginnP = 0;
            int endP = 0;
            string forConversion = "";

            beginnP = match.Groups[1].Value.IndexOf('>');
            endP = match.Groups[1].Value.IndexOf('<', beginnP);
            donation.Party = match.Groups[1].Value.Substring(beginnP + 1, endP - beginnP - 1);

            beginnP = match.Groups[2].Value.IndexOf('>');
            endP = match.Groups[2].Value.IndexOf('<', beginnP);
            forConversion = match.Groups[2].Value.Substring(beginnP + 1, endP - beginnP - 1);

            forConversion = forConversion.Replace("&nbsp;", "");
            forConversion = forConversion.Replace("Euro", "");
            forConversion = forConversion.Replace(" ", "");

            donation.Amount = Convert.ToDouble(forConversion);

            beginnP = match.Groups[3].Value.IndexOf('>');
            endP = match.Groups[3].Value.IndexOf('<', beginnP);
            donation.Donor = match.Groups[3].Value.Substring(beginnP + 1, endP - beginnP - 1);

            beginnP = match.Groups[5].Value.IndexOf('>');
            endP = match.Groups[5].Value.IndexOf('<', beginnP);
            forConversion = match.Groups[5].Value.Substring(beginnP + 1, endP - beginnP - 1);

            forConversion = Regex.Replace(forConversion, "[a-zA-Z]", ""); 
            if (forConversion.Length > 10)
            {
                forConversion = forConversion.Remove(10, forConversion.Length-10);
            }

            donation.ReportLink = Convert.ToDateTime(forConversion);

            beginnP = match.Groups[4].Value.IndexOf('>');
            endP = match.Groups[4].Value.IndexOf('<', beginnP);
            forConversion = match.Groups[4].Value.Substring(beginnP + 1, endP - beginnP - 1);

            if (forConversion.Length == 6)
            {
                forConversion = forConversion + donation.ReportLink.Year;
            }
            forConversion = forConversion.Replace("/", "");
            forConversion = Regex.Replace(forConversion, "[a-zA-Z]", "");

            donation.ReceiptDate = Convert.ToDateTime(forConversion);

            Donations.Add(donation);
        }
    }
}
