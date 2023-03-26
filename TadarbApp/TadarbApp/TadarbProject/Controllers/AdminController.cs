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
    //yazeed edit
    //badr modify
    //abdulhadi
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

        public IActionResult Index()
        {


            return View();
        }


        public IActionResult OrganizationDetails(int? id)
        {

            if (id == null)
            {

                return NotFound();
            }

            var Organization = _DbContext.Organizations.FirstOrDefault(item => item.OrganizationId == id);



            if (Organization == null)
            {
                return NotFound();
            }

            var Ruser = _DbContext.UserAcounts.FirstOrDefault(item => item.UserId == Organization.ResponsibleUserId);

            var city = _DbContext.Cities.FirstOrDefault(item => item.CityId == Organization.MainBranchCityId);

            var Country = _DbContext.Countries.FirstOrDefault(item => item.CountryId == city.Country_CountryId);



            OrganizationVM organizationVM = new OrganizationVM
            {
                organization = Organization,
                userAcount = Ruser,

                CountryListItems = _DbContext.Countries.Where(item => item.CountryId == Country.CountryId).Select(u => new SelectListItem { Text = u.CountryName, Value = u.CountryId.ToString() }),

                CityListItems = _DbContext.Cities.Where(item => item.CityId == city.CityId).Select(u => new SelectListItem { Text = u.CityName, Value = u.CityId.ToString() }),

                FieldListItems = _DbContext.FieldOfSpecialtiesMaster.Where(item => item.FieldId == Organization.FieldOfOrganization_SpecialtiesFieldId).Select(u => new SelectListItem { Text = u.FieldName, Value = u.FieldId.ToString() })

            };


            return View(organizationVM);
        }


        public IActionResult RequestAccept(int? id)
        {
            if (id == null)
            {
                return NotFound();


            }
            var Organization = _DbContext.Organizations.Find(id);

            if (Organization == null) { return NotFound(); }

            Organization.ActivationStatus = "Active";

            _DbContext.Organizations.Update(Organization);

            _DbContext.SaveChanges();
            TempData["success"] = "تم تفعيل حالة المنظمة إلى نشط بنجاح";
            return RedirectToAction("Index");
        }

        public IActionResult RequestReject(int? id)
        {
            if (id == null)
            {
                return NotFound();


            }
            var Organization = _DbContext.Organizations.Find(id);

            if (Organization == null) { return NotFound(); }

            Organization.ActivationStatus = "Rejected";

            _DbContext.Organizations.Update(Organization);

            _DbContext.SaveChanges();
            TempData["success"] = "تم تفعيل حالة المنظمة إلى غير نشط بنجاح";
            return RedirectToAction("Index");
        }



        #region
        public IActionResult GetByActivationStatus(string? status, int? type)
        {
            /* var list = _DbContext.Organizations.ToList();*/

            IEnumerable<Organization> list;



            if (status == null && type == 0)
            {
                list = _DbContext.Organizations.ToList().OrderByDescending((item => item.SubscriptionDate));
                return Json(new { list });
            }


            if (status != null && type == 0)
            {
                list = _DbContext.Organizations.Where(item => item.ActivationStatus.Equals(status)).OrderByDescending((item => item.SubscriptionDate));

                return Json(new { list });
            }


            if (type == 1 && status == null)
            {
                list = _DbContext.Organizations.Where(item => item.Organization_TypeId == 1).OrderByDescending((item => item.SubscriptionDate));
                return Json(new { list });
            }

            if (type == 2 && status == null)
            {
                list = _DbContext.Organizations.Where(item => item.Organization_TypeId == 2).OrderByDescending((item => item.SubscriptionDate));
                return Json(new { list });
            }

            if (type == 1 && status != null)
            {

                list = _DbContext.Organizations.Where(item => item.ActivationStatus.Equals(status) && item.Organization_TypeId == 1).OrderByDescending((item => item.SubscriptionDate));
                return Json(new { list });
            }

            if (type == 2 && status != null)
            {

                list = _DbContext.Organizations.Where(item => item.ActivationStatus.Equals(status) && item.Organization_TypeId == 2).OrderByDescending((item => item.SubscriptionDate));
                return Json(new { list });
            }






            return Json(new { data = "No_data" });
        }


        #endregion




    }
}