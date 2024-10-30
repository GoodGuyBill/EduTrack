using System;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using Microsoft.Maui;
using EduTrak.DB_Models;
namespace EduTrak
{
    public partial class TermCourseListPage : ContentPage
    {
        private readonly DB_Interactions _dbInteractions;
        private readonly int _termId;

        public TermCourseListPage(int termId)
        {
            InitializeComponent();
            _termId = termId;
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "EduTrak.db3");
            _dbInteractions = new DB_Interactions(dbPath);
            LoadCourses(termId);
        }

        private async void LoadCourses(int termId)
        {
            var courses = await _dbInteractions.GetCoursesInTerm(termId);
            CoursesListView.ItemsSource = courses;
        }

        private async void OnAddCourseClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CourseDetailPage(_termId, 0));
        }

        private async void OnCourseTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is Course course)
            {
                await Navigation.PushAsync(new CourseDetailPage(_termId, course.CourseId));
            }
        }
    }
}