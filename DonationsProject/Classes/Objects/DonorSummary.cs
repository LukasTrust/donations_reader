using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationsProject.Classes.Objects
{
    public class DonorSummary
    {
        #region Statics

        public static List<DonorSummary> DonorsSummary = new List<DonorSummary>();

        #endregion

        #region Properties
        public string DonorName { get; set; }
        public int TotalDonations { get; set; }
        public double TotalAmount { get; set; }
        public Dictionary<string, int> DonationsCountPerParty { get; set; }
        public Dictionary<string, double> AmountPerParty { get; set; }
        public List<string> DistinctParties { get; set; }
        public List<Donation> Donations { get; set; }
        public DateTime YearToShow { get; set; }
        public TimeSpan AverageTimeBetweenDonations { get; set; }

        #endregion

        #region Ctors

        public DonorSummary(string donorName)
        {
            this.DonorName = donorName;
        }

        #endregion

        #region Methods

        public static async Task CreateDonors(List<Donation> donations)
        {
            List<string> donors = donations.Select(d => d.Donor).Distinct().ToList();
            foreach (string donor in donors)
            {
                DonorSummary donorSummary = new DonorSummary(donor);
                donorSummary.Donations = donations.Where(d => d.Donor == donorSummary.DonorName).ToList();
                await FillDataObject(donorSummary);
                DonorsSummary.Add(donorSummary);
            }
        }

        public static async Task FillDataObject(DonorSummary donorSummary)
        {
            List<Donation> donations = donorSummary.Donations;
            if (donorSummary.YearToShow.Year != 0001)
            {
                donations = donations.Where(d => d.DonationDate.Year.Equals(donorSummary.YearToShow.Year)).ToList();
            }

            donorSummary.TotalDonations = donations.Count;
            donorSummary.DistinctParties = donorSummary.Donations.Select(d => d.Party).Distinct().ToList();
            donorSummary.DonationsCountPerParty = await GetDonationsCountPerParty(donorSummary);
            donorSummary.AmountPerParty = await GetAmountPerParty(donorSummary);
            donorSummary.TotalAmount = donations.Sum(d => d.Amount);
            double totalTicks = donations.Sum(d => Math.Abs((d.DonationDate - d.ReportDate).Ticks));
            double averageTicks = totalTicks / donations.Count;
            donorSummary.AverageTimeBetweenDonations = TimeSpan.FromTicks((long)averageTicks);
        }

        public static async Task<Dictionary<string, int>> GetDonationsCountPerParty(DonorSummary donorSummary)
        {
            Dictionary<string, int> DictCount = new Dictionary<string, int>();
            await Task.Run(() =>
            {
                foreach (string party in donorSummary.DistinctParties)
                {
                    DictCount.Add(party, donorSummary.Donations.Where(d => d.Party == party).Count());
                }
            });
            return DictCount;
        }

        public static async Task<Dictionary<string, double>> GetAmountPerParty(DonorSummary donorSummary)
        {
            Dictionary<string, double> DictAmount = new Dictionary<string, double>();
            await Task.Run(() =>
            {
                foreach(string party in donorSummary.DistinctParties)
                {
                    DictAmount.Add(party, donorSummary.Donations.Where(d => d.Party == party).Sum(d => d.Amount));
                }
            });
            return DictAmount;
        }

        public static async Task SetYearToShow(bool UpOrDown, DonorSummary donorSummary)
        {
            if (donorSummary.YearToShow == null)
            {
                donorSummary.YearToShow = DateTime.Now;
            }
            else if (UpOrDown)
            {
                donorSummary.YearToShow.AddYears(1);
            }
            else
            {
                donorSummary.YearToShow.AddYears(-1);
            }
            await FillDataObject(donorSummary);
        }

        #endregion
    }
}
