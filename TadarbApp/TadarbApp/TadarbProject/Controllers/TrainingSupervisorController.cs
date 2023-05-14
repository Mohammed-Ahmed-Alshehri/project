﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TadarbProject.Data;
using TadarbProject.Models;
using TEST2.Services;

namespace TadarbProject.Controllers
{
    public class TrainingSupervisorController : Controller
    {
        private readonly AppDbContext _DbContext;
        private readonly IEmailSender _emailSender;
        private readonly IHttpContextAccessor _HttpContextAccessor;


        public TrainingSupervisorController(AppDbContext DbContext, IEmailSender emailSender, IHttpContextAccessor HttpContextAccessor)
        {
            _DbContext = DbContext;

            _emailSender = emailSender;
            _HttpContextAccessor = HttpContextAccessor;

        }
        public IActionResult Index()
        {



            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");

            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;

            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();

            if (user.ActivationStatus == "Not_Active")
            {


                TempData["error"] = "حالة الحساب غير نشط " +
                    "الرجاء تعديل كلمة المرور  ";
                return RedirectToAction("EditAccount");

            }
            var Employee = _DbContext.Employees.Where(item => item.UserAccount_UserId == RUserId).Include(item => item.userAcount).
                Include(item => item.department).AsNoTracking().FirstOrDefault();

            var Department = _DbContext.Departments.Where(item => item.DepartmentId == Employee.Department_DepartmentId).Include(item => item.OrganizationBranch).AsNoTracking().FirstOrDefault();


            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Department.Organization_OrganizationId).AsNoTracking().FirstOrDefault();


            ViewBag.OrganizationName = OrganizationOfR.OrganizationName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;

            ViewBag.Department = Department.OrganizationBranch.BranchName;

            return View(Employee);

        }
        [HttpGet]
        public IActionResult EditAccount()
        {
            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");
            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();




            ViewBag.Username = user.FullName;


            return View();
        }
        [HttpPost]
        public IActionResult EditAccount(UserAcount UserAcount)
        {
            

            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");

            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();


            user.UserPassword = UserAcount.UserPassword;

            user.ActivationStatus = "Active";

            _DbContext.UserAcounts.Update(user);


            _DbContext.SaveChanges();

            return RedirectToAction("Index");

        }


        public IActionResult ViewOpportunities()
        {

            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");

            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();
            var Emplyee = _DbContext.Employees.Where(item => item.UserAccount_UserId == RUserId).FirstOrDefault();

            var Department = _DbContext.Departments.Where(item => item.DepartmentId == Emplyee.Department_DepartmentId).Include(item => item.OrganizationBranch).AsNoTracking().FirstOrDefault();
            


            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Department.Organization_OrganizationId).AsNoTracking().FirstOrDefault();


            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Department.OrganizationBranch.BranchName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;

            var Opportunities = _DbContext.TrainingOpportunities.Where(item => item.SupervisorEmployeeId == Emplyee.EmployeeId).Include(item => item.DetailFiled).AsNoTracking().ToList();



            return View(Opportunities);

        }

        #region

        public IActionResult OpportunitiesApplicants(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");

            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();
            var Emplyee = _DbContext.Employees.Where(item => item.UserAccount_UserId == RUserId).FirstOrDefault();

            var Department = _DbContext.Departments.Where(item => item.DepartmentId == Emplyee.Department_DepartmentId).Include(item => item.OrganizationBranch).AsNoTracking().FirstOrDefault();



            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Department.Organization_OrganizationId).AsNoTracking().FirstOrDefault();


            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Department.OrganizationBranch.BranchName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;


        

            ViewBag.OpportunitiyId = id;



            return View();
        }


        [HttpGet]
        public IActionResult GetStudentsList(int? id, string? gr, int? UPOrDw, int? ArOrWa)
        {


            IEnumerable<UniversityTraineeStudent> Students = Enumerable.Empty<UniversityTraineeStudent>();

            if (ArOrWa == 0)
            {
                if (gr == null && UPOrDw == 0)
                {

                    Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE TraineeId IN " +
                   $"(SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities WHERE TrainingOpportunity_TrainingOpportunityId ={id} AND DecisionStatus='waiting')")
                      .AsNoTracking().Include(item => item.user)
                      .AsNoTracking().Include(item => item.department.universityCollege.organization)
                      .AsNoTracking().Include(item => item.department.universityCollege.city.Country).ToList();

                    return Json(new { Students });
                }

                if (gr != null && UPOrDw == 0)
                {

                    Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE TraineeId IN " +
                   $"(SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities WHERE TrainingOpportunity_TrainingOpportunityId ={id} AND DecisionStatus='waiting') AND Gender ='{gr}' ")
                      .AsNoTracking().Include(item => item.user)
                      .AsNoTracking().Include(item => item.department.universityCollege.organization)
                      .AsNoTracking().Include(item => item.department.universityCollege.city.Country).ToList();

                    return Json(new { Students });
                }



                if (gr != null && UPOrDw == 1)
                {

                    Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE TraineeId IN " +
                   $"(SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities WHERE TrainingOpportunity_TrainingOpportunityId ={id} AND DecisionStatus='waiting' ) AND Gender ='{gr}' ")
                      .AsNoTracking().Include(item => item.user)
                      .AsNoTracking().Include(item => item.department.universityCollege.organization)
                      .AsNoTracking().Include(item => item.department.universityCollege.city.Country).OrderBy(item => item.GPA).ToList();

                    return Json(new { Students });
                }

                if (gr != null && UPOrDw == 2)
                {

                    Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE TraineeId IN " +
                   $"(SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities WHERE TrainingOpportunity_TrainingOpportunityId ={id} AND DecisionStatus='waiting' ) AND Gender ='{gr}' ")
                      .AsNoTracking().Include(item => item.user)
                      .AsNoTracking().Include(item => item.department.universityCollege.organization)
                      .AsNoTracking().Include(item => item.department.universityCollege.city.Country).OrderByDescending(item => item.GPA).ToList();

                    return Json(new { Students });
                }


                if (gr == null && UPOrDw == 1)
                {

                    Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE TraineeId IN " +
                   $"(SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities WHERE TrainingOpportunity_TrainingOpportunityId ={id} AND DecisionStatus='waiting') ")
                      .AsNoTracking().Include(item => item.user)
                      .AsNoTracking().Include(item => item.department.universityCollege.organization)
                      .AsNoTracking().Include(item => item.department.universityCollege.city.Country).OrderBy(item => item.GPA).ToList();

                    return Json(new { Students });
                }

                if (gr == null && UPOrDw == 2)
                {

                    Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE TraineeId IN " +
                   $"(SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities WHERE TrainingOpportunity_TrainingOpportunityId ={id} AND DecisionStatus='waiting' ) ")
                      .AsNoTracking().Include(item => item.user)
                      .AsNoTracking().Include(item => item.department.universityCollege.organization)
                      .AsNoTracking().Include(item => item.department.universityCollege.city.Country).OrderByDescending(item => item.GPA).ToList();

                    return Json(new { Students });
                }

            }



            if (ArOrWa == 1)
            {
                if (gr == null && UPOrDw == 0)
                {

                    Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE TraineeId IN " +
                   $"(SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities WHERE TrainingOpportunity_TrainingOpportunityId ={id} AND DecisionStatus='approved')")
                      .AsNoTracking().Include(item => item.user)
                      .AsNoTracking().Include(item => item.department.universityCollege.organization)
                      .AsNoTracking().Include(item => item.department.universityCollege.city.Country).ToList();

                    return Json(new { Students });
                }

                if (gr != null && UPOrDw == 0)
                {

                    Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE TraineeId IN " +
                   $"(SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities WHERE TrainingOpportunity_TrainingOpportunityId ={id} AND DecisionStatus='approved') AND Gender ='{gr}' ")
                      .AsNoTracking().Include(item => item.user)
                      .AsNoTracking().Include(item => item.department.universityCollege.organization)
                      .AsNoTracking().Include(item => item.department.universityCollege.city.Country).ToList();

                    return Json(new { Students });
                }



                if (gr != null && UPOrDw == 1)
                {

                    Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE TraineeId IN " +
                   $"(SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities WHERE TrainingOpportunity_TrainingOpportunityId ={id} AND DecisionStatus='approved' ) AND Gender ='{gr}' ")
                      .AsNoTracking().Include(item => item.user)
                      .AsNoTracking().Include(item => item.department.universityCollege.organization)
                      .AsNoTracking().Include(item => item.department.universityCollege.city.Country).OrderBy(item => item.GPA).ToList();

                    return Json(new { Students });
                }

                if (gr != null && UPOrDw == 2)
                {

                    Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE TraineeId IN " +
                   $"(SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities WHERE TrainingOpportunity_TrainingOpportunityId ={id} AND DecisionStatus='approved' ) AND Gender ='{gr}' ")
                      .AsNoTracking().Include(item => item.user)
                      .AsNoTracking().Include(item => item.department.universityCollege.organization)
                      .AsNoTracking().Include(item => item.department.universityCollege.city.Country).OrderByDescending(item => item.GPA).ToList();

                    return Json(new { Students });
                }


                if (gr == null && UPOrDw == 1)
                {

                    Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE TraineeId IN " +
                   $"(SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities WHERE TrainingOpportunity_TrainingOpportunityId ={id} AND DecisionStatus='approved') ")
                      .AsNoTracking().Include(item => item.user)
                      .AsNoTracking().Include(item => item.department.universityCollege.organization)
                      .AsNoTracking().Include(item => item.department.universityCollege.city.Country).OrderBy(item => item.GPA).ToList();

                    return Json(new { Students });
                }

                if (gr == null && UPOrDw == 2)
                {

                    Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE TraineeId IN " +
                   $"(SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities WHERE TrainingOpportunity_TrainingOpportunityId ={id} AND DecisionStatus='approved' ) ")
                      .AsNoTracking().Include(item => item.user)
                      .AsNoTracking().Include(item => item.department.universityCollege.organization)
                      .AsNoTracking().Include(item => item.department.universityCollege.city.Country).OrderByDescending(item => item.GPA).ToList();

                    return Json(new { Students });
                }


            }


            return Json(new { Students });
        }

        [HttpGet]
        public IActionResult GetStudentsListByFilter(int? id, int? ArOrWa, string? filter)
        {


            IEnumerable<UniversityTraineeStudent> Students = Enumerable.Empty<UniversityTraineeStudent>();


            if (ArOrWa == 0)
            {
                Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE TraineeId IN " +
                $"(SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities WHERE TrainingOpportunity_TrainingOpportunityId  ={id} AND DecisionStatus='waiting') " +
                $"AND UserAccount_UserId IN (SELECT UserId FROM UserAcounts WHERE FullName = '{filter}' )")
                .AsNoTracking().Include(item => item.user)
                .AsNoTracking().Include(item => item.department.universityCollege.organization)
                .AsNoTracking().Include(item => item.department.universityCollege.city.Country).ToList();


                if (Students.Count() != 0)
                {
                    return Json(new { Students });
                }



                Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw("SELECT * FROM UniversitiesTraineeStudents WHERE TraineeId IN " +
                    $"(SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities WHERE TrainingOpportunity_TrainingOpportunityId ={id} AND DecisionStatus='waiting') " +
                    $"AND Department_DepartmentId IN (SELECT Department_DepartmentId FROM Departments WHERE Organization_OrganizationId " +
                    $"IN( SELECT OrganizationId FROM Organizations WHERE OrganizationName = '{filter}'))")
                    .AsNoTracking().Include(item => item.user)
                    .AsNoTracking().Include(item => item.department.universityCollege.organization)
                    .AsNoTracking().Include(item => item.department.universityCollege.city.Country).ToList();

                return Json(new { Students });
            }



            if (ArOrWa == 1)
            {
                Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE TraineeId IN " +
                $"(SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities WHERE TrainingOpportunity_TrainingOpportunityId  ={id} AND DecisionStatus='approved') " +
                $"AND UserAccount_UserId IN (SELECT UserId FROM UserAcounts WHERE FullName = '{filter}' )")
                .AsNoTracking().Include(item => item.user)
                .AsNoTracking().Include(item => item.department.universityCollege.organization)
                .AsNoTracking().Include(item => item.department.universityCollege.city.Country).ToList();


                if (Students.Count() != 0)
                {
                    return Json(new { Students });
                }



                Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw("SELECT * FROM UniversitiesTraineeStudents WHERE TraineeId IN " +
                    $"(SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities WHERE TrainingOpportunity_TrainingOpportunityId ={id} AND DecisionStatus='approved') " +
                    $"AND Department_DepartmentId IN (SELECT Department_DepartmentId FROM Departments WHERE Organization_OrganizationId " +
                    $"IN( SELECT OrganizationId FROM Organizations WHERE OrganizationName = '{filter}'))")
                    .AsNoTracking().Include(item => item.user)
                    .AsNoTracking().Include(item => item.department.universityCollege.organization)
                    .AsNoTracking().Include(item => item.department.universityCollege.city.Country).ToList();

                return Json(new { Students });

            }



            return Json(new { Students });
        }

        #endregion

    }
}
