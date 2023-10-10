using DonationsProject.Classes.Database;
using DonationsProject.Classes.Objects;
using DonationsProject.Classes.Utils;
using DonationsProject.Classes.Website;
using DonationsProject.View;
using OpenQA.Selenium.DevTools.V115.Browser;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DonationsProject.ViewModel
{
    public class MainWindow_VM : INotifyPropertyChanged
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
        public ICommand ShowPartyViewCommand { get; set; }
        public ICommand ShowDonorViewCommand { get; set; }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

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

        public MainWindow_VM()
        {
            GetDataFromWebsiteCommand = new RelayCommand(GetDataFromWebsite);
            LoadDataFromDBCommand = new RelayCommand(LoadDataFromDB);
            ShowPartyViewCommand = new RelayCommand(ShowPartyView);
            ShowDonorViewCommand = new RelayCommand(ShowDonorView);
            CurrentViewModel = new StartView_UC();
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

        public async void ShowPartyView()
        {
            if (Donation.Donations.Count == 0)
            {
                await DBConnector.Instance.ReadDataFromDB();
                await DonorSummary.CreateDonors(Donation.Donations);
                await PartySummary.CreateParties(Donation.Donations);
            }
            foreach (object vm in ViewModels)
            {
                if (vm is PartySummary_UC)
                {
                    CurrentViewModel = vm;
                    return;
                }
            }
            CurrentViewModel = new DataView_UC();
            await DataView_VM.Instance.ShowPartyView();
            ViewModels.Add(CurrentViewModel);
        }

        public async void ShowDonorView()
        {
            if (Donation.Donations.Count == 0)
            {
                await DBConnector.Instance.ReadDataFromDB();
                await DonorSummary.CreateDonors(Donation.Donations);
                await PartySummary.CreateParties(Donation.Donations);
            }
            
            CurrentViewModel = new DataView_UC();
        }
    }
}
