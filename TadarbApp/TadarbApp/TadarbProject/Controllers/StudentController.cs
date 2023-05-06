using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TadarbProject.Data;
using TadarbProject.Models;
using TadarbProject.Models.ViewModels;
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
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();
            var UnverTre = _DbContext.UniversitiesTraineeStudents.Where(item => item.UserAccount_UserId == RUserId).AsNoTracking().FirstOrDefault();
            var Department = _DbContext.Departments.Where(item => item.DepartmentId == UnverTre.Department_DepartmentId).AsNoTracking().FirstOrDefault();

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Department.Organization_OrganizationId).AsNoTracking().FirstOrDefault();



            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Department.DepartmentName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;


            return View();
        }

        [HttpGet]
        public IActionResult ViewOpportunities()
        {
            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");
            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();
            var UnverTre = _DbContext.UniversitiesTraineeStudents.Where(item => item.UserAccount_UserId == RUserId).AsNoTracking().FirstOrDefault();
            var Department = _DbContext.Departments.Where(item => item.DepartmentId == UnverTre.Department_DepartmentId).AsNoTracking().FirstOrDefault();

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Department.Organization_OrganizationId).AsNoTracking().FirstOrDefault();

            var DepartmentTrainingAreas = _DbContext.DepartmentTrainingAreas.Where(item => item.Department_DepartmenId == Department.DepartmentId).AsNoTracking().FirstOrDefault();

            var DetailFiled = _DbContext.FieldOfSpecialtiesDetails.Where(item => item.DetailFieldId == DepartmentTrainingAreas.TrainArea_DetailFiledId).AsNoTracking().FirstOrDefault();

            //var Opportunities = _DbContext.TrainingOpportunities.Where(item => item.DetailFiled_DetailFiledId == DetailFiled.DetailFieldId).ToList();

            var Opportunities = _DbContext.TrainingOpportunities.FromSqlRaw($"Select * from TrainingOpportunities WHERE DetailFiled_DetailFiledId IN" +
                $"(SELECT TrainArea_DetailFiledId FROM DepartmentTrainingAreas WHERE Department_DepartmenId ={Department.DepartmentId})").AsNoTracking().Include(item => item.DetailFiled)
                .Include(item => item.Department.organization).AsNoTracking().ToList();



            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Department.DepartmentName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;


            return View(Opportunities);
        }

        [HttpGet]
        public IActionResult ViewOpportunitiesStatus()
        {
            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");
            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();
            var UnverTre = _DbContext.UniversitiesTraineeStudents.Where(item => item.UserAccount_UserId == RUserId).FirstOrDefault();
            var Department = _DbContext.Departments.Where(item => item.DepartmentId == UnverTre.Department_DepartmentId).AsNoTracking().FirstOrDefault();

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Department.Organization_OrganizationId).AsNoTracking().FirstOrDefault();

            var Req = _DbContext.StudentRequestsOnOpportunities.Where(item => item.Trainee_TraineeId == UnverTre.TraineeId).FirstOrDefault();

            var reqlist = _DbContext.StudentRequestsOnOpportunities.Where(item => item.Trainee_TraineeId == UnverTre.TraineeId).ToList();

            var OPertunty = _DbContext.TrainingOpportunities.Where(item => item.TrainingOpportunityId == Req.TrainingOpportunity_TrainingOpportunityId)
                .Include(item => item.Branch.organization)
                .Include(item => item.DetailFiled).FirstOrDefault();

            TrainingOpportunityVM viwooper = new()
            {
                TrainingOpportunity= OPertunty,
                StudentRequestsOnOpportunities=Req,

            };    


               ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Department.DepartmentName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;


            return View(viwooper);
        }

        public IActionResult OpportunityInformation(int? id)
        {
            if (id == null)
            {

                return NotFound();
            }

            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");
            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();
            var UnverTre = _DbContext.UniversitiesTraineeStudents.Where(item => item.UserAccount_UserId == RUserId).AsNoTracking().FirstOrDefault();
            var Department = _DbContext.Departments.Where(item => item.DepartmentId == UnverTre.Department_DepartmentId).AsNoTracking().FirstOrDefault();

            var Opportunity = _DbContext.TrainingOpportunities
                .Where(item => item.TrainingOpportunityId == id).Include(item => item.DetailFiled).Include(item => item.trainingType).Include(item => item.Branch.city.Country).Include(item => item.Branch.organization).AsNoTracking().FirstOrDefault();



            //var OrganizationCompany = _DbContext.Organizations.FirstOrDefault(item => item.OrganizationId == 2);

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Department.Organization_OrganizationId).AsNoTracking().FirstOrDefault();

            TrainingOpportunityVM trainingOpportunityVM = new TrainingOpportunityVM
            {
                TrainingOpportunity = Opportunity,




            };

            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Department.DepartmentName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;


            return View(trainingOpportunityVM);
        }


        public IActionResult RequsetOnOpportunity(TrainingOpportunityVM trainingOpportunityVN)
        {

            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();
            var UnverTre = _DbContext.UniversitiesTraineeStudents.Where(item => item.UserAccount_UserId == RUserId).AsNoTracking().FirstOrDefault();
            var opert = _DbContext.TrainingOpportunities.Where(item => item.TrainingOpportunityId == trainingOpportunityVN.TrainingOpportunity.TrainingOpportunityId).FirstOrDefault();


            var reqoper = _DbContext.StudentRequestsOnOpportunities.Where(item => item.Trainee_TraineeId == UnverTre.TraineeId && item.TrainingOpportunity_TrainingOpportunityId == opert.TrainingOpportunityId).FirstOrDefault();

            if (reqoper != null)
            {
                TempData["error"] = "تم التقديم بالفرصة مسبقا ";

                return RedirectToAction("ViewOpportunities");
            }

            var RequestOpportunity = new StudentRequestOpportunity
            {

                TrainingOpportunity_TrainingOpportunityId = trainingOpportunityVN.TrainingOpportunity.TrainingOpportunityId,
                Trainee_TraineeId = UnverTre.TraineeId,
                RequestDate = DateTime.Now.Date,


            };

            _DbContext.StudentRequestsOnOpportunities.Add(RequestOpportunity);


            _DbContext.SaveChanges();

            TempData["success"] = "تم التسجيل بالفرصة بنجاح";

            return RedirectToAction("ViewOpportunities");





        }












    }


}