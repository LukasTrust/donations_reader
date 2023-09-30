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

        public static async Task CreateDonation(Match match)
        {
            Donation donation = new Donation();
            await Task.Run(() => {
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
                    forConversion = forConversion.Remove(10, forConversion.Length - 10);
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
            });

            Donations.Add(donation);
        }

        public static async Task CreateDonationBevor2015(Match match)
        {
            Donation donation = new Donation();
            await Task.Run(() =>
            {
                string forConversion = "";
                int beginnP = 0;
                int endP = 0;

                donation.Party = match.Groups[1].Value;

                forConversion = match.Groups[2].Value;
                if (forConversion.Contains("Korrigierter"))
                {
                    beginnP = forConversion.IndexOf('>');
                    endP = forConversion.IndexOf('<', beginnP);
                    forConversion = forConversion.Substring(beginnP + 1, endP - beginnP - 1);
                }
                else
                if (forConversion.Contains('<'))
                {
                    beginnP = forConversion.IndexOf('<');
                    forConversion = forConversion.Remove(beginnP, forConversion.Length - beginnP);
                }
                donation.Amount = Convert.ToDouble(forConversion);

                forConversion = match.Groups[3].Value;
                beginnP = forConversion.IndexOf('<');
                forConversion = forConversion.Remove(beginnP, forConversion.Length - beginnP);
                donation.Donor = forConversion;

                forConversion = match.Groups[4].Value;
                if (forConversion.Contains('/') && !forConversion.Contains('p'))
                {
                    beginnP = forConversion.IndexOf('/');
                    forConversion = forConversion.Remove(0, beginnP + 1);
                }
                else
                if (forConversion.Length > 10 && forConversion.Contains('p'))
                {
                    forConversion = forConversion.Remove(10, forConversion.Length - 10);
                }
                else
                if (forConversion.Contains('-'))
                {
                    beginnP = forConversion.IndexOf('-');
                    forConversion = forConversion.Substring(beginnP + 1, forConversion.Length - beginnP - 1);
                }
                if (match.Groups[4].Value.Contains("per"))
                {
                    forConversion = match.Groups[4].Value.Remove(0, 4);
                }
                donation.ReceiptDate = Convert.ToDateTime(forConversion);

                forConversion = match.Groups[5].Value;
                if (forConversion.Contains('<'))
                {
                    beginnP = forConversion.IndexOf('<');
                    forConversion = forConversion.Remove(beginnP, forConversion.Length - beginnP);
                }
                try
                {
                    donation.ReportLink = Convert.ToDateTime(forConversion);
                }
                catch
                {
                    donation.ReportLink = Convert.ToDateTime("12.02.2010");
                }
            });
            Donations.Add(donation);
        }
    }
}
