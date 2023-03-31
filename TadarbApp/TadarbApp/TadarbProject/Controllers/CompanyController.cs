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
    public class CompanyController : Controller
    {
        private readonly AppDbContext _DbContext;
        private readonly IWebHostEnvironment _WebHostEnvironment;
        private readonly IEmailSender _emailSender;
       

        public IActionResult Index()
        {


            return View();
        }

        public IActionResult AddSpecialities()
        {


            return View();
        }

        public IActionResult ViewBranches()
        {


            return View();
        }
        public IActionResult Addbranches()
        {


            return View();
        }

        public IActionResult  ViewUsers()
        {


            return View();
        }

        public IActionResult AddUsers()
        {


            return View();
        }




    }
}