using DonationsProject.Classes.Objects;
using DonationsProject.Classes.Utils;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace DonationsProject.ViewModel
{
    public class PartySummary_VM : INotifyPropertyChanged
    {
        public static PartySummary_VM Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new PartySummary_VM();

                }
                return _Instance;
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public const string LabelPretext = "Donation of ";

        public ICommand ShowOtherPartyCommand { get; set; }
        private static PartySummary_VM _Instance { get; set; }

        public List<SeriesCollection> PieViews { get; set; }

        public SeriesCollection PieView { get; set; }

        public List<double> TotalAmounts { get; set; }

        public double TotalAmount { get; set; }

        public List<int> TotalDonations { get; set; }

        public int TotalDonation { get; set; }

        public List<string> Lables { get; set; }

        public string Lable { get; set; }

        public string YearShown { get; set;}

        public PartySummary_VM()
        {
            PieViews = new List<SeriesCollection>();
            ShowOtherPartyCommand = new RelayCommandWithParamter<string>(ShowOtherParty);
        }

        public void ShowOtherParty(string partName)
        {
            int index = Lables.IndexOf(partName);
            PieView = PieViews[index];
            Lable = LabelPretext + Lables[index] + YearShown;
            TotalAmount = TotalAmounts[index];
            TotalDonation = TotalDonations[index];
            AllPropertyChanged();
        }

        public async Task CreatePieCharts()
        {
            if (Lables == null)
            {
                Lables = new List<string>();
                TotalAmounts = new List<double>();
                TotalDonations = new List<int>();
            }
            else
            {
                Lables.Clear();
                TotalAmounts.Clear();
                PieViews.Clear();
                TotalDonations.Clear();
            }

            await PartySummary.CreateParties(Donation.Donations);
            foreach (PartySummary partySummary in PartySummary.PartiesSummary)
            {
                SeriesCollection seriesCollection = new SeriesCollection();
                Lables.Add(partySummary.PartyName);
                TotalAmounts.Add(partySummary.TotalAmount);
                TotalDonations.Add(partySummary.TotalDonations);
                foreach (KeyValuePair<string, double> amountPerDonor in partySummary.AmountPerDonor)
                {
                    PieSeries pieSeries = new PieSeries
                    {
                        Title = amountPerDonor.Key,
                        Values = new ChartValues<double> { amountPerDonor.Value },
                        DataLabels = true,
                        LabelPosition = PieLabelPosition.OutsideSlice,
                    };
                    seriesCollection.Add(pieSeries);
                }
                PieViews.Add(seriesCollection);
            }
            PieView = PieViews[0];
            Lable = LabelPretext + Lables[0] + YearShown;
            TotalAmount = TotalAmounts[0];
            TotalDonation = TotalDonations[0];
        }

        public void AllPropertyChanged()
        {
            OnPropertyChanged(nameof(Lable));
            OnPropertyChanged(nameof(TotalAmount));
            OnPropertyChanged(nameof(PieView));
            OnPropertyChanged(nameof(TotalDonation));
        }

        public async Task ShowYear(DateTime yearToShow)
        {
            PartySummary.YearToShow = yearToShow;
            if (yearToShow.Year == 1)
            {
                YearShown = " form 2002 to " + DateTime.Now.Year;
            }
            else
            {
                YearShown = " form " + yearToShow.Year;
            }
            await CreatePieCharts();
            AllPropertyChanged();
        }
    }
}
