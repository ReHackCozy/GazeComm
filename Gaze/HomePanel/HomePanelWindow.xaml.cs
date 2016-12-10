﻿using System;
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
using EyeXFramework.Wpf;
using Gaze.EyeTracker;
using Gaze.API;

namespace Gaze.HomePanel
{
    /// <summary>
    /// Interaction logic for HomePanel.xaml
    /// </summary>
    public partial class HomePanelWindow : Window
    {
        HomePanelViewModel vm;
        public WpfEyeXHost eyeXHostRef;

        public HomePanelWindow()
        {
            InitializeComponent();
            vm = new HomePanelViewModel();

            this.DataContext = vm;

            var currentApp = Application.Current as App;
            eyeXHostRef = currentApp.eyeXHost;

            if (eyeXHostRef == null)
                Console.WriteLine("EyeX is NULL @ HomePanelWindow");
        }

        private void SuggestionBox_Initialized(object sender, EventArgs e)
        {

        }

        private void OnEyeXActivate(object sender, RoutedEventArgs e)
        {
            var element = e.Source as FrameworkElement;
            if (null == element) { return; }

            var gazableButton = element.DataContext as GazableButton;

            if(gazableButton.type == GazableButton.Type.Suggestion)
                addWordToSendMessageTextFromButton(gazableButton.value);
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift)
            {
                var currentApp = Application.Current as App;
                if (currentApp != null)
                    eyeXHostRef.TriggerActivation();
            }
        }

        private void OnSendSMS(object sender, RoutedEventArgs e)
        {
            Utilities.Util.Speak(vm.MessageToSend, System.Speech.Synthesis.VoiceGender.Female);
            new SendMessage().Invoke(vm.MessageToSend, vm.PhoneNumber);

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
                sugg.type = GazableButton.Type.Suggestion;

                //HACK, couldve done in XAML
                sugg.Click += (o,s) => 
                {
                    if (sugg.type == GazableButton.Type.Suggestion)
                        addWordToSendMessageTextFromButton(sugg.value);
                } ;

                vm.SuggestionsList.Add(sugg);
            }
        }

        private void addWordToSendMessageTextFromButton(string text)
        {

            vm.MessageToSend += text;
            vm.MessageToSend += " ";
            autocompleteInput.Focus();
            autocompleteInput.CaretIndex = autocompleteInput.Text.Length;

        }

        private void PlayText_Activate(object sender, RoutedEventArgs e)
        {
            Utilities.Util.Speak(vm.MessageToSend, System.Speech.Synthesis.VoiceGender.Female);
        }

        private void OnVirtualKeyboardPressed(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            vm.MessageToSend += button.Content;

        }

        private void autocompleteInput_Initialized(object sender, EventArgs e)
        {
            autocompleteInput.Focus();
        }

        private void LetterButton_HasGazeChanged(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
        }

        private void SuggestionButton_HasGazeChanged(object sender, RoutedEventArgs e)
        {
            var element = e.Source as FrameworkElement;
            if (null == element) { return; }

            var gazableButton = element.DataContext as GazableButton;


        }
    }
}
