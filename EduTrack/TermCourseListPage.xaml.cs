//using System;
//using System.Collections.Generic;
//using Microsoft.Maui.Controls;
//using Microsoft.Maui;
using EduTrack.DB_Models;
//using System.Collections.Generic;
using System.Collections.ObjectModel;
namespace EduTrack
{
    public partial class TermCourseListPage : ContentPage
    {
        // variables declared here, can be initialized and used beyond TermCourseListPage construct.
        private ObservableCollection<Course> _courses;     
        private readonly DB_Interactions _dbInteractions;
        private Term _term;
        private bool _didUserSwipe = false;

        public TermCourseListPage(Term term)
        {
            InitializeComponent();
            //This line assigns the term parameter to the _term class field. If term is null,
            //it throws an ArgumentNullException. This ensures that the constructor parameter
            //is not null.
            _term = term ?? throw new ArgumentNullException(nameof(term));
            //==============================================================================

            // Initializes the class variable '_courses' by creating a new instance of 
            // ObservableCollection<Course> and assigning it to '_courses'
            _courses = new ObservableCollection<Course>();

            //==============================================================================

            string dbPath = AppConfig.DbPath; 
            _dbInteractions = new DB_Interactions(dbPath);

            //Sets the ItemsSource property of CollectionCourseView (in the xaml file) to the
            //_courses collection, binding the collection of courses to the UI element for display.
            CollectionCourseView.ItemsSource = _courses;
            //================================================================================
        }

        //Esures courses list/collection is reloaded everytime the screen appears.
        //No need to programmatically refresh the list!
        protected override void OnAppearing()  
        {
            base.OnAppearing();
            LoadCourses();
        }
        //========================================================================

        public async void LoadCourses()
        {
            var courses = await _dbInteractions.GetCoursesInTerm(_term.TermId);
            _courses.Clear();  //If you don't clear the courses here, duplicate courses will be added!
            foreach (var course in courses)
            {
                _courses.Add(course);
            }
        }

        //When swipping on a list/collection item, the swipe-flag is set to true
        //and the HandleCourse_Selected method is flagged for called and processed
        //BEFORE the swipe-flagg is set to false in the HandleSwipe_Ended method.
        private void HandleSwipe_Started(object sender, SwipeStartedEventArgs e)
        {
            _didUserSwipe = true;
        }

        private async void HandleCourse_Selected(object sender, SelectionChangedEventArgs e)
        {
            if (_didUserSwipe)
            {
                _didUserSwipe = false;
                CollectionCourseView.SelectedItem = null;
                return;
            }

        //The e in e.CurrentSelection.FirstOrDefault() refers to the SelectionChangedEventArgs e
        //parameter passed to the HandleCourse_Selected method. SelectionChangedEventArgs
        //provides data (i.e. arguments) for the SelectionChanged event, and its
        //CurrentSelection property contains the list of items that are currently selected.
        //     -- e: Represents the event arguments for the SelectionChanged event.
        //     -- e.CurrentSelection: A collection of the currently selected items.
        //     -- e.CurrentSelection.FirstOrDefault(): Retrieves the first item in the
        //        CurrentSelection collection, or returns null if the collection is empty.
            if (e.CurrentSelection.FirstOrDefault() is Course selectedCourse)
            {
                var courseDetailPage = new CourseDetailPage(selectedCourse);
                await Navigation.PushAsync(courseDetailPage);
                CollectionCourseView.SelectedItem = null;
            }
        //=======================================================================================
        }
       
        private void HandleSwipe_Ended(object sender, SwipeEndedEventArgs e)
        {
            _didUserSwipe = false; 
        }

        private async void HandleEditCourse_Swipe(object sender, EventArgs e)  //Edit-swipe handling method
        {
            if (((SwipeItem)sender).CommandParameter is Course courseToEdit)
            {
                var courseDetailsPage = new CourseDetailPage(courseToEdit);
                await Navigation.PushAsync(courseDetailsPage);
            }
        }

        private async void HandleDeleteCourse_Swipe(object sender, EventArgs e)  //Delete-swipe handling method
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

        private async void HandleAddCourse_Clicked(object sender, EventArgs e)  //Add new course method
        {
            if (_courses.Count >= 6)
            {
                await DisplayAlert("Limit Reached", "Only 6 courses are allowed per term. Please delete a course first.", "OK");
                return;
            }
            var newCourse = new Course { TermId = _term.TermId };   //Ensures new course will be assocuated with current term (via Term_D).
            var courseDetailPage = new CourseDetailPage(newCourse);
            await Navigation.PushAsync(courseDetailPage);
        }
    }
}