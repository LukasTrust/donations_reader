using DonationsProject.Classes.Objects;
using LiveCharts;
using LiveCharts.Definitions.Series;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        private static BarChartPartySummary_VM _Instance { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public List<string> YearLabels { get; set; }

        public List<ISeriesView> SeriesViews { get; set; }

        public BarChartPartySummary_VM()
        {
            SeriesViews = new List<ISeriesView>();
        }

        public async Task CreatBarCharts()
        {
            await PartySummary.CreateParties(Donation.Donations);
            foreach (PartySummary partySummary in PartySummary.PartiesSummary)
            {

                ColumnSeries columnSeries = new ColumnSeries
                {
                    Name = partySummary.PartyName,
                    Values = new ChartValues<double> { partySummary.TotalAmount },
                    DataLabels = true
                };
                SeriesViews.Add(columnSeries);
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
