using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Gaze.API;
using Gaze.Data;
using System.IO;
using RestSharp;
using static Gaze.HomePanel.HomePanelViewModel;
using System.Windows.Threading;
using System.Windows;

namespace Gaze.HomePanel
{
    public class HomePanelViewModel : INotifyPropertyChanged, IKeyboardUpdated
        {
        //panic implementation, this probably shouldnt be here.
        private AuthorizationAPI _authorizationAPI;

        private string _messageToSend;
        private String _name;
        private String _phoneNumber;

        //private bool _isBlinked;
        private UserData userDataRef;
        private KeyboardManager keyboardManager;

        private ObservableCollection<Button> _suggestionsList = new ObservableCollection<Button>();
        private ObservableCollection<Button> _keyboardButtonList = new ObservableCollection<Button>();

        public HomePanelViewModel()
        {
            var currentApp = System.Windows.Application.Current as App;
            userDataRef = currentApp.userData;
            userDataRef.UpdateDataDelegate += OnDataUpdated;

            keyboardManager = new KeyboardManager(this);

            _isKeyboardGazable = true;

            _name = "";
            _phoneNumber = "";
            _messageToSend = "";
            //_isBlinked = false;
            UpdateKeyboard();
            _authorizationAPI = new AuthorizationAPI(SendTTS);

            IsFixationGazeActivate = true;
            IsBlinkEyesGazeActivate = false;
        }

        //AH] TODO crash on gaze bug
        public void LoadNextKeyboard()
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => 
            { _keyboardButtonList.Clear();
                _keyboardButtonList.AddRange(keyboardManager.GetNextKeyboardList());
            }));
        }

        public void LoadPreviousKeyboard()
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                {
                    _keyboardButtonList.Clear();
                    _keyboardButtonList.AddRange(keyboardManager.GetPreviousKeyboardList());
                }));
            }

        public void SendTTS()
        {
            if(String.IsNullOrEmpty(_authorizationAPI.AccessToken))
            {
                //run authentication first
                _authorizationAPI.Invoke();
            } else
            {
                //todo verify format
                var number = "+6" + userDataRef.PhoneNumber;
                new SendTTS().Invoke(_messageToSend, number, _authorizationAPI.AccessToken);
            }
        }

        public void PlayTTS()
        {
            Utilities.Util.Speak(_messageToSend, userDataRef.GenderMale ? System.Speech.Synthesis.VoiceGender.Male : System.Speech.Synthesis.VoiceGender.Female, userDataRef.Age);
        }

        public void UpdateKeyboard()
        {
            if(keyboardManager != null)
                {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                {
                    _keyboardButtonList.Clear();
                    _keyboardButtonList.AddRange(keyboardManager.GetKeyboardList());
                }));
            }
        }
        
        private void OnDataUpdated()
            {
            Name = userDataRef.Name;
            PhoneNumber = userDataRef.PhoneNumber;
            }

        #region Setters Getters

        public string Name
            {
            get
                {
                return _name;
                }

            set
                {
                if (_name == value) return;

                _name = value;
                OnPropertyChanged("Name");
                }
            }

        public string PhoneNumber
            {
            get
                {
                return _phoneNumber;
                }

            set
                {
                if (_phoneNumber == value) return;

                _phoneNumber = value;
                OnPropertyChanged("PhoneNumber");
                }
            }

        public string MessageToSend
        {
            get
            {
                return _messageToSend;
            }

            set
            {
                if (_messageToSend == value) return;

                _messageToSend = value;

                //var tmp_str = _messageToSend.Split(' ').Last();

                //if (tmp_str.Length > 1)
                //    _messageToSend += " ";

                
                OnPropertyChanged("MessageToSend");
            }
        }

        private bool _isFixationGazeActivate;
        public bool IsFixationGazeActivate
        {
            get { return _isFixationGazeActivate; }
            set
            {
                if (_isFixationGazeActivate == value) return;

                _isFixationGazeActivate = value;
                OnPropertyChanged("IsFixationGazeActivate");
            }
        }

        private bool _isKeyboardGazable;
        public bool IsKeyboardGazable
        {
            get { return _isKeyboardGazable; }
            set
            {
                if (_isKeyboardGazable == value) return;

                _isKeyboardGazable = value;
                OnPropertyChanged("IsKeyboardGazable");
            }
        }

        private bool _isBlinkEyesGazeActivate;
        public bool IsBlinkEyesGazeActivate
        {
            get { return _isBlinkEyesGazeActivate; }
            set
            {
                if (_isBlinkEyesGazeActivate == value) return;

                _isBlinkEyesGazeActivate = value;
                OnPropertyChanged("IsBlinkEyesGazeActivate");
            }
        }

        public bool IsLetters
        {
            get { return keyboardManager.IsLetters; }
            set
            {
                if (keyboardManager.IsLetters == value) return;

                keyboardManager.IsLetters = value;
                OnPropertyChanged("IsLetters");
            }
        }


        public bool IsWords
        {
            get { return keyboardManager.IsWords; }
            set
            {
                if (keyboardManager.IsWords == value) return;

                keyboardManager.IsWords = value;
                OnPropertyChanged("IsWords");
            }
        }

        public bool IsActions
        {
            get { return keyboardManager.IsActions; }
            set
            {
                if (keyboardManager.IsActions == value) return;

                keyboardManager.IsActions = value;
                OnPropertyChanged("IsActions");
            }
        }

        public ObservableCollection<Button> SuggestionsList
        {
            get
            {
                return _suggestionsList;
            }

            set
            {
                if (_suggestionsList == value) return;

                _suggestionsList = value;
                OnPropertyChanged("SuggestionsList");
            }
        }

        public ObservableCollection<Button> KeyboardButtonList
        {
            get
            {
                return _keyboardButtonList;
            }

            set
            {
                if (_keyboardButtonList == value) return;

                _keyboardButtonList = value;
                OnPropertyChanged("KeyboardButtonList");
            }
        }
        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region IKeyboardUpdated

        public interface IKeyboardUpdated
            {
            void onKeyboardUpdated();
            }

        public void onKeyboardUpdated()
            {
            UpdateKeyboard();
            }

        #endregion

        }
    }

#region extension method

public static class CollectionExtensions
    {
    public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> newItems)
        {
        foreach (T item in newItems)
            {
            collection.Add(item);
            }
        }
    }

#endregion
