using DonationsProject.Classes.Objects;
using DonationsProject.Classes.Utils;
using DonationsProject.View.PartySummary;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DonationsProject.ViewModel
{
    public class LineChartPartySummary_VM : INotifyPropertyChanged
    {
        public static LineChartPartySummary_VM Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new LineChartPartySummary_VM();

                }
                return _Instance;
            }
        }
        private static LineChartPartySummary_VM _Instance { get; set; }

        public LineChartPartySummary_VM()
        {
            ShowOtherPartyCommand = new RelayCommandWithParamter<string>(ShowOtherParty);
            YearLabels = new List<string>();
            LineViews = new List<ChartValues<double>>();
            LablesParty = new List<string>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand ShowOtherPartyCommand { get; set; }

        public List<string> YearLabels { get; set; }

        public List<ChartValues<double>> LineViews { get; set; }

        public ChartValues<double> LineView { get; set; }

        public List<string> LablesParty { get; set; }

        public string LableCurrent { get; set; }

        public int CurrentLableIndex { get; set; }

        public async Task CreateLineCharts()
        {
            await PartySummary.CreateParties(Donation.Donations);
            foreach (PartySummary partySummary in PartySummary.PartiesSummary)
            {
                LablesParty.Add(partySummary.PartyName);
                var groupedData = partySummary.Donations
                    .GroupBy(d => d.DonationDate.Year)
                    .OrderBy(group => group.Key);

                double[] amountPerYear = groupedData
                    .Select(group => group.Sum(don => don.Amount))
                    .ToArray();

                YearLabels.AddRange(groupedData
                    .Select(group => group.Key.ToString()));

                LineViews.Add(new ChartValues<double>(amountPerYear));
            }
            LineView = LineViews[CurrentLableIndex];
            LableCurrent = LablesParty[CurrentLableIndex];
            AllPropertyChanged();
        }

        public async void ShowOtherParty(string partName)
        {
            CurrentLableIndex = LablesParty.IndexOf(partName);
            await CreateLineCharts();
            AllPropertyChanged();
        }

        public void AllPropertyChanged()
        {
            OnPropertyChanged(nameof(LineView));
            OnPropertyChanged(nameof(YearLabels));
            OnPropertyChanged(nameof(LableCurrent));
            OnPropertyChanged(nameof(LablesParty));
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
