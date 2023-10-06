using DonationsProject.Classes.Database;
using DonationsProject.Classes.Objects;
using DonationsProject.Classes.Utils;
using DonationsProject.Classes.Website;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DonationsProject.ViewModel
{
    internal class MainWindow_VM : INotifyPropertyChanged
    {
        public static MainWindow_VM Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new MainWindow_VM();

                }
                return _Instance;
            }
        }

        private static MainWindow_VM _Instance { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand GetDataFromWebsiteCommand { get; set; }
        public ICommand LoadDataFromDBCommand { get; set; }

        public MainWindow_VM()
        {
            GetDataFromWebsiteCommand = new RelayCommand(GetDataFromWebsite);
            LoadDataFromDBCommand = new RelayCommand(LoadDataFromDB);
        }

        public async void GetDataFromWebsite()
        {
            await WebsiteCrawler.Instance.ConnectToWebsite();
            await DBConnector.Instance.InsertDonations(Donation.Donations);
        }

        public async void LoadDataFromDB()
        {
            await DBConnector.Instance.ReadDataFromDB();
        }
    }
}
