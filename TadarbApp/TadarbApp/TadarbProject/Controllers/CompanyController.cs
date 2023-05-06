using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TadarbProject.Data;
using TadarbProject.Models;
using TadarbProject.Models.ViewModels;
using TEST2.Services;

namespace TadarbProject.Controllers
{

    public class CompanyController : Controller
    {
        private readonly AppDbContext _DbContext;
        private readonly IEmailSender _emailSender;
        private readonly IHttpContextAccessor _HttpContextAccessor;


        public CompanyController(AppDbContext DbContext, IEmailSender emailSender, IHttpContextAccessor HttpContextAccessor)
        {
            _DbContext = DbContext;

            _emailSender = emailSender;
            _HttpContextAccessor = HttpContextAccessor;

        }

        [HttpGet]
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

            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + OrganizationOfR.OrganizationName;

            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;

            SpecialitiesVM specialitiesVM = new()
            {

                MasterFieldsListItems = _DbContext.FieldOfSpecialtiesMaster.AsNoTracking().ToList().Select(u => new SelectListItem { Text = u.FieldName, Value = u.FieldId.ToString() }),


            };

            return View(specialitiesVM);
        }



        [HttpGet]
        public IActionResult ViewBranches()
        {
            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");
            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();
            var OrganizationOfR = _DbContext.Organizations.Where(item => item.ResponsibleUserId == RUserId).AsNoTracking().FirstOrDefault();

            var OrganizationBranches = _DbContext.OrganizationBranches_TrainProv.Where(item => item.Organization_OrganizationId == OrganizationOfR.OrganizationId).AsNoTracking().FirstOrDefault();

            var Branches = _DbContext.OrganizationBranches_TrainProv.Where(item => item.Organization_OrganizationId == OrganizationOfR.OrganizationId)
                .Include(item => item.city).Include(item => item.user).AsNoTracking().ToList();

            ViewBag.OrganizationName = OrganizationOfR.OrganizationName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;

            return View(Branches);
        }

        [HttpGet]
        public IActionResult Addbranches()
        {
            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");
            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();
            var OrganizationOfR = _DbContext.Organizations.Where(item => item.ResponsibleUserId == RUserId).AsNoTracking().FirstOrDefault();

            var DEPOfR = _DbContext.Departments.Where(item => item.Organization_OrganizationId == OrganizationOfR.OrganizationId && item.DepartmentName.Equals("قسم ادارة الفروع")).AsNoTracking().FirstOrDefault();

            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + OrganizationOfR.OrganizationName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;
            IEnumerable<UserAcount> OrgEMP = Enumerable.Empty<UserAcount>(); ;


            if (DEPOfR != null)
            {

                OrgEMP = _DbContext.UserAcounts.FromSqlRaw($"Select * from UserAcounts WHERE UserAcounts.UserId IN (Select Employees.UserAccount_UserId from Employees where Employees.Department_DepartmentId ={DEPOfR.DepartmentId})" +
                "AND UserAcounts.UserId NOT IN (select Responsible_UserId from dbo.OrganizationBranches_TrainProv);").AsNoTracking().ToList();

            }

            BranchVM branchVM = new()
            {
                Branch = new(),
                CountryListItems = _DbContext.Countries.AsNoTracking().ToList().Select(u => new SelectListItem { Text = u.CountryName, Value = u.CountryId.ToString() }),

                CityListItems = _DbContext.Cities.AsNoTracking().ToList().Select(u => new SelectListItem { Text = u.CityName, Value = u.CityId.ToString() }),
                UserListItems = OrgEMP.ToList().Select(u => new SelectListItem { Text = u.FullName, Value = u.UserId.ToString() }),
                organization = OrganizationOfR


            };

            return View(branchVM);
        }



        [HttpPost]
        public IActionResult Addbranches(BranchVM branchVM)
        {

            if (ModelState.IsValid)
            {

                int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;

                var OrganizationOfR = _DbContext.Organizations.Where(item => item.ResponsibleUserId == RUserId).AsNoTracking().FirstOrDefault();

                var Branch = new OrganizationBranch_TrainProv
                {
                    BranchName = branchVM.Branch.BranchName,
                    City_CityId = branchVM.Branch.City_CityId,
                    Responsible_UserId = branchVM.Branch.Responsible_UserId,
                    Organization_OrganizationId = OrganizationOfR.OrganizationId,
                    Zoon = branchVM.Branch.Zoon,
                    Location = branchVM.Branch.Location,




                };

                _DbContext.OrganizationBranches_TrainProv.Add(Branch);


                var RBranchManger = _DbContext.UserAcounts.AsNoTracking().FirstOrDefault(item => item.UserId == branchVM.Branch.Responsible_UserId);


                RBranchManger.City_CityId = branchVM.Branch.City_CityId;



                _DbContext.UserAcounts.Update(RBranchManger);


                _DbContext.SaveChanges();

                TempData["success"] = "تم إضافة حساب الفرع  بنجاح";

                return RedirectToAction("ViewBranches");

            }

            return View(branchVM);
        }


        [HttpGet]
        public IActionResult EditBranche(int? id)
        {
            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");
            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();
            var OrganizationOfR = _DbContext.Organizations.Where(item => item.ResponsibleUserId == RUserId).AsNoTracking().FirstOrDefault();
            //var CountryOfr = _DbContext.Countries.Where(item => item.CountryId==)

            ViewBag.OrganizationName = OrganizationOfR.OrganizationName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;
            BranchVM branchVM = new()
            {

                Branch = new OrganizationBranch_TrainProv()

            };


            if (id != null || id != 0)
            {

                branchVM.Branch = _DbContext.OrganizationBranches_TrainProv.Where(u => u.BranchId == id).AsNoTracking().FirstOrDefault();

                var city = _DbContext.Cities.Where(Ci => Ci.CityId == branchVM.Branch.City_CityId).AsNoTracking();

                branchVM.CityListItems = city.Select(u => new SelectListItem { Text = u.CityName, Value = u.CityId.ToString() });

                branchVM.CountryListItems = _DbContext.Countries.Where(u => u.CountryId == city.First().Country_CountryId).Select(u => new SelectListItem { Text = u.CountryName, Value = u.CountryId.ToString() });

                var DEPOfR = _DbContext.Departments.Where(item => item.Organization_OrganizationId == OrganizationOfR.OrganizationId && item.DepartmentName.Equals("قسم ادارة الفروع")).AsNoTracking().FirstOrDefault();


                branchVM.UserListItems = _DbContext.UserAcounts.FromSqlRaw($"Select * from UserAcounts WHERE UserAcounts.UserId IN (Select Employees.UserAccount_UserId FROM Employees WHERE Employees.Department_DepartmentId ={DEPOfR.DepartmentId})" +
                    $"AND UserAcounts.UserId NOT IN (Select Responsible_UserId FROM OrganizationBranches_TrainProv WHERE Responsible_UserId!={branchVM.Branch.Responsible_UserId})")
                    .AsNoTracking().Select(u => new SelectListItem { Text = u.FullName, Value = u.UserId.ToString() });


                return View(branchVM);
            }


            return NotFound();
        }

        [HttpPost]
        public IActionResult EditBranche(BranchVM branchVM)
        {

            if (!ModelState.IsValid)
            {
                return View(branchVM);
            }



            _DbContext.OrganizationBranches_TrainProv.Update(branchVM.Branch);

            var User = _DbContext.UserAcounts.AsNoTracking().FirstOrDefault(item => item.UserId == branchVM.Branch.Responsible_UserId);
            User.City_CityId = branchVM.Branch.City_CityId;
            _DbContext.UserAcounts.Update(User);
            _DbContext.SaveChanges();

            TempData["success"] = "تم تعديل معلومات الفرع  بنجاح";

            return RedirectToAction("ViewBranches");
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

            var DEPOfR = _DbContext.Departments.Where(item => item.Organization_OrganizationId == OrganizationOfR.OrganizationId && item.DepartmentName.Equals("قسم ادارة الفروع")).AsNoTracking().FirstOrDefault();

            IEnumerable<UserAcount> OrgEMP = Enumerable.Empty<UserAcount>();

            if (DEPOfR != null)
            {
                OrgEMP = _DbContext.UserAcounts.FromSqlRaw($"select * from UserAcounts WHERE UserAcounts.UserId in" +
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
                UserType = "Branch_Admin",
                ActivationStatus = "Active"




            };

            _DbContext.UserAcounts.Add(user);

            _DbContext.SaveChanges();



            var DEPOfR = _DbContext.Departments.Where(item => item.Organization_OrganizationId == OrganizationOfR.OrganizationId && item.DepartmentName.Equals("قسم ادارة الفروع")).AsNoTracking().FirstOrDefault();


            if (DEPOfR == null)
            {
                var DEP = new Department
                {

                    DepartmentName = "قسم ادارة الفروع",
                    Organization_OrganizationId = OrganizationOfR.OrganizationId,
                    Responsible_UserId = RUser.UserId,



                };

                _DbContext.Departments.Add(DEP);

                _DbContext.SaveChanges();
            }



            int DEPId = _DbContext.Departments.Where(item => item.Organization_OrganizationId == OrganizationOfR.OrganizationId && item.DepartmentName.Equals("قسم ادارة الفروع")).First().DepartmentId;

            var EMP = new Employee
            {

                Department_DepartmentId = DEPId,
                Job_JobId = 1,
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
        public IActionResult AddViewDepartment()
        {
            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");
            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();
            var OrganizationOfR = _DbContext.Organizations.Where(item => item.ResponsibleUserId == RUserId).AsNoTracking().FirstOrDefault();

            var DEPOfR = _DbContext.Departments.Where(item => item.Organization_OrganizationId == OrganizationOfR.OrganizationId).AsNoTracking().ToList();



            ViewBag.OrganizationName = OrganizationOfR.OrganizationName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;
            return View(DEPOfR);

        }






        #region



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
                var Cities = _DbContext.Cities.Where(item => item.Country_CountryId == Id).AsNoTracking().Select(item => new

                {
                    CityId = item.CityId,

                    CityName = item.CityName

                }


                ).AsNoTracking().ToList();



                return Json(new { Cities });

            }


            return Json(new { Exists = false });
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


                ).AsNoTracking().ToList();


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
                    $"(Select DetailField_DetailFieldId From OrganizationsProvidTrainingInArea WHERE Organization_OrganizationId={OrganizationOfR.OrganizationId})").AsNoTracking().Include(item => item.FieldOfSpecialty).AsNoTracking().ToList();
            }

            return Json(new { Specialities });

        }

        #endregion
    }


}

