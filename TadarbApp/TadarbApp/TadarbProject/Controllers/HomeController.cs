﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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


            //IEnumerable<UserAcount> userAcount = _DbContext.UserAcounts.AsNoTracking().ToList();




            //return View(userAcount);

            return View();
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
            var UserInDb = _DbContext.UserAcounts.Where(item => item.UserEmail.Equals(user.UserEmail) && item.UserPassword.Equals(user.UserPassword)).AsNoTracking().FirstOrDefault();

            ViewBag.LogInMassage = "";

            if (UserInDb == null)
            {

                ViewBag.LogInMassage = "معلومات التسجيل غير صحيحة.";

                return View();
            }

            if (UserInDb.UserType.Equals("System_Admin"))

            {
                _HttpContextAccessor.HttpContext.Session.SetString("Name", UserInDb.UserEmail);

                _HttpContextAccessor.HttpContext.Session.SetInt32("UserId", UserInDb.UserId);
                return RedirectToAction("Index", "Admin");
            }


            if (UserInDb.UserType.Equals("Company_Admin"))

            {
                if (UserInDb.ActivationStatus.Equals("Not_Active"))
                {
                    ViewBag.LogInMassage = "الرجاء تأكيد الحساب من خلال البريد الالكتروني.";
                    return View();
                }

                var Organization = _DbContext.Organizations.Where(item => item.ResponsibleUserId == UserInDb.UserId).AsNoTracking().FirstOrDefault();

                if (Organization.ActivationStatus.Equals("Not_Active"))
                {
                    ViewBag.LogInMassage = "الحساب مسجل ولكن لم يتم اعتمادكم من قبل المنصه بعد.";
                    return View();
                }

                _HttpContextAccessor.HttpContext.Session.SetString("Name", UserInDb.UserEmail);

                _HttpContextAccessor.HttpContext.Session.SetInt32("UserId", UserInDb.UserId);

                return RedirectToAction("Index", "Company");
            }

            if (UserInDb.UserType.Equals("Branch_Admin"))

            {
                var Branch = _DbContext.OrganizationBranches_TrainProv.Where(item => item.Responsible_UserId == UserInDb.UserId).AsNoTracking().FirstOrDefault();

                if (Branch == null)
                {
                    ViewBag.LogInMassage = "حساب مسؤول الفرع مسجل ولكن لم يتم تعيينه على فرع بعد.";
                    return View();
                }

                _HttpContextAccessor.HttpContext.Session.SetString("Name", UserInDb.UserEmail);

                _HttpContextAccessor.HttpContext.Session.SetInt32("UserId", UserInDb.UserId);

                return RedirectToAction("Index", "Branch");
            }


            if (UserInDb.UserType.Equals("University_Admin"))
            {

                if (UserInDb.ActivationStatus.Equals("Not_Active"))
                {
                    ViewBag.LogInMassage = "الرجاء تأكيد الحساب من خلال البريد الالكتروني.";
                    return View();
                }

                var Organization = _DbContext.Organizations.Where(item => item.ResponsibleUserId == UserInDb.UserId).AsNoTracking().FirstOrDefault();

                if (Organization.ActivationStatus.Equals("Not_Active"))
                {
                    ViewBag.LogInMassage = "الحساب مسجل ولكن لم يتم اعتمادكم من قبل المنصه بعد.";
                    return View();
                }

                _HttpContextAccessor.HttpContext.Session.SetString("Name", UserInDb.UserEmail);

                _HttpContextAccessor.HttpContext.Session.SetInt32("UserId", UserInDb.UserId);

                return RedirectToAction("Index", "University");

            }


            if (UserInDb.UserType.Equals("College_Admin"))
            {

                var College = _DbContext.UniversityColleges.Where(item => item.Responsible_UserId == UserInDb.UserId).AsNoTracking().FirstOrDefault();

                if (College == null)
                {
                    ViewBag.LogInMassage = "حساب مسؤول الكلية مسجل ولكن لم يتم تعيينه على كلية بعد.";
                    return View();
                }

                _HttpContextAccessor.HttpContext.Session.SetString("Name", UserInDb.UserEmail);

                _HttpContextAccessor.HttpContext.Session.SetInt32("UserId", UserInDb.UserId);

                return RedirectToAction("Index", "College");
            }

            if (UserInDb.UserType.Equals("DepUni_Admin"))
            {

                var department = _DbContext.Departments.Where(item => item.Responsible_UserId == UserInDb.UserId).AsNoTracking().FirstOrDefault();

                if (department == null)
                {
                    ViewBag.LogInMassage = "حساب مسؤول القسم مسجل ولكن لم يتم تعيينه على قسم بعد.";
                    return View();
                }

                _HttpContextAccessor.HttpContext.Session.SetString("Name", UserInDb.UserEmail);

                _HttpContextAccessor.HttpContext.Session.SetInt32("UserId", UserInDb.UserId);

                return RedirectToAction("Index", "DepartmentUniversity");
            }

            if (UserInDb.UserType.Equals("Training_Supervisor"))
            {
                _HttpContextAccessor.HttpContext.Session.SetString("Name", UserInDb.UserEmail);

                _HttpContextAccessor.HttpContext.Session.SetInt32("UserId", UserInDb.UserId);

                return RedirectToAction("Index", "TrainingSupervisor");
            }


            if (UserInDb.UserType.Equals("Academic_supervisor"))
            {
                _HttpContextAccessor.HttpContext.Session.SetString("Name", UserInDb.UserEmail);

                _HttpContextAccessor.HttpContext.Session.SetInt32("UserId", UserInDb.UserId);

                return RedirectToAction("Index", "AcadmicSupervisor");
            }

            if (UserInDb.UserType.Equals("Student"))
            {
                _HttpContextAccessor.HttpContext.Session.SetString("Name", UserInDb.UserEmail);

                _HttpContextAccessor.HttpContext.Session.SetInt32("UserId", UserInDb.UserId);

                return RedirectToAction("Index", "Student");
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

                CountryListItems = _DbContext.Countries.AsNoTracking().ToList().Select(u => new SelectListItem { Text = u.CountryName, Value = u.CountryId.ToString() }),

                // CityListItems = _DbContext.Cities.ToList().Select(u => new SelectListItem { Text = u.CityName, Value = u.CityId.ToString() }),

                FieldListItems = _DbContext.FieldOfSpecialtiesMaster.AsNoTracking().ToList().Select(u => new SelectListItem { Text = u.FieldName, Value = u.FieldId.ToString() })
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
                var EmailBody = _emailSender.PopulateMessageBody(wwwRootPath, "verificationtemplate.html", organizationVM.organization.OrganizationName, $"{Url.ActionLink("UdateUserAcountActivationStatus", "Home", new { id = user.UserId })}");

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

            var item = _DbContext.UserAcounts.Where(item => item.UserEmail.Equals(Email)).AsNoTracking().FirstOrDefault();

            if (item == null)
            {
                return Json(new { Exists = false });
            }



            return Json(new { Exists = true });
        }


        [HttpGet]
        public IActionResult PhoneExists(string? Phone)
        {
            if (Phone == null)
            {
                return Json(new { Exists = false });
            }

            var item = _DbContext.UserAcounts.Where(item => item.Phone.Equals(Phone)).AsNoTracking().FirstOrDefault();

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


                ).AsNoTracking().ToList();



                return Json(new { Cities });

            }


            return Json(new { Exists = false });
        }
    }

    #endregion




}