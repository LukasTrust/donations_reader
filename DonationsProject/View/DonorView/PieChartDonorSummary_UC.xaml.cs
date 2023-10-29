using DonationsProject.ViewModel;
using DonationsProject.ViewModel.DonorSummary_VMs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DonationsProject.View.DonorView
{
    /// <summary>
    /// Interaction logic for PieChartDonorSummary_UC.xaml
    /// </summary>
    public partial class PieChartDonorSummary_UC : UserControl
    {
        public PieChartDonorSummary_UC()
        {
            InitializeComponent();
            DataContext = PieChartDonorSummary_VM.Instance;
            UpdateLayout();
        }
    }
}
