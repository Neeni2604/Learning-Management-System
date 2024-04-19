using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
[assembly: InternalsVisibleTo( "LMSControllerTests" )]
namespace LMS.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private LMSContext db;
        public StudentController(LMSContext _db)
        {
            db = _db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Catalog()
        {
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


        public IActionResult ClassListings(string subject, string num)
        {
            System.Diagnostics.Debug.WriteLine(subject + num);
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            return View();
        }


        /*******Begin code to modify********/

        /// <summary>
        /// Returns a JSON array of the classes the given student is enrolled in.
        /// Each object in the array should have the following fields:
        /// "subject" - The subject abbreviation of the class (such as "CS")
        /// "number" - The course number (such as 5530)
        /// "name" - The course name
        /// "season" - The season part of the semester
        /// "year" - The year part of the semester
        /// "grade" - The grade earned in the class, or "--" if one hasn't been assigned
        /// </summary>
        /// <param name="uid">The uid of the student</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetMyClasses(string uid)
        {
            var query = from s in db.Students
                        join e in db.Enrolleds
                        on s.SId equals e.SId
                        join c in db.Classes
                        on e.ClassId equals c.ClassId
                        join co in db.Courses
                        on c.CId equals co.CId
                        join d in db.Departments
                        on co.DId equals d.DId
                        where s.UId == uid
                        select new
                        {
                            subject = d.Subject,
                            number = co.Num,
                            name = co.Name,
                            season = c.SemesterSeason,
                            year = c.SemesterYear,
                            grade = e.Grade
                        };

            return Json(query.ToArray());
        }

        /// <summary>
        /// Returns a JSON array of all the assignments in the given class that the given student is enrolled in.
        /// Each object in the array should have the following fields:
        /// "aname" - The assignment name
        /// "cname" - The category name that the assignment belongs to
        /// "due" - The due Date/Time
        /// "score" - The score earned by the student, or null if the student has not submitted to this assignment.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="uid"></param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentsInClass(string subject, int num, string season, int year, string uid)
        {
            var query = from st in db.Students
                        join e in db.Enrolleds on st.SId equals e.SId
                        join cl in db.Classes on e.ClassId equals cl.ClassId
                        join co in db.Courses on cl.CId equals co.CId
                        join d in db.Departments on co.DId equals d.DId
                        join ac in db.AssignmentCategories on cl.ClassId equals ac.ClassId
                        join a in db.Assignments on ac.AcId equals a.AcId
                        //join s in db.Submissions on a.AId equals s.AId
                        where d.Subject == subject && co.Num == num && cl.SemesterSeason == season &&
                        cl.SemesterYear == year && st.UId == uid
                        select new
                        {
                            aname = a.AName,
                            cname = ac.Name,
                            due = a.DueDate,
                            score = from s in a.Submissions where s.SIdNavigation.UId == uid 
                                    select s.Score
                                    
                        };

            return Json(query.ToArray());
        }



        /// <summary>
        /// Adds a submission to the given assignment for the given student
        /// The submission should use the current time as its DateTime
        /// You can get the current time with DateTime.Now
        /// The score of the submission should start as 0 until a Professor grades it
        /// If a Student submits to an assignment again, it should replace the submission contents
        /// and the submission time (the score should remain the same).
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The new assignment name</param>
        /// <param name="uid">The student submitting the assignment</param>
        /// <param name="contents">The text contents of the student's submission</param>
        /// <returns>A JSON object containing {success = true/false}</returns>
        public IActionResult SubmitAssignmentText(string subject, int num, string season, int year,
          string category, string asgname, string uid, string contents)
        {
            // Check if submission exists
            var get_sID = from st in db.Students
                          where st.UId == uid
                          select st.SId;

            
            var get_aID = from a in db.Assignments
                          join ac in db.AssignmentCategories on a.AcId equals ac.AcId
                          join cl in db.Classes on ac.ClassId equals cl.ClassId
                          join co in db.Courses on cl.CId equals co.CId
                          join d in db.Departments on co.DId equals d.DId
                          where d.Subject == subject && co.Num == num && cl.SemesterSeason == season &&
                          cl.SemesterYear == year && ac.Name == category && a.AName == asgname
                          select a.AId;


            var submitted = from su in db.Submissions
                            where su.SId == get_sID.FirstOrDefault() && su.AId == get_aID.FirstOrDefault()
                            select su;

            // If there is no submission for that student and assignment name, create a new submission and 
            // insert it.
            // Else, delete the existing submission, create a new submission and submit the new one
            if(submitted.FirstOrDefault() == null)
            {
                Submission sub = new Submission();

                sub.SId = get_sID.FirstOrDefault();
                sub.AId = get_aID.FirstOrDefault();
                sub.Contents = contents;
                sub.Score = 0;
                sub.SubmissionTime = DateTime.Now;

                db.Submissions.Add(sub);

                db.SaveChanges();

                return Json(new { success = true });
            }
            else
            {
                // Getting the current submission
                var getCurrentSubmission = from st in db.Students
                                           join e in db.Enrolleds on st.SId equals e.SId
                                           join cl in db.Classes on e.ClassId equals cl.ClassId
                                           join co in db.Courses on cl.CId equals co.CId
                                           join d in db.Departments on co.DId equals d.DId
                                           join ac in db.AssignmentCategories on cl.ClassId equals ac.ClassId
                                           join a in db.Assignments on ac.AcId equals a.AcId
                                           join su in db.Submissions on a.AId equals su.AId
                                           where d.Subject == subject && co.Num == num &&
                                           cl.SemesterSeason == season && cl.SemesterYear == year
                                           && ac.Name == category && a.AName == asgname && st.UId == uid
                                           select su;


                // Removing the current submission
                db.Submissions.RemoveRange(getCurrentSubmission);
                db.SaveChanges();

                // Creating a new 'current' Submission and inserting it into the database
                Submission sub = new Submission();

                sub.SId = get_sID.FirstOrDefault();
                sub.AId = get_aID.FirstOrDefault();
                sub.Contents = contents;
                sub.Score = 0;
                sub.SubmissionTime = DateTime.Now;

                db.Submissions.Add(sub);

                db.SaveChanges();

                return Json(new { success = true });
            }

        }


        /// <summary>
        /// Enrolls a student in a class.
        /// </summary>
        /// <param name="subject">The department subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester</param>
        /// <param name="year">The year part of the semester</param>
        /// <param name="uid">The uid of the student</param>
        /// <returns>A JSON object containing {success = {true/false}. 
        /// false if the student is already enrolled in the class, true otherwise.</returns>
        public IActionResult Enroll(string subject, int num, string season, int year, string uid)
        {
            // Checking if a student is already enrolled
            var enrolled = from st in db.Students
                           join e in db.Enrolleds on st.SId equals e.SId
                           join cl in db.Classes on e.ClassId equals cl.ClassId
                           join co in db.Courses on cl.CId equals co.CId
                           join d in db.Departments on co.DId equals d.DId
                           where d.Subject == subject && co.Num == num && cl.SemesterSeason == season
                           && cl.SemesterYear == year && st.UId == uid
                           select e;

            if(enrolled.FirstOrDefault() == null)
            {
                var get_sID = from st in db.Students
                              where st.UId == uid
                              select st.SId;

                var get_classID = from cl in db.Classes
                                  join co in db.Courses on cl.CId equals co.CId
                                  join d in db.Departments on co.DId equals d.DId
                                  where d.Subject == subject && co.Num == num && cl.SemesterSeason == season
                                  && cl.SemesterYear == year
                                  select cl.ClassId;

                Enrolled enroll = new Enrolled();
                enroll.SId = get_sID.FirstOrDefault();
                enroll.ClassId = get_classID.FirstOrDefault();
                enroll.Grade = "";

                db.Enrolleds.Add(enroll);
                db.SaveChanges();

                return Json(new { success = true });
            }

            return Json(new { success = false});
        }



        /// <summary>
        /// Calculates a student's GPA
        /// A student's GPA is determined by the grade-point representation of the average grade in all their classes.
        /// Assume all classes are 4 credit hours.
        /// If a student does not have a grade in a class ("--"), that class is not counted in the average.
        /// If a student is not enrolled in any classes, they have a GPA of 0.0.
        /// Otherwise, the point-value of a letter grade is determined by the table on this page:
        /// https://advising.utah.edu/academic-standards/gpa-calculator-new.php
        /// </summary>
        /// <param name="uid">The uid of the student</param>
        /// <returns>A JSON object containing a single field called "gpa" with the number value</returns>
        public IActionResult GetGPA(string uid)
        {
            var grades = from st in db.Students
                           join e in db.Enrolleds on st.SId equals e.SId
                           where st.UId == uid
                           select e.Grade;

            double gpaValue = 0.0;
            var test = grades.ToArray();

            if(grades.FirstOrDefault() == null)
            {
                var jsonObject = new { gpa = gpaValue };
                return Json(jsonObject);
            }
            else
            {
                double numberOfClasses = grades.Count();

                double GpaSum = 0.0;

                foreach(var grade in grades)
                {
                    if (grade.ToUpper() == "A")
                        GpaSum += 4.0;
                    else if (grade.ToUpper() == "A-")
                        GpaSum += 3.7;
                    else if (grade.ToUpper() == "B+")
                        GpaSum += 3.3;
                    else if (grade.ToUpper() == "B")
                        GpaSum += 3.0;
                    else if (grade.ToUpper() == "B-")
                        GpaSum += 2.7;
                    else if (grade.ToUpper() == "C+")
                        GpaSum += 2.3;
                    else if (grade.ToUpper() == "C")
                        GpaSum += 2.0;
                    else if (grade.ToUpper() == "D+")
                        GpaSum += 1.3;
                    else if (grade.ToUpper() == "D")
                        GpaSum += 1.0;
                    else if (grade.ToUpper() == "D-")
                        GpaSum += 0.7;
                    else if (grade.ToUpper() == "E")
                        GpaSum += 0.0;
                }

                gpaValue = GpaSum / numberOfClasses;

                var jsonObject = new { gpa = gpaValue };

                

                return Json(jsonObject);
            }
        }
                
        /*******End code to modify********/

    }
}

