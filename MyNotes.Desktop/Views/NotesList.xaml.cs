using System.Windows.Controls;
using MyNotes.Desktop.ViewModels;

namespace MyNotes.Desktop.Views
{
    /// <summary>
    /// Interaction logic for NotesList.xaml
    /// </summary>
    public partial class NotesList : UserControl
    {
        public NotesList()
        {
            InitializeComponent();

            DataContext = new NotesListViewModel();
        }
    }
}
