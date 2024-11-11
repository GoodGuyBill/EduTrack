using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrack.DB_Models
{

    public static class AppConfig //used as a global variable that holds the database path.
    { 
        public static string DbPath { get; set; } 
    }
    
    public class Term
    {
        [PrimaryKey, AutoIncrement]
        public int TermId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }


    public class Course
    {
        [PrimaryKey, AutoIncrement]
        public int CourseId { get; set; }
        public int TermId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string InstructorName { get; set; } = string.Empty;
        public string InstructorEmail { get; set; } = string.Empty;
        public string InstructorPhone { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public bool NotifyStart { get; set; } 
        public bool NotifyEnd { get; set; }   
    }


    public class Assessment
    {
        [PrimaryKey, AutoIncrement]
        public int AssessmentId { get; set; }
        public int CourseId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Type { get; set; } = string.Empty;
        public bool NotifyStart { get; set; }
        public bool NotifyEnd { get; set; }
    }
}
