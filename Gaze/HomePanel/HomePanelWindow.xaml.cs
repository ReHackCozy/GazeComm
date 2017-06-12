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
using System.Collections.Specialized;
using System.Media;
using System.Windows.Media.Animation;
using Gaze.Control;
using Gaze.Data;
using Gaze.UserPanel;
using System.Windows.Controls.Primitives;

namespace Gaze.HomePanel
{
    /// <summary>
    /// Interaction logic for HomePanel.xaml
    /// </summary>
    public partial class HomePanelWindow : Window
    {
        bool IsKeyboardGazable = true;

        //Others
        public static int SuggestionButtonHeight = 100;
        public static int SuggestionButtonWidth = 150;

        public static double SuggestionBoxHeight = 120;

        bool hasMiscPanelDownGazed = false;
        bool hasMiscPanelUpGazed = false;

        HomePanelViewModel vm;
        public WpfEyeXHost eyeXHostRef;

        System.Windows.Threading.DispatcherTimer statusTimer = new System.Windows.Threading.DispatcherTimer();

        //HACK
        double fixationBeginTimeStamp = 0;
        double fixationActivateDuration = 400; //In milisecond
        bool fixationStart = false;

        //Blink
        private bool EyesBlinked = false;
        System.Windows.Threading.DispatcherTimer EyeBlinkCooldownTimer = new System.Windows.Threading.DispatcherTimer();

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        public HomePanelWindow()
        {
            InitializeComponent();
            vm = new HomePanelViewModel();

            this.DataContext = vm;

            var currentApp = Application.Current as App;
            eyeXHostRef = currentApp.eyeXHost;

            if (eyeXHostRef == null)
                Console.WriteLine("EyeX is NULL @ HomePanelWindow");

            #region BlinkTracker

            EyeBlinkCooldownTimer.Tick += (s,e) =>
            {
                EyeBlinkCooldownTimer.Stop();
                EyesBlinked = false;
            };

            var userPresence = eyeXHostRef.UserPresence;
            EyeBlinkCooldownTimer.Interval = new TimeSpan(0, 0, 0, 1);
            var eyePositionStream = eyeXHostRef.CreateEyePositionDataStream();

            eyePositionStream.Next += (s,e) => 
            {
                if (vm.IsBlinkEyesGazeActivate)
                {
                    //If both eyes blinked
                    if (userPresence.IsValid && !e.LeftEye.IsValid && !e.RightEye.IsValid && !EyesBlinked)
                    {
                        EyesBlinked = true;

                        EyeBlinkCooldownTimer.Start();

                        eyeXHostRef.TriggerActivation();
                    }
                }
            };

            #endregion

            var fixationStream = eyeXHostRef.CreateFixationDataStream(Tobii.EyeX.Framework.FixationDataMode.Sensitive);
            fixationStream.Next += OnFixatedGaze;
            }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void SuggestionBox_Initialized(object sender, EventArgs e)
        {
            var st = this.TryFindResource("SuggestionButtonStyle") as Style;
            SuggestionBox.Height = 0;

            ((INotifyCollectionChanged)SuggestionBox.Items).CollectionChanged += HomePanelWindow_CollectionChanged;

        }

        private void HomePanelWindow_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnEyeXActivateSuggestion(object sender, RoutedEventArgs e)
        {
            if (!IsKeyboardGazable)
                return;

            var element = e.Source as FrameworkElement;

            if (null == element) { return; }

            var gazableButton = element.DataContext as Button;

            ColorFadeOut(gazableButton);

            if (gazableButton != null)
                addWordToSendMessageTextFromButton(gazableButton.Content.ToString());
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Window_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.RightShift && !e.IsRepeat)
            {
                var currentApp = Application.Current as App;
                if (currentApp != null) currentApp.eyeXHost.TriggerPanningBegin();
            }
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

            else if (e.Key == Key.RightShift)
            {
                var currentApp = Application.Current as App;
                if (currentApp != null) currentApp.eyeXHost.TriggerPanningEnd();
            }
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
                sugg.Height = SuggestionButtonHeight;
                sugg.Width = SuggestionButtonWidth;
                sugg.Content = (string)enumerate.Current;
                sugg.FontSize = 50;
                sugg.BorderThickness = new Thickness(0);

                var st = this.TryFindResource("SuggestionButtonStyle") as Style;

                if (st != null)
                    sugg.Style = this.TryFindResource("SuggestionButtonStyle") as Style;

                sugg.Click += (o, s) =>
                {
                    addWordToSendMessageTextFromButton(sugg.Content.ToString());
                };

                vm.SuggestionsList.Add(sugg);
            }

            //Grow Shrink animation
            Storyboard myStoryboard = new Storyboard();
            double animSpeed = 0.2;

            if (vm.SuggestionsList.Count > 0)
            {
                if (SuggestionBox.Height != SuggestionBoxHeight)
                {
                    DoubleAnimation myDoubleAnimation = new DoubleAnimation();
                    myDoubleAnimation.From = 0;
                    myDoubleAnimation.To = SuggestionBoxHeight;
                    myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(animSpeed));
                    Storyboard.SetTargetName(myDoubleAnimation, SuggestionBox.Name);
                    Storyboard.SetTargetProperty(myDoubleAnimation,
                        new PropertyPath(Rectangle.HeightProperty));

                    myStoryboard.Children.Add(myDoubleAnimation);
                    myStoryboard.Begin(this);
                }
            }
            else
            {
                if(SuggestionBox.Height != 0)
                {
                    DoubleAnimation myDoubleAnimation = new DoubleAnimation();
                    myDoubleAnimation.From = SuggestionBoxHeight;
                    myDoubleAnimation.To = 0;
                    myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(animSpeed));
                    Storyboard.SetTargetName(myDoubleAnimation, SuggestionBox.Name);
                    Storyboard.SetTargetProperty(myDoubleAnimation,
                        new PropertyPath(Rectangle.HeightProperty));

                    myStoryboard.Children.Add(myDoubleAnimation);
                    myStoryboard.Begin(this);
                }
            }

        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void GrowShrinkAnimator(string controlName, double from, double to)
        {
            //Grow Shrink animation
            Storyboard myStoryboard = new Storyboard();
            double animSpeed = 0.2;

            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.From = from;
            myDoubleAnimation.To = to;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(animSpeed));
            Storyboard.SetTargetName(myDoubleAnimation, controlName);
            Storyboard.SetTargetProperty(myDoubleAnimation,
                new PropertyPath(Rectangle.HeightProperty));

            myStoryboard.Children.Add(myDoubleAnimation);
            myStoryboard.Begin(this);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void addWordToSendMessageTextFromButton(string text)
        {
            if (!IsKeyboardGazable)
                return;

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
            if (!IsKeyboardGazable)
                return;

            if (vm.MessageToSend.Length == 0)
                return;

            ColorFadeOut(sender as Button);

            vm.PlayTTS();
            Status.Opacity = 0;
            Status.Content = "Text Spoken";
            FadeIn(Status);
            _startStatusTimer();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnVirtualKeyboardPressed(object sender, RoutedEventArgs e)
        {
            if (!IsKeyboardGazable)
                return;

            var button = sender as Button;

            ColorFadeOut(button);

            if (button.Tag.Equals("next"))
                {
                vm.LoadNextKeyboard();
                }
            else if (button.Tag.Equals("back"))
                {
                vm.LoadPreviousKeyboard();
                }
            else
                {
                vm.MessageToSend += button.Content;

                autocompleteInput.Focus();
                autocompleteInput.CaretIndex = autocompleteInput.Text.Length;
                }
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
            if (!IsKeyboardGazable)
                return;

            var button = sender as Button;

            ColorFadeOut(button);

            if (vm.MessageToSend.Length > 0)
            {
                if(!vm.IsLetters)
                    vm.MessageToSend = strcut(vm.MessageToSend);
                else
                    vm.MessageToSend = vm.MessageToSend.Remove(vm.MessageToSend.Length - 1);
            }

            autocompleteInput.Focus();
            autocompleteInput.CaretIndex = autocompleteInput.Text.Length;
        }

        private string strcut(string str)
        {
            string[] a = str.Trim().Split(' ');
            string str1 = string.Empty;
            for (int i = 0; i < a.Count() - 1; i++)
            {
                str1 = str1 + a[i];
                if (a.Count() - 2 != i)
                { str1 += " "; }
            }
            return str1;
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

        private void FocusOnInputText(object sender, RoutedEventArgs e)
        {
            if (!IsKeyboardGazable)
                return;

            vm.MessageToSend += " ";

            autocompleteInput.Focus();
            autocompleteInput.CaretIndex = autocompleteInput.Text.Length;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnSpaceBtn_Acctivate(object sender, RoutedEventArgs e)
        {
            if (!IsKeyboardGazable)
                return;

            ColorFadeOut(sender as Button);

            vm.MessageToSend += " ";

            autocompleteInput.Focus();
            autocompleteInput.CaretIndex = autocompleteInput.Text.Length;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void KeysetButton_Activate(object sender, RoutedEventArgs e)
        {
            if (!IsKeyboardGazable)
                return;

            var rad_btn = sender as RadioButton;

            autocompleteInput.Focus();
            autocompleteInput.CaretIndex = autocompleteInput.Text.Length;

            if (rad_btn != null)
                rad_btn.IsChecked = true;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void OnGazeActivateButton()
        {
            if(vm.IsFixationGazeActivate)
                eyeXHostRef.TriggerActivation();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnFixatedGaze(object sender, EyeXFramework.FixationEventArgs e)
        {
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
            if (!IsKeyboardGazable)
                return;

            vm.SendTTS();
            Utilities.Util.Speak("TTS Sent", System.Speech.Synthesis.VoiceGender.Female);
            Status.Opacity = 0;
            Status.Content = "TTS sent";
            FadeIn(Status);
            _startStatusTimer();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnSendSMS(object sender, RoutedEventArgs e)
        {
            if (!IsKeyboardGazable)
                return;

            Utilities.Util.Speak("SMS Sent", System.Speech.Synthesis.VoiceGender.Female);
            new SendMessage().Invoke(vm.MessageToSend, vm.PhoneNumber);
            Status.Opacity = 0;
            Status.Content = "SMS sent";
            FadeIn(Status);
            _startStatusTimer();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Clr_Activate(object sender, RoutedEventArgs e)
        {
            if (!IsKeyboardGazable)
                return;

            vm.MessageToSend = "";
            autocompleteInput.Focus();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void KeysetRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            vm.UpdateKeyboard();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void GazeActivationMode_Checked(object sender, RoutedEventArgs e)
        {
            //[AH]TODO
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void GazeActivationMode_Activate(object sender, RoutedEventArgs e)
        {
            if (!IsKeyboardGazable)
                return;

            var rad_btn = sender as RadioButton;

            if (rad_btn == null)
                vm.MessageToSend += " ";

            autocompleteInput.Focus();
            autocompleteInput.CaretIndex = autocompleteInput.Text.Length;

            if (rad_btn != null)
                rad_btn.IsChecked = true;
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
            if(Status.Opacity != 0)
                FadeOut(Status);

            statusTimer.Stop();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void ScrollUpMiscPanel()
        {
            // Get the border of the listview (first child of a listview)
            Decorator border = VisualTreeHelper.GetChild(MiscPanelLV, 0) as Decorator;

            MiscPanelLV.IsEnabled = false;

            // Get scrollviewer
            ScrollViewer scrollViewer = border.Child as ScrollViewer;

            DoubleAnimation verticalAnimation = new DoubleAnimation();

            double stepping = scrollViewer.ActualHeight - (scrollViewer.ActualHeight / 4);
            double currHeight = scrollViewer.VerticalOffset;

            if (currHeight == 0)
            {
                //return;
            }

            if (currHeight - stepping > 0)
            {
                //Bugged...some reason it will never go all the way top, work around is this
                if(currHeight - stepping < scrollViewer.ActualHeight)
                    currHeight = 0;
                else
                    currHeight -= stepping;
            }
            else
            {
                currHeight = 0;
            }

            verticalAnimation.From = scrollViewer.VerticalOffset;
            verticalAnimation.To = currHeight;
            verticalAnimation.Duration = TimeSpan.FromSeconds(0.5);
            verticalAnimation.Completed += ScrollUpMiscPanelVerticalAnimation_Completed;

            Storyboard storyboard = new Storyboard();

            storyboard.Children.Add(verticalAnimation);
            Storyboard.SetTarget(verticalAnimation, scrollViewer);
            Storyboard.SetTargetProperty(verticalAnimation, new PropertyPath(ScrollAnimationBehavior.VerticalOffsetProperty)); // Attached dependency property
            storyboard.Begin();
        }

        private void ScrollUpMiscPanelVerticalAnimation_Completed(object sender, EventArgs e)
        {
            if (hasMiscPanelUpGazed)
            {
                ScrollUpMiscPanel();
            }

            MiscPanelLV.IsEnabled = true;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void FadeOut(System.Windows.Controls.Control control)
        {

            var a = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                FillBehavior = FillBehavior.Stop,
                BeginTime = TimeSpan.FromSeconds(0),
                Duration = new Duration(TimeSpan.FromSeconds(0.5))
            };
            var storyboard = new Storyboard();

            storyboard.Children.Add(a);
            Storyboard.SetTarget(a, control);
            Storyboard.SetTargetProperty(a, new PropertyPath(OpacityProperty));
            storyboard.Completed += (s, e) =>
            {
                control.Opacity = 0.0;
            };
            storyboard.Begin();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void FadeIn(System.Windows.Controls.Control control)
        {
            var a = new DoubleAnimation
            {
                From = 0,
                To = 1.0,
                FillBehavior = FillBehavior.Stop,
                BeginTime = TimeSpan.FromSeconds(0),
                Duration = new Duration(TimeSpan.FromSeconds(0.5))
            };
            var storyboard = new Storyboard();

            storyboard.Children.Add(a);
            Storyboard.SetTarget(a, control);
            Storyboard.SetTargetProperty(a, new PropertyPath(OpacityProperty));
            storyboard.Completed += (s, e) =>
            {
                control.Opacity = 1.0;
            };
            storyboard.Begin();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void ScrollDownMiscPanel()
        {
            // Get the border of the listview (first child of a listview)
            Decorator border = VisualTreeHelper.GetChild(MiscPanelLV, 0) as Decorator;

            MiscPanelLV.IsEnabled = false;

            // Get scrollviewer
            ScrollViewer scrollViewer = border.Child as ScrollViewer;

            DoubleAnimation verticalAnimation = new DoubleAnimation();

            double stepping = scrollViewer.ActualHeight - (scrollViewer.ActualHeight/4);
            double totalHeight = scrollViewer.ScrollableHeight;
            double currHeight = scrollViewer.VerticalOffset;

            if (currHeight == totalHeight)
            {
                //return;
            }

            if (currHeight + stepping < totalHeight)
            {
                currHeight += stepping;
            }
            else
            {
                currHeight += (totalHeight - currHeight);
            }

            verticalAnimation.From = scrollViewer.VerticalOffset;
            verticalAnimation.To = currHeight;
            verticalAnimation.Duration = TimeSpan.FromSeconds(0.5);
            verticalAnimation.Completed += ScrollDownMiscPanelVerticalAnimation_Completed;

            Storyboard storyboard = new Storyboard();

            storyboard.Children.Add(verticalAnimation);
            Storyboard.SetTarget(verticalAnimation, scrollViewer);
            Storyboard.SetTargetProperty(verticalAnimation, new PropertyPath(ScrollAnimationBehavior.VerticalOffsetProperty)); // Attached dependency property
            storyboard.Begin();
        }


        private void ScrollDownMiscPanelVerticalAnimation_Completed(object sender, EventArgs e)
        {
            if (hasMiscPanelDownGazed)
            {
                ScrollDownMiscPanel();
            }
            MiscPanelLV.IsEnabled = true;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void MiscPanelUp_Activate(object sender, RoutedEventArgs e)
        {
            ScrollUpMiscPanel();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void MiscPanelDown_Activate(object sender, RoutedEventArgs e)
        {
            ScrollDownMiscPanel();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void MiscPanelUp_HasGazeChanged(object sender, RoutedEventArgs e)
        {
            hasMiscPanelUpGazed = !hasMiscPanelUpGazed;
            if (hasMiscPanelUpGazed)
            {
                ScrollUpMiscPanel();
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void MiscPanelDown_HasGazeChanged(object sender, RoutedEventArgs e)
        {
            hasMiscPanelDownGazed = !hasMiscPanelDownGazed;

            if(hasMiscPanelDownGazed)
            {
                ScrollDownMiscPanel();
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void MenuItemNew_Click(object sender, RoutedEventArgs e)
            {
            UserWindow userPanel = new UserWindow();
            userPanel.ShowDialog();
            }

        private void MenuItemExit_Click(object sender, RoutedEventArgs e)
            {
            this.Close();
            Application.Current.Shutdown();
            }

        private void NewUser_CanExecute(object sender, CanExecuteRoutedEventArgs e)
            {
            e.CanExecute = true;
            }

        private void NewUser_Executed(object sender, ExecutedRoutedEventArgs e)
            {
            UserWindow userPanel = new UserWindow();
            userPanel.ShowDialog();
            }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        //HACK, lazy to do
        private void EyeEnabler_Checked(object sender, RoutedEventArgs e)
        {
            IsKeyboardGazable = !IsKeyboardGazable;

            var bc = new BrushConverter();
            if (IsKeyboardGazable)
            {
                EyeEnabler.Content = "Gazeraction ON";
                EyeEnabler.Background = bc.ConvertFrom("#17af29") as Brush;
            }
            else
            {
                EyeEnabler.Content = "Gazeraction OFF";
                EyeEnabler.Background = bc.ConvertFrom("#e85151") as Brush;
            }
                
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void EyeEnabler_Click(object sender, RoutedEventArgs e)
        {

        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void ColorFadeOut(System.Windows.Controls.Control control)
        {
            //Uri uri = new Uri("Resources/ClickSound.mp3", UriKind.Relative);
            //var player = new MediaPlayer();
            //player.Open(uri);
            //player.Play();

            var oriColor = control.Background.Clone() as SolidColorBrush;
 
            ColorAnimation ca = new ColorAnimation(Colors.Red, oriColor.Color, new Duration(TimeSpan.FromSeconds(1)));
            Storyboard.SetTarget(ca, control);
            Storyboard.SetTargetProperty(ca, new PropertyPath("Background.Color"));

            Storyboard stb = new Storyboard();
            stb.Children.Add(ca);
            stb.Begin();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////

    public static class CustomCommands
        {
        public static readonly RoutedUICommand NewUser = new RoutedUICommand
                (
                        "New",
                        "New",
                        typeof(CustomCommands),
                        new InputGestureCollection()
                        {
                                        new KeyGesture(Key.N, ModifierKeys.Control)
                        }
                );

        }
    }

