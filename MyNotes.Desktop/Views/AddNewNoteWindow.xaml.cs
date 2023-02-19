using MahApps.Metro.Controls;
using MyNotes.Desktop.Models;
using MyNotes.Desktop.ViewModels;

namespace MyNotes.Desktop.Views
{
    /// <summary>
    /// Interaction logic for AddNewNoteWindow.xaml
    /// </summary>
    public partial class AddNewNoteWindow : MetroWindow
    {
        public AddNewNoteWindow()
        {
            InitializeComponent();

            DataContext = new AddNewNoteViewModel();
        }

        public AddNewNoteWindow(Note noteUpload)
        {
            InitializeComponent();

            DataContext = new AddNewNoteViewModel(noteUpload, true);
        }
    }
}
