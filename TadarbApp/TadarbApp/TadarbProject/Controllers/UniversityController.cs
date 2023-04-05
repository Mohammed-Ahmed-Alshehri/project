using Microsoft.AspNetCore.Mvc;
using TadarbProject.Data;
using TEST2.Services;

namespace TadarbProject.Controllers
{
    //yazeed edit
    //badr modify
    //abdulhadi
    public class UniversityController : Controller
    {
        private readonly AppDbContext _DbContext;
        private readonly IWebHostEnvironment _WebHostEnvironment;
        private readonly IEmailSender _emailSender;


        public IActionResult Index()
        {


            return View();
        }

        public IActionResult AddColleges()
        {


            return View();
        }






    }
}