using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gaze.Data
    {
    public class UserData
        {
        public delegate void DataUpdateDelegate();

        public DataUpdateDelegate UpdateDataDelegate { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public bool GenderMale { get; set; }
        public bool GenderFemale { get; set; }
        public int Age { get; set; }
        public bool EnglishLanguage { get; set; }
        public bool MalayLanguage { get; set; }

        public UserData()
            {
            Clear();
            }

        public void Clear()
            {
            Name = "";
            PhoneNumber = "";
            GenderMale = true;
            GenderFemale = false;
            Age = 30;
            EnglishLanguage = true;
            MalayLanguage = false;
            }

        }
    }
