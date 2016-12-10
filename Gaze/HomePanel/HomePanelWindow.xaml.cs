using System;
using System.Collections;
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

        private void SuggestionBox_Initialized(object sender, EventArgs e)
        {

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var sugg = new GazableButton();
            sugg.Height = 150;
            sugg.Width = 150;
            sugg.Content = "Suggestion";
            sugg.value = "Suggestion";

            vm.SuggestionsList.Add(sugg);
        }

        private void autocompleteInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            vm.SuggestionsList.Clear();
            ArrayList mathingValues = autocompleteInput.getMatchingValues();
            List<string> withDupes = mathingValues.OfType<string>().ToList();
            List<string> noDupes = withDupes.Distinct().ToList();
            IEnumerator enumerate = noDupes.GetEnumerator();

            while (enumerate.MoveNext())
            {
                var sugg = new GazableButton();
                sugg.Height = 150;
                sugg.Width = 150;
                sugg.Content = (string)enumerate.Current;
                sugg.value = (string)enumerate.Current;

                vm.SuggestionsList.Add(sugg);
            }
                
        }
    }
}
