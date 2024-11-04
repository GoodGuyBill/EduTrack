using System;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using Microsoft.Maui;
using EduTrack.DB_Models;
using System.Collections.ObjectModel;
namespace EduTrack
{
    public partial class TermCourseListPage : ContentPage
    {
        private ObservableCollection<Course> _courses;
        private readonly DB_Interactions _dbInteractions;
        private Term _term;
        private bool _didUserSwipe = false;

        public TermCourseListPage(Term term)
        {
            InitializeComponent();
            //_term = term;
            _term = term ?? throw new ArgumentNullException(nameof(term));
            _courses = new ObservableCollection<Course>();
            string dbPath = AppConfig.DbPath; 
            _dbInteractions = new DB_Interactions(dbPath);
            CollectionCourseView.ItemsSource = _courses;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadCourses();
        }


        public async void LoadCourses()
        {
            var courses = await _dbInteractions.GetCoursesInTerm(_term.TermId);
            _courses.Clear();
            foreach (var course in courses)
            {
                _courses.Add(course);
            }
        }

        private async void OnCourseSelected(object sender, SelectionChangedEventArgs e)
        {
            if (_didUserSwipe)
            {

                _didUserSwipe = false;
                CollectionCourseView.SelectedItem = null;
                return;
            }

            if (e.CurrentSelection.FirstOrDefault() is Course selectedCourse)
            {
                var courseDetailPage = new CourseDetailPage(selectedCourse);
                await Navigation.PushAsync(courseDetailPage);
                CollectionCourseView.SelectedItem = null;
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

        private async void OnEditCourseSwipe(object sender, EventArgs e)
        {
            if (((SwipeItem)sender).CommandParameter is Course courseToEdit)
            {
                var courseDetailsPage = new CourseDetailPage(courseToEdit);
                await Navigation.PushAsync(courseDetailsPage);
            }
        }

        private async void OnDeleteCourseSwipe(object sender, EventArgs e)
        {
            if (((SwipeItem)sender).CommandParameter is Course courseToDelete)
            {
                bool confirmDelete = await DisplayAlert("Confirm Delete", $"Are you sure you want to delete {courseToDelete.Name}?", "Yes", "No");
                if (confirmDelete)
                {
                    await _dbInteractions.DeleteCourse(courseToDelete);
                    _courses.Remove(courseToDelete);
                    await DisplayAlert("Deleted", $"{courseToDelete.Name} has been deleted.", "OK");
                }
            }
        }


        private async void OnAddCourseClicked(object sender, EventArgs e)
        {
            if (_courses.Count >= 6)
            {
                await DisplayAlert("Limit Reached", "You cannot add more than 6 courses to a term.", "OK");
                return;
            }

            //var courseDetailsPage = new CourseDetailPage(_term);
            //var courseDetailsPage = new CourseDetailPage(_course);
            var newCourse = new Course { TermId = _term.TermId }; 
            var courseDetailPage = new CourseDetailPage(newCourse);
            await Navigation.PushAsync(courseDetailPage);
        }

    }
}