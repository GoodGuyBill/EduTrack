using EduTrak.DB_Models;
namespace EduTrak;

public partial class CourseDetailPage : ContentPage
{
    private readonly DB_Interactions _dbInteractions;
    private readonly int _termId;
    private readonly int _courseId;

    public CourseDetailPage(int termId, int courseId)
    {
        InitializeComponent();
        _termId = termId;
        _courseId = courseId;
        string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "EduTrak.db3");
        _dbInteractions = new DB_Interactions(dbPath);
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