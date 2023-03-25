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

        public IActionResult Index(int? TypeId)
        {

            /* var OrganizationList = _DbContext.Organizations.ToList();

             if (TypeId == null)
             {
                 return View(OrganizationList);
             }

             if (TypeId == 1)
             {

                 var OrganizationListU = OrganizationList.Where(item => item.Organization_TypeId == 1).ToList();

                 return View(OrganizationListU);
             }

             if (TypeId == 2)
             {
                 var OrganizationListC = OrganizationList.Where(item => item.Organization_TypeId == 2).ToList();

                 return View(OrganizationListC);

             }
 */
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

            OrganizationVM organizationVM = new OrganizationVM { organization = Organization, userAcount = Ruser };


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

        public IActionResult RejectAccept(int? id)
        {
            if (id == null)
            {
                return NotFound();


            }
            var Organization = _DbContext.Organizations.Find(id);

            if (Organization == null) { return NotFound(); }

            Organization.ActivationStatus = "Not_Active";

            _DbContext.Organizations.Update(Organization);

            _DbContext.SaveChanges();
            TempData["success"] = "تم تفعيل حالة المنظمة إلى غير نشط بنجاح";
            return RedirectToAction("Index");
        }



        #region
        public IActionResult GetByActivationStatus(string? status)
        {
            /* var list = _DbContext.Organizations.ToList();*/

            IEnumerable<Organization> list;

            if (status == null)
            {
                list = _DbContext.Organizations.ToList();

                return Json(new { list });
            }


            list = _DbContext.Organizations.Where(item => item.ActivationStatus.Equals(status)).OrderByDescending((item => item.SubscriptionDate));



            return Json(new { list });
        }


        #endregion




    }
}