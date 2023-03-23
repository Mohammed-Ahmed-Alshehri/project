using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TadarbProject.Data;
using TadarbProject.Models;
using TadarbProject.Models.ViewModels;
using TEST2.Services;

namespace TadarbProject.Controllers
{
    public class AdminController : Controller
    {

        private readonly AppDbContext _DbContext;
        private readonly IWebHostEnvironment _WebHostEnvironment;
        private readonly IEmailSender _emailSender;
        public AdminController(AppDbContext DbContext, IWebHostEnvironment webHostEnvironment, IEmailSender emailSender)
        {
            _DbContext = DbContext;
            _WebHostEnvironment = webHostEnvironment;
            _emailSender = emailSender;


        }

        public IActionResult Index(int? TypeId)
        {

            var OrganizationList = _DbContext.Organizations.ToList();

            if (TypeId == null)
            {
                return View(OrganizationList);
            }

            if (TypeId == 1)
            {

                OrganizationList = _DbContext.Organizations.Where(item => item.Organization_TypeId == 1).ToList();


            }

            if (TypeId == 2)
            {
                OrganizationList = _DbContext.Organizations.Where(item => item.Organization_TypeId == 2).ToList();


            }

            return View(OrganizationList);
        }




        #region
        public IActionResult GetAllUsers()
        {

            var UserAcountList = _DbContext.UserAcounts.ToList();

            var CityList = _DbContext.Cities.ToList();

            return Json(new { data = CityList });
        }


        #endregion




    }
}