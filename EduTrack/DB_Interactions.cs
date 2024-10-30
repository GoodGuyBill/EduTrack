using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using System.IO;
using EduTrak.DB_Models;
using Microsoft.VisualBasic.FileIO;
using static Android.Graphics.ImageDecoder;

namespace EduTrak
{
    internal class DB_Interactions
    {
        private readonly SQLiteAsyncConnection _database;


        //+++++++++++++++++++Database operations+++++++++++++++
        public DB_Interactions(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Assessment>().Wait();
            _database.CreateTableAsync<Course>().Wait();
            _database.CreateTableAsync<Term>().Wait();
        }

        public async Task InitializeData()
        {
            await _database.DeleteAllAsync<Term>();
            await _database.DeleteAllAsync<Course>();
            await _database.DeleteAllAsync<Assessment>();
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
            if (term.TermId == 0)
            {
                return await _database.InsertAsync(term);
            }
            else
            {
                return await _database.UpdateAsync(term);
            }
        }

        //Save Course
        public async Task<int> SaveCourse(Course course)
        {
            if (course.CourseId == 0)
            {
                return await _database.InsertAsync(course);
            }
            else
            {
                return await _database.UpdateAsync(course);
            }
        }

        //Save Assessment
        public async Task<int> SaveAssessment(Assessment assessment)
        {
            if (assessment.AssessmentId == 0)
            {
                return await _database.InsertAsync(assessment);
            }
            else
            {
                return await _database.UpdateAsync(assessment);
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