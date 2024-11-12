//using Microsoft.Maui.Hosting;
using System.Collections.ObjectModel;
using EduTrack.DB_Models;

namespace EduTrack
{
    public partial class TermListPage : ContentPage
    {
        private readonly DB_Interactions _dbInteractions;
        private ObservableCollection<Term> _terms;

        private bool _didUserSwipe = false;   // used to determine if user right-swipped term record on term list.


        public TermListPage()
        {
            InitializeComponent();
            string dbPath = AppConfig.DbPath;
            _dbInteractions = new DB_Interactions(dbPath);
            _terms = new ObservableCollection<Term>();
            TermsListView.ItemsSource = _terms;
            LoadTerms();
        }

        protected override void OnAppearing()   //loads term list data at each 'landing' on the TermListPage.
        {
            base.OnAppearing();
            LoadTerms();
        }

        private async void LoadTerms()
        {
            var terms = await _dbInteractions.GetTerms();
            System.Diagnostics.Debug.WriteLine("********* LOAD TERMS ********* Terms Retrieved Post-Insertion:");

            _terms.Clear(); 
            foreach (var term in terms) 
            {
                System.Diagnostics.Debug.WriteLine($"Retrieved Term: ID={term.TermId}, Name={term.Name}, StartDate={term.StartDate}, EndDate={term.EndDate}");
                _terms.Add(term);
            }            
        }

        private async void HandleTermSelection(object sender, SelectionChangedEventArgs e)
        {
            if (_didUserSwipe)
            {
                _didUserSwipe = false; 
                TermsListView.SelectedItem = null; 
                return;
            }

            if (e.CurrentSelection.FirstOrDefault() is Term selectedTerm)
            {
                var termCourseListPage = new TermCourseListPage(selectedTerm);
                await Navigation.PushAsync(termCourseListPage);
                TermsListView.SelectedItem = null;
            }
        }

        private void HandleSwipeStarted(object sender, SwipeStartedEventArgs e)
        {
            _didUserSwipe = true;
        }

        private void HandleSwipeEnded(object sender, SwipeEndedEventArgs e)
        {
            _didUserSwipe = false;
        }

        private async void HandleAddTerm_Clicked(object sender, EventArgs e)
        {
            var termDetailPage = new TermDetailPage();
            await Navigation.PushAsync(termDetailPage);
        }

        private async void HandleEditTerm_Swiped(object sender, EventArgs e)
        {
            if (((SwipeItem)sender).CommandParameter is Term selectedTerm)
            {
                var termDetailPage = new TermDetailPage(selectedTerm);
                await Navigation.PushAsync(termDetailPage);
            }
        }

        private async void HandleDeleteTerm_Swiped(object sender, EventArgs e)
        {
            if (((SwipeItem)sender).CommandParameter is Term selectedTerm)
            {
                bool confirmDelete = await DisplayAlert("Confirm Delete", $"Please confirm the '{selectedTerm.Name}'term deletion", "Yes", "No");
                if (confirmDelete)
                {
                    // The section gets a list of courses associated with the term
                    // and deletes the courses before deleting the term. Otherwise,
                    // in the real world, there would be numerous orphaned assessments.
                    List<Course> courses = await _dbInteractions.GetCoursesInTerm(selectedTerm.TermId);

                    foreach (var course in courses)
                    {
                        await _dbInteractions.DeleteCourse(course);
                    }

                    await _dbInteractions.DeleteTerm(selectedTerm);
                    LoadTerms();
                }
            }
        }
    }
}

