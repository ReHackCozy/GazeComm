﻿using System;
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

        private bool _isBlinked;

        private ObservableCollection<GazableButton> _suggestionsList;

        private HomePanelWindow _viewRef;

        public HomePanelViewModel(HomePanelWindow viewRef)
        {
            _name = "John Doe";
            _phoneNumber = "0196031591";
            _messageToSend = "";
            _isBlinked = false;

            _suggestionsList = new ObservableCollection<GazableButton>();

            _authorizationAPI = new AuthorizationAPI(sendTTS);

            //HACK
            _viewRef = viewRef;
        }

        public void sendTTS()
        {
            if(String.IsNullOrEmpty(_authorizationAPI.AccessToken))
            {
                //run authentication first
                _authorizationAPI.Invoke();
            } else
            {
                var number = "+" + _phoneNumber;
                new SendTTS().Invoke(_messageToSend, number, _authorizationAPI.AccessToken);
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

        public bool IsBlinked
        {
            get
            {
                return _isBlinked;
            }

            set
            {
                if (_isBlinked == value) return;

                _isBlinked = value;

                if(IsBlinked)
                {
                    _viewRef.OnGazeActivateButton();
                }

                OnPropertyChanged("IsBlinked");
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
