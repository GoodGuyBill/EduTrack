using Microsoft.Maui.Controls;
using Plugin.LocalNotification;
using System;
using System.Linq;
using EduTrack.DB_Models;
using System.Collections.ObjectModel;
//using HealthKit;

namespace EduTrack
{
    public partial class AssessmentDetailPage : ContentPage
    {
        private readonly DB_Interactions _dbInteractions;
        private readonly int courseId;
        private Assessment _assessment;
        private int assessmentId;
        private readonly ObservableCollection<Assessment> _assessments;

        //public AssessmentDetailPage(Assessment selectedAssessment, ObservableCollection<Assessment> assessments, CourseDetailPage courseDetailPage) // Ensure this constructor exists
        //{
        //    InitializeComponent();
        //    _assessment = selectedAssessment;
        //    _assessments = assessments;

        //    string dbPath = AppConfig.DbPath;
        //    _dbInteractions = new DB_Interactions(dbPath);
        //    courseId = _assessment.CourseId;
        //    assessmentId = _assessment.AssessmentId;

        //    AssessmentNameEntry.Text = _assessment.Name;
        //    AssessmentStartDatePicker.Date = _assessment.StartDate;
        //    AssessmentEndDatePicker.Date = _assessment.EndDate;
        //    AssessmentTypePicker.SelectedItem = _assessment.Type;
        //    AssessmentNotifyStartCheckBox.IsChecked = _assessment.NotifyStart;
        //    AssessmentNotifyEndCheckBox.IsChecked = _assessment.NotifyEnd;
        //}




        public AssessmentDetailPage(Assessment selectedAssessment) // Ensure this constructor exists
        {
            InitializeComponent();
            _assessment = selectedAssessment;
            string dbPath = AppConfig.DbPath;
            _dbInteractions = new DB_Interactions(dbPath);
            courseId = _assessment.CourseId;
            assessmentId = _assessment.AssessmentId;
            AssessmentNameEntry.Text = _assessment.Name;
            AssessmentStartDatePicker.Date = _assessment.StartDate;
            AssessmentEndDatePicker.Date = _assessment.EndDate;
            AssessmentTypePicker.SelectedItem = _assessment.Type;
            AssessmentNotifyStartCheckBox.IsChecked = _assessment.NotifyStart; 
            AssessmentNotifyEndCheckBox.IsChecked = _assessment.NotifyEnd;
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            var assessment = new Assessment
            {
                CourseId = courseId,
                AssessmentId = _assessment.AssessmentId,
                Name = AssessmentNameEntry.Text ?? string.Empty,
                StartDate = AssessmentStartDatePicker.Date,
                EndDate = AssessmentEndDatePicker.Date,
                Type = AssessmentTypePicker.SelectedItem.ToString() ?? string.Empty,
                NotifyStart = AssessmentNotifyStartCheckBox.IsChecked,  
                NotifyEnd = AssessmentNotifyEndCheckBox.IsChecked
            };

            await _dbInteractions.SaveAssessment(assessment);
            await DisplayAlert("Success", "The course was saved!", "OK");
            await Navigation.PopAsync();

            //Update the TermCourseListPage data
            if (Navigation.NavigationStack.LastOrDefault() is TermCourseListPage termCourseListPage)
            {
                termCourseListPage.LoadCourses();
            }

            //Go back to the TermsCourseListPage.
            await Navigation.PopAsync();
        }

        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            // Construct a new Assessment object using the form fields
            var assessment = new Assessment
            {
                AssessmentId = assessmentId,
                CourseId = courseId,
                Name = AssessmentNameEntry.Text,
                StartDate = AssessmentStartDatePicker.Date,
                EndDate = AssessmentEndDatePicker.Date,
                Type = AssessmentTypePicker.SelectedItem.ToString(),
                NotifyStart = AssessmentNotifyStartCheckBox.IsChecked,
                NotifyEnd = AssessmentNotifyEndCheckBox.IsChecked
            };

            bool confirmDelete = await DisplayAlert("Confirm Delete", $"Are you sure you want to delete the assessment, {assessment.Name}?", "Yes", "No");
            if (confirmDelete)
            {
                await _dbInteractions.DeleteAssessment(assessment);
                //await DisplayAlert("Deleted", $"{assessment.Name} has been deleted.", "OK");
                await Navigation.PopAsync();
            }
        }




    }
}