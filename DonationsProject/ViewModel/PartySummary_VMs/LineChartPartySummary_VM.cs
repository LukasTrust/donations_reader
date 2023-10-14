using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DonationsProject.ViewModel
{
    public class LineChartPartySummary_VM
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

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public const string LabelPretext = "Donation of ";

        public ICommand ShowOtherPartyCommand { get; set; }
    }
}
