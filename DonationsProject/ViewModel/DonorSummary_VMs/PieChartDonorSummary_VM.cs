using DonationsProject.Classes.Objects;
using LiveCharts.Wpf;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DonationsProject.Classes.Utils;
using System.Windows.Input;

namespace DonationsProject.ViewModel.DonorSummary_VMs
{
    public class PieChartDonorSummary_VM : INotifyPropertyChanged
    {
        public const string LabelPretext = "Donation of ";

        public static PieChartDonorSummary_VM Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new PieChartDonorSummary_VM();

                }
                return _Instance;
            }
        }

        private static PieChartDonorSummary_VM _Instance { get; set; }

        #region Commands

        public ICommand ShowOtherPartyCommand { get; set; }
        public ICommand ShowYearBeforeCommand { get; set; }
        public ICommand ShowYearAfterCommand { get; set; }
        public ICommand ShowTotalSummaryCommand { get; set; }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        public string YearShown { get; set; }
        public string TotalAmount { get; set; }
        public List<double> TotalAmounts { get; set; }
        public int TotalDonation { get; set; }
        public List<int> TotalDonations { get; set; }
        public SeriesCollection PieView { get; set; }
        public List<SeriesCollection> PieViews { get; set; }
        public int CurrentLableIndex { get; set; }
        public string LableCurrent { get; set; }
        public List<string> LablesDonor { get; set; }
        public DateTime YearBefore { get; set; }
        public DateTime YearAfter { get; set; }

        public PieChartDonorSummary_VM()
        {
            PieViews = new List<SeriesCollection>();
            ShowOtherPartyCommand = new RelayCommandWithParamter<string>(ShowOtherParty);
            ShowYearBeforeCommand = new RelayCommand(ShowYearBefore);
            ShowYearAfterCommand = new RelayCommand(ShowYearAfter);
            ShowTotalSummaryCommand = new RelayCommand(ShowTotalSummary);

            YearBefore = DateTime.Now.AddYears(-1);
            YearAfter = DateTime.Now.AddYears(1);
        }

        public async void ShowYearBefore()
        {
            await PieChartDonorSummary_VM.Instance.ShowYear(YearBefore);

            YearAfter = YearAfter.AddYears(-1);
            YearBefore = YearBefore.AddYears(-1);

            AllPropertyChanged();
        }

        public async void ShowYearAfter()
        {
            await PieChartDonorSummary_VM.Instance.ShowYear(YearAfter);

            YearBefore = YearBefore.AddYears(1);
            YearAfter = YearAfter.AddYears(1);
            AllPropertyChanged();
        }

        public async void ShowTotalSummary()
        {
            await PieChartDonorSummary_VM.Instance.ShowYear(new DateTime(0001, 1, 1));

            AllPropertyChanged();
        }

        public void ShowOtherParty(string partName)
        {
            CurrentLableIndex = LablesDonor.IndexOf(partName);
            PieView = PieViews[CurrentLableIndex];
            LableCurrent = LabelPretext + LablesDonor[CurrentLableIndex] + YearShown;
            TotalAmount = TotalAmounts[CurrentLableIndex].ToString("N1", new CultureInfo("de-DE"));
            TotalDonation = TotalDonations[CurrentLableIndex];
            AllPropertyChanged();
        }

        public async Task ShowYear(DateTime yearToShow)
        {
            DonorSummary.YearToShow = yearToShow;
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
            if (LablesDonor == null)
            {
                LablesDonor = new List<string>();
                TotalAmounts = new List<double>();
                TotalDonations = new List<int>();
            }
            else
            {
                LablesDonor.Clear();
                TotalAmounts.Clear();
                PieViews.Clear();
                TotalDonations.Clear();
            }

            await DonorSummary.CreateDonors(Donation.Donations);
            foreach (DonorSummary donorSummary in DonorSummary.DonorsSummary)
            {
                SeriesCollection seriesCollection = new SeriesCollection();
                if (donorSummary.AmountPerParty.Count > 2)
                {
                    LablesDonor.Add(donorSummary.DonorName);
                    TotalAmounts.Add(donorSummary.TotalAmount);
                    TotalDonations.Add(donorSummary.TotalDonations);
                    foreach (KeyValuePair<string, double> amountPerDonor in donorSummary.AmountPerParty)
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
            }

            if (PieViews.Count > 0)
            {
                PieView = PieViews[CurrentLableIndex];
                LableCurrent = LabelPretext + LablesDonor[CurrentLableIndex] + YearShown;
                TotalAmount = TotalAmounts[CurrentLableIndex].ToString("N1", new CultureInfo("de-DE"));
                TotalDonation = TotalDonations[CurrentLableIndex];
            }
            else
            {
                PieView = null;
                LableCurrent = "No donations in this year";
                TotalAmount = "0";
                TotalDonation = 0;
            }
            AllPropertyChanged();
        }

        public void AllPropertyChanged()
        {
            OnPropertyChanged(nameof(LableCurrent));
            OnPropertyChanged(nameof(TotalAmount));
            OnPropertyChanged(nameof(PieView));
            OnPropertyChanged(nameof(TotalDonation));
            OnPropertyChanged(nameof(LablesDonor));
            OnPropertyChanged(nameof(YearBefore));
            OnPropertyChanged(nameof(YearAfter));
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
