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
    public class PieChartPartySummary_VM : INotifyPropertyChanged
    {
        public static PieChartPartySummary_VM Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new PieChartPartySummary_VM();

                }
                return _Instance;
            }
        }
        private static PieChartPartySummary_VM _Instance { get; set; }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public const string LabelPretext = "Donation of ";

        public ICommand ShowOtherPartyCommand { get; set; }
        public ICommand ShowYearBeforeCommand { get; set; }
        public ICommand ShowYearAfterCommand { get; set; }
        public ICommand ShowTotalSummaryCommand { get; set; }

        public List<SeriesCollection> PieViews { get; set; }

        public SeriesCollection PieView { get; set; }

        public List<double> TotalAmounts { get; set; }

        public double TotalAmount { get; set; }

        public List<int> TotalDonations { get; set; }

        public int TotalDonation { get; set; }

        public List<string> Lables { get; set; }

        public string Lable { get; set; }

        public int CurrentLableIndex { get; set; }

        public string YearShown { get; set;}

        public DateTime YearBefore { get; set; }
        public DateTime YearAfter { get; set; }

        public PieChartPartySummary_VM()
        {
            PieViews = new List<SeriesCollection>();
            ShowOtherPartyCommand = new RelayCommandWithParamter<string>(ShowOtherParty);
            ShowYearAfterCommand = new RelayCommand(ShowYearAfter);
            ShowYearBeforeCommand = new RelayCommand(ShowYearBefore);
            ShowTotalSummaryCommand = new RelayCommand(ShowTotalSummary);

            YearBefore = DateTime.Now.AddYears(-1);
            YearAfter = DateTime.Now.AddYears(1);
        }

        public async void ShowYearBefore()
        {
            await ViewModel.PieChartPartySummary_VM.Instance.ShowYear(YearBefore);

            YearAfter = YearAfter.AddYears(-1);
            YearBefore = YearBefore.AddYears(-1);

            AllPropertyChanged();
        }

        public async void ShowYearAfter()
        {
            await ViewModel.PieChartPartySummary_VM.Instance.ShowYear(YearAfter);

            YearBefore = YearBefore.AddYears(1);
            YearAfter = YearAfter.AddYears(1);
            AllPropertyChanged();
        }

        public async void ShowTotalSummary()
        {
            await ViewModel.PieChartPartySummary_VM.Instance.ShowYear(new DateTime(0001, 1, 1));

            AllPropertyChanged();
        }

        public void ShowOtherParty(string partName)
        {
            CurrentLableIndex = Lables.IndexOf(partName);
            PieView = PieViews[CurrentLableIndex];
            Lable = LabelPretext + Lables[CurrentLableIndex] + YearShown;
            TotalAmount = TotalAmounts[CurrentLableIndex];
            TotalDonation = TotalDonations[CurrentLableIndex];
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
            

            PieView = PieViews[CurrentLableIndex];
            Lable = LabelPretext + Lables[CurrentLableIndex] + YearShown;
            TotalAmount = TotalAmounts[CurrentLableIndex];
            TotalDonation = TotalDonations[CurrentLableIndex]; 
            AllPropertyChanged();
        }

        public void AllPropertyChanged()
        {
            OnPropertyChanged(nameof(Lable));
            OnPropertyChanged(nameof(TotalAmount));
            OnPropertyChanged(nameof(PieView));
            OnPropertyChanged(nameof(TotalDonation));
            OnPropertyChanged(nameof(Lables));
            OnPropertyChanged(nameof(YearBefore));
            OnPropertyChanged(nameof(YearAfter));
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
