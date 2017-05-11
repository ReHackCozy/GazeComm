using Gaze.Data;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using static Gaze.HomePanel.HomePanelViewModel;

namespace Gaze.HomePanel
    {
    // Supports up to maximum 62 buttons (2 pages)
    public class KeyboardManager
        {
        //private IList<String> _lettersKeyboard = new List<string> { "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P", "A", "S", "D", "F", "G", "H", "J", "K", "L", "Z", "X", "C", "V", "B", "N", "M" };
        //private IList<String> _wordsKeyboard = new List<string> { "You ", "Help ", "Give ", "Try ", "Me ", "Near ", "Far ", "Height ", "Width ", "Sorry ", "Sure ", "Goodbye ", "Possible ", "Impossible ", "Meal ", "Hello ", "Yes ", "No ", "Maybe ", "Make ", "Good ", "Bad ", "Okay ", "Meh ", "Opss ", "Hoorah " };
        //private IList<String> _actionsKeyboard = new List<string> { "Thank You ", "You're welcome ", "I'm hungry ", "I love you ", "I don't know ", "You're beautiful ", "I need a hug ", "I'm tired ", "Good job ", "I agree ", "I disagree ", "I think so ", "I'm happy ", "I'm sad ", "See you soon! ", "Happy birthday! ", "I'm sorry ", "You're brilliant ", "The cake is a lie ", "That's fantastic ", "I know ", "Game is hard ", "My apologies ", "LOL that's funny ", "OMG ", "The end " };

        private bool _isEnglish;
        private bool _isLetters;
        private bool _isWords;
        private bool _isActions;

        // words data
        private Words _letters;
        private Words _words;
        private Words _actions;

        private UserData userDataRef;

        private static readonly int MAX_KEYBOARD_BUTTON = 31;
        private static readonly string WORDS_FILE = "words";
        private static readonly string LETTERS_FILE = "letters";
        private static readonly string ACTIONS_FILE = "actions";
        private static readonly string FILE_FORMAT = ".json";

        private KeyboardWords _activeKeyboard;
        private KeyboardWords _lettersKeyboard;
        private KeyboardWords _actionsKeyboard;
        private KeyboardWords _wordsKeyboard;

        private IKeyboardUpdated callback;

        #region Setters Getters

        public bool IsLetters
            {
            get
                {
                return _isLetters;
                }

            set
                {
                if (_isLetters == value) return;
                _isLetters = value;
                if(_isLetters)
                    {
                    _activeKeyboard = _lettersKeyboard;
                    }
                
                }
            }

        public bool IsWords
            {
            get
                {
                return _isWords;
                }

            set
                {
                if (_isWords == value) return;
                _isWords = value;
                if (_isWords)
                    {
                    _activeKeyboard = _wordsKeyboard;
                    }
                }
            }

        public bool IsActions
            {
            get
                {
                return _isActions;
                }

            set
                {
                if (_isActions == value) return;
                _isActions = value;
                if (_isActions)
                    {
                    _activeKeyboard = _actionsKeyboard;
                    }
                }
            }

        #endregion

        #region KeyboardWords

        internal class KeyboardWords
            {
            public int Page { get; set; }
            public int Index { get; set; }
            public IList<String> Words { get; set; }

            public KeyboardWords(IList<String> words)
                {
                Words = words;
                Page = 1;
                Index = 0;
                }

            public void updateList(IList<String> words)
                {
                Words = words;
                }

            public IList<Button> GetNextKeyboardList()
                {
                Page++;
                return GetKeyboardList();
                }

            public IList<Button> GetPreviousKeyboardList()
                {
                Page--;
                Debug.Assert(Page > 0);
                return GetKeyboardList();
                }

            private void updateIndex()
                {
                if (Page == 1)
                    {
                    Index = 0;
                    }
                else
                    {
                    //when theres no next page
                    if(Words.Count <= MAX_KEYBOARD_BUTTON + 1)
                        {
                        Index = 0;
                        }
                    else
                        {
                        Index = (Page - 1) * MAX_KEYBOARD_BUTTON;
                        }
                    }
                }

            private int getUpperLimit()
                {
                var upperLimit = Page * MAX_KEYBOARD_BUTTON;

                if(upperLimit > Words.Count)
                    {
                    upperLimit = Words.Count;
                    }

                return upperLimit;
                }

            public IList<Button> GetKeyboardList()
                {
                IList<Button> _keyboardButtonList = new List<Button>();
                updateIndex();
                var index = Index;
                var upperLimit = getUpperLimit();

                for (int i = index; i < upperLimit; i++)
                    {
                    Index++;
                    _keyboardButtonList.Add(CreateKeyboardButton(Words[i], ""));
                    }

               if(Page < 2)
                    {
                    _keyboardButtonList.Add(CreateKeyboardButton(">", "next"));
                    }
               else
                    {
                    _keyboardButtonList.Add(CreateKeyboardButton("<", "back"));
                    }

                return _keyboardButtonList;
                }

            public void Clear()
                {
                Page = 1;
                Index = 0;
                }
            }

        #endregion

        public KeyboardManager(IKeyboardUpdated callback)
            {
            var currentApp = System.Windows.Application.Current as App;
            userDataRef = currentApp.userData;
            this.callback = callback;
            userDataRef.UpdateDataDelegate += OnLanguageChanged;

            _isEnglish = true;
            IsLetters = false;
            IsWords = false;
            IsActions = true;
            _letters = new Words();
            _words = new Words();
            _actions = new Words();
            LoadLanguageFiles(_isEnglish);
            }

        private static Button CreateKeyboardButton(string content, string tag)
            {
            Button btn = new Button();
            btn.Content = content;
            btn.Tag = tag;
            return btn;
            }

        private void OnLanguageChanged()
            {
            if (_isEnglish != userDataRef.EnglishLanguage)
                {
                _isEnglish = userDataRef.EnglishLanguage;
                LoadLanguageFiles(_isEnglish);
                }
            }

        private void LoadLanguageFiles(bool isEnglish)
            {
            var language = isEnglish ? "_en" : "_my";
            var resourceFolder = Directory.GetCurrentDirectory() + "/Resources/";
            var wordsFile = new StringBuilder().Append(WORDS_FILE).Append(language).Append(FILE_FORMAT).ToString();
            var lettersFile = new StringBuilder().Append(LETTERS_FILE).Append(language).Append(FILE_FORMAT).ToString();
            var actionsfile = new StringBuilder().Append(ACTIONS_FILE).Append(language).Append(FILE_FORMAT).ToString();

            using (StreamReader r = new StreamReader(resourceFolder + wordsFile))
                {
                string json = r.ReadToEnd();
                _words = SimpleJson.DeserializeObject<Words>(json);
                }

            using (StreamReader r = new StreamReader(resourceFolder + lettersFile))
                {
                string json = r.ReadToEnd();
                _letters = SimpleJson.DeserializeObject<Words>(json);
                }

            using (StreamReader r = new StreamReader(resourceFolder + actionsfile))
                {
                string json = r.ReadToEnd();
                _actions = SimpleJson.DeserializeObject<Words>(json);
                }

            if(_lettersKeyboard == null)
                {
                _lettersKeyboard = new KeyboardWords(_letters.words);
                }
            else
                {
                _lettersKeyboard.updateList(_letters.words);
                }

            if (_actionsKeyboard == null)
                {
                _actionsKeyboard = new KeyboardWords(_actions.words);
                }
            else
                {
                _actionsKeyboard.updateList(_actions.words);
                }

            if (_wordsKeyboard == null)
                {
                _wordsKeyboard = new KeyboardWords(_words.words);
                }
            else
                {
                _wordsKeyboard.updateList(_words.words);
                }

            callback.onKeyboardUpdated();
            }

        public IList<Button> GetKeyboardList()
            {
            _activeKeyboard = getKeyboardType();
            return _activeKeyboard.GetKeyboardList();
            }

        public IList<Button> GetNextKeyboardList()
            {
            _activeKeyboard = getKeyboardType();
            return _activeKeyboard.GetNextKeyboardList();
            }

        public IList<Button> GetPreviousKeyboardList()
            {
            _activeKeyboard = getKeyboardType();
            return _activeKeyboard.GetPreviousKeyboardList();
            }

        private KeyboardWords getKeyboardType()
            {
            if(IsLetters)
                {
                return _lettersKeyboard;
                }
            if(IsActions)
                {
                return _actionsKeyboard;
                }

            return _wordsKeyboard;
            }

        public void Reset()
            {
            _activeKeyboard = getKeyboardType();
            _activeKeyboard.Clear();
            }
        }
    }
