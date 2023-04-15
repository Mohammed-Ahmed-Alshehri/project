using Microsoft.AspNetCore.Mvc;
using TadarbProject.Data;
using TadarbProject.Models.ViewModels;
using TadarbProject.Models;
using TEST2.Services;
using Microsoft.EntityFrameworkCore;

namespace TadarbProject.Controllers
{
    public class BranchController : Controller
    {
        private readonly AppDbContext _DbContext;
        private readonly IEmailSender _emailSender;
        private readonly IHttpContextAccessor _HttpContextAccessor;


        public BranchController(AppDbContext DbContext, IEmailSender emailSender, IHttpContextAccessor HttpContextAccessor)
        {
            _DbContext = DbContext;

            _emailSender = emailSender;
            _HttpContextAccessor = HttpContextAccessor;

        }
        public IActionResult Index()
        {

            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");

            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;

            var Branch = _DbContext.OrganizationBranches_TrainProv.Where(item => item.Responsible_UserId == RUserId).FirstOrDefault();

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Branch.Organization_OrganizationId).FirstOrDefault();


            ViewBag.OrganizationName = OrganizationOfR.OrganizationName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;

            return View();

        }

        [HttpGet]
        public IActionResult ViewSupervisorsUser()
        {

            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");

            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;

            var Branch = _DbContext.OrganizationBranches_TrainProv.Where(item => item.Responsible_UserId == RUserId).FirstOrDefault();

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Branch.Organization_OrganizationId).FirstOrDefault();


            ViewBag.OrganizationName = OrganizationOfR.OrganizationName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;

            var DEPOfR = _DbContext.Departments.Where(item => item.Branch_BranchId == Branch.BranchId && item.DepartmentName.Equals("قسم ادارة مشرفين التدريب")).FirstOrDefault();


            IEnumerable<UserAcount> Employees = Enumerable.Empty<UserAcount>(); ;


            if (DEPOfR != null)
            {

                Employees = _DbContext.UserAcounts.FromSqlRaw($"Select * from UserAcounts WHERE UserId IN (Select UserAccount_UserId from Employees WHERE Department_DepartmentId = {DEPOfR.DepartmentId});").ToList();


            }



            return View(Employees);
        }


        [HttpGet]
        public IActionResult AddSupervisorUser()
        {
            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");

            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;

            var Branch = _DbContext.OrganizationBranches_TrainProv.Where(item => item.Responsible_UserId == RUserId).FirstOrDefault();

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Branch.Organization_OrganizationId).FirstOrDefault();


            ViewBag.OrganizationName = OrganizationOfR.OrganizationName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;



            return View();

        }
        [HttpPost]
        public IActionResult AddSupervisorUser(EmployeeVM employeeVM)
        {
            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;

            var RUser = _DbContext.UserAcounts.Find(RUserId);

            var Branch = _DbContext.OrganizationBranches_TrainProv.Where(item => item.Responsible_UserId == RUserId).FirstOrDefault();
            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Branch.Organization_OrganizationId).FirstOrDefault();


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
                City_CityId = Branch.City_CityId,
                UserType = "Training_Supervisor",
                ActivationStatus = "Active"

            };

            _DbContext.UserAcounts.Add(user);

            _DbContext.SaveChanges();

            var DEPOfR = _DbContext.Departments.Where(item => item.Branch_BranchId == Branch.BranchId && item.DepartmentName.Equals("قسم ادارة مشرفين التدريب")).FirstOrDefault();

            if (DEPOfR == null)
            {
                var DEP = new Department
                {

                    DepartmentName = "قسم ادارة مشرفين التدريب",
                    Branch_BranchId = Branch.BranchId,
                    Responsible_UserId = Branch.Responsible_UserId,
                    Organization_OrganizationId = Branch.Organization_OrganizationId


                };

                _DbContext.Departments.Add(DEP);

                _DbContext.SaveChanges();
            }

            int DEPId = _DbContext.Departments.Where(item => item.Branch_BranchId == Branch.BranchId && item.DepartmentName.Equals("قسم ادارة مشرفين التدريب")).First().DepartmentId;

            var EMP = new Employee
            {

                Department_DepartmentId = DEPId,
                Job_JobId = 3,
                SSN = employeeVM.employee.SSN,
                UserAccount_UserId = user.UserId,



            };

            _DbContext.Employees.Add(EMP);

            _DbContext.SaveChanges();
            TempData["success"] = "تم إضافة حساب الموظف  بنجاح";
            return RedirectToAction("ViewSupervisorsUser");

        }

        public IActionResult ViewOpportunities()
        {

            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");

            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;

            var Branch = _DbContext.OrganizationBranches_TrainProv.Where(item => item.Responsible_UserId == RUserId).FirstOrDefault();

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Branch.Organization_OrganizationId).FirstOrDefault();


            ViewBag.OrganizationName = OrganizationOfR.OrganizationName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;


            return View();

        }

        public IActionResult AddOpportunities()
        {


            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");

            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;

            var Branch = _DbContext.OrganizationBranches_TrainProv.Where(item => item.Responsible_UserId == RUserId).FirstOrDefault();

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Branch.Organization_OrganizationId).FirstOrDefault();


            ViewBag.OrganizationName = OrganizationOfR.OrganizationName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;

            return View();

        }
        [HttpGet]
        public IActionResult AddDepartmentFiledSpecialties()
        {
            //ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");

            //int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;

            //var Branch = _DbContext.OrganizationBranches_TrainProv.Where(item => item.Responsible_UserId == RUserId).FirstOrDefault();

            //var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Branch.Organization_OrganizationId).FirstOrDefault();


            //ViewBag.OrganizationName = OrganizationOfR.OrganizationName;
            //ViewBag.OrganizationImage = OrganizationOfR.LogoPath;



            return View();



            

        }
        [HttpGet]
        public IActionResult ViewDepartmentFiledSpecialties()
        {
            //ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");

            //int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;

            //var Branch = _DbContext.OrganizationBranches_TrainProv.Where(item => item.Responsible_UserId == RUserId).FirstOrDefault();

            //var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Branch.Organization_OrganizationId).FirstOrDefault();


            //ViewBag.OrganizationName = OrganizationOfR.OrganizationName;
            //ViewBag.OrganizationImage = OrganizationOfR.LogoPath;



            return View();



          

        }


    }
}
