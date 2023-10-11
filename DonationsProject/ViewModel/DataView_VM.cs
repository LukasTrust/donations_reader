using DonationsProject.Classes.Utils;
using DonationsProject.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DonationsProject.ViewModel
{
    public class DataView_VM : INotifyPropertyChanged
    {
        public static DataView_VM Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new DataView_VM();

                }
                return _Instance;
            }
        }

        private static DataView_VM _Instance { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand ShowYearBeforeCommand { get; set; }
        public ICommand ShowYearAfterCommand { get; set; }
        public ICommand ShowTotalSummaryCommand { get; set; }
        public ICommand ShowPieChartCommand { get; set; }
        public ICommand ShowLineChartCommand { get; set; }
        public ICommand ShowBarChartCommand { get; set; }

        private List<object> ViewModels = new List<object>();

        private object _currentViewModel;
        public object CurrentViewModel
        {
            get { return _currentViewModel; }
            set
            {
                _currentViewModel = value;
                OnPropertyChanged(nameof(CurrentViewModel));
            }
        }

        public DateTime YearBefore { get; set; }
        public DateTime YearAfter { get; set; }

        public void PropertyDateChanged()
        {
            OnPropertyChanged(nameof(YearBefore));
            OnPropertyChanged(nameof(YearAfter));
        }

        public DataView_VM()
        {
            ShowYearAfterCommand = new RelayCommand(ShowYearAfter);
            ShowYearBeforeCommand = new RelayCommand(ShowYearBefore);
            ShowTotalSummaryCommand = new RelayCommand(ShowTotalSummary);

            ShowPieChartCommand = new RelayCommand(ShowPieChart);
            ShowLineChartCommand = new RelayCommand(ShowLineChart);
            ShowBarChartCommand = new RelayCommand(ShowBarChart);
            YearBefore = DateTime.Now.AddYears(-1);
            YearAfter = DateTime.Now.AddYears(1);
        }

        public async void ShowYearBefore()
        {
            if (CurrentViewModel is PartySummary_UC)
            {
                await PartySummary_VM.Instance.ShowYear(YearBefore);
            }
            YearAfter = YearAfter.AddYears(-1);
            YearBefore = YearBefore.AddYears(-1);

            PropertyDateChanged();
        }

        public async void ShowYearAfter()
        {
            if (CurrentViewModel is PartySummary_UC)
            {
                await PartySummary_VM.Instance.ShowYear(YearAfter);
            }
            YearBefore = YearBefore.AddYears(1);
            YearAfter = YearAfter.AddYears(1);
            PropertyDateChanged();
        }

        public async void ShowTotalSummary()
        {
            if (CurrentViewModel is PartySummary_UC)
            {
                await PartySummary_VM.Instance.ShowYear(new DateTime(0001, 1, 1));
            }
            PropertyDateChanged();
        }

        public async void ShowPieChart()
        {
            
        }

        public async void ShowLineChart()
        {

        }

        public async void ShowBarChart()
        {

        }

        public async Task ShowPartyView()
        {
            foreach (var item in ViewModels)
            {
                if (item is PartySummary_UC)
                {
                    CurrentViewModel = item;
                    return;
                }
            }
            CurrentViewModel = new PartySummary_UC();
            await PartySummary_VM.Instance.ShowYear(DateTime.Now);
            ViewModels.Add(CurrentViewModel);
        }
    }
}
