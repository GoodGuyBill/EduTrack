using Plugin.LocalNotification;
using Plugin.LocalNotification.EventArgs;
using EduTrack.DB_Models;
//using static Android.Graphics.ImageDecoder;

namespace EduTrack
{
    public partial class App : Application
    {
        public static string DatabasePath { get; private set; }
        private int term_result = 0;

        public App(string dbPath)
        {
            InitializeComponent();
            DatabasePath = dbPath;   // dbPath is defined in the MauiProgram.cs page.
            var dbInteractions = new DB_Interactions(DatabasePath); // Initialize database interactions
            MainPage = new NavigationPage(new TermListPage());   // Set the main page to TermListPage (allows navigation features).

            //LoadSampleData(dbInteractions);
            // for 3 days, I attempted to run this asynchronously (i.e. calling "await LoadSampleData(dbInteractions);"
            // in a seperate async method. I just could not get it working. So I learned how to run an asynchronous
            //synchrononously.
            Task.Run(() => LoadSampleData(dbInteractions)).Wait();  // Load sample data

            LocalNotificationCenter.Current.RequestNotificationPermission();  // Request notification permission
            LocalNotificationCenter.Current.NotificationActionTapped += OnNotificationActionTapped;  // Handle notification action tapped
        }

        private async Task InitializeApp(string dbPath)  
        {            
            var dbInteractions = new DB_Interactions(dbPath);
            await LoadSampleData(dbInteractions);              
        }


        private async Task LoadSampleData(DB_Interactions dbInteractions)
        {
            // Initialize data (clears existing data)
            int initialize_result = await dbInteractions.InitializeData();

            //Load your initial sample data
            var sampleTerm = new Term
               {
                   Name = "Spring 2024",
                   StartDate = DateTime.Now.AddDays(5),
                   EndDate = DateTime.Now.AddMonths(3)
               };
            int term_result = await dbInteractions.SaveTerm(sampleTerm);            

            var sampleCourse = new Course
            {
                TermId = sampleTerm.TermId,
                Name = "Beginning C# Programming",
                StartDate = sampleTerm.StartDate,
                EndDate = sampleTerm.EndDate,
                Status = "In Progress",
                InstructorName = "Anika Patel",
                InstructorPhone = "555-123-4567",
                InstructorEmail = "anika.patel@strimeuniversity.edu",
                Notes = "This course covers advanced topics in C#.",
                NotifyStart = false,
                NotifyEnd = true
            };
            int course_result = await dbInteractions.SaveCourse(sampleCourse);


            var sampleAssessment1 = new Assessment
            {
                CourseId = sampleCourse.CourseId,
                Name = "Midterm Exam",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(2),
                Type = "Performance",
                NotifyStart = true,
                NotifyEnd = true
            };
            int Assessment1_result = await dbInteractions.SaveAssessment(sampleAssessment1);

            var sampleAssessment2 = new Assessment
            {
                CourseId = sampleCourse.CourseId,
                Name = "Final Exam",
                StartDate = DateTime.Now.AddDays(15),
                EndDate = DateTime.Now.AddMonths(1),
                Type = "Objective",
                NotifyStart = true,
                NotifyEnd = true
            };
            int assesment2_result = await dbInteractions.SaveAssessment(sampleAssessment2);
        }

        private void OnNotificationActionTapped(NotificationActionEventArgs e)
        {
            if (e.IsDismissed) return;

            if (e.IsTapped)
            {
                // Handle the notification tap action
                // Navigate to the relevant page or perform any other action
            }
        }
    }
}