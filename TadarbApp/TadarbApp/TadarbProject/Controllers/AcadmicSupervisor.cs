using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TadarbProject.Data;
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

            var Employee = _DbContext.Employees.Where(item => item.UserAccount_UserId == RUserId).AsNoTracking().FirstOrDefault();

            var Department = _DbContext.Departments.Where(item => item.DepartmentId == Employee.Department_DepartmentId).AsNoTracking().FirstOrDefault();


            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Department.Organization_OrganizationId).AsNoTracking().FirstOrDefault();


            ViewBag.OrganizationName = OrganizationOfR.OrganizationName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;

            return View();
        }
    }
}
