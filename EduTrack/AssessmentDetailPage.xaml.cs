//using Microsoft.Maui.Controls;
using Plugin.LocalNotification;
//using System;
using System.Diagnostics;
//using System.Linq;
using EduTrack.DB_Models;
//using System.Collections.ObjectModel;
//using System.Xml.Linq;
//using HealthKit;

namespace EduTrack
{
    public partial class AssessmentDetailPage : ContentPage
    {
        private readonly DB_Interactions _dbInteractions;
        private readonly int courseId;
        private Assessment _assessment;
        private List<Assessment> existingAssessments;
        private int assessmentId;
        private Course _course;
        bool typeIsDuplicate;

        public AssessmentDetailPage(Assessment selectedAssessment)
        {
            InitializeComponent();
            _assessment = selectedAssessment;
            string dbPath = AppConfig.DbPath;
            _dbInteractions = new DB_Interactions(dbPath);
            courseId = _assessment.CourseId;
            assessmentId = _assessment.AssessmentId;
            AssessmentNameInput.Text = _assessment.Name;
            AssessmentStartDatePicker.Date = _assessment.StartDate;
            AssessmentEndDatePicker.Date = _assessment.EndDate;
            AssessmentTypePicker.SelectedItem = _assessment.Type;
            AssessmentNotifyStartCheckBox.IsChecked = _assessment.NotifyStart;
            AssessmentNotifyEndCheckBox.IsChecked = _assessment.NotifyEnd;

            if (_assessment.AssessmentId == 0)
            {
                AssessmentStartDatePicker.Date = DateTime.Today;
                AssessmentEndDatePicker.Date = DateTime.Today;
            }
        }

        
        
        
        private async void HandleSave_Clicked(object sender, EventArgs e)
        {
            if (!await ValidateAssessmentData())   //validate assessment. If issues, exit method.
            {
                return;
            }

            if (_assessment == null)    //Adding new assessment
            {
                _assessment = new Assessment
                {
                    CourseId = _course.CourseId   //If adding a new assessment the courseID must be associated with it.
                };
            }

            var assessment = new Assessment    // move screen fileds to the assessment class variable
            {
                CourseId = courseId,
                AssessmentId = _assessment.AssessmentId,
                Name = AssessmentNameInput.Text ?? string.Empty,
                StartDate = AssessmentStartDatePicker.Date,
                EndDate = AssessmentEndDatePicker.Date,
                Type = AssessmentTypePicker.SelectedItem.ToString() ?? string.Empty,
                NotifyStart = AssessmentNotifyStartCheckBox.IsChecked,
                NotifyEnd = AssessmentNotifyEndCheckBox.IsChecked
            };

            //await DisplayAlert("Assessment START Date", $"The START date is: {assessment.StartDate}", "OK");
            //await DisplayAlert("Assessment START Date", $"The END date is: {assessment.EndDate}", "OK");

            await _dbInteractions.SaveAssessment(assessment); //Call the method that saves assessment data to database.
            await DisplayAlert("Success", "The Assessment course was saved!", "OK");

            SetAssessmentNotification(assessment);

            //Update the TermCourseListPage data
            if (Navigation.NavigationStack.LastOrDefault() is TermCourseListPage termCourseListPage)
            {
                termCourseListPage.LoadCourses();
            }

            //Go back to the TermsCourseListPage.
            await Navigation.PopAsync();
        }

        private async void HandleDelete_Clicked(object sender, EventArgs e)
        {
            if (_assessment != null)  //Need to test this ensure _assess is always !null.
            {
                // Initialize a new class varible, assessment. Move the form fields (keyed data)
                // to assessment class variable. NOTE: a better way would be to get permission first.
                var assessment = new Assessment
                {
                    AssessmentId = assessmentId,
                    CourseId = courseId,
                    Name = AssessmentNameInput.Text,
                    StartDate = AssessmentStartDatePicker.Date,
                    EndDate = AssessmentEndDatePicker.Date,
                    Type = AssessmentTypePicker.SelectedItem.ToString(),
                    NotifyStart = AssessmentNotifyStartCheckBox.IsChecked,
                    NotifyEnd = AssessmentNotifyEndCheckBox.IsChecked
                };

                //Get permission to delete assessment
                bool PermissionToDelete = await DisplayAlert("Confirm Delete", $"Delete the {assessment.Name} assessment?", "Yes", "No");
                if (PermissionToDelete)
                {
                    await _dbInteractions.DeleteAssessment(assessment);
                    await DisplayAlert("Success", $"{assessment.Name} assessment has been deleted.", "OK");
                    await Navigation.PopAsync();
                }
            }
        }

        private async Task<bool> ValidateAssessmentData()
        {
            if (string.IsNullOrWhiteSpace(AssessmentNameInput.Text))
            {
                await DisplayAlert("Validation Error", "An assessment name is required. Please enter an assessment name.", "OK");
                return false;
            }

            if (AssessmentStartDatePicker.Date == default || AssessmentEndDatePicker.Date == default)
            {
                await DisplayAlert("Validation Error", "Please select a valid start and end date.", "OK");
                return false;
            }

            if (AssessmentStartDatePicker.Date >= AssessmentEndDatePicker.Date)
            {
                await DisplayAlert("Validation Error", "Invalid start date. The start date must be BEFORE the end date.", "OK");
                return false;
            }

            if (string.IsNullOrWhiteSpace(AssessmentTypePicker.SelectedItem?.ToString()))
            {
                await DisplayAlert("Validation Error", "An assessment type is required. Please enter an assessment type.", "OK");
                return false;
            }

            //check to see if assessment type is dupplicate. If current type already exist, instruct user that 
            //only one type of objective is allowed.
            typeIsDuplicate = await CheckDuplicateAssessment(AssessmentTypePicker.SelectedItem?.ToString(), assessmentId);
            if (typeIsDuplicate)
            {
                return false;
            }

            return true;
        }

        private async Task<bool> CheckDuplicateAssessment(string currentType, int currentAssessmentId)
        { // 1. Load assessments to existingAssessments
            existingAssessments = await _dbInteractions.GetAssessments(courseId);

            // 2. Check if the type of the current assessment to be added already exists in existingAssessments 
            bool isDuplicate = existingAssessments.Any(a => a.Type.Equals(currentType, 
                                                                          StringComparison.OrdinalIgnoreCase) && 
                                                                          a.AssessmentId != currentAssessmentId); 
            if (isDuplicate)
            {
                // 3. If it does, warn the user
                await DisplayAlert("Duplicate Type", "Only one type of objective and one type of performance assessment is allowed.", "OK");
                return true;
            }
            else
            {
                // Proceed with adding the assessment
                //existingAssessments.Add(new Assessment { Type = currentType });
                //await DisplayAlert("Success", "Assessment added successfully.", "OK");
                // Optionally, save the new assessment to the database 
                // _dbInteractions.AddAssessment(new Assessment { Type = currentType });
                return false;
            }
        }

        private async void SetAssessmentNotification(Assessment assessment)
        {
            Debug.WriteLine($"********Performing SetAssessmentNotification Method*********");
            DateTime now = DateTime.Now;
            if (AssessmentNotifyStartCheckBox != null && AssessmentNotifyStartCheckBox.IsChecked == true)
            {
                Debug.WriteLine($"*************Processing Start-Date notifications************");
                if (assessment != null)
                {
                    //await DisplayAlert("Assessment START Date", $"The START date is: {assessment.StartDate}", "OK");
                    DateTime startNotifyTime = assessment.StartDate;
                    if (assessment.StartDate.Date == now.Date)
                    {
                        startNotifyTime = now.AddSeconds(10); // setup notification to communicate immediately (10 second)
                    }
                    Debug.WriteLine($"***startNotifyTime:  {startNotifyTime}");


                    var startNotifyRequest = new NotificationRequest
                    {
                        NotificationId = assessment.AssessmentId + 1000,
                        Title = $"Assessment Start Reminder",
                        Description = $"The {assessment.Name} assessment starts today!",
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

            if (AssessmentNotifyEndCheckBox != null && AssessmentNotifyEndCheckBox.IsChecked == true)
            {
                Debug.WriteLine($"********Processing End-Date notifications************");
                if (assessment != null)
                {

                    DateTime endNotifyTime = assessment.EndDate;
                    if (assessment.EndDate.Date == now.Date)
                    {
                        endNotifyTime = now.AddSeconds(10); // Send notification immediately (in 10 seconds) if the date is today
                    }
                    Debug.WriteLine($"********endNotifyTime:  {endNotifyTime}");

                    var endNotifyRequest = new NotificationRequest
                    {
                        NotificationId = assessment.CourseId + 2000,
                        Title = $"Assessment End Reminder",
                        Description = $"The {assessment.Name} assessment ends today!",
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
                        Console.WriteLine($"***********CATCH Error showing notification: {ex.Message}");
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
    }
}