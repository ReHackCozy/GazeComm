using System;
using System.Diagnostics;
using System.Windows.Controls;
using System.ComponentModel;
using System.IO;
using System.Collections;
using System.Windows.Input;
using System.Linq;

namespace Gaze.EyeTracker
{
    public class AutoComplete : TextBox
    {

        #region "Variables"
        //List that holds the values to be loaded
        //by the textbox
        private ArrayList _reloadValues = new ArrayList();
        //Flag used to tell the keypress 
        //handler if it should autocomplete
        private bool _updateValues = false;
        //The filename of the file that the 
        //list is read from and saved to.
        private string _file = "";
        //Set to true when the _reloadValues is loaded.
        private bool _init = false;
        #endregion

        #region "Constructors"
        /// <summary>
        /// Constructor that initializes
        /// filename to ""
        /// </summary>
        /// <remarks></remarks>
        public AutoComplete()
        {
            _file = string.Empty;
        }

        /// <summary>
        /// Constructor that sets the
        /// file to an actual file
        /// </summary>
        /// <param name="file"></param>
        public AutoComplete(string file)
        {
            _file = file;
        }
        #endregion

        #region "Properties"

        /// <summary>
        /// Sets the list of choices directly
        /// </summary>
        [Description("Sets the list of values")]
        public string[] Values
        {
            get { return (string[])_reloadValues.ToArray(typeof(string)); }
            set { _reloadValues = new ArrayList(value); }
        }

        /// <summary>
        /// The filename of the file to get the values from
        /// </summary>
        [Description("The filename of the file to load from")]
        public string ValuesFile
        {
            get { return _file; }
            set { _file = value; }
        }

        #endregion

        #region "Methods"
        /// <summary>
        /// Causes the control to reload the values file.
        /// </summary>
        /// <remarks></remarks>
        [Description("Causes the control to realod the values.")]
        public void RefreshTextBox()
        {
            RefreshValues();
        }

        public void SaveFeedURL(string url)
        {
            //create our objects
            string str;
            FileStream stream;
            StreamWriter writer;
            //check to make sure a value was passed
            if (url.Trim() == "") return;   //Don't save empty strings
            bool isFound = IsPresent();
            //check to see if the file already exists
            //if it does we want to add to it, not
            //create a new one
            if (File.Exists(_file))
            {
                //check to make sure the value passed
                //doesnt already exist in our file
                if (isFound) return;
                try
                {
                    //add the value to our ArrayList
                    _reloadValues.Add(url);
                    //open our file
                    stream = new FileStream(_file, FileMode.Append);
                    writer = new StreamWriter(stream);
                    //write the value
                    writer.WriteLine(url);
                    //close everthing up
                    writer.Close();
                    stream.Close();
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                    throw new Exception("Could not save the value: " + url + " to the list", ex);
                }
            }
            //the file was not found, so create it.
            else
            {
                if (!isFound)
                    _reloadValues.Add(url);
                try
                {
                    //create an enumerator to iterate
                    //through our ArrayList with
                    IEnumerator enumerate = _reloadValues.GetEnumerator();
                    //create our file
                    stream = new FileStream(_file, FileMode.Create);
                    //generate a StreamWrite to write to
                    //our file with
                    writer = new StreamWriter(stream);
                    //now loop through the ArrayList adding
                    //each of the values to our file
                    while (enumerate.MoveNext())
                    {
                        str = (string)enumerate.Current;
                        writer.WriteLine(str);
                        //close everything up
                        writer.Close();
                        stream.Close();
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                    throw new Exception("Could not save the value: " + url + " to the list", ex);
                }
            }
        }
        #endregion

        #region "Private Functions"
        /// <summary>
        /// Searches the file with the value
        /// being passed for save
        /// </summary>
        /// <returns>True/False</returns>
        private bool IsPresent()
        {
            //create an enumerator to iterate
            //through our ArrayList
            IEnumerator enumerate = _reloadValues.GetEnumerator();
            string str = this.Text.ToUpper();
            //loop through all the values looking
            //for the value being daved. If it exists
            //return true, e;se return false
            while (enumerate.MoveNext())
            {
                string text = (string)enumerate.Current;
                if (text.ToUpper().IndexOf(str) == 0)
                    return true;
            }
            return false;
        }


        /// <summary>
        /// Refreshes the values already saved in the text file,
        /// if no file exists then create one
        /// </summary>
        /// <remarks></remarks>
        private void RefreshValues()
        {
            try
            {
                //check to see if a file name was provides, if
                //one wasnt provided then create a new file with
                //a name of the Control.txt
                if (_file == "")
                    _file = this.Name + ".txt";
                //re instantiate our array list object
                _reloadValues = new ArrayList();
                //open our file
                FileStream stream = new FileStream(_file, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                //read the file
                StreamReader reader = new StreamReader(stream);
                //loop through each line in the file adding the value
                //to our ArrayList object
                while (reader.Peek() != -1)
                    _reloadValues.Add(reader.ReadLine());
                //close out reader and stream objects
                reader.Close();
                stream.Close();
                _init = true;
            }
            catch (Exception ee)
            {
                throw new Exception("Could not open the value file for the control: " + this.Name, ee);
            }
        }
        #endregion

        /// <summary>
        /// Overrides the TextChanged event.  We will use this
        /// to check and see if what the user is typing resembles anything
        /// in our file
        /// </summary>
        /// <param name="e">An System.EventArgs that contains the event data.</param>
        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            if (_reloadValues.Count == 0 && !_init) //Load the values from the file.
            {
                RefreshValues();
            }
            string tmp = this.Text.ToUpper();
            if ((tmp == "") || _updateValues)
            {
                base.OnTextChanged(e);
            }
            else
            {
                string lastWord = tmp.Split(' ').Last();
                //create our enumerator to iterate through our ArrayList
                IEnumerator enumerate = _reloadValues.GetEnumerator();
                //set out bool value to false
                bool isFound = false;
                //loop while theres a value is our ArrayList
                //and our boolean value is false
                while ((enumerate.MoveNext()) && (!isFound))
                {
                    //set our string variable to the value of
                    //the current value in the enumerator
                    string str = (string)enumerate.Current;
                    //check to see if it holds one of our values
                    if (str.ToUpper().IndexOf(lastWord) == 0)
                    {
                        //update our values
                        _updateValues = true;
                        //set the value from our file
                        //to the value in the textbox
                        this.Text = str;
                        //re-highlight the remaining text
                        //as the user types
                        this.SelectionStart = lastWord.Length;
                        this.SelectionLength = str.Length - lastWord.Length;
                        //set our boolean value to true
                        isFound = true;
                        _updateValues = false;
                    }
                }
                base.OnTextChanged(e);
            }
        }


        /// <summary>
        /// Overrides KeyDown event.
        /// </summary>
        /// <param name="e">A System.Windows.Forms.KeyEventArgs that contains the event data.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key.Equals(8) || e.Key.Equals(46))
                _updateValues = true;
            base.OnKeyDown(e);
        }

        /// <summary>
        /// Overrides the KeyUp event.
        /// </summary>
        /// <param name="e">A System.Windows.Forms.KeyEventArgs that contains the event data.</param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.Key.Equals(8) || e.Key.Equals(46))
                _updateValues = false;
            base.OnKeyUp(e);
        }
    }
}
