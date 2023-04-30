﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TadarbProject.Data;
using TadarbProject.Models;
using TadarbProject.Models.ViewModels;
using TEST2.Services;

namespace TadarbProject.Controllers
{
    public class CollegeController : Controller
    {
        private readonly AppDbContext _DbContext;
        private readonly IEmailSender _emailSender;
        private readonly IHttpContextAccessor _HttpContextAccessor;


        public CollegeController(AppDbContext DbContext, IEmailSender emailSender, IHttpContextAccessor HttpContextAccessor)
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
            var College = _DbContext.UniversityColleges.Where(item => item.Responsible_UserId == RUserId).FirstOrDefault();

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == College.Organization_OrganizationId).FirstOrDefault();


            ViewBag.OrganizationName = OrganizationOfR.OrganizationName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;
            return View();


        }




        [HttpGet]
        public IActionResult ViewDepartmentUser()
        {

            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");

            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).FirstOrDefault();
            var College = _DbContext.UniversityColleges.Where(item => item.Responsible_UserId == RUserId).FirstOrDefault();

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == College.Organization_OrganizationId).FirstOrDefault();


            ViewBag.OrganizationName = OrganizationOfR.OrganizationName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;
            var DEPOfR = _DbContext.Departments.Where(item => item.Organization_OrganizationId == OrganizationOfR.OrganizationId && item.DepartmentName.Equals("قسم ادارة مسؤولين اقسام الجامعة")).FirstOrDefault();

            IEnumerable<UserAcount> OrgEMP = Enumerable.Empty<UserAcount>(); ;

            if (DEPOfR != null)
            {
                OrgEMP = _DbContext.UserAcounts.FromSqlRaw($"select * from UserAcounts WHERE UserAcounts.UserId in (select Employees.UserAccount_UserId from Employees where Employees.Department_DepartmentId = {DEPOfR.DepartmentId});").ToList();

            }



            return View(OrgEMP);





        }

        [HttpGet]
        public IActionResult AddDepartmentUser()
        {

            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");

            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).FirstOrDefault();
            var College = _DbContext.UniversityColleges.Where(item => item.Responsible_UserId == RUserId).FirstOrDefault();

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == College.Organization_OrganizationId).FirstOrDefault();


            ViewBag.OrganizationName = OrganizationOfR.OrganizationName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;


            return View();


        }

        [HttpPost]
        public IActionResult AddDepartmentUser(EmployeeVM employeeVM)
        {

            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;

            var RUser = _DbContext.UserAcounts.Find(RUserId);

            var College = _DbContext.UniversityColleges.Where(item => item.Responsible_UserId == RUserId).FirstOrDefault();

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == College.Organization_OrganizationId).FirstOrDefault();



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
                City_CityId = College.City_CityId,
                UserType = "DepUni_Admin",
                ActivationStatus = "Active"




            };

            _DbContext.UserAcounts.Add(user);

            _DbContext.SaveChanges();



            var DEPOfR = _DbContext.Departments.Where(item => item.Organization_OrganizationId == OrganizationOfR.OrganizationId && item.DepartmentName.Equals("قسم ادارة مسؤولين اقسام الجامعة")).FirstOrDefault();



            if (DEPOfR == null)
            {
                var DEP = new Department
                {

                    DepartmentName = "قسم ادارة مسؤولين اقسام الجامعة",
                    Organization_OrganizationId = OrganizationOfR.OrganizationId,
                    Responsible_UserId = RUser.UserId,
                    College_CollegeId = College.CollegeId,



                };

                _DbContext.Departments.Add(DEP);

                _DbContext.SaveChanges();
            }



            int DEPId = _DbContext.Departments.Where(item => item.Organization_OrganizationId == OrganizationOfR.OrganizationId && item.DepartmentName.Equals("قسم ادارة مسؤولين اقسام الجامعة")).First().DepartmentId;

            var EMP = new Employee
            {

                Department_DepartmentId = DEPId,
                Job_JobId = 5,
                SSN = employeeVM.employee.SSN,
                UserAccount_UserId = user.UserId,



            };

            _DbContext.Employees.Add(EMP);

            _DbContext.SaveChanges();
            TempData["success"] = "تم إضافة حساب مسؤول القسم  بنجاح";
            return RedirectToAction("ViewDepartmentUser");

        }


        [HttpGet]
        public IActionResult AddViewDepartmentUni()
        {
            //ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");
            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;
            //var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).FirstOrDefault();
            var College = _DbContext.UniversityColleges.Where(item => item.Responsible_UserId == RUserId).FirstOrDefault();

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == College.Organization_OrganizationId).FirstOrDefault();


            ViewBag.OrganizationName = OrganizationOfR.OrganizationName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            //ViewBag.Username = user.FullName;


            IEnumerable<UserAcount> CollegeEMP = Enumerable.Empty<UserAcount>(); ;


            var DEPOfC = _DbContext.Departments.Where(item => item.Organization_OrganizationId == OrganizationOfR.OrganizationId && item.College_CollegeId == College.CollegeId).FirstOrDefault();


            if (DEPOfC != null)
            {

                CollegeEMP = _DbContext.UserAcounts.FromSqlRaw($"Select * from UserAcounts WHERE UserAcounts.UserId IN (Select Employees.UserAccount_UserId from Employees where Employees.Department_DepartmentId = {DEPOfC.DepartmentId})" +
                "AND UserAcounts.UserId NOT IN(select Responsible_UserId from dbo.Departments);").ToList();


            }




            DepartmentVM departmentVM = new()
            {
                department = new(),

                UserListItems = CollegeEMP.ToList().Select(u => new SelectListItem { Text = u.FullName, Value = u.UserId.ToString() }),


            };



            return View(departmentVM);




        }




        [HttpPost]
        public IActionResult AddViewDepartmentUni(DepartmentVM departmentVM)
        {

            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;
            var College = _DbContext.UniversityColleges.Where(item => item.Responsible_UserId == RUserId).FirstOrDefault();
            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == College.Organization_OrganizationId).FirstOrDefault();
            ViewBag.OrganizationName = OrganizationOfR.OrganizationName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;


            if (ModelState.IsValid)
            {

                var DEP = new Department
                {
                    DepartmentName = departmentVM.department.DepartmentName,

                    Responsible_UserId = departmentVM.department.Responsible_UserId,


                    Organization_OrganizationId = OrganizationOfR.OrganizationId,

                    College_CollegeId = College.CollegeId,






                };

                _DbContext.Departments.Add(DEP);



                _DbContext.SaveChanges();

                TempData["success"] = "تم إضافة الفسم  بنجاح";



            }

            return View(departmentVM);












        }




        [HttpGet]
        public IActionResult GetAllDEPs()
        {

            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;

            var College = _DbContext.UniversityColleges.Where(item => item.Responsible_UserId == RUserId).FirstOrDefault();

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == College.Organization_OrganizationId).FirstOrDefault();

            var depList = _DbContext.Departments.Where(item => item.Organization_OrganizationId == OrganizationOfR.OrganizationId && item.College_CollegeId == College.CollegeId && !item.DepartmentName.Equals("قسم ادارة مسؤولين اقسام الجامعة"))
                .Include(item => item.User)
                .ToList();

            if (depList == null)
            {
                return Json(new { Exists = false });
            }



            return Json(new { Exists = true, depList });
        }



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


        [HttpGet]
        public IActionResult PhoneExists(string? Phone)
        {
            if (Phone == null)
            {
                return Json(new { Exists = false });
            }

            var item = _DbContext.UserAcounts.Where(item => item.Phone.Equals(Phone)).FirstOrDefault();

            if (item == null)
            {
                return Json(new { Exists = false });
            }



            return Json(new { Exists = true });
        }







        //[HttpGet]
        //public IActionResult ViewDepartment()
        //{
        //    ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");

        //    int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;

        //    var College = _DbContext.UniversityColleges.Where(item => item.Responsible_UserId == RUserId).FirstOrDefault();

        //    var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == College.Organization_OrganizationId).FirstOrDefault();


        //    ViewBag.OrganizationName = OrganizationOfR.OrganizationName;
        //    ViewBag.OrganizationImage = OrganizationOfR.LogoPath;



        //    var DEPOfR = _DbContext.Departments.Where(item => item.Organization_OrganizationId == OrganizationOfR.OrganizationId).Include(item => item.User).ToList();

        //    //IEnumerable<UserAcount> OrgEMP = Enumerable.Empty<UserAcount>(); ;

        //    //if (DEPOfR != null)
        //    //{
        //    //    OrgEMP = _DbContext.UserAcounts.FromSqlRaw($"select * from UserAcounts WHERE UserAcounts.UserId in (select Employees.UserAccount_UserId from Employees where Employees.Department_DepartmentId = {DEPOfR.DepartmentId});").ToList();

        //    //}





        //    return View(DEPOfR);



        //}

    }
}
