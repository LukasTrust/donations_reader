using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationsProject.Classes.Objects
{
    public class DonationDateSummary
    {
        // Need to think about this class
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
    }
}
