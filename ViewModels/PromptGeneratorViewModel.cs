using Prism.Commands;
using PromptRandomizer.Models;
using PromptRandomizer.Utils.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PromptRandomizer.ViewModels
{
    public class PromptGeneratorViewModel : PropertyChangedBase
    {
        private PromptList _availablePrompts;
        private PromptList _usedPrompts;

        private string _selectedAvailablePrompt;
        private string _selectedUsedPrompt;

        private int _maxCountUsedPrompts;

        public PromptGeneratorViewModel()
        {
            MaxCountUsedPrompts = 150;

            UsedPrompts = new PromptList();
            UsedPrompts.LoadPrompts(UsedPromptsFile);

            AvailablePrompts = new PromptList();
            AvailablePrompts.LoadPrompts(DefaultPromptsFile);
            AvailablePrompts.FilterPrompts(UsedPrompts);

            if (AvailablePrompts.UnusedPrompts.Count > 0)
                SelectedAvailablePrompt = AvailablePrompts.UnusedPrompts.First();

            if (UsedPrompts.AllPrompts.Count > 0)
                SelectedUsedPrompt = UsedPrompts.AllPrompts.Last();

            PickRandomPromptCommand = new DelegateCommand(PickRandomPromptExecute, PickRandomPromptCanExecute);
            UsePromptCommand = new DelegateCommand(UsePromptExecute, UsePromptCanExecute).ObservesProperty(() => SelectedAvailablePrompt);
            RemoveUsedPromptCommand = new DelegateCommand(RemoveUsedPromptExecute, RemoveUsedPromptCanExecute).ObservesProperty(() => SelectedUsedPrompt);
            ClearAllAvailablePromptsCommand = new DelegateCommand(ClearAllAvailablePromptsExecute, ClearAllAvailablePromptsCanExecute);
            ClearAllUsedPromptsCommand = new DelegateCommand(ClearAllUsedPromptsExecute, ClearAllUsedPromptsCanExecute);
            AddAvailablePromptCommand = new DelegateCommand(AddAvailablePromptExecute, AddAvailablePromptCanExecute);
            EditAvailablePromptCommand = new DelegateCommand(EditAvailablePromptExecute, EditAvailablePromptCanExecute).ObservesProperty(() => SelectedAvailablePrompt);
            SaveAvailablePromptsCommand = new DelegateCommand(SaveAvailablePromptsExecute, SaveAvailablePromptsCanExecute).ObservesProperty(() => AvailablePrompts.IsModified);
            SaveUsedPromptsCommand = new DelegateCommand(SaveUsedPromptsExecute, SaveUsedPromptsCanExecute).ObservesProperty(() => UsedPrompts.IsModified);
        }

        #region UI Properties
        public string UsedPromptsFile { get; set; } = @"Files\UsedPrompts.txt";
        public string DefaultPromptsFile { get; set; } = @"Files\DefaultPrompts.txt";

        public PromptList AvailablePrompts
        {
            get => _availablePrompts;
            set
            {
                _availablePrompts = value;
                NotifyPropertyChanged();
            }
        }

        public string SelectedAvailablePrompt
        {
            get => _selectedAvailablePrompt;
            set
            {
                _selectedAvailablePrompt = value;
                NotifyPropertyChanged();
            }
        }

        public PromptList UsedPrompts
        {
            get => _usedPrompts;
            set
            {
                _usedPrompts = value;
                NotifyPropertyChanged();
            }
        }

        public string SelectedUsedPrompt
        {
            get => _selectedUsedPrompt;
            set
            {
                _selectedUsedPrompt = value;
                NotifyPropertyChanged();
            }
        }

        public int MaxCountUsedPrompts
        {
            get => _maxCountUsedPrompts;
            set
            {
                _maxCountUsedPrompts = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        #region Command Properties
        /// <summary>
        /// Picks a Random Prompt
        /// </summary>
        public DelegateCommand PickRandomPromptCommand { get; set; }

        /// <summary>
        /// Picks the currently selected available prompt
        /// </summary>
        public DelegateCommand UsePromptCommand { get; set; }

        /// <summary>
        /// Removes the currently selected used prompt and adds it to the selected list again
        /// </summary>
        public DelegateCommand RemoveUsedPromptCommand { get; set; }

        /// <summary>
        /// Removes all prompts in the available prompts
        /// </summary>
        public DelegateCommand ClearAllAvailablePromptsCommand { get; set; }

        /// <summary>
        /// Removes all prompts in the used prompts
        /// </summary>
        public DelegateCommand ClearAllUsedPromptsCommand { get; set; }

        public DelegateCommand AddAvailablePromptCommand { get; set; }

        public DelegateCommand EditAvailablePromptCommand { get; set; }

        public DelegateCommand SaveAvailablePromptsCommand { get; set; }

        public DelegateCommand SaveUsedPromptsCommand { get; set; }
        #endregion

        #region Command Implementation
        private void PickRandomPromptExecute()
        {
            var rdm = new Random();
            var index = rdm.Next(0, AvailablePrompts.UnusedPrompts.Count);

            RemoveFirstIfNeeded(UsedPrompts.AllPrompts);

            RemoveFirstIfNeeded(UsedPrompts.AllPrompts);
            UsedPrompts.AllPrompts.Add(AvailablePrompts.UnusedPrompts[index]);
            AvailablePrompts.FilterPrompts(UsedPrompts);
            UsedPrompts.UpdateAllPrompts();

            NotifyPropertyChanged(nameof(UsedPrompts));
            NotifyPropertyChanged(nameof(AvailablePrompts));

            SelectedUsedPrompt = UsedPrompts.AllPrompts.Last();
        }

        private bool PickRandomPromptCanExecute()
        {
            if (AvailablePrompts?.UnusedPrompts == null)
                return false;

            return (AvailablePrompts.UnusedPrompts.Count > 0);
        }

        private void UsePromptExecute()
        {
            RemoveFirstIfNeeded(UsedPrompts.AllPrompts);
            UsedPrompts.AllPrompts.Add(SelectedAvailablePrompt);

            var index = AvailablePrompts.UnusedPrompts.IndexOf(SelectedAvailablePrompt);
            AvailablePrompts.FilterPrompts(UsedPrompts);
            UsedPrompts.UpdateAllPrompts();

            NotifyPropertyChanged(nameof(UsedPrompts));
            NotifyPropertyChanged(nameof(AvailablePrompts));

            if(index >=0 && index < AvailablePrompts.UnusedPrompts.Count)
                SelectedAvailablePrompt = AvailablePrompts.UnusedPrompts[index];

            SelectedUsedPrompt = UsedPrompts.AllPrompts.Last();

            //SaveAvailablePromptsCommand.RaiseCanExecuteChanged();
            //SaveUsedPromptsCommand.RaiseCanExecuteChanged();
        }

        private bool UsePromptCanExecute()
        {
            return SelectedAvailablePrompt != null;
        }

        private void RemoveUsedPromptExecute()
        {
            var index = UsedPrompts.AllPrompts.IndexOf(SelectedUsedPrompt);

            var prompts = UsedPrompts.AllPrompts;
            prompts.Remove(SelectedUsedPrompt);
            UsedPrompts.AllPrompts = prompts;
            AvailablePrompts.FilterPrompts(UsedPrompts);
            UsedPrompts.UpdateAllPrompts();

            NotifyPropertyChanged(nameof(UsedPrompts));
            NotifyPropertyChanged(nameof(AvailablePrompts));

            if (index == UsedPrompts.AllPrompts.Count) //if the last element was deleted, set the index to the new last element
                index -= 1;

            if (index >= 0 && index < UsedPrompts.AllPrompts.Count)
                SelectedUsedPrompt = UsedPrompts.AllPrompts[index];

            //SaveAvailablePromptsCommand.RaiseCanExecuteChanged();
            //SaveUsedPromptsCommand.RaiseCanExecuteChanged();
        }

        private bool RemoveUsedPromptCanExecute()
        {
            return SelectedUsedPrompt != null;
        }

        private void ClearAllAvailablePromptsExecute()
        {
            AvailablePrompts.AllPrompts.Clear();
            AvailablePrompts.FilterPrompts(UsedPrompts);

            NotifyPropertyChanged(nameof(AvailablePrompts));

            //SaveAvailablePromptsCommand.RaiseCanExecuteChanged();
            //SaveUsedPromptsCommand.RaiseCanExecuteChanged();
        }

        private bool ClearAllAvailablePromptsCanExecute()
        {
            return AvailablePrompts.AllPrompts.Count > 0;
        }

        private void ClearAllUsedPromptsExecute()
        {
            UsedPrompts.AllPrompts.Clear();
            AvailablePrompts.FilterPrompts(UsedPrompts);

            NotifyPropertyChanged(nameof(UsedPrompts));

            //SaveAvailablePromptsCommand.RaiseCanExecuteChanged();
            //SaveUsedPromptsCommand.RaiseCanExecuteChanged();
        }

        private bool ClearAllUsedPromptsCanExecute()
        {
            return UsedPrompts.AllPrompts.Count > 0;
        }

        private void AddAvailablePromptExecute()
        {
            throw new NotImplementedException();
        }

        private bool AddAvailablePromptCanExecute()
        {
            return !String.IsNullOrEmpty(SelectedAvailablePrompt);
        }

        private void EditAvailablePromptExecute()
        {
            throw new NotImplementedException();
        }

        private bool EditAvailablePromptCanExecute()
        {
            return !String.IsNullOrEmpty(SelectedAvailablePrompt);
        }

        private void SaveAvailablePromptsExecute()
        {
            AvailablePrompts.SavePrompts();
        }

        private bool SaveAvailablePromptsCanExecute()
        {
            return AvailablePrompts.IsModified;
        }

        private void SaveUsedPromptsExecute()
        {
            UsedPrompts.SavePrompts();
        }

        private bool SaveUsedPromptsCanExecute()
        {
            return UsedPrompts.IsModified;
        }
        #endregion

        #region Utils
        private void RemoveFirstIfNeeded(List<string> prompts)
        {
            if (prompts.Count + 1 > MaxCountUsedPrompts)
                prompts.RemoveAt(0);
        }

        #endregion

    }
}
