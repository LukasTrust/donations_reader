using DonationsProject.ViewModel.PartySummary_VMs;
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

namespace DonationsProject.View.PartySummary
{
    /// <summary>
    /// Interaction logic for BarChartPartySummary_UC.xaml
    /// </summary>
    public partial class BarChartPartySummary_UC : UserControl
    {
        public BarChartPartySummary_UC()
        {
            InitializeComponent();
            DataContext = BarChartPartySummary_VM.Instance;
            Func<double, string> formatFunc = (x) => string.Format("{0:N}", x);
            YAxis.LabelFormatter = formatFunc;
        }
    }
}
