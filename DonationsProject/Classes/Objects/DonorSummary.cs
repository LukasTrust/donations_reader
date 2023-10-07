using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationsProject.Classes.Objects
{
    public class DonorSummary
    {
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

        public async Task CreateDonors(List<Donation> donations)
        {
            List<string> donors = donations.Select(d => d.Donor).Distinct().ToList();
            foreach (string donor in donors)
            {
                this.Donations = donations.Where(d => d.Donor == this.DonorName).ToList();
                await FillDataObject();
            }
        }

        public async Task FillDataObject()
        {
            List<Donation> donations = this.Donations;
            if (YearToShow != null)
            {
                donations = donations.Where(d => d.DonationDate.Year.Equals(YearToShow.Year)).ToList();
            }

            this.TotalDonations = donations.Count;
            this.DistinctParties = this.Donations.Select(d => d.Party).Distinct().ToList();
            this.DonationsCountPerParty = await GetDonationsCountPerParty();
            this.AmountPerParty = await GetAmountPerParty();
            this.TotalAmount = donations.Sum(d => d.Amount);
            double totalTicks = donations.Sum(d => Math.Abs((d.DonationDate - d.ReportDate).Ticks));
            double averageTicks = totalTicks / donations.Count;
            this.AverageTimeBetweenDonations = TimeSpan.FromTicks((long)averageTicks);
        }

        public async Task<Dictionary<string, int>> GetDonationsCountPerParty()
        {
            Dictionary<string, int> DictCount = new Dictionary<string, int>();
            await Task.Run(() =>
            {
                foreach (string party in DistinctParties)
                {
                    DictCount.Add(party, this.Donations.Where(d => d.Party == party).Count());
                }
            });
            return DictCount;
        }

        public async Task<Dictionary<string, double>> GetAmountPerParty()
        {
            Dictionary<string, double> DictAmount = new Dictionary<string, double>();
            await Task.Run(() =>
            {
                foreach(string party in DistinctParties)
                {
                    DictAmount.Add(party, this.Donations.Where(d => d.Party == party).Sum(d => d.Amount));
                }
            });
            return DictAmount;
        }

        #endregion
    }
}
