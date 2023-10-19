using DonationsProject.Classes.Utils;
using DonationsProject.View;
using DonationsProject.View.PartySummary;
using DonationsProject.ViewModel.PartySummary_VMs;
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

        private LineChartPartySummary_UC _LineChartPartySummary;

        public LineChartPartySummary_UC LineChartPartySummary
        {
            get { return _LineChartPartySummary; }
            set
            {
                _LineChartPartySummary = value;
                OnPropertyChanged(nameof(LineChartPartySummary));
            }
        }

        private PieChartPartySummary_UC _PieChartPartySummary;
        public PieChartPartySummary_UC PieChartPartySummary
        {
            get { return _PieChartPartySummary; }
            set
            {
                _PieChartPartySummary = value;
                OnPropertyChanged(nameof(PieChartPartySummary));
            }
        }

        private BarChartPartySummary_UC _BarChartPartySummary;
        public BarChartPartySummary_UC BarChartPartySummary
        {
            get { return _BarChartPartySummary; }
            set
            {
                _BarChartPartySummary = value;
                OnPropertyChanged(nameof(BarChartPartySummary));
            }
        }

        public DataView_VM()
        {
            
        }

        public async Task ShowPartyView()
        {
            if (PieChartPartySummary == null)
            {
                PieChartPartySummary = new PieChartPartySummary_UC();
            }
            if (LineChartPartySummary == null)
            {
                LineChartPartySummary = new LineChartPartySummary_UC();
            }
            if (BarChartPartySummary == null)
            {
                BarChartPartySummary = new BarChartPartySummary_UC();
            }

            await PieChartPartySummary_VM.Instance.ShowYear(new DateTime(0001, 1, 1));

            await LineChartPartySummary_VM.Instance.CreateLineCharts();

            await BarChartPartySummary_VM.Instance.CreatBarCharts();
        }
    }
}
