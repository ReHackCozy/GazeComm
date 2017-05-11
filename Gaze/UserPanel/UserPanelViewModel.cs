using Gaze.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gaze.UserPanel
{
    public class UserPanelViewModel : INotifyPropertyChanged
        {
        private UserData userDataRef;

        public UserPanelViewModel()
            {
            var currentApp = System.Windows.Application.Current as App;
            userDataRef = currentApp.userData;
            userDataRef.Clear();
            }

        public void onUserPanelClosed()
            {
            userDataRef.UpdateDataDelegate();
            }

        #region Setters Getters

        public string Name
            {
            get
                {
                return userDataRef.Name;
                }

            set
                {
                if (userDataRef.Name == value) return;

                userDataRef.Name = value;
                OnPropertyChanged("Name");
                }
            }

        public string PhoneNumber
            {
            get
                {
                return userDataRef.PhoneNumber;
                }

            set
                {
                if (userDataRef.PhoneNumber == value) return;

                userDataRef.PhoneNumber = value;
                OnPropertyChanged("PhoneNumber");
                }
            }


        public bool GenderMale
            {
            get { return userDataRef.GenderMale; }
            set
                {
                if (userDataRef.GenderMale == value) return;

                userDataRef.GenderMale = value;
                OnPropertyChanged("GenderMale");
                }
            }


        public bool GenderFemale
            {
            get { return userDataRef.GenderFemale; }
            set
                {
                if (userDataRef.GenderFemale == value) return;

                userDataRef.GenderFemale = value;
                OnPropertyChanged("GenderFemale");
                }
            }

        public int Age
            {
            get { return userDataRef.Age; }
            set
                {
                if (userDataRef.Age == value) return;

                userDataRef.Age = value;
                OnPropertyChanged("Age");
                }
            }

        public bool EnglishLanguage
            {
            get { return userDataRef.EnglishLanguage; }
            set
                {
                if (userDataRef.EnglishLanguage == value) return;

                userDataRef.EnglishLanguage = value;
                OnPropertyChanged("EnglishLanguage");
                }
            }


        public bool MalayLanguage
            {
            get { return userDataRef.MalayLanguage; }
            set
                {
                if (userDataRef.MalayLanguage == value) return;

                userDataRef.MalayLanguage = value;
                OnPropertyChanged("MalayLanguage");
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
