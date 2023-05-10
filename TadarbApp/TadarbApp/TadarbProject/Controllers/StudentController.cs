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
        private readonly IWebHostEnvironment _WebHostEnvironment;
        private readonly IEmailSender _emailSender;
        private readonly IHttpContextAccessor _HttpContextAccessor;


        public StudentController(AppDbContext DbContext, IEmailSender emailSender, IHttpContextAccessor HttpContextAccessor, IWebHostEnvironment webHostEnvironment)
        {
            _DbContext = DbContext;
            _WebHostEnvironment = webHostEnvironment;
            _emailSender = emailSender;
            _HttpContextAccessor = HttpContextAccessor;

        }

        public IActionResult Index()
        {
            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");
            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();
            var UnverTre = _DbContext.UniversitiesTraineeStudents.Where(item => item.UserAccount_UserId == RUserId).Include(item => item.user).Include(item => item.department.User)
                .AsNoTracking().FirstOrDefault();
            var Department = _DbContext.Departments.Where(item => item.DepartmentId == UnverTre.Department_DepartmentId).AsNoTracking().FirstOrDefault();

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Department.Organization_OrganizationId).AsNoTracking().FirstOrDefault();

            if (user.ActivationStatus == "Not_Active")
            {


                TempData["error"] = "حالة الحساب غير نشط " +
                    "الرجاء تعديل كلمة المرور وإضافة سيرة ذاتية ";
                return RedirectToAction("EditAccount");

            }

            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Department.DepartmentName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;





            return View(UnverTre);
        }
        [HttpGet]
        public IActionResult EditAccount()
        {
            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");
            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();



            ViewBag.Username = user.FullName;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditAccount(UniversityTraineeStudent UniversityTraineeStudent, IFormFile? CvFile, IFormFile? otherDoc)
        {
            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");

            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();

            var Student = _DbContext.UniversitiesTraineeStudents.Where(item => item.UserAccount_UserId == RUserId).Include(item => item.user).AsNoTracking().FirstOrDefault();




            //--------------------------------------organization LogoPath to ceate a file in the project and save the Path to the data base ---------------------------------------

            if (CvFile != null && UniversityTraineeStudent.user.UserPassword != null)
            {
                string fileName = Guid.NewGuid().ToString();

                string wwwRootPath = _WebHostEnvironment.WebRootPath;

                var uploadTo = Path.Combine(wwwRootPath, @"StudentDocments\CV\");

                var extension = Path.GetExtension(CvFile.FileName);


                //her to check if the Posted item (product) has an  existing file if true then delete it before creating a new file.
                if (Student.CV_Path != null)
                {
                    var OldCVPath = Path.Combine(wwwRootPath, Student.CV_Path.TrimStart('\\'));

                    if (System.IO.File.Exists(OldCVPath))
                    {
                        System.IO.File.Delete(OldCVPath);
                    }
                }


                //her is to Create a file to has the user uploaded file
                using (var fileStreams = new FileStream(Path.Combine(uploadTo, fileName + extension), FileMode.Create))
                {
                    CvFile.CopyTo(fileStreams);
                }

                //Here what will be in the database.
                var DbLogoPath = @"StudentDocments\CV\" + fileName + extension;
                //-----------------------------------------------------------------------------
                Student.CV_Path = DbLogoPath;
                Student.user.ActivationStatus = "Active";
                Student.user.UserPassword = UniversityTraineeStudent.user.UserPassword;

            }


            if (CvFile != null && UniversityTraineeStudent.user.UserPassword == null)
            {
                string fileName = Guid.NewGuid().ToString();

                string wwwRootPath = _WebHostEnvironment.WebRootPath;

                var uploadTo = Path.Combine(wwwRootPath, @"StudentDocments\CV\");

                var extension = Path.GetExtension(CvFile.FileName);

                //her to check if the Posted item (product) has an  existing file if true then delete it before creating a new file.
                if (Student.CV_Path != null)
                {
                    var OldCVPath = Path.Combine(wwwRootPath, Student.CV_Path.TrimStart('\\'));

                    if (System.IO.File.Exists(OldCVPath))
                    {
                        System.IO.File.Delete(OldCVPath);
                    }
                }

                //her is to Create a file to has the user uploaded file
                using (var fileStreams = new FileStream(Path.Combine(uploadTo, fileName + extension), FileMode.Create))
                {
                    CvFile.CopyTo(fileStreams);
                }

                //Here what will be in the database.
                var DbLogoPath = @"StudentDocments\CV\" + fileName + extension;
                //-----------------------------------------------------------------------------
                Student.CV_Path = DbLogoPath;


            }


            if (otherDoc != null)
            {
                string fileName = Guid.NewGuid().ToString();

                string wwwRootPath = _WebHostEnvironment.WebRootPath;

                var uploadTo = Path.Combine(wwwRootPath, @"StudentDocments\OtherDocmnets\");

                var extension = Path.GetExtension(otherDoc.FileName);

                //her to check if the Posted item (product) has an  existing file if true then delete it before creating a new file.
                if (Student.ExtraDocuments_Path != null)
                {
                    var OldExtraDocumentsPath = Path.Combine(wwwRootPath, Student.ExtraDocuments_Path.TrimStart('\\'));

                    if (System.IO.File.Exists(OldExtraDocumentsPath))
                    {
                        System.IO.File.Delete(OldExtraDocumentsPath);
                    }
                }

                using (var fileStreams = new FileStream(Path.Combine(uploadTo, fileName + extension), FileMode.Create))
                {
                    otherDoc.CopyTo(fileStreams);
                }

                //Here what will be in the database.
                var DbLogoPath = @"StudentDocments\OtherDocmnets\" + fileName + extension;
                //-----------------------------------------------------------------------------
                Student.ExtraDocuments_Path = DbLogoPath;


            }



            if (UniversityTraineeStudent.SkillsDescription != null)
            {
                Student.SkillsDescription = UniversityTraineeStudent.SkillsDescription;
            }




            _DbContext.UniversitiesTraineeStudents.Update(Student);


            _DbContext.SaveChanges();

            return RedirectToAction("Index");

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



            //var OPertunty = _DbContext.TrainingOpportunities.FromSqlRaw("SELECT * FROM TrainingOpportunities WHERE TrainingOpportunityId IN " +
            //    $"(SELECT TrainingOpportunity_TrainingOpportunityId FROM StudentRequestsOnOpportunities WHERE Trainee_TraineeId = {UnverTre.TraineeId});")
            //    .AsNoTracking()
            //    .Include(item => item.DetailFiled)
            //    .Include(item => item.Branch.organization).ToList();

            var StudentReq = _DbContext.StudentRequestsOnOpportunities.Where(item => item.Trainee_TraineeId == UnverTre.TraineeId)
                .AsNoTracking().Include(item => item.trainingOpportunity.Branch.organization).Include(item => item.trainingOpportunity.DetailFiled).ToList();

            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Department.DepartmentName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;


            return View(StudentReq);
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

            var studentreq = _DbContext.StudentRequestsOnOpportunities.Where(item => item.Trainee_TraineeId == UnverTre.TraineeId).FirstOrDefault();

            if (reqoper != null)
            {
                TempData["error"] = "تم التقديم بالفرصة مسبقا ";

                return RedirectToAction("ViewOpportunities");
            }
            if (studentreq != null)
            {
                if (studentreq.DecisionStatus != "waiting")
                {
                    TempData["error"] = "تم قبولك بفرصة اخرى لا تستطيع التفديم ";

                    return RedirectToAction("ViewOpportunities");
                }
            }
            var RequestOpportunity = new StudentRequestOpportunity
            {

                TrainingOpportunity_TrainingOpportunityId = trainingOpportunityVN.TrainingOpportunity.TrainingOpportunityId,
                Trainee_TraineeId = UnverTre.TraineeId,
                RequestDate = DateTime.Now.Date,


            };

            _DbContext.StudentRequestsOnOpportunities.Add(RequestOpportunity);

            opert.RequestedOpportunities = opert.RequestedOpportunities + 1;

            _DbContext.TrainingOpportunities.Update(opert);

            _DbContext.SaveChanges();

            TempData["success"] = "تم التسجيل بالفرصة بنجاح";

            return RedirectToAction("ViewOpportunities");





        }












    }


}