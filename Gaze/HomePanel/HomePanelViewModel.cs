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
            _phoneNumber = "019-666 4444";
            _messageToSend = "Message to send";

            _suggestionsList = new ObservableCollection<GazableButton>();

            //for (int i = 0; i < 15; ++i)
            //{
            //    var sugg = new GazableButton();
            //    sugg.Height = 150;
            //    sugg.Width = 150;
            //    sugg.Content = "Suggestion";
            //    sugg.value = "Suggestion";

            //    _suggestionsList.Add(sugg);
            //}
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
