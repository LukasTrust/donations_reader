using DonationsProject.Classes.Objects;
using DonationsProject.Classes.Utils;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace DonationsProject.ViewModel
{
    public class PieChartPartySummary_VM : INotifyPropertyChanged
    {
        #region Statics

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

        public const string LabelPretext = "Donation of ";

        #endregion

        #region Commands

        public ICommand ShowOtherPartyCommand { get; set; }
        public ICommand ShowYearBeforeCommand { get; set; }
        public ICommand ShowYearAfterCommand { get; set; }
        public ICommand ShowTotalSummaryCommand { get; set; }

        #endregion

        #region Properties

        public event PropertyChangedEventHandler PropertyChanged;

        public List<SeriesCollection> PieViews { get; set; }

        public SeriesCollection PieView { get; set; }

        public List<double> TotalAmounts { get; set; }

        public string TotalAmount { get; set; }

        public List<int> TotalDonations { get; set; }

        public int TotalDonation { get; set; }

        public List<string> LablesParty { get; set; }

        public string LableCurrent { get; set; }

        public int CurrentLableIndex { get; set; }

        public string YearShown { get; set; }

        public DateTime YearBefore { get; set; }
        public DateTime YearAfter { get; set; }

        #endregion

        #region Ctors

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

        #endregion

        #region Methods

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
            CurrentLableIndex = LablesParty.IndexOf(partName);
            PieView = PieViews[CurrentLableIndex];
            LableCurrent = LabelPretext + LablesParty[CurrentLableIndex] + YearShown;
            TotalAmount = TotalAmounts[CurrentLableIndex].ToString("N1", new CultureInfo("de-DE"));
            TotalDonation = TotalDonations[CurrentLableIndex];
            AllPropertyChanged();
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

        public async Task CreatePieCharts()
        {
            if (LablesParty == null)
            {
                LablesParty = new List<string>();
                TotalAmounts = new List<double>();
                TotalDonations = new List<int>();
            }
            else
            {
                LablesParty.Clear();
                TotalAmounts.Clear();
                PieViews.Clear();
                TotalDonations.Clear();
            }

            await PartySummary.CreateParties(Donation.Donations);
            foreach (PartySummary partySummary in PartySummary.PartiesSummary)
            {
                SeriesCollection seriesCollection = new SeriesCollection();
                LablesParty.Add(partySummary.PartyName);
                TotalAmounts.Add(partySummary.TotalAmount);
                TotalDonations.Add(partySummary.TotalDonations);
                foreach (KeyValuePair<string, double> amountPerDonor in partySummary.AmountPerDonor)
                {
                    PieSeries pieSeries = new PieSeries
                    {
                        Title = amountPerDonor.Key,
                        Values = new ChartValues<double> { amountPerDonor.Value },
                        DataLabels = true,
                        LabelPoint = point => string.Format("{0:N0} € ({1:P})", point.Y, point.Participation),
                        LabelPosition = PieLabelPosition.OutsideSlice,
                    };
                    seriesCollection.Add(pieSeries);
                }
                PieViews.Add(seriesCollection);
            }


            PieView = PieViews[CurrentLableIndex];
            LableCurrent = LabelPretext + LablesParty[CurrentLableIndex] + YearShown;
            TotalAmount = TotalAmounts[CurrentLableIndex].ToString("N1", new CultureInfo("de-DE"));
            TotalDonation = TotalDonations[CurrentLableIndex];
            AllPropertyChanged();
        }

        public void AllPropertyChanged()
        {
            OnPropertyChanged(nameof(LableCurrent));
            OnPropertyChanged(nameof(TotalAmount));
            OnPropertyChanged(nameof(PieView));
            OnPropertyChanged(nameof(TotalDonation));
            OnPropertyChanged(nameof(LablesParty));
            OnPropertyChanged(nameof(YearBefore));
            OnPropertyChanged(nameof(YearAfter));
            OnPropertyChanged(nameof(LablesParty));
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
