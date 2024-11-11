using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using System.IO;
using EduTrack.DB_Models;
using Microsoft.VisualBasic.FileIO;
using static SQLite.SQLite3;

namespace EduTrack
{
    internal class DB_Interactions
    {
        private readonly SQLiteAsyncConnection _database;


        //++++++++++Initial Database operations+++++++++++++++
        //On execution, the app does the following:
        //      1. Creates the term, course and assessment database tables (if they don't exist).
        //      2. deletes all data from the tables. 
        public DB_Interactions(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Assessment>().Wait();
            _database.CreateTableAsync<Course>().Wait();
            _database.CreateTableAsync<Term>().Wait();
        }

            public async Task<int> InitializeData()
        {
            int term_result = await _database.DeleteAllAsync<Term>();
            int course_result = await _database.DeleteAllAsync<Course>();
            int assessment_result = await _database.DeleteAllAsync<Assessment>();
            if ((term_result > 0) && (course_result > 0) && (assessment_result > 0))
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        //+++++++++++++++++++++++++++++++++++++++++++++++++++++




        //+++++++++++Database Queries-- GETS++++++++++++++++++++
        //Get Terms
        public Task<List<Term>> GetTerms()
        {
            return _database.Table<Term>().ToListAsync();
        }

        //Get Courses
        public Task<List<Course>> GetCoursesInTerm(int termId)
        {
            return _database.Table<Course>()
                            .Where(c => c.TermId == termId)
                            .ToListAsync();
        }

        //Get Assessments
        public Task<List<Assessment>> GetAssessments(int courseId)
        {
            return _database.Table<Assessment>().Where(a => a.CourseId == courseId).ToListAsync();
        }
        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


        //+++++++++++Database Queries-- Inserts/Updates++++++++++++++++++++       
        //Save Term
        public async Task<int> SaveTerm(Term term)
        {
            System.Diagnostics.Debug.WriteLine($"******* TERM ******* SaveTerm: Name={term.Name}, TermId={term.TermId}, StartDate={{term.StartDate}}, EndDate={{term.EndDate}}\"); ");
            if (term.TermId == 0) 
            { 
                var result = await _database.InsertAsync(term); 
                System.Diagnostics.Debug.WriteLine($"******* TERM ******* Inserted Term: Name={term.Name}, TermId={term.TermId}, StartDate={term.StartDate}, EndDate={term.EndDate}"); 
                return result; 
            } 
            else 
            { 
                var result = await _database.UpdateAsync(term); 
                System.Diagnostics.Debug.WriteLine($"******* TERM ******* Updated Term: Name={term.Name}, TermId={term.TermId}, StartDate={{term.StartDate}}, EndDate={{term.EndDate}}\"); "); 
                return result; 
            }
        }

        //Save Course
        public async Task<int> SaveCourse(Course course)
        {
            System.Diagnostics.Debug.WriteLine($"******* COURSE ******* SaveCourse: Name={course.Name}, CourseId={course.CourseId}");
            if (course.CourseId == 0) 
            { 
                var result = await _database.InsertAsync(course); 
                System.Diagnostics.Debug.WriteLine($"******* COURSE ******* Inserted Course: Name={course.Name}, CourseId={course.CourseId}"); 
                return result; 
            } 
            else 
            { 
                var result = await _database.UpdateAsync(course); 
                System.Diagnostics.Debug.WriteLine($"******* COURSE ******* Updated Course: Name={course.Name}, CourseId={course.CourseId}"); 
                return result; 
            }
        }

        //Save Assessment
        public async Task<int> SaveAssessment(Assessment assessment)
        {
            System.Diagnostics.Debug.WriteLine($"******* Assessment ******* SaveAssessment: Name={assessment.Name}, AssessmentId={assessment.AssessmentId}");
            if (assessment.AssessmentId == 0)
            {
                var result = await _database.InsertAsync(assessment);
                System.Diagnostics.Debug.WriteLine($"******* Assessment ******* Inserted Assessment: Name={assessment.Name}, AssessmentId={assessment.AssessmentId}");
                return result;
            }
            else
            {
                var result = await _database.UpdateAsync(assessment);
                System.Diagnostics.Debug.WriteLine($"******* Assessment ******* Updated Assessment: Name={assessment.Name}, AssessmentId={assessment.AssessmentId}");
                return result;
            }
        }
        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        //++++++++++++++++Database Queries-- Deletes+++++++++++++++++++++
        //Delete Term
        public Task<int> DeleteTerm(Term term)
        {
            return _database.DeleteAsync(term);
        }

        //Delete Course
        public Task<int> DeleteCourse(Course course)
        {
            return _database.DeleteAsync(course);
        }

        //Delete Assessment
        public Task<int> DeleteAssessment(Assessment assessment)
        {
            return _database.DeleteAsync(assessment);
        }
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    }
}
