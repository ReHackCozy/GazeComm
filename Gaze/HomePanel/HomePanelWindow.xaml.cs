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
using EyeXFramework.Wpf;
using Gaze.EyeTracker;
using Gaze.API;
using System.Diagnostics;
using System.Windows.Automation.Peers;

namespace Gaze.HomePanel
{
    /// <summary>
    /// Interaction logic for HomePanel.xaml
    /// </summary>
    public partial class HomePanelWindow : Window
    {
        public  static string HighlightColor = "#FFE59400";
        public static string OriginalColor = "#FF373737";

        HomePanelViewModel vm;
        public WpfEyeXHost eyeXHostRef;
        Stopwatch stopWatch;

        System.Windows.Threading.DispatcherTimer activationCoolDown = new System.Windows.Threading.DispatcherTimer();
        //bool activationReady = true;

        System.Windows.Threading.DispatcherTimer statusTimer = new System.Windows.Threading.DispatcherTimer();

        System.Windows.Threading.DispatcherTimer blinkTimer = new System.Windows.Threading.DispatcherTimer();
        bool blinkTimerStarted = false;
        //HACK
        double fixationBeginTimeStamp = 0;
        double fixationActivateDuration = 500; //In milisecond
        bool fixationStart = false;

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        public HomePanelWindow()
        {
            InitializeComponent();
            vm = new HomePanelViewModel();

            this.DataContext = vm;

            var currentApp = Application.Current as App;
            eyeXHostRef = currentApp.eyeXHost;

            stopWatch = new Stopwatch();

            if (eyeXHostRef == null)
                Console.WriteLine("EyeX is NULL @ HomePanelWindow");


            var eyePositionStream = eyeXHostRef.CreateEyePositionDataStream();
            eyePositionStream.Next += (s,e) => 
            {

                //if (!activationReady)
                //    return;

                //    if (!e.LeftEye.IsValid && !e.RightEye.IsValid)
                //{
                //    if (!blinkTimerStarted)
                //        _startBlinkTimer();
                    
                //    //Try make it work
                //    //vm.IsBlinked = true; //setting this to True will call OnGazeActivateButton() but not false;

                //}
                //else
                //{
                //    if (blinkTimerStarted)
                //        _stopBlinkTimer();
                //}

            };

            var fixationStream = eyeXHostRef.CreateFixationDataStream(Tobii.EyeX.Framework.FixationDataMode.Sensitive);
            fixationStream.Next += OnFixatedGaze;

        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void SuggestionBox_Initialized(object sender, EventArgs e)
        {

        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnEyeXActivateSuggestion(object sender, RoutedEventArgs e)
        {
            var element = e.Source as FrameworkElement;
            if (null == element) { return; }

            var gazableButton = element.DataContext as Button;

            addWordToSendMessageTextFromButton(gazableButton.Content.ToString());
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift)
            {
                var currentApp = Application.Current as App;
                if (currentApp != null)
                    eyeXHostRef.TriggerActivation();
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnSendSMS(object sender, RoutedEventArgs e)
        {
            Utilities.Util.Speak(vm.MessageToSend, System.Speech.Synthesis.VoiceGender.Female);
            new SendMessage().Invoke(vm.MessageToSend, vm.PhoneNumber);
            Status.Text = "SMS sent";
            _startStatusTimer();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void autocompleteInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            vm.SuggestionsList.Clear();
            ArrayList mathingValues = autocompleteInput.getMatchingValues();
            List<string> withDupes = mathingValues.OfType<string>().ToList();
            List<string> noDupes = withDupes.Distinct().ToList();
            IEnumerator enumerate = noDupes.GetEnumerator();

            while (enumerate.MoveNext())
            {
                var sugg = new Button();
                sugg.Height = 150;
                sugg.Width = 150;
                sugg.Content = (string)enumerate.Current;
                sugg.FontSize = 50;
                sugg.BorderThickness = new Thickness(0);

                var st = this.TryFindResource("SuggestionButtonStyle") as Style;

                if(st != null)
                    sugg.Style = this.TryFindResource("SuggestionButtonStyle") as Style;

                //HACK, couldve done in XAML
                sugg.Click += (o,s) => 
                {
                        addWordToSendMessageTextFromButton((string)enumerate.Current);
                } ;

                vm.SuggestionsList.Add(sugg);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void addWordToSendMessageTextFromButton(string text)
        {
            var tmp_msg_list = vm.MessageToSend.Split(' ').ToList();
            tmp_msg_list.Remove(tmp_msg_list.Last());

            vm.MessageToSend = "";

            foreach (var str in tmp_msg_list)
            {
                vm.MessageToSend += str;
                vm.MessageToSend += " ";
            }
            
            vm.MessageToSend += text;
            vm.MessageToSend += " ";
            autocompleteInput.Focus();
            autocompleteInput.CaretIndex = autocompleteInput.Text.Length;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void PlayText_Activate(object sender, RoutedEventArgs e)
        {
            if (vm.MessageToSend.Length == 0)
                return;

            vm.PlayTTS();
            Status.Text = "Text played";
            _startStatusTimer();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnVirtualKeyboardPressed(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            vm.MessageToSend += button.Content;

            autocompleteInput.Focus();
            autocompleteInput.CaretIndex = autocompleteInput.Text.Length;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void autocompleteInput_Initialized(object sender, EventArgs e)
        {
            autocompleteInput.Focus();
            autocompleteInput.CaretIndex = autocompleteInput.Text.Length;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void KeyboardButton_HasGazeChanged(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            var bc = new BrushConverter();

            //[AH] Temporary solution
            if(button.Background.ToString() == HighlightColor)
                button.Background = bc.ConvertFrom(OriginalColor) as Brush;
            else
                button.Background = bc.ConvertFrom(HighlightColor) as Brush;
            
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void SuggestionButton_HasGazeChanged(object sender, RoutedEventArgs e)
        {
            var element = e.Source as FrameworkElement;
            if (null == element) { return; }

            var gazableButton = element.DataContext as Button;


        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void VK_BKSpace_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if(vm.MessageToSend.Length > 0)
                vm.MessageToSend = vm.MessageToSend.Remove(vm.MessageToSend.Length - 1);

            autocompleteInput.Focus();
            autocompleteInput.CaretIndex = autocompleteInput.Text.Length;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void VK_BKSpace_HasGazeChanged(object sender, RoutedEventArgs e)
        {

        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void SendSMS_HasGazeChanged(object sender, RoutedEventArgs e)
        {

        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void PlayText_HasGazeChanged(object sender, RoutedEventArgs e)
        {

        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void GazableButton_Activate(object sender, RoutedEventArgs e)
        {
            var rad_btn = sender as RadioButton;

            if(rad_btn == null)
                vm.MessageToSend += " ";

            autocompleteInput.Focus();
            autocompleteInput.CaretIndex = autocompleteInput.Text.Length;

           

            if(rad_btn != null)
                rad_btn.IsChecked = true;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void GazableButton_HasGazeChanged(object sender, RoutedEventArgs e)
        {

        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void OnGazeActivateButton()
        {
   
            eyeXHostRef.TriggerActivation();

            //_startActivationCooldown();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnFixatedGaze(object sender, EyeXFramework.FixationEventArgs e)
        {

            //if (!activationReady)
            //    return;

            if (e.EventType == Tobii.EyeX.Framework.FixationDataEventType.Begin)
            {
                fixationStart = true;
                fixationBeginTimeStamp = e.Timestamp;
            }
                

            if (e.EventType == Tobii.EyeX.Framework.FixationDataEventType.Data)
            {
                if ((e.Timestamp - fixationBeginTimeStamp) > fixationActivateDuration)
                {
                    fixationBeginTimeStamp = 0;

                    if(fixationStart)
                    {
                        OnGazeActivateButton();
                        fixationStart = false;
                    }
                        
                }
                    
            }
    

            if (e.EventType == Tobii.EyeX.Framework.FixationDataEventType.End)
            {
                fixationStart = false;
                fixationBeginTimeStamp = 0;
            }

        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var currentApp = Application.Current as App;
            if (currentApp != null)
                eyeXHostRef.TriggerActivation();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void SendTTS_Activate(object sender, RoutedEventArgs e)
        {
            vm.SendTTS();
            vm.PlayTTS();
            Status.Text = "TTS sent";
            _startStatusTimer();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Clr_Activate(object sender, RoutedEventArgs e)
        {
            vm.MessageToSend = "";
            autocompleteInput.Focus();
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            vm.UpdateKeyboard();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void _startStatusTimer()
        {
            statusTimer.Tick += new EventHandler(statusTimer_Tick);
            statusTimer.Interval = new TimeSpan(0, 0, 0, 5);
            statusTimer.Start();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void statusTimer_Tick(object sender, EventArgs e)
        {
            Status.Text = "";
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        //private void _startBlinkTimer()
        //{
        //    blinkTimerStarted = true;
        //    blinkTimer.Tick += new EventHandler(blinkTimer_Tick);
        //    blinkTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
        //    blinkTimer.Start();
        //}

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //private void _stopBlinkTimer()
        //{
        //    blinkTimer.Stop();
        //    blinkTimerStarted = false;
        //}

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //private void blinkTimer_Tick(object sender, EventArgs e)
        //{
        //    blinkTimerStarted = false;
        //    if(activationReady)
        //        OnGazeActivateButton();
        //    //Really close long enough, unsure which action to trigger
        //}

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //private void _startActivationCooldown()
        //{
        //    activationReady = false;
        //    activationCoolDown.Tick += new EventHandler(_activationCooldownTick);
        //    activationCoolDown.Interval = new TimeSpan(0, 0, 0, 0, 1000);
        //    activationCoolDown.Start();
        //}

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //private void _activationCooldownTick(object sender, EventArgs e)
        //{
        //    activationReady = true;
        //}

    }
}
