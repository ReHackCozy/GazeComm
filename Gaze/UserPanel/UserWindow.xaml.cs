using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace Gaze.UserPanel
    {
    /// <summary>
    /// Interaction logic for UserWindow.xaml
    /// </summary>
    public partial class UserWindow : Window
        {
        private UserPanelViewModel vm;
        private String configurationPath = @"C:\Program Files (x86)\Tobii\Tobii EyeX Config\Tobii.EyeX.Configuration.exe";

        public UserWindow()
            {
            InitializeComponent();
            vm = new UserPanelViewModel();

            this.DataContext = vm;
            }

        private void buttonCalibrate_Click(object sender, RoutedEventArgs e)
            {
            using (Process calibrate = Process.Start(configurationPath))
                {
                calibrate.WaitForExit();
                }

            vm.onUserPanelClosed();
            this.Close();
            }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
            {
            this.Close();
            }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
            {
            vm.onUserPanelClosed();
            this.Close();
            }
        }
    }
