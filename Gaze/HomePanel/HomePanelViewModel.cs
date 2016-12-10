using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;


namespace Gaze.HomePanel
{
    public class HomePanelViewModel : INotifyPropertyChanged
    {
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
