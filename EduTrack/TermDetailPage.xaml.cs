using EduTrack.DB_Models;
namespace EduTrack
{
    public partial class TermDetailPage : ContentPage
    {
        private Term _term;
        private readonly DB_Interactions _dbInteractions;
        private List<Term> existingTerms;
        private int termCount;

        public TermDetailPage()
        {
            InitializeComponent();
            string dbPath = AppConfig.DbPath;
            _dbInteractions = new DB_Interactions(dbPath);
        }

        public TermDetailPage(Term term)
        {
            InitializeComponent();

            //assigns the contructor parameter 'term' to the class variable "_term",
            //which is necessary if you want the term class, _term, to be accessible 
            //to other methods outside of the constructor scope.
            _term = term; 
            //======================================================================

            string dbPath = AppConfig.DbPath;
            _dbInteractions = new DB_Interactions(dbPath);

            if (_term != null)
            {
                TermName.Text = _term.Name;
                StartDatePicker.Date = _term.StartDate;
                EndDatePicker.Date = _term.EndDate;
            }
        }



        private async void HandleTermSave_Clicked(object sender, EventArgs e)
        {
            //Validations: 
            //#1: Ensure no more than 6 terms exist
            existingTerms = await _dbInteractions.GetTerms();
            if (existingTerms.Count >= 6)
            {
                await DisplayAlert("Validation Error", "A limit of six terms is already reached. You cannot add another term.", "OK");
                return;
            }

            //  #1: Ensure start-date is before end-date?
            if (StartDatePicker.Date > EndDatePicker.Date)
            {
                await DisplayAlert("Validation Error:", "The term cannot start after its end date. Please ensure start-date is BEFORE end-date. ", "OK");
                StartDatePicker.Focus();
                return;
            }

            // #2: Ensure Term name is not empty.
            if (string.IsNullOrWhiteSpace(TermName.Text))
            {
                await DisplayAlert("Validation Error", "Please enter a term name.", "OK");
                TermName.Focus();
                return;
            }

                // move form fields to _term construct and save data to database.
                if (_term == null)  //A null term indicates a new term was added.
            {
                _term = new Term();
            }
            //Prior to saving term data, move the data from the screen form to set the _term construct.
            _term.Name = TermName.Text;
            _term.StartDate = StartDatePicker.Date;
            _term.EndDate = EndDatePicker.Date;

            //calling the SaveTerm() method which saves the data to the database and then navigates to the previous page.
            await _dbInteractions.SaveTerm(_term);
            await Navigation.PopAsync();
        }

        private async void HandleTermDelete_Clicked(object sender, EventArgs e)
        { 
            if (_term != null)
                {
                    bool confirmDelete = await DisplayAlert("Confirm Delete", "Please confirm that you want to delete this term?", "Yes", "No");
                    if (confirmDelete)
                    {
                        // The section gets a list of assessments associated with the course
                        // and deletes them before deleting the course. Otherwise, in the real world, there would be 
                        // numerous orphaned assessments.
                        List<Course> courses = await _dbInteractions.GetCoursesInTerm(_term.TermId);

                        foreach (var course in courses)
                        {
                        await _dbInteractions.DeleteCourse(course);
                        }

                        await _dbInteractions.DeleteTerm(_term);
                        await DisplayAlert("Success", "The term and all associated courses have been deleted.", "OK");
                        await Navigation.PopAsync();
                    }
                }
        }
    }
}