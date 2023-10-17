using LiveCharts.Wpf;
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
        public List<int> ElectionYears = new List<int>() { 2002, 2005, 2009, 2013, 2017, 2021 };

        public LineChartPartySummary_UC()
        {
            InitializeComponent();
            DataContext = ViewModel.LineChartPartySummary_VM.Instance;
            Func<double, string> formatFunc = (x) => string.Format("{0:N}", x);
            YAxis.LabelFormatter = formatFunc;
        }

        private void XAxis_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            if (XAxis == null ||XAxis.Labels == null)
                return;
            List<string> lables = XAxis.Labels.ToList();
            for (int i = 0; i < lables.Count; i++)
            {
                if (ElectionYears.Contains(int.Parse(lables[i])))
                {
                    AxisSection section = new AxisSection
                    {
                        Value = i,
                        Stroke = Brushes.Red,
                        StrokeThickness = 2,
                    };
                    XAxis.Sections.Add(section);
                }
            }
            UpdateLayout();
        }
    }
}
