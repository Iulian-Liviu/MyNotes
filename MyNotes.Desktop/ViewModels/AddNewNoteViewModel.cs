using System;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyNotes.Desktop.Models;
using MyNotes.Desktop.Services;
using MessageBox = Xceed.Wpf.Toolkit.MessageBox;

namespace MyNotes.Desktop.ViewModels
{

    public partial class AddNewNoteViewModel : ObservableObject
    {
        private bool _isUpdate;
        [ObservableProperty]
        string? title;

        [ObservableProperty]
        string? description;


        [ObservableProperty]
        Guid noteId;

        public AddNewNoteViewModel()
        {

        }

        public AddNewNoteViewModel(Note noteUpdate, bool isUpdate = false)
        {
            NoteId = noteUpdate.NoteId;
            Title = noteUpdate.Title;
            Description = noteUpdate.Body;
            _isUpdate = isUpdate;
        }

        [RelayCommand]
        async void SaveAndClose(Window window)
        {
            if (!_isUpdate)
            {
                if (window.IsEnabled)
                {
                    if (Title != null && Description != null)
                    {
                        if (!IsStringEmpty(Title) && !IsStringEmpty(Description))
                        {
                            await DatabaseService.AddNote(new Models.NoteUpload
                            {
                                Title = Title,
                                Body = Description
                            });
                            window.Close();


                        }
                    }
                    else
                    {
                        ShowInformationMessage("Information.", "There was an unknown error. Please try again.");
                        return;
                    }
                }
            }
            else
            {
                if (Title != null && Description != null)
                {
                    if (!IsStringEmpty(Title) && !IsStringEmpty(Description))
                    {
                        var result = await DatabaseService.UpdateNote(NoteId, new Models.NoteUpload
                        {
                            Title = Title,
                            Body = Description
                        });

                        if (result == true)
                        {
                            return;
                        }
                        else
                        {
                            MessageBox.Show("Error");
                            return;
                        }
                    }
                }
                else
                {
                    ShowInformationMessage("Information.", "There was an unknown error. Please try again.");
                    return;
                }
            }
        }

        static bool IsStringEmpty(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return true;
            }
            return false;
        }
        static void ShowInformationMessage(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);

        }
    }
}
