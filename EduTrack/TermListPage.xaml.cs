using Microsoft.Maui.Hosting;
using System.Collections.ObjectModel;
using EduTrack.DB_Models;

namespace EduTrack
{
    public partial class TermListPage : ContentPage
    {
        private readonly DB_Interactions _dbInteractions;
        private ObservableCollection<Term> _terms;
        private bool _didUserSwipe = false;

        public TermListPage()
        {
            InitializeComponent();
            string dbPath = AppConfig.DbPath;
            _dbInteractions = new DB_Interactions(dbPath);
            _terms = new ObservableCollection<Term>();
            TermsListView.ItemsSource = _terms;
            LoadTerms();
        }

        protected override void OnAppearing()
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
                System.Diagnostics.Debug.WriteLine($"Retrieved Term: Name={term.Name}, StartDate={term.StartDate}, EndDate={term.EndDate}");
                _terms.Add(term);
                //await DisplayAlert("Term Details", $"Name: {term.Name}\nStart Date: {term.StartDate}\nEnd Date: {term.EndDate}", "OK");
            }
            
        }

        private async void OnAddTermClicked(object sender, EventArgs e)
        {
            var termDetailPage = new TermDetailPage();
            await Navigation.PushAsync(termDetailPage);
        }

        private async void OnTermSelected(object sender, SelectionChangedEventArgs e)
        {

            if (_didUserSwipe)
            {
                //EDIT swiping now works. Ironed out bugs 
                _didUserSwipe = false; // Reset swiping
                TermsListView.SelectedItem = null; // Deselect
                return;
            }

            if (e.CurrentSelection.FirstOrDefault() is Term selectedTerm)
            {
                var termCourseListPage = new TermCourseListPage(selectedTerm);
                await Navigation.PushAsync(termCourseListPage);
                TermsListView.SelectedItem = null;
            }
        }

        private void OnSwipeStarted(object sender, SwipeStartedEventArgs e)
        {
            _didUserSwipe = true;
        }

        private void OnSwipeEnded(object sender, SwipeEndedEventArgs e)
        {
            _didUserSwipe = false; // Reset swiping
        }

        private async void OnEditTermClicked(object sender, EventArgs e)
        {
            if (((SwipeItem)sender).CommandParameter is Term selectedTerm)
            {
                var termDetailPage = new TermDetailPage(selectedTerm);
                await Navigation.PushAsync(termDetailPage);
            }
        }

        private async void OnDeleteTermClicked(object sender, EventArgs e)
        {
            if (((SwipeItem)sender).CommandParameter is Term selectedTerm)
            {
                bool confirmDelete = await DisplayAlert("Confirm Delete", $"Are you sure you want to delete the term '{selectedTerm.Name}'?", "Yes", "No");
                if (confirmDelete)
                {
                    await _dbInteractions.DeleteTerm(selectedTerm);
                    LoadTerms();
                }
            }




        }
    }
}

