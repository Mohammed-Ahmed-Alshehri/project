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
    public class UniversityController : Controller
    {
        private readonly AppDbContext _DbContext;
        private readonly IEmailSender _emailSender;
        private readonly IHttpContextAccessor _HttpContextAccessor;


        public UniversityController(AppDbContext DbContext, IEmailSender emailSender, IHttpContextAccessor HttpContextAccessor)
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
            var OrganizationOfR = _DbContext.Organizations.Where(item => item.ResponsibleUserId == RUserId).AsNoTracking().FirstOrDefault();


            ViewBag.OrganizationName = OrganizationOfR.OrganizationName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;
            return View();
        }

        public IActionResult ViewColleges()
        {
            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");

            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();
            var OrganizationOfR = _DbContext.Organizations.Where(item => item.ResponsibleUserId == RUserId).AsNoTracking().FirstOrDefault();


            var OrganizationCollege = _DbContext.UniversityColleges.Where(item => item.Organization_OrganizationId == OrganizationOfR.OrganizationId).AsNoTracking().FirstOrDefault();





            var College = _DbContext.UniversityColleges.Where(item => item.Organization_OrganizationId == OrganizationOfR.OrganizationId)
                .AsNoTracking().Include(item => item.city).Include(item => item.User).AsNoTracking().ToList();


            ViewBag.OrganizationName = OrganizationOfR.OrganizationName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;
            return View(College);

        }
        [HttpGet]
        public IActionResult AddColleges()
        {
            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");

            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();
            var OrganizationOfR = _DbContext.Organizations.Where(item => item.ResponsibleUserId == RUserId).AsNoTracking().FirstOrDefault();

            var DEPOfR = _DbContext.Departments.Where(item => item.Organization_OrganizationId == OrganizationOfR.OrganizationId && item.DepartmentName.Equals("قسم ادارة الكليات")).AsNoTracking().FirstOrDefault();


            ViewBag.OrganizationName = OrganizationOfR.OrganizationName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;

            IEnumerable<UserAcount> OrgEMP = Enumerable.Empty<UserAcount>(); ;

            var OrgCountryId = _DbContext.Cities.Where(item => item.CityId == OrganizationOfR.MainBranchCityId).AsNoTracking().FirstOrDefault().Country_CountryId;

            if (DEPOfR != null)
            {

                OrgEMP = _DbContext.UserAcounts.FromSqlRaw($"Select * from UserAcounts WHERE UserAcounts.UserId IN (Select Employees.UserAccount_UserId from Employees where Employees.Department_DepartmentId ={DEPOfR.DepartmentId})" +
                "AND UserAcounts.UserId NOT IN (select Responsible_UserId from dbo.UniversityColleges);").AsNoTracking().ToList();

            }

            CollegeVM CollegeVM = new()
            {
                College = new(),

                FieldListItems = _DbContext.FieldOfSpecialtiesMaster.ToList().Select(u => new SelectListItem { Text = u.FieldName, Value = u.FieldId.ToString() }),

                CityListItems = _DbContext.Cities.Where(item => item.Country_CountryId == OrgCountryId).AsNoTracking().ToList().Select(u => new SelectListItem { Text = u.CityName, Value = u.CityId.ToString() }),
                UserListItems = OrgEMP.ToList().Select(u => new SelectListItem { Text = u.FullName, Value = u.UserId.ToString() }),
                organization = OrganizationOfR


            };

            return View(CollegeVM);




        }
        [HttpPost]
        public IActionResult AddColleges(CollegeVM CollegeVM)
        {
            if (ModelState.IsValid)
            {
                int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;

                var OrganizationOfR = _DbContext.Organizations.Where(item => item.ResponsibleUserId == RUserId).AsNoTracking().FirstOrDefault();

                var Collegee = new UniversityCollege
                {
                    CollegeName = CollegeVM.College.CollegeName,
                    City_CityId = CollegeVM.College.City_CityId,
                    Responsible_UserId = CollegeVM.College.Responsible_UserId,
                    Organization_OrganizationId = OrganizationOfR.OrganizationId,
                    FieldOfOrganization_SpecialtiesField = CollegeVM.College.FieldOfOrganization_SpecialtiesField,
                    Zoon = CollegeVM.College.Zoon,
                    Location = CollegeVM.College.Location,




                };

                _DbContext.UniversityColleges.Add(Collegee);




                var RCollegeManger = _DbContext.UserAcounts.AsNoTracking().FirstOrDefault(item => item.UserId == CollegeVM.College.Responsible_UserId);


                RCollegeManger.City_CityId = CollegeVM.College.City_CityId;



                _DbContext.UserAcounts.Update(RCollegeManger);


                _DbContext.SaveChanges();
                TempData["success"] = "تم إضافة الكلية بنجاح";


                return RedirectToAction("ViewColleges");

            }

            return View(CollegeVM);




        }


        [HttpGet]
        public IActionResult EditColleges(int? id)
        {
            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");
            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();
            var OrganizationOfR = _DbContext.Organizations.Where(item => item.ResponsibleUserId == RUserId).AsNoTracking().FirstOrDefault();
            //var CountryOfr = _DbContext.Countries.Where(item => item.CountryId==)

            ViewBag.OrganizationName = OrganizationOfR.OrganizationName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;
            CollegeVM collegeVM = new()
            {

                College = new UniversityCollege()

            };


            if (id != null || id != 0)
            {

                collegeVM.College = _DbContext.UniversityColleges.Where(u => u.CollegeId == id).AsNoTracking().FirstOrDefault();

                var city = _DbContext.Cities.AsNoTracking().Where(Ci => Ci.CityId == collegeVM.College.City_CityId);
                var FiledMster = _DbContext.FieldOfSpecialtiesMaster.AsNoTracking().Where(Fi => Fi.FieldId == collegeVM.College.FieldOfOrganization_SpecialtiesField);

                collegeVM.CityListItems = city.Select(u => new SelectListItem { Text = u.CityName, Value = u.CityId.ToString() });

                collegeVM.CountryListItems = _DbContext.Countries.AsNoTracking()
                    .Where(u => u.CountryId == city.First().Country_CountryId).Select(u => new SelectListItem { Text = u.CountryName, Value = u.CountryId.ToString() });
                collegeVM.FieldListItems = _DbContext.FieldOfSpecialtiesMaster.AsNoTracking()
                    .Where(u => u.FieldId == collegeVM.College.FieldOfOrganization_SpecialtiesField).Select(u => new SelectListItem { Text = u.FieldName, Value = u.FieldId.ToString() });


                var DEPOfR = _DbContext.Departments.Where(item => item.Organization_OrganizationId == OrganizationOfR.OrganizationId && item.DepartmentName.Equals("قسم ادارة الكليات")).AsNoTracking().FirstOrDefault();


                collegeVM.UserListItems = _DbContext.UserAcounts.FromSqlRaw($"Select * from UserAcounts WHERE UserAcounts.UserId IN (Select Employees.UserAccount_UserId FROM Employees WHERE Employees.Department_DepartmentId ={DEPOfR.DepartmentId})" +
                    $"AND UserAcounts.UserId NOT IN (Select Responsible_UserId FROM UniversityColleges WHERE Responsible_UserId!={collegeVM.College.Responsible_UserId})")
                    .AsNoTracking().Select(u => new SelectListItem { Text = u.FullName, Value = u.UserId.ToString() });


                return View(collegeVM);
            }

            return NotFound();
        }



        [HttpPost]
        public IActionResult EditColleges(CollegeVM collegeVM)
        {

            if (!ModelState.IsValid)
            {
                return View(collegeVM);
            }

            _DbContext.UniversityColleges.Update(collegeVM.College);

            var User = _DbContext.UserAcounts.AsNoTracking().FirstOrDefault(item => item.UserId == collegeVM.College.Responsible_UserId);
            User.City_CityId = collegeVM.College.City_CityId;
            _DbContext.UserAcounts.Update(User);


            _DbContext.SaveChanges();

            TempData["success"] = "تم تعديل معلومات الكلية بنجاح";

            return RedirectToAction("ViewColleges");
        }

        [HttpGet]
        public IActionResult ViewSpecialities()
        {
            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");
            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();
            var OrganizationOfR = _DbContext.Organizations.Where(item => item.ResponsibleUserId == RUserId).AsNoTracking().FirstOrDefault();
            ViewBag.OrganizationName = OrganizationOfR.OrganizationName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;


            return View();
        }


        [HttpGet]
        public IActionResult AddSpecialities()
        {
            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");
            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();
            var OrganizationOfR = _DbContext.Organizations.Where(item => item.ResponsibleUserId == RUserId).AsNoTracking().FirstOrDefault();


            ViewBag.OrganizationName = OrganizationOfR.OrganizationName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;

            SpecialitiesVM specialitiesVM = new()
            {

                MasterFieldsListItems = _DbContext.FieldOfSpecialtiesMaster.AsNoTracking().ToList().Select(u => new SelectListItem { Text = u.FieldName, Value = u.FieldId.ToString() }),


            };

            return View(specialitiesVM);
        }


        public IActionResult GetDetailFields(string? id)
        {

            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;

            var RUser = _DbContext.UserAcounts.Find(RUserId);

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.ResponsibleUserId == RUserId).AsNoTracking().FirstOrDefault();





            if (!string.IsNullOrEmpty(id))
            {


                //var HasFileds = _DbContext.OrganizationsProvidTrainingInArea.Where(item => item.Organization_OrganizationId == OrganizationOfR.OrganizationId).FirstOrDefault();

                var Id = Convert.ToInt64(id);

                var Detailfields = _DbContext.FieldOfSpecialtiesDetails.FromSqlRaw($"Select * from FieldOfSpecialtiesDetails WHERE Field_FieldId= {Id} AND DetailFieldId NOT IN" +
                    $"(Select DetailField_DetailFieldId from OrganizationsProvidTrainingInArea WHERE Organization_OrganizationId={OrganizationOfR.OrganizationId})")
                    .AsNoTracking().Select(item => new

                    {
                        DetailFieldId = item.DetailFieldId,

                        SpecializationName = item.SpecializationName

                    }


                ).ToList();


                return Json(new { Detailfields });


            }


            return Json(new { Exists = false });
        }



        public IActionResult AddDetailFields(string dFieldIds)
        {

            if (dFieldIds != "[]")
            {
                var charsToRemove = new string[] { "[", "]" };

                foreach (var c in charsToRemove)
                {
                    dFieldIds = dFieldIds.Replace(c, string.Empty);
                }





                string[] Ids = dFieldIds.Split(",");

                // Console.WriteLine(Ids[0]);



                int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;

                var RUser = _DbContext.UserAcounts.Find(RUserId);

                var OrganizationOfR = _DbContext.Organizations.Where(item => item.ResponsibleUserId == RUserId).AsNoTracking().FirstOrDefault();

                foreach (var i in Ids)
                {
                    int id = Convert.ToInt32(i);

                    var OPTA = new OrganizationProvidTrainingInArea
                    {
                        Organization_OrganizationId = OrganizationOfR.OrganizationId,
                        DetailField_DetailFieldId = id


                    };


                    _DbContext.OrganizationsProvidTrainingInArea.Add(OPTA);
                }

                _DbContext.SaveChanges();

            }




            return RedirectToAction("ViewSpecialities");

        }


        public IActionResult DeleteFieldOfSpecialty(int? id)
        {




            if (id != null || id == 0)
            {

                int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;

                var RUser = _DbContext.UserAcounts.Find(RUserId);

                var OrganizationOfR = _DbContext.Organizations.Where(item => item.ResponsibleUserId == RUserId).AsNoTracking().FirstOrDefault();


                var OrgFOS = _DbContext.OrganizationsProvidTrainingInArea.Where(u => u.DetailField_DetailFieldId == id).AsNoTracking().FirstOrDefault();

                _DbContext.OrganizationsProvidTrainingInArea.Remove(OrgFOS);


                _DbContext.SaveChanges();


                return Json(new { success = true, message = "تم الحذف بنجاح" });
            }



            return Json(new { success = false, message = "حصل خطاء لم يتم الحذف" });

        }


        public IActionResult GetSpecialities()
        {
            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.ResponsibleUserId == RUserId).AsNoTracking().FirstOrDefault();



            IEnumerable<FieldOfSpecialtyDetails> Specialities = Enumerable.Empty<FieldOfSpecialtyDetails>();
            var HasFileds = _DbContext.OrganizationsProvidTrainingInArea.Where(item => item.Organization_OrganizationId == OrganizationOfR.OrganizationId).AsNoTracking().FirstOrDefault();

            if (HasFileds != null)
            {

                Specialities = _DbContext.FieldOfSpecialtiesDetails.FromSqlRaw("Select * from FieldOfSpecialtiesDetails WHERE DetailFieldId IN" +
                    $"(Select DetailField_DetailFieldId From OrganizationsProvidTrainingInArea WHERE Organization_OrganizationId={OrganizationOfR.OrganizationId})").AsNoTracking().Include(item => item.FieldOfSpecialty).ToList();
            }

            return Json(new { Specialities });

        }


        public IActionResult ViewUsers()

        {
            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");
            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();
            var OrganizationOfR = _DbContext.Organizations.Where(item => item.ResponsibleUserId == RUserId).AsNoTracking().FirstOrDefault();

            ViewBag.OrganizationName = OrganizationOfR.OrganizationName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;

            var DEPOfR = _DbContext.Departments.Where(item => item.Organization_OrganizationId == OrganizationOfR.OrganizationId && item.DepartmentName.Equals("قسم ادارة الكليات")).AsNoTracking().FirstOrDefault();

            IEnumerable<UserAcount> OrgEMP = Enumerable.Empty<UserAcount>(); ;

            if (DEPOfR != null)
            {
                OrgEMP = _DbContext.UserAcounts.FromSqlRaw($"select * from UserAcounts WHERE UserAcounts.UserId in " +
                    $"(select Employees.UserAccount_UserId from Employees where Employees.Department_DepartmentId = {DEPOfR.DepartmentId});").AsNoTracking().ToList();

            }



            return View(OrgEMP);
        }



        [HttpGet]
        public IActionResult AddUsers()
        {
            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");
            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();
            var OrganizationOfR = _DbContext.Organizations.Where(item => item.ResponsibleUserId == RUserId).AsNoTracking().FirstOrDefault();


            ViewBag.OrganizationName = OrganizationOfR.OrganizationName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;
            return View();
        }






        [HttpPost]

        public IActionResult AddUsers(EmployeeVM employeeVM)
        {


            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;

            var RUser = _DbContext.UserAcounts.Find(RUserId);

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.ResponsibleUserId == RUserId).AsNoTracking().FirstOrDefault();

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
                City_CityId = OrganizationOfR.MainBranchCityId,
                UserType = "College_Admin",
                ActivationStatus = "Active"




            };

            _DbContext.UserAcounts.Add(user);

            _DbContext.SaveChanges();



            var DEPOfR = _DbContext.Departments.Where(item => item.Organization_OrganizationId == OrganizationOfR.OrganizationId && item.DepartmentName.Equals("قسم ادارة الكليات")).AsNoTracking().FirstOrDefault();


            if (DEPOfR == null)
            {
                var DEP = new Department
                {

                    DepartmentName = "قسم ادارة الكليات",
                    Organization_OrganizationId = OrganizationOfR.OrganizationId,
                    Responsible_UserId = RUser.UserId,



                };

                _DbContext.Departments.Add(DEP);

                _DbContext.SaveChanges();
            }



            int DEPId = _DbContext.Departments.AsNoTracking().Where(item => item.Organization_OrganizationId == OrganizationOfR.OrganizationId && item.DepartmentName.Equals("قسم ادارة الكليات")).First().DepartmentId;

            var EMP = new Employee
            {

                Department_DepartmentId = DEPId,
                Job_JobId = 2,
                SSN = employeeVM.employee.SSN,
                UserAccount_UserId = user.UserId,




            };

            _DbContext.Employees.Add(EMP);

            _DbContext.SaveChanges();
            TempData["success"] = "تم إضافة حساب المسؤول  بنجاح";
            return RedirectToAction("ViewUsers");

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






}
