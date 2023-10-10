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

namespace DonationsProject.View
{
    /// <summary>
    /// Interaction logic for DataView_UC.xaml
    /// </summary>
    public partial class DataView_UC : UserControl
    {
        public DataView_UC()
        {
            InitializeComponent();
            DataContext = ViewModel.DataView_VM.Instance;
        }
    }
}
