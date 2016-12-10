using System;
using System.Collections.Generic;
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

        public HomePanelViewModel()
        {
            _name = "John Doe";
            _phoneNumber = "019-666 4444";
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
