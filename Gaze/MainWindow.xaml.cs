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
using Gaze.ViewModelMain;
using Gaze.HomePanel;
using EyeXFramework.Wpf;

namespace Gaze
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainView mainVM;
        public WpfEyeXHost eyeXHostRef;


        TextBlock DebugOutput;

        public MainWindow()
        {
            InitializeComponent();

            mainVM = new MainView();
            this.DataContext = mainVM;

            var currentApp = Application.Current as App;
            eyeXHostRef = currentApp.eyeXHost;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            HomePanel.HomePanelWindow homePanel = new HomePanelWindow();
            homePanel.Show();

        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.RightShift)
            {
                eyeXHostRef.TriggerActivation();

            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.RightShift)
            {
                eyeXHostRef.TriggerActivationModeOn(); 
            }

           
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {

            

            


        }

        private void DebugTextBog_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textbox = sender as TextBox;
            DebugOutput.Text = textbox.Text;
        }

        private void DebugOutPut_Initialized(object sender, EventArgs e)
        {
            DebugOutput = sender as TextBlock;
        }
    }
}
