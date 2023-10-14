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
using DonationsProject.ViewModel;

namespace DonationsProject.View
{
    /// <summary>
    /// Interaction logic for PartySummary_UC.xaml
    /// </summary>
    public partial class PieChartPartySummary_UC : UserControl
    {
        public PieChartPartySummary_UC()
        {
            InitializeComponent();
            DataContext = PieChartPartySummary_VM.Instance;
            UpdateLayout();
        }


    }
}
