using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationsProject.Classes.Objects
{
    public class PartySummary
    {
        #region Properties
        public string PartyName { get; set; }
        public int TotalDonations { get; set; }
        public int TotalDonors { get; set; }
        public double TotalAmount { get; set; }
        public List<string> DistinctDoners { get; set; }
        public List<Donation> Donations { get; set; }
        public DateTime YearToShow { get; set; }
        public TimeSpan AverageTimeBetweenDonationAndReport { get; set; }

        #endregion

        #region Ctors

        public PartySummary(string partyName)
        {
            this.PartyName = partyName;
        }

        #endregion

        #region Methods

        public async Task CreateParties(List<Donation> donations)
        {
            List<string> distinctPartys = donations.Select(d => d.Party).Distinct().ToList();
            foreach(string party in distinctPartys)
            {
                PartySummary partySummary = new PartySummary(party);
                this.Donations = donations.Where(d => d.Party == this.PartyName).ToList();
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

            await Task.Run(() =>
            {
                this.TotalDonations = donations.Count;
                this.DistinctDoners = donations.Select(d => d.Donor).Distinct().ToList();
                this.TotalDonors = DistinctDoners.Count();
                this.TotalAmount = donations.Sum(d => d.Amount);
                double totalTicks = donations.Sum(d => Math.Abs((d.DonationDate - d.ReportDate).Ticks));
                double averageTicks = totalTicks / donations.Count;
                this.AverageTimeBetweenDonationAndReport = TimeSpan.FromTicks((long)averageTicks);
            });
        }

        public async Task SetYearToShow(bool UpOrDown)
        {
            if (YearToShow == null)
            {
                YearToShow = DateTime.Now;
            }
            else if (UpOrDown)
            {
                this.YearToShow.AddYears(1);
            }
            else
            {
                this.YearToShow.AddYears(-1);
            }
            await FillDataObject();
        }

        #endregion
    }
}
