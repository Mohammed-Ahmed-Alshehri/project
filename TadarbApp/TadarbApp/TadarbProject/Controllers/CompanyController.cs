﻿using Microsoft.AspNetCore.Mvc;
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

        public IActionResult Index()
        {
            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");

            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.ResponsibleUserId == RUserId).FirstOrDefault();


            ViewBag.OrganizationName = OrganizationOfR.OrganizationName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;

            return View();
        }

        public IActionResult ViewSpecialities()
        {

            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.ResponsibleUserId == RUserId).FirstOrDefault();


            ViewBag.OrganizationName = OrganizationOfR.OrganizationName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            return View();
        }

        public IActionResult AddSpecialities()
        {
            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.ResponsibleUserId == RUserId).FirstOrDefault();


            ViewBag.OrganizationName = OrganizationOfR.OrganizationName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;

            return View();
        }




        public IActionResult ViewBranches()
        {

            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;
            var OrganizationOfR = _DbContext.Organizations.Where(item => item.ResponsibleUserId == RUserId).FirstOrDefault();

            var OrganizationBranches = _DbContext.OrganizationBranches_TrainProv.Where(item => item.Organization_OrganizationId == OrganizationOfR.OrganizationId).FirstOrDefault();





            var Branches = _DbContext.OrganizationBranches_TrainProv.Where(item => item.Organization_OrganizationId == OrganizationOfR.OrganizationId).Include(item => item.city).Include(item => item.user).ToList();



            ViewBag.OrganizationName = OrganizationOfR.OrganizationName;

            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;

            return View(Branches);
        }

        [HttpGet]
        public IActionResult Addbranches()
        {
            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.ResponsibleUserId == RUserId).FirstOrDefault();

            var DEPOfR = _DbContext.Departments.Where(item => item.Organization_OrganizationId == OrganizationOfR.OrganizationId && item.DepartmentName.Equals("قسم ادارة الفروع")).FirstOrDefault();

            ViewBag.OrganizationName = OrganizationOfR.OrganizationName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;

            IEnumerable<UserAcount> OrgEMP = Enumerable.Empty<UserAcount>(); ;


            if (DEPOfR != null)
            {

                OrgEMP = _DbContext.UserAcounts.FromSqlRaw($"select * from UserAcounts WHERE UserAcounts.UserId IN (select Employees.UserAccount_UserId from Employees where Employees.Department_DepartmentId ={DEPOfR.DepartmentId})" +
                "AND UserAcounts.UserId NOT IN (select Responsible_UserId from dbo.OrganizationBranches_TrainProv);").ToList();

            }

            BranchVM branchVM = new()
            {
                Branch = new(),
                CountryListItems = _DbContext.Countries.ToList().Select(u => new SelectListItem { Text = u.CountryName, Value = u.CountryId.ToString() }),

                CityListItems = _DbContext.Cities.ToList().Select(u => new SelectListItem { Text = u.CityName, Value = u.CityId.ToString() }),
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

                var OrganizationOfR = _DbContext.Organizations.Where(item => item.ResponsibleUserId == RUserId).FirstOrDefault();

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


                var RBranchManger = _DbContext.UserAcounts.FirstOrDefault(item => item.UserId == branchVM.Branch.Responsible_UserId);


                RBranchManger.City_CityId = branchVM.Branch.City_CityId;



                _DbContext.UserAcounts.Update(RBranchManger);


                _DbContext.SaveChanges();

                return RedirectToAction("ViewBranches");

            }

            return View(branchVM);
        }

        [HttpGet]
        public IActionResult AddViewDepartment()
        {
            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.ResponsibleUserId == RUserId).FirstOrDefault();

            ViewBag.OrganizationName = OrganizationOfR.OrganizationName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;


            return View();
        }

        //[HttpPost]
        //public IActionResult AddViewDepartment(Department department)
       

        //    return View();

        //}


        public IActionResult ViewUsers()

        {
            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.ResponsibleUserId == RUserId).FirstOrDefault();

            ViewBag.OrganizationName = OrganizationOfR.OrganizationName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;


            var DEPOfR = _DbContext.Departments.Where(item => item.Organization_OrganizationId == OrganizationOfR.OrganizationId && item.DepartmentName.Equals("قسم ادارة الفروع")).FirstOrDefault();

            IEnumerable<UserAcount> OrgEMP = Enumerable.Empty<UserAcount>(); ;

            if (DEPOfR != null)
            {
                OrgEMP = _DbContext.UserAcounts.FromSqlRaw($"select * from UserAcounts WHERE UserAcounts.UserId in (select Employees.UserAccount_UserId from Employees where Employees.Department_DepartmentId = {DEPOfR.DepartmentId});").ToList();

            }



            return View(OrgEMP);
        }

        [HttpGet]
        public IActionResult AddUsers()
        {
            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.ResponsibleUserId == RUserId).FirstOrDefault();


            ViewBag.OrganizationName = OrganizationOfR.OrganizationName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            return View();
        }






        [HttpPost]

        public IActionResult AddUsers(EmployeeVM employeeVM)
        {


            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;

            var RUser = _DbContext.UserAcounts.Find(RUserId);

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.ResponsibleUserId == RUserId).FirstOrDefault();

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



            var DEPOfR = _DbContext.Departments.Where(item => item.Organization_OrganizationId == OrganizationOfR.OrganizationId && item.DepartmentName.Equals("قسم ادارة الفروع")).FirstOrDefault();


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

        public IActionResult DeleteUser(int? id, EmployeeVM employeeVM)
        {

            var getid = id;
            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;

            var RUser = _DbContext.UserAcounts.Find(getid);

            //var OrganizationOfR = _DbContext.Organizations.Where(item => item.ResponsibleUserId == RUserId).FirstOrDefault();

            //var Branches = _DbContext.Employees.Where(item => item.UserAccount_UserId == RUser.UserId).Include(item => item.).Include(item => item.user).ToList();

            var viewuserdetail = _DbContext.UserAcounts.FromSqlRaw($"Select * From UserAcounts join Employees  on UserAcounts.UserId = Employees.UserAccount_UserId where UserAcounts.UserId={getid};").FirstOrDefault();

            if (id == null)
            {

                return View();

            }


            _DbContext.UserAcounts.Remove(viewuserdetail);

            _DbContext.SaveChanges();



            TempData["success"] = "تم حذف حساب المسؤول  بنجاح";
            return RedirectToAction("ViewUsers");



        }




        #region



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
        #endregion
    }


}

