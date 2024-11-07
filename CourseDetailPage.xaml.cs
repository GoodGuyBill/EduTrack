using EduTrack.DB_Models;
using System.Collections.ObjectModel;
using System.Diagnostics;
namespace EduTrack;

public partial class CourseDetailPage : ContentPage
{
    private readonly DB_Interactions _dbInteractions;
    private Course _course;
    private Term _term;
    private ObservableCollection<Assessment> _assessments;

    private readonly int _termId;
    private readonly int _courseId;
    bool _didUserSwipe;


    //public CourseDetailPage(Term term)
    //{
    //    InitializeComponent();
    //    _term = term;
    //    string dbPath = AppConfig.DbPath;
    //    _dbInteractions = new DB_Interactions(dbPath);
    // }


    


    public CourseDetailPage(Course selectedCourse)
    {
        InitializeComponent();
        _termId = selectedCourse.TermId;
        _courseId = selectedCourse.CourseId;
        _course = selectedCourse;
        _assessments = new ObservableCollection<Assessment>();
        AssessmentsCollectionView.ItemsSource = _assessments;

        BindingContext = this;
        string dbPath = AppConfig.DbPath;
        _dbInteractions = new DB_Interactions(dbPath);
        LoadAssessments();

        if (selectedCourse != null)
        {
            CourseNameEntry.Text = selectedCourse.Name;
            CourseStartDatePicker.Date = selectedCourse.StartDate;
            CourseEndDatePicker.Date = selectedCourse.EndDate;
            CourseStatusPicker.SelectedItem = selectedCourse.Status;
            InstructorNameEntry.Text = selectedCourse.InstructorName;
            InstructorPhoneEntry.Text = selectedCourse.InstructorPhone;
            InstructorEmailEntry.Text = selectedCourse.InstructorEmail;
            NotifyStartCheckBox.IsChecked = selectedCourse.NotifyStart;
            NotifyEndCheckBox.IsChecked = selectedCourse.NotifyEnd;
            NotesEditor.Text = selectedCourse.Notes;
        }
        
        // app is adding a new course and therefore need to set the datepickers to today.
        if (selectedCourse.CourseId == 0)
        {
            CourseStartDatePicker.Date = DateTime.Today;
            CourseEndDatePicker.Date = DateTime.Today;
        }


    }




    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadAssessments();
    }


    private async void LoadAssessments()
    {
        try
        {
            if (_dbInteractions == null) 
            { 
                Debug.WriteLine("Error: _dbInteractions is null"); return; 
            }
            
            Debug.WriteLine("LoadAssessments: Before await");
            var assessments = await _dbInteractions.GetAssessments(_course.CourseId);
            Debug.WriteLine("LoadAssessments: After await");

            if (assessments == null || assessments.Count == 0)
            {
                // Display an alert if no assessments are found
                await DisplayAlert("No Assessments", "There are no assessments for this course.", "OK");
                return;
            }

            _assessments.Clear();
            foreach (var assessment in assessments)
            {
                _assessments.Add(assessment);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error: {ex.Message}");
        }
    }


    private void OnSwipeStarted(object sender, SwipeStartedEventArgs e)
    {
        _didUserSwipe = true;
    }

    private void OnSwipeEnded(object sender, SwipeEndedEventArgs e)
    {
        _didUserSwipe = false;
    }

    private async void OnEditAssessmentSwipe(object sender, EventArgs e)
    {
        if (_didUserSwipe) return;

        if (((SwipeItem)sender).CommandParameter is Assessment assessmentToEdit)
        {
            await Navigation.PushAsync(new AssessmentDetailPage(assessmentToEdit));
        }
    }


    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (!ValidateCourseData())
        {
            return;
        }

        if (_course == null)
        {
            _course = new Course
            {
                TermId = _term.TermId
            };
        }
        _course.Name = CourseNameEntry.Text;
        _course.StartDate = CourseStartDatePicker.Date;
        _course.EndDate = CourseEndDatePicker.Date;
        _course.Status = CourseStatusPicker.SelectedItem?.ToString();
        _course.InstructorName = InstructorNameEntry.Text;
        _course.InstructorPhone = InstructorPhoneEntry.Text;
        _course.InstructorEmail = InstructorEmailEntry.Text;
        _course.NotifyStart = NotifyStartCheckBox.IsChecked;
        _course.NotifyEnd = NotifyEndCheckBox.IsChecked;
        _course.Notes = NotesEditor.Text;

        // Add prompt to confirm save here.
        await _dbInteractions.SaveCourse(_course);
        await DisplayAlert("Success", "The course was saved!", "OK");

        //Update the TermCourseListPage data
        if (Navigation.NavigationStack.LastOrDefault() is TermCourseListPage termCourseListPage)
        {
            termCourseListPage.LoadCourses();
        }

        //Go back to the TermsCourseListPage.
        await Navigation.PopAsync(); 
    }

    private async void OnShareNotesClicked(object sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(NotesEditor.Text))
        {
            await Share.RequestAsync(new ShareTextRequest
            {
                Text = NotesEditor.Text,
                Title = "Share Course Notes"
            });
        }
    }
    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (_course != null)
        {
            bool PermissionToDelete = await DisplayAlert("Confirm Delete", "Please confirm course deletion?", "Yes", "No");
            if (PermissionToDelete)
            {
                await _dbInteractions.DeleteCourse(_course);

                // update courses page to remove course from the list
                if (Navigation.NavigationStack.LastOrDefault() is TermCourseListPage termCourseListPage)
                {
                    termCourseListPage.LoadCourses();
                }

                await DisplayAlert("Success", "Course deleted successfully!", "OK");
                await Navigation.PopAsync();
            }
        }
    }

    private async void OnDeleteAssessmentSwipe(object sender, EventArgs e)
    {
        if (_didUserSwipe) return;

        if (((SwipeItem)sender).CommandParameter is Assessment assessmentToDelete)
        {
            bool confirmDelete = await DisplayAlert("Confirm Delete", $"Are you sure you want to delete {assessmentToDelete.Name}?", "Yes", "No");
            if (confirmDelete)
            {
                await _dbInteractions.DeleteAssessment(assessmentToDelete);
                _assessments.Remove(assessmentToDelete);
                await DisplayAlert("Deleted", $"{assessmentToDelete.Name} has been deleted.", "OK");
            }
        }
    }


    private async void OnAddAssessmentClicked(object sender, EventArgs e)
    {
        
        var newAssessment = new Assessment
        {
            CourseId = _course.CourseId
        };
        await Navigation.PushAsync(new AssessmentDetailPage(newAssessment));
    }


    private bool ValidateCourseData()
    {
        
        if (string.IsNullOrWhiteSpace(CourseNameEntry.Text))
        {
            DisplayAlert("Validation Error", "A course name is required.", "OK");
            return false;
        }

        
        if (string.IsNullOrWhiteSpace(CourseStatusPicker.SelectedItem?.ToString()))
        {
            DisplayAlert("Validation Error", "A course status is required.", "OK");
            return false;
        }

        
        if (string.IsNullOrWhiteSpace(InstructorNameEntry.Text))
        {
            DisplayAlert("Validation Error", "The instructor's name is required.", "OK");
            return false;
        }

        
        if (string.IsNullOrWhiteSpace(InstructorPhoneEntry.Text))
        {
            DisplayAlert("Validation Error", "The instructor's phone number is required.", "OK");
            return false;
        }

        
        if (string.IsNullOrWhiteSpace(InstructorEmailEntry.Text))
        {
            DisplayAlert("Validation Error", "The instructor's email address is required.", "OK");
            return false;
        }

        
        if (CourseStartDatePicker.Date >= CourseEndDatePicker.Date)
        {
            DisplayAlert("Validation Error", "The course's start date must be before the end date.", "OK");
            return false;
        }

        return true; // If code gets to this point, no validation issues were identified.
    }







}





