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
using System.Windows.Shapes;

namespace Gaze.HomePanel
{
    /// <summary>
    /// Interaction logic for HomePanel.xaml
    /// </summary>
    public partial class HomePanelWindow : Window
    {
        HomePanelViewModel vm;

        public HomePanelWindow()
        {
            InitializeComponent();
            vm = new HomePanelViewModel();

            this.DataContext = vm;

        }
    }
}
