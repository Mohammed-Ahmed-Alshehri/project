using Microsoft.AspNetCore.Mvc;
using TadarbProject.Data;
using TEST2.Services;

namespace TadarbProject.Controllers
{
    public class StudentController : Controller
    {
        private readonly AppDbContext _DbContext;
        private readonly IEmailSender _emailSender;
        private readonly IHttpContextAccessor _HttpContextAccessor;


        public StudentController(AppDbContext DbContext, IEmailSender emailSender, IHttpContextAccessor HttpContextAccessor)
        {
            _DbContext = DbContext;

            _emailSender = emailSender;
            _HttpContextAccessor = HttpContextAccessor;

        }
        public IActionResult Index()
        {
            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");
            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).FirstOrDefault();
            var UnverTre = _DbContext.UniversitiesTraineeStudents.Where(item => item.UserAccount_UserId == RUserId).FirstOrDefault();
            var Department = _DbContext.Departments.Where(item => item.DepartmentId == UnverTre.Department_DepartmentId).FirstOrDefault();

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Department.Organization_OrganizationId).FirstOrDefault();


            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Department.DepartmentName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;
      

            return View();
        }
    }
}
