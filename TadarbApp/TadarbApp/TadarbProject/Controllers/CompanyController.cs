using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TadarbProject.Data;
using TadarbProject.Models;
using TadarbProject.Models.ViewModels;
using TEST2.Services;

namespace TadarbProject.Controllers
{
    //yazeed edit
    //badr modify
    //abdulhadi
    public class CompanyController : Controller
    {
        private readonly AppDbContext _DbContext;
        private readonly IEmailSender _emailSender;

        public CompanyController(AppDbContext DbContext, IEmailSender emailSender)
        {
            _DbContext = DbContext;

            _emailSender = emailSender;


        }

        public IActionResult Index()
        {


            return View();
        }

        public IActionResult ViewSpecialities()
        {


            return View();
        }
        public IActionResult AddSpecialities()
        {


            return View();
        }

        public IActionResult ViewBranches()
        {


            return View();
        }
        public IActionResult Addbranches()
        {


            return View();
        }

        public IActionResult ViewUsers()
        {


            return View();
        }

        [HttpGet]
        public IActionResult AddUsers()
        {


            return View();
        }


        //[HttpPost]
        //public IActionResult AddUsers(EmployeeVM employeeVM)
        //{

        //    if (employeeVM == null)
        //    {

        //        return View();

        //    }

        //    var user = new UserAcount
        //    {
        //        UserEmail = employeeVM.userAcount.UserEmail,
        //        UserPassword = employeeVM.userAcount.UserPassword,
        //        FullName = employeeVM.userAcount.FullName,
        //        Phone = employeeVM.userAcount.Phone,
        //        City_CityId = 1,
        //        UserType = "Branch_Admin",
        //        ActivationStatus = "Active"




        //    };

        //    _DbContext.UserAcounts.Add(user);

        //    _DbContext.SaveChanges();

        //    int RUserId = _DbContext.UserAcounts.Where(item => item.UserEmail == user.UserEmail).First().UserId;

        //    ////var DEP = new Department
        //    ////{

        //    ////    DepartmentName = "قسم ادارة الفروع",
        //    ////    Organization_OrganizationId = 4,
        //    ////    Responsible_UserId = 7,



        //    ////};

        //    //_DbContext.Departments.Add(DEP);

        //    //_DbContext.SaveChanges();

        //    int DEPId = _DbContext.Departments.Where(item => item.Organization_OrganizationId == 4 && item.DepartmentName.Equals("قسم ادارة الفروع")).First().DepartmentId;

        //    var EMP = new Employee
        //    {

        //        Department_DepartmentId = DEPId,
        //        Job_JobId = 1,
        //        SSN = employeeVM.employee.SSN,
        //        UserAccount_UserId = RUserId,



        //    };

        //    _DbContext.Employees.Add(EMP);

        //    _DbContext.SaveChanges();

        //    return View();
        //}




        //#region
        //public IActionResult GetAllEMP()
        //{
        //    var OR = _DbContext.Organizations.Where(item => item.OrganizationId == 4).FirstOrDefault();

        //    var  dpt = _DbContext.Departments.Where(item => item.Organization_OrganizationId == OR.OrganizationId && item.DepartmentName.Equals("قسم ادارة الفروع")).FirstOrDefault();

        //    var emp = _DbContext.Employees.Where(item => item.Department_DepartmentId == dpt.DepartmentId).Include(item => item.userAcount);


        //    //var UserAcountList = _DbContext.UserAcounts.Where(item => item.UserId == );



        //    return Json(new { data = emp });
    }
}
//#endregion
//}
//}