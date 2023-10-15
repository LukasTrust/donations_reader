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
    /// Interaction logic for LineChartPartySummary_UC.xaml
    /// </summary>
    public partial class LineChartPartySummary_UC : UserControl
    {
        public LineChartPartySummary_UC()
        {
            InitializeComponent();
            DataContext = ViewModel.LineChartPartySummary_VM.Instance;
        }
    }
}
