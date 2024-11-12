using EduTrack.DB_Models;
//using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Plugin.LocalNotification;
//using System.Threading.Tasks;
//using Microsoft.Maui.Controls;
using System.Text;
//using Android.App;
//using static Android.Util.EventLogTags;
//using static Android.InputMethodServices.Keyboard;
namespace EduTrack;

public partial class CourseDetailPage : ContentPage
{
    //Variables declared here can be used in and outside of the CourseDetailPage class constructor.
    private readonly DB_Interactions _dbInteractions;
    private Course _course;
    private Term _term;
    private ObservableCollection<Assessment> _assessments;

    private readonly int _termId;
    private readonly int _courseId;
    bool _didUserSwipe;
    private int _assessmentCount;

    public CourseDetailPage(Course selectedCourse)
    {
        InitializeComponent();

        _termId = selectedCourse.TermId;
        _courseId = selectedCourse.CourseId;
        _course = selectedCourse;
        _assessments = new ObservableCollection<Assessment>();
        AssessmentsCollectionView.ItemsSource = _assessments;

        string dbPath = AppConfig.DbPath;
        _dbInteractions = new DB_Interactions(dbPath);

        //BindingContext = this;

        //Assessments = new ObservableCollection<Assessment>();
        LoadAssessments();


        // editing a course, so move the data from the selectedCourse class variable
        // parameter to the edit form field.
        if (selectedCourse != null)
        {
            CourseNameInput.Text = selectedCourse.Name;
            CourseStartDatePicker.Date = selectedCourse.StartDate;
            CourseEndDatePicker.Date = selectedCourse.EndDate;
            CourseStatusPicker.SelectedItem = selectedCourse.Status;
            InstructorNameInput.Text = selectedCourse.InstructorName;
            InstructorPhoneInput.Text = selectedCourse.InstructorPhone;
            InstructorEmailInput.Text = selectedCourse.InstructorEmail;
            NotifyStartCheckBox.IsChecked = selectedCourse.NotifyStart;
            NotifyEndCheckBox.IsChecked = selectedCourse.NotifyEnd;
            NotesEditor.Text = selectedCourse.Notes;
        }

        //User is adding a new course and therefore need to set the datepickers to today.
        //without this code, the datepickers will display a default date of 1/1/1900,
        //making the datepicker unusable.
        if (selectedCourse.CourseId == 0)
        {
            CourseStartDatePicker.Date = DateTime.Today;
            CourseEndDatePicker.Date = DateTime.Today;
        }
    }

    protected override void OnAppearing()
    {
        //ensures that assessment list will automatically reload when the the
        //CourseDetailPage appears, without the need to programatically
        //reset/reinitialize the list.
        base.OnAppearing();
        LoadAssessments();
    }

    private async void LoadAssessments()
    {
        //MAJOR issues (understatement!) with this routine! Initially, I did not declare
        //_dbInteractions outside of the CourseDetailPage class constructor. So it was
        //null did not contain the database path... but it flagged as an error.      
        try
        {
            if (_dbInteractions == null)
            {
                Debug.WriteLine("Error: _dbInteractions is null"); return;
            }

            //Debug.WriteLine("LoadAssessments: Before await");
            var assessments = await _dbInteractions.GetAssessments(_course.CourseId);
            //Debug.WriteLine("LoadAssessments: After await");

            //_assessments.Clear();

            //if (assessments == null || assessments.Count == 0)
            //{
            //    // Display an alert if no assessments are found
            //    Debug.WriteLine("No assessments found");
            //    await DisplayAlert("No Assessments", "There are no assessments for this course. Please add up to 2 assessments.", "OK");
            //    return;
            //}

            Debug.WriteLine("Clearing _assessments collection");
            _assessments.Clear();  //assessments must be cleared before .Add or duplicate assessments will be loaded!!!
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

    private void HandleAssessmentSwipe_Started(object sender, SwipeStartedEventArgs e)
    {
        _didUserSwipe = true;
    }

    private async void HandleEditAssessment_Swipe(object sender, EventArgs e)
    {
        if (_didUserSwipe) return;

        if (((SwipeItem)sender).CommandParameter is Assessment assessmentToEdit)
        {
            await Navigation.PushAsync(new AssessmentDetailPage(assessmentToEdit));
        }
    }

    private async void HandleDeleteAssessment_Swipe(object sender, EventArgs e)
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

    private void HandleAssessmentSwipe_Ended(object sender, SwipeEndedEventArgs e)
    {
        _didUserSwipe = false;
    }

    private async void HandleSave_Clicked(object sender, EventArgs e)
    {
        
        bool noAssessments = false;
        //if the ValidateCourseData method did not return 'true', exit save method.
        //the user would have received a display alert informing him of the issue.
        if (!ValidateCourseData())
        {
            return;
        }

        //Adding new course, load the term id to the class variable _course to ensure new course
        //is associated with the term. Otherwise app will crash when saving to the database
        //due to not having a termId (it isn't available from the screen to populate _course).
        if (_course == null)
        {
            _course = new Course
            {
                TermId = _term.TermId
            };
        }
        //Load the data keyed by user into the _course class variable fields.
        _course.Name = CourseNameInput.Text;
        _course.StartDate = CourseStartDatePicker.Date;
        _course.EndDate = CourseEndDatePicker.Date;
        _course.Status = CourseStatusPicker.SelectedItem?.ToString();
        _course.InstructorName = InstructorNameInput.Text;
        _course.InstructorPhone = InstructorPhoneInput.Text;
        _course.InstructorEmail = InstructorEmailInput.Text;
        _course.NotifyStart = NotifyStartCheckBox.IsChecked;
        _course.NotifyEnd = NotifyEndCheckBox.IsChecked;
        _course.Notes = NotesEditor.Text;

        
        // Load assessments for the course asynchronously
        List<Assessment> assessments = await _dbInteractions.GetAssessments(_course.CourseId);

        //Convert to ObservableCollection
        _assessments = new ObservableCollection<Assessment>(assessments);

        // Check if there are no assessments and display an alert
        if (_assessments != null && _assessments.Count == 0)
        {
            noAssessments = true;
            //// Display an alert if no assessments are found
            //Debug.WriteLine("No assessments found");
            //await DisplayAlert("Course Has No Assessments", "Don't forget to add assessments", "OK");
        }

        bool PermissionToDelete = await DisplayAlert("Confirm Save", "Are you sure you want to save this course?", "Yes", "No");
        if (PermissionToDelete)
        {
            await _dbInteractions.SaveCourse(_course);
            if (noAssessments)
            {
                await DisplayAlert("Success", "The course was saved! Don't forget to add assessments.", "OK");
            }
            PermissionToDelete = false;
        }

        //Now that the course is saved, start and end date notifications need to processed by the following method
        SetCourseNotification(_course);

        //Update the TermCourseListPage data. I shouldn't need this because of the Onappearing method,
        //but on several occasions (I don't understand why) the course list would not display added
        //assessments. So I added this and it resolved the problem.
        if (Navigation.NavigationStack.LastOrDefault() is TermCourseListPage termCourseListPage)
        {
            termCourseListPage.LoadCourses();
        }

        //Go back to the TermsCourseListPage.
        await Navigation.PopAsync();
    }

    private async void HandleShareNotes_Clicked(object sender, EventArgs e)
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

    private async void HandleDelete_Clicked(object sender, EventArgs e)
    {
        if (_course != null)
        {
            bool PermissionToDelete = await DisplayAlert("Confirm Delete", "Please confirm course deletion?", "Yes", "No");
            if (PermissionToDelete)
            {
                // The section gets a list of assessments associated with the course
                // and deletes them before deleting the course. Otherwise, in the real world, there would be 
                // numerous orphaned assessments.
                List<Assessment> assessments = await _dbInteractions.GetAssessments(_course.CourseId);

                foreach (var assessment in assessments)
                    { 
                        await _dbInteractions.DeleteAssessment(assessment);
                    }
                
                await _dbInteractions.DeleteCourse(_course);

                // update courses page to remove course from the list. Shouldn't have to do this because of
                // the OnAppear method on the CourseDetailPage, but without this routine sometimes the 
                //coursse list doesn't re-load and the deleted course still shows on the list.
                if (Navigation.NavigationStack.LastOrDefault() is TermCourseListPage termCourseListPage)
                {
                    termCourseListPage.LoadCourses();
                }
                await DisplayAlert("Success", "Courses and associated assessments were deleted successfully!", "OK");
                await Navigation.PopAsync();
                PermissionToDelete = false;
            }
        }
    }

    private async void HandleAddAssessment_Clicked(object sender, EventArgs e)
    {
        // Check if the course is saved (assuming _course.CourseId is 0 or null when not saved)
        if (_course == null || _course.CourseId == 0)
        {
            // Display an alert if the course is not saved 
            await DisplayAlert("Save Course", "You MUST save a course, first, before adding assessments.", "OK"); return;
            return;
        }
 
        var assessments = await _dbInteractions.GetAssessments(_course.CourseId);
        if (assessments.Count >= 2)
        {
            await DisplayAlert("Assessment Limit Reached", "You cannot add more than 2 assessments to a course.", "OK");
            return;
        }
        var newAssessment = new Assessment
        {
            CourseId = _course.CourseId
        };
        await Navigation.PushAsync(new AssessmentDetailPage(newAssessment));
    }

    private bool ValidateCourseData()
    {

        if (string.IsNullOrWhiteSpace(CourseNameInput.Text))
        {
            DisplayAlert("Validation Error", "A course name is required.", "OK");
            return false;
        }


        if (string.IsNullOrWhiteSpace(CourseStatusPicker.SelectedItem?.ToString()))
        {
            DisplayAlert("Validation Error", "A course status is required.", "OK");
            return false;
        }


        if (string.IsNullOrWhiteSpace(InstructorNameInput.Text))
        {
            DisplayAlert("Validation Error", "The instructor's name is required.", "OK");
            return false;
        }


        if (string.IsNullOrWhiteSpace(InstructorPhoneInput.Text))
        {
            DisplayAlert("Validation Error", "The instructor's phone number is required.", "OK");
            return false;
        }


        if (string.IsNullOrWhiteSpace(InstructorEmailInput.Text))
        {
            DisplayAlert("Validation Error", "The instructor's email address is required.", "OK");
            return false;
        }


        if (CourseStartDatePicker.Date >= CourseEndDatePicker.Date)
        {
            DisplayAlert("Validation Error", "The course's start date must be BEFORE the end date.", "OK");
            return false;
        }

        return true; // If code gets to this point, no validation issues were identified.
    }

    private async void SetCourseNotification(Course course)
    {
        Debug.WriteLine($"********Performing SetCourseNotification Method*********"); 
        DateTime now = DateTime.Now;
        if (NotifyStartCheckBox != null && NotifyStartCheckBox.IsChecked == true)
        {
            Debug.WriteLine($"*************Processing NotifyStartCheckbox************");
            if (course != null)
            {
                DateTime startNotifyTime = course.StartDate; 
                if (course.StartDate.Date == now.Date)
                {
                    startNotifyTime = now.AddSeconds(10); // Send notification immediately if the date is today
                }
                Debug.WriteLine($"***startNotifyTime:  {startNotifyTime}");


                var startNotifyRequest = new NotificationRequest
                {
                    NotificationId = course.CourseId + 1000,
                    Title = $"Course Start Reminder",
                    Description = $"The {course.Name} course starts today!",
                    Schedule = new NotificationRequestSchedule { NotifyTime = startNotifyTime }
                };
                Debug.WriteLine($"***NotificationId:  {startNotifyRequest.NotificationId}");
                Debug.WriteLine($"***Title:  {startNotifyRequest.Title}");
                Debug.WriteLine($"***Description:  {startNotifyRequest.Description}");
                Debug.WriteLine($"***Schedule:  {startNotifyRequest.Schedule}");
                try
                {
                    // Check if notification permissions are granted. If not not make the request. It may be safer
                    // to make a permissions request before each Notification request.
                    Debug.WriteLine($"**************Asking for notification permissions:  {startNotifyRequest.Schedule}");
                    bool isNotifyPermissionsGranted = await LocalNotificationCenter.Current.AreNotificationsEnabled();
                    
                    if (!isNotifyPermissionsGranted)
                    {
                        await LocalNotificationCenter.Current.RequestNotificationPermission();
                    }
                    Debug.WriteLine($"**************Sent notification request*************");
                    bool wasNotifyRequestSuccessful = await LocalNotificationCenter.Current.Show(startNotifyRequest);
                    Debug.WriteLine($"***did request execute successfully: {wasNotifyRequestSuccessful}");
                }
                catch (Exception ex)
                {
                    // Log any exceptions that occur regarding notification permissions or request.
                    Console.WriteLine($"***********Error showing notification: {ex.Message}");
                }

            }
            else
            {
                // code would not have reached this point if course was null.                
                Console.WriteLine("********Course is null.");
                return;
            }
        }

        if (NotifyEndCheckBox != null && NotifyEndCheckBox.IsChecked == true)
        {
            Debug.WriteLine($"********Processing NotifyEndCheckbox************");
            if (course != null)
            {

                DateTime endNotifyTime = course.EndDate;
                if (course.EndDate.Date == now.Date)
                {
                    endNotifyTime = now.AddSeconds(10); // Send notification immediately if the date is today
                }
                Debug.WriteLine($"********endNotifyTime:  {endNotifyTime}");

                var endNotifyRequest = new NotificationRequest
                {
                    NotificationId = course.CourseId + 2000,
                    Title = $"Course End Reminder",
                    Description = $"The {course.Name} course ends today!",
                    Schedule = new NotificationRequestSchedule { NotifyTime = endNotifyTime }
                };
                Debug.WriteLine($"********NotificationId:  {endNotifyRequest.NotificationId}");
                Debug.WriteLine($"********Title:  {endNotifyRequest.Title}");
                Debug.WriteLine($"********Description:  {endNotifyRequest.Description}");
                Debug.WriteLine($"********Schedule:  {endNotifyRequest.Schedule}");

                try
                {
                    // Check if notification permissions are granted. If not not make the request. It may be safer
                    // to make a permissions request before each Notification request.
                    bool isNotifyPermissionsGranted = await LocalNotificationCenter.Current.AreNotificationsEnabled();
                    if (!isNotifyPermissionsGranted)
                    {
                        await LocalNotificationCenter.Current.RequestNotificationPermission();
                    }
                    bool wasNotifyRequestSuccessful = await LocalNotificationCenter.Current.Show(endNotifyRequest);
                }
                catch (Exception ex)
                {
                    // Log any exceptions that occur regarding notification permissions or request.
                    Console.WriteLine($"***********Error showing notification: {ex.Message}");
                }

            }
            else
            {
                // code would not have reached this point if course was null.                
                Console.WriteLine("********Course is null.");
                return;
            }
        }
    }

    private async void HandleAssessment_Selection(object sender, SelectionChangedEventArgs e)
    {
        if (_didUserSwipe)
        {
            _didUserSwipe = false;
            AssessmentsCollectionView.SelectedItem = null;
            return;
        }

        if (e.CurrentSelection.FirstOrDefault() is Assessment selectedAssessment)
        {
            var assessmentDetailPage = new AssessmentDetailPage(selectedAssessment);
            await Navigation.PushAsync(assessmentDetailPage);
            AssessmentsCollectionView.SelectedItem = null;
        }
    }

    private void HandlePhoneInput(object sender, TextChangedEventArgs e)
    {
        var entry = sender as Entry;
        var phoneNumber = InstructorPhoneInput.Text;
        
        // Remove non-numeric characters
        var cleaned = new StringBuilder(); 
        foreach (var c in phoneNumber) 
        { 
            if (char.IsDigit(c)) cleaned.Append(c); 
        } 
        
        // Format the phone number
        if (cleaned.Length > 0) 
        { 
            if (cleaned.Length > 3)
                cleaned.Insert(3, ") ");
            if (cleaned.Length > 8) cleaned.Insert(8, "-");
                cleaned.Insert(0, "("); 
        }

        // Update the entry text and position the cursor at the end
        InstructorPhoneInput.Text = cleaned.ToString();
        InstructorPhoneInput.CursorPosition = InstructorPhoneInput.Text.Length;
    }





}   
    





