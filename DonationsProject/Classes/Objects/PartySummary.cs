using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationsProject.Classes.Objects
{
    public class PartySummary
    {
        #region Statics

        public static List<PartySummary> PartiesSummary = new List<PartySummary>();

        #endregion

        #region Properties
        public string PartyName { get; set; }
        public int TotalDonations { get; set; }
        public int TotalDonors { get; set; }
        public double TotalAmount { get; set; }
        public List<string> DistinctDoners { get; set; }
        public List<Donation> Donations { get; set; }
        public Dictionary<string, double> AmountPerDonor { get; set; }
        public TimeSpan AverageTimeBetweenDonationAndReport { get; set; }
        public static DateTime YearToShow { get; set; }

        #endregion

        #region Ctors

        public PartySummary(string partyName)
        {
            this.PartyName = partyName;
        }

        #endregion

        #region Methods

        public static async Task CreateParties(List<Donation> donations)
        {
            PartiesSummary.Clear();
            List<string> distinctPartys = donations.Select(d => d.Party).Distinct().ToList();
            foreach (string party in distinctPartys)
            {
                PartySummary partySummary = new PartySummary(party);
                partySummary.Donations = donations.Where(d => d.Party == partySummary.PartyName).ToList();
                await FillDataObject(partySummary);
                PartiesSummary.Add(partySummary);
            }
        }

        public static async Task FillDataObject(PartySummary partySummary)
        {
            List<Donation> donations = partySummary.Donations;
            if (YearToShow.Year != 0001)
            {
                donations = donations.Where(d => d.DonationDate.Year.Equals(YearToShow.Year)).ToList();
            }

            partySummary.TotalDonations = donations.Count;
            partySummary.DistinctDoners = donations.Select(d => d.Donor).Distinct().ToList();
            partySummary.TotalDonors = partySummary.DistinctDoners.Count();
            partySummary.TotalAmount = donations.Sum(d => d.Amount);
            partySummary.AmountPerDonor = await GetAmountPerDonor(partySummary);
            double totalTicks = donations.Sum(d => Math.Abs((d.DonationDate - d.ReportDate).Ticks));
            double averageTicks = totalTicks / donations.Count;
            partySummary.AverageTimeBetweenDonationAndReport = TimeSpan.FromTicks((long)averageTicks);
        }

        public static async Task<Dictionary<string, double>> GetAmountPerDonor(PartySummary partySummary)
        {
            Dictionary<string, double> DictAmount = new Dictionary<string, double>();

            await Task.Run(() =>
            {
                foreach (string donor in partySummary.DistinctDoners)
                {
                    DictAmount.Add(donor, partySummary.Donations.Where(x => x.Donor == donor).Sum(d => d.Amount));
                }
            });

            return DictAmount;
        }

        #endregion
    }
}
