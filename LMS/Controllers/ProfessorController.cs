using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
[assembly: InternalsVisibleTo( "LMSControllerTests" )]
namespace LMS_CustomIdentity.Controllers
{
    [Authorize(Roles = "Professor")]
    public class ProfessorController : Controller
    {

        private readonly LMSContext db;

        public ProfessorController(LMSContext _db)
        {
            db = _db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Students(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult Class(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult Categories(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult CatAssignments(string subject, string num, string season, string year, string cat)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            return View();
        }

        public IActionResult Assignment(string subject, string num, string season, string year, string cat, string aname)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            return View();
        }

        public IActionResult Submissions(string subject, string num, string season, string year, string cat, string aname)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            return View();
        }

        public IActionResult Grade(string subject, string num, string season, string year, string cat, string aname, string uid)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            ViewData["uid"] = uid;
            return View();
        }

        /*******Begin code to modify********/


        /// <summary>
        /// Returns a JSON array of all the students in a class.
        /// Each object in the array should have the following fields:
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "dob" - date of birth
        /// "grade" - the student's grade in this class
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetStudentsInClass(string subject, int num, string season, int year)
        {
            var query = from e in db.Enrolleds
                        join s in db.Students
                        on e.SId equals s.SId
                        join c in db.Classes
                        on e.ClassId equals c.ClassId
                        join co in db.Courses
                        on c.CId equals co.CId
                        join d in db.Departments
                        on co.DId equals d.DId
                        where d.Subject == subject && co.Num == num && c.SemesterSeason == season && c.SemesterYear == year
                        select new
                        {
                            fname = s.FName,
                            lname = s.LName,
                            uid = s.UId,
                            dob = s.Dob,
                            grade = e.Grade
                        };

            var test = query.ToArray();
            return Json(query.ToArray());
        }



        /// <summary>
        /// Returns a JSON array with all the assignments in an assignment category for a class.
        /// If the "category" parameter is null, return all assignments in the class.
        /// Each object in the array should have the following fields:
        /// "aname" - The assignment name
        /// "cname" - The assignment category name.
        /// "due" - The due DateTime
        /// "submissions" - The number of submissions to the assignment
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class, 
        /// or null to return assignments from all categories</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentsInCategory(string subject, int num, string season, int year, string category)
        {
            var aID = from a in db.Assignments
                      join s in db.Submissions
                      on a.AId equals s.AId
                      select a.AId;

            int submissionCount = db.Submissions.Count(submission => submission.AId == aID.FirstOrDefault());

            var query = from ac in db.AssignmentCategories
                        join c in db.Classes
                        on ac.ClassId equals c.ClassId
                        join a in db.Assignments
                        on ac.AcId equals a.AcId
                        join co in db.Courses
                        on c.CId equals co.CId
                        join d in db.Departments
                        on co.DId equals d.DId
                        where d.Subject == subject && co.Num == num && c.SemesterSeason == season && c.SemesterYear == year && ac.Name == category
                        select new
                        {
                            aname = a.AName,
                            cname = ac.Name,
                            due = a.DueDate,
                            submissions = submissionCount
                        };



            return Json(query.ToArray());
        }


        /// <summary>
        /// Returns a JSON array of the assignment categories for a certain class.
        /// Each object in the array should have the folling fields:
        /// "name" - The category name
        /// "weight" - The category weight
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentCategories(string subject, int num, string season, int year)
        {
            var query = from ac in db.AssignmentCategories
                        join c in db.Classes
                        on ac.ClassId equals c.ClassId
                        join co in db.Courses
                        on c.CId equals co.CId
                        join d in db.Departments
                        on co.DId equals d.DId
                        where d.Subject == subject && co.Num == num && c.SemesterSeason == season && c.SemesterYear == year
                        select new
                        {
                            name = ac.Name,
                            weight = ac.Weight
                        };

            return Json(query.ToArray());
        }

        /// <summary>
        /// Creates a new assignment category for the specified class.
        /// If a category of the given class with the given name already exists, return success = false.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The new category name</param>
        /// <param name="catweight">The new category weight</param>
        /// <returns>A JSON object containing {success = true/false} </returns>
        public IActionResult CreateAssignmentCategory(string subject, int num, string season, int year, string category, int catweight)
        {
            var categoryExists = from ac in db.AssignmentCategories
                                 join cl in db.Classes on ac.ClassId equals cl.ClassId
                                 join co in db.Courses on cl.CId equals co.CId
                                 join d in db.Departments on co.DId equals d.DId
                                 where d.Subject == subject && co.Num == num && cl.SemesterSeason == season &&
                                 cl.SemesterYear == year && ac.Name == category && ac.Weight == catweight
                                 select ac.ClassId;

            // If the category doesn't exist, add it to the database
            if(categoryExists.Count() == 0)
            {
                var get_classID = from cl in db.Classes
                                  join co in db.Courses on cl.CId equals co.CId
                                  join d in db.Departments on co.DId equals d.DId
                                  where d.Subject == subject && co.Num == num && cl.SemesterSeason == season &&
                                  cl.SemesterYear == year
                                  select cl.ClassId;

                AssignmentCategory newCategory = new AssignmentCategory();

                newCategory.ClassId = get_classID.First();
                newCategory.Name = category;
                newCategory.Weight = (uint)catweight;

                db.AssignmentCategories.Add(newCategory);
                db.SaveChanges();

                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        /// <summary>
        /// Creates a new assignment for the given class and category.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The new assignment name</param>
        /// <param name="asgpoints">The max point value for the new assignment</param>
        /// <param name="asgdue">The due DateTime for the new assignment</param>
        /// <param name="asgcontents">The contents of the new assignment</param>
        /// <returns>A JSON object containing success = true/false</returns>
        public IActionResult CreateAssignment(string subject, int num, string season, int year, string category, string asgname, int asgpoints, DateTime asgdue, string asgcontents)
        {
            var assignmentExists = from a in db.Assignments
                                   join ac in db.AssignmentCategories on a.AcId equals ac.AcId
                                   join cl in db.Classes on ac.ClassId equals cl.ClassId
                                   join co in db.Courses on cl.CId equals co.CId
                                   join d in db.Departments on co.DId equals d.DId
                                   where d.Subject == subject && co.Num == num && cl.SemesterSeason == season
                                   && cl.SemesterYear == year && ac.Name == category && a.AName == asgname
                                   && a.Points == asgpoints && a.DueDate == asgdue && a.Contents == asgcontents
                                   select a;

            // If assignment doesn't exist, add it
            if(assignmentExists.FirstOrDefault() == null)
            {
                var get_acID = from ac in db.AssignmentCategories
                               join cl in db.Classes on ac.ClassId equals cl.ClassId
                               join co in db.Courses on cl.CId equals co.CId
                               join d in db.Departments on co.DId equals d.DId
                               where d.Subject == subject && co.Num == num && cl.SemesterSeason == season
                               && cl.SemesterYear == year && ac.Name == category
                               select ac.AcId;

                Assignment newAssignment = new Assignment();
                newAssignment.AcId = get_acID.First();
                newAssignment.AName = asgname;
                newAssignment.Points = asgpoints;
                newAssignment.DueDate = asgdue;
                newAssignment.Contents = asgcontents;

                db.Assignments.Add(newAssignment);
                db.SaveChanges();

                return Json(new { success = true });
            }
            return Json(new { success = false });
        }


        /// <summary>
        /// Gets a JSON array of all the submissions to a certain assignment.
        /// Each object in the array should have the following fields:
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "time" - DateTime of the submission
        /// "score" - The score given to the submission
        /// 
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetSubmissionsToAssignment(string subject, int num, string season, int year, string category, string asgname)
        {
            var submissions = from su in db.Submissions
                              join a in db.Assignments on su.AId equals a.AId
                              join ac in db.AssignmentCategories on a.AcId equals ac.AcId
                              join cl in db.Classes on ac.ClassId equals cl.ClassId
                              join co in db.Courses on cl.CId equals co.CId
                              join d in db.Departments on co.DId equals d.DId
                              join st in db.Students on su.SId equals st.SId
                              where d.Subject == subject && co.Num == num && cl.SemesterSeason == season &&
                              cl.SemesterYear == year && ac.Name == category && a.AName == asgname
                              select new 
                              {
                                  fname = st.FName,
                                  lname = st.LName,
                                  uid = st.UId,
                                  time = su.SubmissionTime,
                                  score = su.Score
                              };

            if(submissions.First() != null)
            {
                return Json(submissions.ToArray());
            }
            return Json(null);
        }


        /// <summary>
        /// Set the score of an assignment submission
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <param name="uid">The uid of the student who's submission is being graded</param>
        /// <param name="score">The new score for the submission</param>
        /// <returns>A JSON object containing success = true/false</returns>
        public IActionResult GradeSubmission(string subject, int num, string season, int year, string category, string asgname, string uid, int score)
        {
            var submission = from su in db.Submissions
                             join a in db.Assignments on su.AId equals a.AId
                             join ac in db.AssignmentCategories on a.AcId equals ac.AcId
                             join cl in db.Classes on ac.ClassId equals cl.ClassId
                             join co in db.Courses on cl.CId equals co.CId
                             join d in db.Departments on co.DId equals d.DId
                             join st in db.Students on su.SId equals st.SId
                             where d.Subject == subject && co.Num == num && cl.SemesterSeason == season &&
                             cl.SemesterYear == year && ac.Name == category && a.AName == asgname &&
                             st.UId == uid
                             select su;

            if(submission.FirstOrDefault() != null)
            {
                submission.ToArray()[0].Score = (uint)score;
                db.SaveChanges();

                return Json(new { success = true });
            }

            return Json(new { success = false });
        }


        /// <summary>
        /// Returns a JSON array of the classes taught by the specified professor
        /// Each object in the array should have the following fields:
        /// "subject" - The subject abbreviation of the class (such as "CS")
        /// "number" - The course number (such as 5530)
        /// "name" - The course name
        /// "season" - The season part of the semester in which the class is taught
        /// "year" - The year part of the semester in which the class is taught
        /// </summary>
        /// <param name="uid">The professor's uid</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetMyClasses(string uid)
        {
            var myClasses = from p in db.Professors
                            join d in db.Departments on p.WorksIn equals d.DId
                            join co in db.Courses on d.DId equals co.DId
                            join cl in db.Classes on co.CId equals cl.CId
                            where p.UId == uid && p.PId == cl.TaughtBy
                            select new
                            {
                                subject = d.Subject,
                                number = co.Num,
                                name = co.Name,
                                season = cl.SemesterSeason,
                                year = cl.SemesterYear
                            };

            if(myClasses.FirstOrDefault() != null)
            {
                return Json(myClasses.ToArray());
            }

            return Json(null);
        }


        
        /*******End code to modify********/
    }
}

