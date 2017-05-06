using System; 
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Gaze.API;

namespace Gaze.HomePanel
{
    public class HomePanelViewModel : INotifyPropertyChanged
    {
        //panic implementation, this probably shouldnt be here.
        private AuthorizationAPI _authorizationAPI;

        private string _name;
        private string _phoneNumber;
        private bool _genderMale;
        private bool _genderFemale;
        private int _age;
        private string _messageToSend;

        private bool _isLetters;
        private bool _isWords;
        private bool _isActions;

        private bool _isBlinked;

        private ObservableCollection<Button> _suggestionsList = new ObservableCollection<Button>();
        private ObservableCollection<Button> _keyboardButtonList = new ObservableCollection<Button>();

        #region keyboard data

        private IList<String> _lettersKeyboard = new List<string> { "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P", "A", "S", "D", "F", "G", "H", "J", "K", "L", "Z", "X", "C", "V", "B", "N", "M" };
        private IList<String> _wordsKeyboard = new List<string> {"You ", "Help ", "Give ", "Try ", "Me ", "Near ", "Far ", "Height ", "Width ", "Sorry ", "Sure ", "Goodbye ", "Possible ", "Impossible ", "Meal ", "Hello ", "Yes ", "No ", "Maybe ", "Make ", "Good ", "Bad ", "Okay ", "Meh ", "Opss " , "Hoorah "};
        private IList<String> _actionsKeyboard = new List<string> { "Thank You ", "You're welcome ", "I'm hungry ", "I love you ", "I don't know ", "You're beautiful ", "I need a hug ", "I'm tired ", "Good job ", "I agree ", "I disagree ", "I think so ", "I'm happy ", "I'm sad ", "See you soon! ", "Happy birthday! ", "I'm sorry ", "You're brilliant ", "The cake is a lie ", "That's fantastic ", "I know ", "Game is hard ", "My apologies ", "LOL that's funny ", "OMG ", "The end " };

        #endregion

        public HomePanelViewModel()
        {
            _name = "Pak Ali";
            _phoneNumber = "0196031591";
            _genderMale = true;
            _genderFemale = false;
            _age = 30;
            _messageToSend = "";
            _isBlinked = false;

            _isLetters = false;
            _isWords = false;
            _isActions = true;
            UpdateKeyboard();
            _authorizationAPI = new AuthorizationAPI(SendTTS);
        }

        public void SendTTS()
        {
            if(String.IsNullOrEmpty(_authorizationAPI.AccessToken))
            {
                //run authentication first
                _authorizationAPI.Invoke();
            } else
            {
                var number = "+6" + _phoneNumber;
                new SendTTS().Invoke(_messageToSend, number, _authorizationAPI.AccessToken);
            }
        }

        public void PlayTTS()
        {
            Utilities.Util.Speak(_messageToSend, _genderMale ? System.Speech.Synthesis.VoiceGender.Male : System.Speech.Synthesis.VoiceGender.Female, _age);
        }

        public void UpdateKeyboard()
        {
            if(_isLetters)
            {
                PopulateKeyboardList(_lettersKeyboard);
            } else if (_isWords)
            {
                PopulateKeyboardList(_wordsKeyboard);
            } else {
                PopulateKeyboardList(_actionsKeyboard);
            }
        }

        private void PopulateKeyboardList(IList<String> list)
        {
            _keyboardButtonList.Clear();
            foreach (var i in list)
            {
                Button btn = new Button();
                btn.Content = i;
                
                _keyboardButtonList.Add(btn);
            }
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

        
        public bool GenderMale
        {
            get { return _genderMale; }
            set
            {
                if (_genderMale == value) return;

                _genderMale = value;
                OnPropertyChanged("GenderMale");
            }
        }

        
        public bool GenderFemale
        {
            get { return _genderFemale; }
            set
            {
                if (_genderFemale == value) return;

                _genderFemale = value;
                OnPropertyChanged("GenderFemale");
            }
        }

        public int Age
        {
            get { return _age; }
            set
            {
                if (_age == value) return;

                _age = value;
                OnPropertyChanged("Age");
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

        public bool IsLetters
        {
            get { return _isLetters; }
            set
            {
                if (_isLetters == value) return;

                _isLetters = value;
                OnPropertyChanged("IsLetters");
            }
        }


        public bool IsWords
        {
            get { return _isWords; }
            set
            {
                if (_isWords == value) return;

                _isWords = value;
                OnPropertyChanged("IsWords");
            }
        }

        public bool IsActions
        {
            get { return _isActions; }
            set
            {
                if (_isActions == value) return;

                _isActions = value;
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

    }
}
