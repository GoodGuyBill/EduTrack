using EduTrack.DB_Models;
namespace EduTrack;

public partial class CourseDetailPage : ContentPage
{
    private readonly DB_Interactions _dbInteractions;
    private readonly int _termId;
    private readonly int _courseId;


    public CourseDetailPage(Term term)
    {
        InitializeComponent();
        _termId = term.TermId;
        string dbPath = AppConfig.DbPath;
        _dbInteractions = new DB_Interactions(dbPath);
    }


    public CourseDetailPage(Course selectedCourse)
    {
        InitializeComponent();
        _termId = selectedCourse.TermId;
        _courseId = selectedCourse.CourseId;
        string dbPath = AppConfig.DbPath;
        _dbInteractions = new DB_Interactions(dbPath);

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
        }


    }


    






    private async void OnSaveClicked(object sender, EventArgs e)
    {
        var course = new Course
        {
            TermId = _termId,
            Name = CourseNameEntry.Text,
            StartDate = CourseStartDatePicker.Date,
            EndDate = CourseEndDatePicker.Date,
            Status = CourseStatusPicker.SelectedItem.ToString() ?? string.Empty,
            InstructorName = InstructorNameEntry.Text,
            InstructorPhone = InstructorPhoneEntry.Text,
            InstructorEmail = InstructorEmailEntry.Text,

            NotifyStart = NotifyStartCheckBox.IsChecked,
            NotifyEnd = NotifyEndCheckBox.IsChecked,


            Notes = NotesEditor.Text
        };

        await _dbInteractions.SaveCourse(course);
        await Navigation.PopAsync();
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        // Confirm deletion dialog (optional)
        var course = (Course)BindingContext;
        await _dbInteractions.DeleteCourse(course);
        await Navigation.PopAsync();
    }

    private async void OnAddAssessmentClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AssessmentDetailPage(_courseId));
    }
}