using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Gaze.API;

namespace Gaze.HomePanel
{
    public class HomePanelViewModel : INotifyPropertyChanged
    {
        //panic implementation, this probably shouldnt be here.
        private AuthorizationAPI _authorizationAPI;

        private string _name;
        private string _phoneNumber;
        private string _messageToSend;

        private ObservableCollection<GazableButton> _suggestionsList;

        public HomePanelViewModel()
        {
            _name = "John Doe";
            _phoneNumber = "0196031591";
            _messageToSend = "";

            _suggestionsList = new ObservableCollection<GazableButton>();

            _authorizationAPI = new AuthorizationAPI(sendTTS);
        }

        public void sendTTS()
        {
            if(String.IsNullOrEmpty(_authorizationAPI.AccessToken))
            {
                //run authentication first
                _authorizationAPI.Invoke();
            } else
            {
                //send tts here
                new SendTTS().Invoke("hello this call is to test sending tts, you rock", "+60142725192", _authorizationAPI.AccessToken);
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
                OnPropertyChanged("MessageToSend");
            }
        }

        public ObservableCollection<GazableButton> SuggestionsList
        {
            get
            {
                return _suggestionsList;
            }

            set
            {
                if (_suggestionsList == value) return;

                _suggestionsList = value;
                
                foreach (var sugg in _suggestionsList)
                {
                    sugg.type = GazableButton.Type.Suggestion;
                }

                OnPropertyChanged("SuggestionsList");
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
