using EduTrack.DB_Models;
namespace EduTrack
{
    public partial class TermDetailPage : ContentPage
    {
        private Term _term;
        private readonly DB_Interactions _dbInteractions;

        public TermDetailPage()
        {
            InitializeComponent();
            string dbPath = AppConfig.DbPath;
            _dbInteractions = new DB_Interactions(dbPath);
        }

        public TermDetailPage(Term term)
        {
            InitializeComponent();
            _term = term;
            string dbPath = AppConfig.DbPath;
            _dbInteractions = new DB_Interactions(dbPath);

            if (_term != null)
            {
                TitleName.Text = _term.Name;
                StartDatePicker.Date = _term.StartDate;
                EndDatePicker.Date = _term.EndDate;
            }
        }



        private async void OnSaveClicked(object sender, EventArgs e)
        {
            if (_term == null)
            {
                _term = new Term();
            }

            // Set term values
            _term.Name = TitleName.Text;
            _term.StartDate = StartDatePicker.Date;
            _term.EndDate = EndDatePicker.Date;

            await _dbInteractions.SaveTerm(_term);
            await Navigation.PopAsync();
        }

        private async void OnDeleteClicked(object sender, EventArgs e)
        { 
            if (_term != null)
                {
                    bool confirmDelete = await DisplayAlert("Confirm Delete", "Are you sure you want to delete this term?", "Yes", "No");
                    if (confirmDelete)
                    {
                        await _dbInteractions.DeleteTerm(_term);
                        await DisplayAlert("Success", "Term deleted successfully!", "OK");
                        await Navigation.PopAsync();
                    }
                }
        }
    }
}