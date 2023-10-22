using DonationsProject.Classes.Objects;
using DonationsProject.Classes.Utils;
using LiveCharts;
using LiveCharts.Definitions.Series;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DonationsProject.ViewModel.PartySummary_VMs
{
    public class BarChartPartySummary_VM : INotifyPropertyChanged
    {
        public static BarChartPartySummary_VM Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new BarChartPartySummary_VM();

                }
                return _Instance;
            }
        }

        public ICommand ShowYearBeforeCommand { get; set; }
        public ICommand ShowYearAfterCommand { get; set; }
        public ICommand ShowTotalSummaryCommand { get; set; }

        private static BarChartPartySummary_VM _Instance { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public List<string> YearLabels { get; set; }

        public ChartValues<double> BarView { get; set; }

        public List<string> LablesParty { get; set; }

        public string LableCurrent { get; set; }

        public string YearShown { get; set; }

        public DateTime YearBefore { get; set; }
        public DateTime YearAfter { get; set; }

        public BarChartPartySummary_VM()
        {
            BarView = new ChartValues<double>();

            ShowYearAfterCommand = new RelayCommand(ShowYearAfter);
            ShowYearBeforeCommand = new RelayCommand(ShowYearBefore);
            ShowTotalSummaryCommand = new RelayCommand(ShowTotalSummary);

            YearBefore = DateTime.Now.AddYears(-1);
            YearAfter = DateTime.Now.AddYears(1);
        }

        public async Task CreatBarCharts()
        {
            if (LablesParty == null)
            {
                LablesParty = new List<string>();
            }
            else
            {
                LablesParty.Clear();
                BarView.Clear();
            }
            await PartySummary.CreateParties(Donation.Donations);
            List<double> values = new List<double>();
            List<string> labels = new List<string>();
            foreach (PartySummary partySummary in PartySummary.PartiesSummary)
            {
                if (partySummary.TotalAmount != 0)
                {
                    BarView.Add(partySummary.TotalAmount);
                    labels.Add(partySummary.PartyName);
                }
            }

            LablesParty = labels;
        }

        public async void ShowYearBefore()
        {
            await BarChartPartySummary_VM.Instance.ShowYear(YearBefore);

            LableCurrent = "Donations of the year: " + YearBefore.Year.ToString();

            YearAfter = YearAfter.AddYears(-1);
            YearBefore = YearBefore.AddYears(-1);

            AllPropertyChanged();
        }

        public async void ShowYearAfter()
        {
            await BarChartPartySummary_VM.Instance.ShowYear(YearAfter);

            LableCurrent = "Donations of the year: " + YearAfter.Year.ToString();

            YearBefore = YearBefore.AddYears(1);
            YearAfter = YearAfter.AddYears(1);
            AllPropertyChanged();
        }

        public async void ShowTotalSummary()
        {
            await BarChartPartySummary_VM.Instance.ShowYear(new DateTime(0001, 1, 1));

            LableCurrent = "Total donation summary";

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
            await CreatBarCharts();
            AllPropertyChanged();
        }

        public void AllPropertyChanged()
        {
            OnPropertyChanged(nameof(LableCurrent));
            OnPropertyChanged(nameof(BarView));
            OnPropertyChanged(nameof(LablesParty));
            OnPropertyChanged(nameof(YearBefore));
            OnPropertyChanged(nameof(YearAfter));
            OnPropertyChanged(nameof(LablesParty));
        }


        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
