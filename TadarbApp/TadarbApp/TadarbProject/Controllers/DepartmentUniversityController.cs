using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TadarbProject.Data;
using TadarbProject.Models;
using TadarbProject.Models.ViewModels;
using TEST2.Services;

namespace TadarbProject.Controllers
{
    public class DepartmentUniversityController : Controller
    {
        private readonly AppDbContext _DbContext;
        private readonly IEmailSender _emailSender;
        private readonly IHttpContextAccessor _HttpContextAccessor;


        public DepartmentUniversityController(AppDbContext DbContext, IEmailSender emailSender, IHttpContextAccessor HttpContextAccessor)
        {
            _DbContext = DbContext;

            _emailSender = emailSender;
            _HttpContextAccessor = HttpContextAccessor;

        }
        public IActionResult Index()
        {
            //ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");

            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;

            var Department = _DbContext.Departments.Where(item => item.Responsible_UserId == RUserId).FirstOrDefault();

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Department.Organization_OrganizationId).FirstOrDefault();



            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Department.DepartmentName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;


            return View();
        }




        public IActionResult AddAcademicSupervisor()
        {
            // ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");

            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;

            var Department = _DbContext.Departments.Where(item => item.Responsible_UserId == RUserId).FirstOrDefault();

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Department.Organization_OrganizationId).FirstOrDefault();


            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Department.DepartmentName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;


            return View();
        }


        [HttpPost]
        public IActionResult AddAcademicSupervisor(EmployeeVM employeeVM)
        {



            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;

            var RUser = _DbContext.UserAcounts.Find(RUserId);

            var Department = _DbContext.Departments.Where(item => item.Responsible_UserId == RUserId).FirstOrDefault();

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Department.Organization_OrganizationId).FirstOrDefault();

            var College = _DbContext.UniversityColleges.Where(item => item.CollegeId == Department.College_CollegeId).FirstOrDefault();

            if (employeeVM == null)
            {

                return View();

            }

            var user = new UserAcount
            {
                UserEmail = employeeVM.userAcount.UserEmail,
                UserPassword = employeeVM.userAcount.UserPassword,
                FullName = employeeVM.userAcount.FullName,
                Phone = employeeVM.userAcount.Phone,
                City_CityId = College.City_CityId,
                UserType = "Academic_supervisor",
                ActivationStatus = "Not_Active"

            };

            _DbContext.UserAcounts.Add(user);

            _DbContext.SaveChanges();




            var EMP = new Employee
            {

                Department_DepartmentId = Department.DepartmentId,
                Job_JobId = 4,
                SSN = employeeVM.employee.SSN,
                UserAccount_UserId = user.UserId,



            };

            _DbContext.Employees.Add(EMP);

            _DbContext.SaveChanges();
            TempData["success"] = "تم إضافة حساب الموظف  بنجاح";
            return RedirectToAction("ViewAcademicSupervisors");

        }




        public IActionResult ViewAcademicSupervisors()
        {
            // ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");

            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;

            var Department = _DbContext.Departments.Where(item => item.Responsible_UserId == RUserId).FirstOrDefault();

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Department.Organization_OrganizationId).FirstOrDefault();


            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Department.DepartmentName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;

            IEnumerable<UserAcount> Employees = Enumerable.Empty<UserAcount>();


            Employees = _DbContext.UserAcounts.FromSqlRaw("Select * from UserAcounts WHERE UserId IN " +
            $"(Select UserAccount_UserId from Employees WHERE Department_DepartmentId = {Department.DepartmentId});").ToList();

            //try
            //{
            //    Employees = _DbContext.UserAcounts.FromSqlRaw("Select * from UserAcounts WHERE UserId IN " +
            //  $"(Select UserAccount_UserId from Employees WHERE Department_DepartmentId = {Department.DepartmentId});").ToList();
            //}

            //catch (Exception ex)
            //{
            //    Employees = Enumerable.Empty<UserAcount>();
            //}



            return View(Employees);
        }






    }
}
