using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using TadarbProject.Data;
using TadarbProject.Models;
using TEST2.Services;

namespace TadarbProject.Controllers
{
    public class AcadmicSupervisor : Controller
    {
        private readonly AppDbContext _DbContext;
        private readonly IEmailSender _emailSender;
        private readonly IHttpContextAccessor _HttpContextAccessor;


        public AcadmicSupervisor(AppDbContext DbContext, IEmailSender emailSender, IHttpContextAccessor HttpContextAccessor)
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

            var Employee = _DbContext.Employees.Where(item => item.UserAccount_UserId == RUserId).Include(item => item.userAcount).Include(item => item.department).AsNoTracking().FirstOrDefault();

            var Department = _DbContext.Departments.Where(item => item.DepartmentId == Employee.Department_DepartmentId).AsNoTracking().FirstOrDefault();


            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Department.Organization_OrganizationId).AsNoTracking().FirstOrDefault();
            if (user.ActivationStatus == "Not_Active")
            {


                TempData["error"] = "حالة الحساب غير نشط " +
                    "الرجاء تعديل كلمة المرور  ";
                return RedirectToAction("EditAccount");

            }


            ViewBag.OrganizationName = OrganizationOfR.OrganizationName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;

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



        [HttpGet]
        public IActionResult AssignedStudents()
        {
            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");

            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;

            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();

            var Employee = _DbContext.Employees.Where(item => item.UserAccount_UserId == RUserId).Include(item => item.userAcount).Include(item => item.department).AsNoTracking().FirstOrDefault();

            var Department = _DbContext.Departments.Where(item => item.DepartmentId == Employee.Department_DepartmentId).AsNoTracking().FirstOrDefault();


            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Department.Organization_OrganizationId).AsNoTracking().FirstOrDefault();

            ViewBag.OrganizationName = OrganizationOfR.OrganizationName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;


            IEnumerable<StudentRequestOpportunity> students = Enumerable.Empty<StudentRequestOpportunity>();


            students = _DbContext.StudentRequestsOnOpportunities.FromSqlRaw("SELECT * FROM StudentRequestsOnOpportunities WHERE  StudentRequestOpportunityId IN " +
                "(SELECT StudentRequest_StudentRequestId FROM SemestersStudentAndEvaluationDetails WHERE AcademicSupervisor_EmployeeId = " +
                $"(SELECT EmployeeId FROM  Employees WHERE UserAccount_UserId ={RUserId}))").Include(item => item.student.user).Include(item => item.trainingOpportunity.Branch.organization).AsNoTracking().ToList();


            return View(students);

        }


    }
}
