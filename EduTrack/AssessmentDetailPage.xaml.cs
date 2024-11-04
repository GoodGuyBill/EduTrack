using EduTrack.DB_Models;

namespace EduTrack
{
    public partial class AssessmentDetailPage : ContentPage
    {
        private readonly DB_Interactions _dbInteractions;
        private readonly int _courseId;

        public AssessmentDetailPage(int courseId) // Ensure this constructor exists
        {
            InitializeComponent();
            _courseId = courseId;
            string dbPath = AppConfig.DbPath;
            _dbInteractions = new DB_Interactions(dbPath);
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            var assessment = new Assessment
            {
                CourseId = _courseId,
                Name = AssessmentNameEntry.Text ?? string.Empty,
                StartDate = AssessmentStartDatePicker.Date,
                EndDate = AssessmentEndDatePicker.Date,
                Type = AssessmentTypePicker.SelectedItem.ToString() ?? string.Empty
            };

            await _dbInteractions.SaveAssessment(assessment);
            await Navigation.PopAsync();
        }

        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            var assessment = (Assessment)BindingContext;
            await _dbInteractions.DeleteAssessment(assessment);
            await Navigation.PopAsync();
        }
    }
}