﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TadarbProject.Data;
using TadarbProject.Models;
using TEST2.Services;

namespace TadarbProject.Controllers
{
    public class StudentController : Controller
    {
        private readonly AppDbContext _DbContext;
        private readonly IEmailSender _emailSender;
        private readonly IHttpContextAccessor _HttpContextAccessor;


        public StudentController(AppDbContext DbContext, IEmailSender emailSender, IHttpContextAccessor HttpContextAccessor)
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
            var UnverTre = _DbContext.UniversitiesTraineeStudents.Where(item => item.UserAccount_UserId == RUserId).FirstOrDefault();
            var Department = _DbContext.Departments.Where(item => item.DepartmentId == UnverTre.Department_DepartmentId).FirstOrDefault();

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Department.Organization_OrganizationId).FirstOrDefault();

            var DepartmentTrainingAreas = _DbContext.DepartmentTrainingAreas.Where(item => item.Department_DepartmenId == Department.DepartmentId).FirstOrDefault();

            var DetailFiled = _DbContext.FieldOfSpecialtiesDetails.Where(item => item.DetailFieldId == DepartmentTrainingAreas.TrainArea_DetailFiledId).FirstOrDefault();

            //var Opportunities = _DbContext.TrainingOpportunities.Where(item => item.DetailFiled_DetailFiledId == DetailFiled.DetailFieldId).ToList();

            var Opportunities = _DbContext.TrainingOpportunities.FromSqlRaw($"Select * from TrainingOpportunities WHERE DetailFiled_DetailFiledId IN (SELECT TrainArea_DetailFiledId FROM DepartmentTrainingAreas WHERE Department_DepartmenId ={Department.DepartmentId})").ToList();



            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Department.DepartmentName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;


            return View(Opportunities);
        }





        public IActionResult OpportunityInformation()
        {
            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");
            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).FirstOrDefault();
            var UnverTre = _DbContext.UniversitiesTraineeStudents.Where(item => item.UserAccount_UserId == RUserId).FirstOrDefault();
            var Department = _DbContext.Departments.Where(item => item.DepartmentId == UnverTre.Department_DepartmentId).FirstOrDefault();

            //var OrganizationCompany = _DbContext.Organizations.FirstOrDefault(item => item.OrganizationId == 2);

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Department.Organization_OrganizationId).FirstOrDefault();






            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Department.DepartmentName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;


            return View();
        }




    }














}

