using Plugin.LocalNotification;
using Plugin.LocalNotification.EventArgs;
using EduTrack.DB_Models;

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

        private async Task InitializeApp(string dbPath)  // not needed be remains to work on later.
        {            
            var dbInteractions = new DB_Interactions(dbPath);
            await LoadSampleData(dbInteractions);              // I could never get this to work!
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

            // initialize term data to test the maximum limit of 6 terms
            // Define a list of terms
            //var terms = new List<Term>
            //{
            //    new Term
            //    {
            //        Name = "2-Spring 2024",
            //        StartDate = DateTime.Now.AddDays(5),
            //        EndDate = DateTime.Now.AddMonths(3)
            //    },
            //    new Term
            //    {
            //        Name = "3-Summer 2024",
            //        StartDate = DateTime.Now.AddMonths(4),
            //        EndDate = DateTime.Now.AddMonths(6)
            //    },
            //    new Term
            //    {
            //        Name = "4-Fall 2024",
            //        StartDate = DateTime.Now.AddMonths(7),
            //        EndDate = DateTime.Now.AddMonths(9)
            //    },
            //    new Term
            //    {
            //        Name = "5-Winter 2024",
            //        StartDate = DateTime.Now.AddMonths(10),
            //        EndDate = DateTime.Now.AddMonths(12)
            //    },
            //    new Term
            //    {
            //        Name = "6-Spring 2025",
            //        StartDate = DateTime.Now.AddMonths(13),
            //        EndDate = DateTime.Now.AddMonths(15)
            //    }
            //};

            //// Iterate over each term and save it to the database
            //term_result = 0;
            //foreach (var term in terms)
            //{
            //    term_result = await dbInteractions.SaveTerm(term);
            //    if (term_result > 0)
            //    {
            //        Console.WriteLine($"Term '{term.Name}' saved successfully.");
            //    }
            //    else
            //    {
            //        Console.WriteLine($"Failed to save term '{term.Name}'.");
            //    }
            //}

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




//namespace EduTrack
//{
//    public partial class App : Application
//    {
//        public App()
//        {
//            InitializeComponent();

//            MainPage = new TermListPage();
//        }
//    }
//}
