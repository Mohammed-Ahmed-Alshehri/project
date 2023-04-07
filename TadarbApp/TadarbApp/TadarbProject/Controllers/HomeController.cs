using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Protocol;
using TadarbProject.Data;
using TadarbProject.Models;
using TadarbProject.Models.ViewModels;
using TEST2.Services;

namespace TadarbProject.Controllers
{
    public class HomeController : Controller
    {

        private readonly AppDbContext _DbContext;
        private readonly IWebHostEnvironment _WebHostEnvironment;
        private readonly IEmailSender _emailSender;
        private readonly IHttpContextAccessor _HttpContextAccessor;
        public HomeController(AppDbContext DbContext, IWebHostEnvironment webHostEnvironment, IEmailSender emailSender, IHttpContextAccessor httpContext)
        {
            _DbContext = DbContext;
            _WebHostEnvironment = webHostEnvironment;
            _emailSender = emailSender;

            _HttpContextAccessor = httpContext;

        }

        public IActionResult Index()


        {


            IEnumerable<UserAcount> userAcount = _DbContext.UserAcounts.ToList();




            return View(userAcount);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }



        [HttpPost]
        public IActionResult Login(UserAcount user)
        {

            if (user == null)
            {
                return View();

            }

            var UserInDb = _DbContext.UserAcounts.Where(item => item.UserType.Equals("System_Admin") && item.UserEmail.Equals(user.UserEmail) && item.UserPassword.Equals(user.UserPassword)).FirstOrDefault();

            if (UserInDb != null)
            {
                _HttpContextAccessor.HttpContext.Session.SetString("Name", UserInDb.UserEmail);

                _HttpContextAccessor.HttpContext.Session.SetInt32("UserId", UserInDb.UserId);
                return RedirectToAction("Index", "Admin");

            }


            var UserInDb2 = _DbContext.UserAcounts.Where(item => item.UserType.Equals("Company_Admin") && item.UserEmail.Equals(user.UserEmail) && item.UserPassword.Equals(user.UserPassword)).FirstOrDefault();


            if (UserInDb2 != null)
            {
                _HttpContextAccessor.HttpContext.Session.SetString("Name", UserInDb2.UserEmail);

                _HttpContextAccessor.HttpContext.Session.SetInt32("UserId", UserInDb2.UserId);

                return RedirectToAction("Index", "Company");

            }

            return View();










        }

        [HttpGet]
        public IActionResult Registration()
        {
            OrganizationVM organizationVM = new()
            {
                organization = new(),
                userAcount = new(),

                OrganizationTypeListItems = _DbContext.OrganizationTypes.ToList().Select(u => new SelectListItem { Text = u.TypeName, Value = u.TypeId.ToString() }),

                CountryListItems = _DbContext.Countries.ToList().Select(u => new SelectListItem { Text = u.CountryName, Value = u.CountryId.ToString() }),

                // CityListItems = _DbContext.Cities.ToList().Select(u => new SelectListItem { Text = u.CityName, Value = u.CityId.ToString() }),

                FieldListItems = _DbContext.FieldOfSpecialtiesMaster.ToList().Select(u => new SelectListItem { Text = u.FieldName, Value = u.FieldId.ToString() })
            };

            ViewData["EmailSend"] = false;
            return View(organizationVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Registration(OrganizationVM organizationVM, IFormFile? imgFile)
        {
            var userType = "";



            ViewData["EmailSend"] = false;
            if (ModelState.IsValid)
            {

                if (organizationVM.organization.Organization_TypeId == 1)
                {
                    userType = "University_Admin";
                }
                else
                {
                    userType = "Company_Admin";
                }


                //--------------------------------------UserAcount part ---------------------------------------
                var user = new UserAcount
                {
                    UserEmail = organizationVM.userAcount.UserEmail,
                    UserPassword = organizationVM.userAcount.UserPassword,
                    FullName = organizationVM.userAcount.FullName,
                    Phone = organizationVM.userAcount.Phone,
                    City_CityId = organizationVM.organization.MainBranchCityId,
                    UserType = userType
                };


                _DbContext.Add(user);

                _DbContext.SaveChanges();


                //--------------------------------------organization part ---------------------------------------

                //find the user how created the organization
                int RUserId = _DbContext.UserAcounts.Where(item => item.UserEmail == user.UserEmail).First().UserId;


                //--------------------------------------organization LogoPath to ceate a file in the project and save the Path to the data base ---------------------------------------
                string fileName = Guid.NewGuid().ToString();

                string wwwRootPath = _WebHostEnvironment.WebRootPath;

                var uploadTo = Path.Combine(wwwRootPath, @"images\Organizations\Logos");

                var extension = Path.GetExtension(imgFile.FileName);

                using (var fileStreams = new FileStream(Path.Combine(uploadTo, fileName + extension), FileMode.Create))
                {
                    imgFile.CopyTo(fileStreams);
                }

                //Here what will be in the database.
                var DbLogoPath = @"images\Organizations\Logos\" + fileName + extension;
                //-----------------------------------------------------------------------------

                var organization = new Organization
                {
                    OrganizationName = organizationVM.organization.OrganizationName,

                    Organization_TypeId = organizationVM.organization.Organization_TypeId,

                    CommercialRegistrationNumber = organizationVM.organization.CommercialRegistrationNumber,

                    ResponsibleUserId = RUserId,

                    FieldOfOrganization_SpecialtiesFieldId = organizationVM.organization.FieldOfOrganization_SpecialtiesFieldId,

                    LogoPath = DbLogoPath,

                    MainBranchCityId = organizationVM.organization.MainBranchCityId,

                    OrganizationURL = organizationVM.organization.OrganizationURL,

                    Location = organizationVM.organization.Location


                };

                _DbContext.Add(organization);
                _DbContext.SaveChanges();

                //-----------------------------------------------------------------------------

                //لكي تعبي جسم رسالة الايميل بالتمبليت الذي تريدة + تحديد المرسل له والموضوع و اخيرا رابط التاكيد 
                var EmailBody = _emailSender.PopulateMessageBody(wwwRootPath, "verificationtemplate.html", organizationVM.organization.OrganizationName, $"https://localhost:7122/home/UdateUserAcountActivationStatus/{user.UserId}");

                var result = _emailSender.SendEmail(organizationVM.userAcount.UserEmail, "رسالة بشأن التحقق من عنوان البريد الإلكتروني - منصة تدرب", EmailBody);

                if (result.Equals("email hass ben send"))
                {
                    ViewData["EmailSend"] = true;
                }

                return View(organizationVM);
            }


            return View(organizationVM);





        }


        public IActionResult UdateUserAcountActivationStatus(int? id)
        {
            if (id == null)
            {
                return NotFound();


            }
            var User = _DbContext.UserAcounts.Find(id);

            if (User == null) { return NotFound(); }

            User.ActivationStatus = "Active";

            _DbContext.UserAcounts.Update(User);

            _DbContext.SaveChanges();

            return RedirectToAction("Login");
        }




        #region
        public IActionResult GetAllUsers()
        {

            var UserAcountList = _DbContext.UserAcounts.ToList();

            var CityList = _DbContext.Cities.ToList();

            return Json(new { data = CityList });
        }

        /*public IActionResult SnedEmailService()
        {
            //to get wwwRootPath to to read Email Template.html
            var wwwRootPath = _WebHostEnvironment.WebRootPath;

            //لكي تعبي جسم رسالة الايميل بالتمبليت الذي تريدة + تحديد المرسل له والموضوع و اخيرا رابط التاكيد 
            var EmailBody = _emailSender.PopulateMessageBody(wwwRootPath, "verificationtemplate.html", "mohammed", "www.google.com");

            var result = _emailSender.SendEmail("mjrm5071@gmail.com", "اختبار", EmailBody);


            return Json(new { data = result });

        }*/

        [HttpGet]
        public IActionResult EmailExists(string? Email)
        {
            if (Email == null)
            {
                return Json(new { Exists = false });
            }

            var item = _DbContext.UserAcounts.Where(item => item.UserEmail.Equals(Email)).FirstOrDefault();

            if (item == null)
            {
                return Json(new { Exists = false });
            }



            return Json(new { Exists = true });
        }

        public IActionResult GetCities(string? id)
        {

            if (!string.IsNullOrEmpty(id))
            {
                var Id = Convert.ToInt64(id);
                var Cities = _DbContext.Cities.Where(item => item.Country_CountryId == Id).Select(item => new

                {
                    CityId = item.CityId,

                    CityName = item.CityName

                }


                ).ToList();



                return Json(new { Cities });

            }


            return Json(new { Exists = false });
        }
    }

    #endregion




}