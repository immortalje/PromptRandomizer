using PromptRandomizer.Utils.Bases;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromptRandomizer.Models
{
    public class PromptList : PropertyChangedBase
    {
        private string _path = string.Empty;
        private List<string> _availablePrompts;
        private List<string> _unusedPrompts;
        private List<string> _persistedPrompts;

        public List<string> AllPrompts
        {
            get => _availablePrompts; 
            set
            { 
                _availablePrompts = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(IsModified));
            }
        }

        public List<string> UnusedPrompts 
        {
            get => _unusedPrompts;
            set
            {
                _unusedPrompts = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(IsModified));
            }
        }

        public bool IsModified => !_availablePrompts.SequenceEqual(_persistedPrompts);

        public void LoadPrompts(string path)
        {
            if (String.IsNullOrEmpty(path) || !File.Exists(path)) //Nothing to load, reset everything and return
            {
                AllPrompts = new List<string>();
                _path = path;
                return;
            }

            if (!String.IsNullOrEmpty(_path) && _path.Equals(path)) //If paths match, do nothing as data has already been loaded
                return;

            var fileContent = File.ReadAllLines(path); //Read file, one prompt per line
            if (fileContent?.Length > 0)
                AllPrompts = fileContent.ToList<string>(); //if prompts are available, set it to the list
            else
                AllPrompts = new List<string>(); //if no data is available, set the list to an empty list

            _persistedPrompts = new List<string>();
            _persistedPrompts.AddRange(AllPrompts);
            _path = path; //remember the path to avoid unnecessary loading

            NotifyPropertyChanged(nameof(IsModified));
        }

        public void FilterPrompts(PromptList usedPrompts)
        {
            var _filteredPrompts = new List<string>();
            foreach (var prompt in AllPrompts)
                if (!usedPrompts.AllPrompts.Contains(prompt))
                    _filteredPrompts.Add(prompt);

            UnusedPrompts = _filteredPrompts;

            NotifyPropertyChanged(nameof(IsModified));
        }

        public void UpdateAllPrompts()
        {
            var tmp = new List<string>();
            foreach (var prompt in AllPrompts)
                tmp.Add(prompt);

            AllPrompts = tmp;
            NotifyPropertyChanged(nameof(AllPrompts));
            NotifyPropertyChanged(nameof(IsModified));
        }

        public void SavePrompts()
        {
            if (!Directory.Exists(Path.GetDirectoryName(_path)))
                Directory.CreateDirectory(Path.GetDirectoryName(_path));

            File.WriteAllLines(_path, AllPrompts);

            _persistedPrompts = new List<string>();
            _persistedPrompts.AddRange(AllPrompts);

            NotifyPropertyChanged(nameof(IsModified));
        }
    }
}
