using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyNotes.Desktop.Models;
using MyNotes.Desktop.Services;
using MyNotes.Desktop.Views;
using Xceed.Wpf.Toolkit;

namespace MyNotes.Desktop.ViewModels {
    public partial class NotesListViewModel : ObservableObject {
        private ObservableCollection<Note>? notes;

        [ObservableProperty]
        bool isLoading;

        [ObservableProperty]
        Note? selectedNote;


        private Dispatcher _dispatcher = Dispatcher.CurrentDispatcher;
        private AddNewNoteWindow? addNewNote;

        public NotesListViewModel() {
            IsLoading = false;
            SelectedNote = null;
            notes = new ObservableCollection<Note>();
            DatabaseService.DatabaseOperation += DatabaseService_DatabaseOperation;
            LoadDataAsync();
        }

        [RelayCommand]
        void OpenNewNoteWindow() {
            addNewNote = new AddNewNoteWindow();

            addNewNote.ShowDialog();

        }

        [RelayCommand]
        async void RemoveSelectedNote() {
            if (SelectedNote != null) {
                var response = MessageBox.Show("Do you really want to delete this note.", "Delete Note?", System.Windows.MessageBoxButton.YesNo);
                if (response == System.Windows.MessageBoxResult.Yes) {
                    await DatabaseService.RemoveNote(SelectedNote.NoteId);
                    SelectedNote = null;
                }
                else {
                    return;
                }
            }
        }

        [RelayCommand]
        void UpdateSelectedNote() {
            if (SelectedNote != null) {
                addNewNote = new AddNewNoteWindow(SelectedNote);
                addNewNote.ShowDialog();

            }
        }

        private void DatabaseService_DatabaseOperation(object? sender, System.EventArgs e) {
            LoadDataAsync();
        }

        public ObservableCollection<Note>? Notes {
            get { return notes; }
            set { SetProperty(ref notes, value); }
        }
        private async void LoadDataAsync() {
            // Show a loading indicator if needed
            IsLoading = true;
            var data = await DatabaseService.GetAllNotes();

            // Update view model on UI thread
            await _dispatcher.InvokeAsync(() => {

                // Hide loading indicator if needed
                CheckAndInsert(data);
                IsLoading = false;

            });
        }

        private void CheckAndInsert(List<Note> data) {
            if (Notes != null) {
                Notes.Clear();
                foreach (var item in data) {

                    if (item != null && CheckIsXaml(item.Body!)) {
                        Notes.Add(item);
                    }
                    else if (string.IsNullOrEmpty(item!.Body)) {
                        item.Body = "";
                        Notes.Add(item);
                    }

                }
            }
        }

        private bool CheckIsXaml(string xamlString) {

            bool containsSection = XamlCheck().IsMatch(xamlString);
            if (containsSection) {
                return true;
            }
            else {
                return false;
            }
        }

        [GeneratedRegex("<Section\\s")]
        private static partial Regex XamlCheck();
    }


}
