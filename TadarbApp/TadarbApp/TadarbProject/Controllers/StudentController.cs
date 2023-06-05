using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        private static string Name;
        private static int UserId;
        private static UserAcount User;
        private static UniversityTraineeStudent trainee;
        private static Department department;
        private static Organization organizationOfR;


        public StudentController(AppDbContext DbContext, IEmailSender emailSender, IHttpContextAccessor HttpContextAccessor, IWebHostEnvironment webHostEnvironment)
        {
            _DbContext = DbContext;
            _WebHostEnvironment = webHostEnvironment;
            _emailSender = emailSender;
            _HttpContextAccessor = HttpContextAccessor;

        }

        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(_HttpContextAccessor.HttpContext.Session.GetString("Name")) || string.IsNullOrEmpty(_HttpContextAccessor.HttpContext.Session.GetInt32("UserId").ToString()))
            {

                return RedirectToAction("Login", "Home");

            }

            Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");

            ViewBag.Name = Name;

            UserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;

            int RUserId = UserId;

            User = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();

            var user = User;

            trainee = _DbContext.UniversitiesTraineeStudents.Where(item => item.UserAccount_UserId == RUserId).Include(item => item.user.City.Country).Include(item => item.department.User)
                .AsNoTracking().FirstOrDefault();

            var UnverTre = trainee;

            var AcadmicSuper = _DbContext.SemestersStudentAndEvaluationDetails.Where(item => item.studentRequest.Trainee_TraineeId == trainee.TraineeId)
               .Include(item => item.EmployeeAcademicSupervisor.userAcount).AsNoTracking().FirstOrDefault();


            if (AcadmicSuper == null)
            {
                ViewBag.AcadmicSuper = "لم يتعين مشرف";
            }
            else
            {
                ViewBag.AcadmicSuper = AcadmicSuper.EmployeeAcademicSupervisor.userAcount.FullName;

            }

            department = _DbContext.Departments.Where(item => item.DepartmentId == UnverTre.Department_DepartmentId).AsNoTracking().FirstOrDefault();

            var Department = department;

            organizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Department.Organization_OrganizationId).AsNoTracking().FirstOrDefault();

            var OrganizationOfR = organizationOfR;

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
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(UserId.ToString()))
            {

                return RedirectToAction("Login", "Home");

            }

            ViewBag.Name = Name;
            int RUserId = UserId;
            var user = User;
            ViewBag.Username = user.FullName;

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditAccount(UniversityTraineeStudent UniversityTraineeStudent, IFormFile? CvFile, IFormFile? otherDoc)
        {



            ViewBag.Name = Name;
            int RUserId = UserId;
            var user = User;

            var Student = trainee;




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
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(UserId.ToString()))
            {

                return RedirectToAction("Login", "Home");

            }

            ViewBag.Name = Name;
            int RUserId = UserId;
            var user = User;
            var UnverTre = trainee;
            var Department = department;
            var OrganizationOfR = organizationOfR;

            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Department.DepartmentName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;

            IEnumerable<SelectListItem> CountryListItems = Enumerable.Empty<SelectListItem>();
            CountryListItems = _DbContext.Countries.AsNoTracking().ToList().Select(u => new SelectListItem { Text = u.CountryName, Value = u.CountryId.ToString() });

            ViewBag.CountryListItems = CountryListItems;

            IEnumerable<SelectListItem> DepartmentDetailFileds = Enumerable.Empty<SelectListItem>();

            DepartmentDetailFileds = _DbContext.FieldOfSpecialtiesDetails.FromSqlRaw("SELECT * FROM FieldOfSpecialtiesDetails WHERE DetailFieldId " +
             $"IN (SELECT TrainArea_DetailFiledId FROM DepartmentTrainingAreas WHERE Department_DepartmenId ={Department.DepartmentId})")
             .AsNoTracking().ToList().Select(u => new SelectListItem { Text = u.SpecializationName, Value = u.DetailFieldId.ToString() });

            ViewBag.DepartmentDetailFileds = DepartmentDetailFileds;

            //var DepartmentTrainingAreas = _DbContext.DepartmentTrainingAreas.Where(item => item.Department_DepartmenId == Department.DepartmentId).AsNoTracking().FirstOrDefault();

            //var DetailFiled = _DbContext.FieldOfSpecialtiesDetails.Where(item => item.DetailFieldId == DepartmentTrainingAreas.TrainArea_DetailFiledId).AsNoTracking().FirstOrDefault();

            //IEnumerable<TrainingOpportunity> Opportunities = Enumerable.Empty<TrainingOpportunity>();

            //Opportunities = _DbContext.TrainingOpportunities.FromSqlRaw($"Select * from TrainingOpportunities WHERE DetailFiled_DetailFiledId IN" +
            //   $"(SELECT TrainArea_DetailFiledId FROM DepartmentTrainingAreas WHERE Department_DepartmenId ={Department.DepartmentId}) " +
            //   $" AND OpportunityStatus='Available' AND AbilityofSubmissionStatus='Available'").AsNoTracking().Include(item => item.DetailFiled)
            //   .Include(item => item.Department.organization).AsNoTracking().ToList();





            return View();
        }

        [HttpGet]
        public IActionResult ViewOpportunitiesStatus()
        {
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(UserId.ToString()))
            {

                return RedirectToAction("Login", "Home");

            }

            ViewBag.Name = Name;
            int RUserId = UserId;
            var user = User;
            var UnverTre = trainee;
            var Department = department;
            var OrganizationOfR = organizationOfR;

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


        [HttpGet]
        public IActionResult OpportunityInformation(int? id)
        {
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(UserId.ToString()))
            {

                return RedirectToAction("Login", "Home");

            }
            if (id == null)
            {

                return NotFound();
            }

            ViewBag.Name = Name;
            int RUserId = UserId;
            var user = User;
            var UnverTre = trainee;
            var Department = department;
            var OrganizationOfR = organizationOfR;
            var Opportunity = _DbContext.TrainingOpportunities
                .Where(item => item.TrainingOpportunityId == id).Include(item => item.DetailFiled).Include(item => item.trainingType).Include(item => item.Branch.city.Country).Include(item => item.Branch.organization).AsNoTracking().FirstOrDefault();



            //var OrganizationCompany = _DbContext.Organizations.FirstOrDefault(item => item.OrganizationId == 2);;

            TrainingOpportunityVM trainingOpportunityVM = new TrainingOpportunityVM
            {
                TrainingOpportunity = Opportunity,




            };

            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Department.DepartmentName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;


            return View(trainingOpportunityVM);
        }

        [HttpPost]
        public IActionResult RequsetOnOpportunity(TrainingOpportunityVM trainingOpportunityVN)
        {

            int RUserId = UserId;
            var user = User;
            var UnverTre = trainee;

            var opert = _DbContext.TrainingOpportunities.Where(item => item.TrainingOpportunityId == trainingOpportunityVN.TrainingOpportunity.TrainingOpportunityId).FirstOrDefault();


            var reqoper = _DbContext.StudentRequestsOnOpportunities.Where(item => item.Trainee_TraineeId == UnverTre.TraineeId && item.TrainingOpportunity_TrainingOpportunityId == opert.TrainingOpportunityId
          ).ToList();

            var studentreq = _DbContext.StudentRequestsOnOpportunities.Where(item => item.Trainee_TraineeId == UnverTre.TraineeId).ToList();



            if (reqoper != null)
            {
                foreach (var i in reqoper)
                {
                    if (i.DecisionStatus == "waiting")
                    {
                        TempData["error"] = "تم التقديم بالفرصة مسبقا ";

                        return RedirectToAction("ViewOpportunities");
                    }

                    if (i.DecisionStatus == "waitingStudentApprove")
                    {
                        TempData["error"] = "تم قبولك بالفرصة الرجاء الموافقة ";

                        return RedirectToAction("ViewOpportunities");
                    }

                    if (i.DecisionStatus == "approved")
                    {
                        TempData["error"] = "تم قبولك بالفرصة مسبقا ";

                        return RedirectToAction("ViewOpportunities");
                    }


                    if (i.DecisionStatus == "system disable" || i.DecisionStatus == "rejected")
                    {
                        TempData["error"] = "لايمكنك التقديم على هذه الفرصة ";

                        return RedirectToAction("ViewOpportunities");
                    }

                }
            }

            if (studentreq != null)
            {

                foreach (var i in studentreq)
                {
                    if (i.DecisionStatus == "approved")
                    {
                        TempData["error"] = "تم قبولك بفرصة اخرى لا تستطيع التفديم ";

                        return RedirectToAction("ViewOpportunities");
                    }

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



        [HttpGet]
        public IActionResult ViewAssignments()
        {
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(UserId.ToString()))
            {

                return RedirectToAction("Login", "Home");

            }

            ViewBag.Name = Name;
            int RUserId = UserId;
            var user = User;
            var UnverTre = trainee;
            var Department = department;
            var OrganizationOfR = organizationOfR;
            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Department.DepartmentName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;

            IEnumerable<StudentSemesterEvaluationMark> Assignments = Enumerable.Empty<StudentSemesterEvaluationMark>();


            Assignments = _DbContext.StudentSemesterEvaluationMarks.FromSqlRaw("SELECT * FROM StudentSemesterEvaluationMarks WHERE SemesterStudentAndEvaluationDetail_DetailId IN" +
                "(SELECT SemesterStudentAndEvaluationDetailId FROM SemestersStudentAndEvaluationDetails WHERE StudentRequest_StudentRequestId = " +
                $"(SELECT StudentRequestOpportunityId FROM StudentRequestsOnOpportunities WHERE Trainee_TraineeId = {UnverTre.TraineeId} AND DecisionStatus ='approved'))")
                .Include(item => item.assessmentTypeDetail.assessmentType).AsNoTracking().ToList();


            return View(Assignments);
        }


        [HttpGet]
        public IActionResult SubmitAssignment(int? SSEMId)
        {

            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(UserId.ToString()))
            {

                return RedirectToAction("Login", "Home");

            }
            if (SSEMId == 0 || SSEMId == null)
            {
                return NotFound();
            }


            ViewBag.Name = Name;
            int RUserId = UserId;
            var user = User;
            var UnverTre = trainee;
            var Department = department;
            var OrganizationOfR = organizationOfR;
            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Department.DepartmentName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;


            var Assignment = _DbContext.StudentSemesterEvaluationMarks.Where(item => item.StudentSemesterEvaluationMarkId == SSEMId)
                 .Include(item => item.assessmentTypeDetail.assessmentType).AsNoTracking().FirstOrDefault();

            if (Assignment == null)
            {
                return NotFound();
            }


            return View(Assignment);

        }

        [HttpPost]
        public IActionResult SubmitAssignment(StudentSemesterEvaluationMark obj, IFormFile? ReportFile)
        {




            ViewBag.Name = Name;
            int RUserId = UserId;
            var user = User;
            var UnverTre = trainee;
            var Department = department;
            var OrganizationOfR = organizationOfR;
            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Department.DepartmentName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;


            //Startt UploadFile

            string fileName = Guid.NewGuid().ToString();

            string wwwRootPath = _WebHostEnvironment.WebRootPath;

            var uploadTo = Path.Combine(wwwRootPath, @"StudentDocments\Assignments\");

            var extension = Path.GetExtension(ReportFile.FileName);

            //here is to Create a file to has the user uploaded file
            using (var fileStreams = new FileStream(Path.Combine(uploadTo, fileName + extension), FileMode.Create))
            {
                ReportFile.CopyTo(fileStreams);
            }

            //Here what will be in the database.
            obj.SupportiveDocumentsPath = @"StudentDocments\Assignments\" + fileName + extension;


            _DbContext.StudentSemesterEvaluationMarks.Update(obj);

            _DbContext.SaveChanges();

            return RedirectToAction("ViewAssignments");

        }



        #region



        [HttpGet]
        public IActionResult GetOpportunities(int CityID, int DetailFiledId)
        {
            int RUserId = UserId;
            var user = User;
            var UnverTre = trainee;
            var Department = department;
            var OrganizationOfR = organizationOfR;

            IEnumerable<TrainingOpportunity> Opportunities = Enumerable.Empty<TrainingOpportunity>();

            if (CityID == 0 && DetailFiledId == 0)
            {
                Opportunities = _DbContext.TrainingOpportunities.FromSqlRaw($"Select * from TrainingOpportunities WHERE DetailFiled_DetailFiledId IN" +
             $"(SELECT TrainArea_DetailFiledId FROM DepartmentTrainingAreas WHERE Department_DepartmenId ={Department.DepartmentId}) " +
             $" AND OpportunityStatus='Available' AND AbilityofSubmissionStatus='Available'").AsNoTracking().Include(item => item.DetailFiled)
             .Include(item => item.Department.organization).AsNoTracking().ToList();

                if (Opportunities?.Any() != true)
                {
                    return Json(new { Opportunities, Exists = false, message = "تخصصك" });

                }

                return Json(new { Opportunities, Exists = true });
            }


            if (CityID != 0 && DetailFiledId == 0)
            {
                Opportunities = _DbContext.TrainingOpportunities.FromSqlRaw($"Select * from TrainingOpportunities WHERE DetailFiled_DetailFiledId IN " +
                    $"(SELECT TrainArea_DetailFiledId FROM DepartmentTrainingAreas WHERE Department_DepartmenId = {Department.DepartmentId})" +
                    $" AND OpportunityStatus='Available' AND AbilityofSubmissionStatus='Available' AND Branch_BranchId " +
                    $"IN (SELECT BranchId FROM OrganizationBranches_TrainProv WHERE City_CityId ={CityID})").AsNoTracking().Include(item => item.DetailFiled)
             .Include(item => item.Department.organization).AsNoTracking().ToList();

                if (Opportunities?.Any() != true)
                {
                    return Json(new { Opportunities, Exists = false, message = "مدخلاتك" });

                }

                return Json(new { Opportunities, Exists = true });
            }


            if (CityID == 0 && DetailFiledId != 0)
            {
                Opportunities = _DbContext.TrainingOpportunities.FromSqlRaw($"Select * from TrainingOpportunities WHERE DetailFiled_DetailFiledId IN " +
                    $"(SELECT TrainArea_DetailFiledId FROM DepartmentTrainingAreas WHERE Department_DepartmenId = {Department.DepartmentId} AND DetailFiled_DetailFiledId = {DetailFiledId})" +
                    $" AND OpportunityStatus='Available' AND AbilityofSubmissionStatus='Available'").AsNoTracking().Include(item => item.DetailFiled)
             .Include(item => item.Department.organization).AsNoTracking().ToList();

                if (Opportunities?.Any() != true)
                {
                    return Json(new { Opportunities, Exists = false, message = "مدخلاتك" });

                }

                return Json(new { Opportunities, Exists = true });
            }



            if (CityID != 0 && DetailFiledId != 0)
            {
                Opportunities = _DbContext.TrainingOpportunities.FromSqlRaw($" Select * from TrainingOpportunities WHERE DetailFiled_DetailFiledId IN " +
                    $"(SELECT TrainArea_DetailFiledId FROM DepartmentTrainingAreas WHERE Department_DepartmenId = {Department.DepartmentId} AND DetailFiled_DetailFiledId = {DetailFiledId}) " +
                    $"AND OpportunityStatus='Available' AND AbilityofSubmissionStatus='Available' AND Branch_BranchId " +
                    $"IN (SELECT BranchId FROM OrganizationBranches_TrainProv WHERE City_CityId = {CityID})").AsNoTracking().Include(item => item.DetailFiled)
             .Include(item => item.Department.organization).AsNoTracking().ToList();

                if (Opportunities?.Any() != true)
                {
                    return Json(new { Opportunities, Exists = false, message = "مدخلاتك" });

                }

                return Json(new { Opportunities, Exists = true });
            }



            //if (Opportunities?.Any() == true)
            //{
            //    return Json(new { Opportunities, Exists = true });
            //}

            //if (Opportunities?.Any() != true)
            //{
            //    // if collection is null
            //    // if collection does not contain any item


            //}


            return Json(new { Opportunities, Exists = false, message = "تخصصك" });
        }


        [HttpPost]
        public IActionResult StudentCancelBefore(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }


            var req = _DbContext.StudentRequestsOnOpportunities.Where(item => item.StudentRequestOpportunityId == id).Include(item => item.trainingOpportunity).AsNoTracking().FirstOrDefault();

            if (req == null)
            {
                return Json(new { success = false });
            }

            if (req.DecisionStatus != "waiting")
            {
                TempData["error"] = "لايمكن الغاء الطلب";

                return Json(new { success = false });

            }
            req.DecisionDate = DateTime.Now.Date;
            req.DecisionStatus = "CancelBeforeApprove";

            _DbContext.StudentRequestsOnOpportunities.Update(req);


            req.trainingOpportunity.RequestedOpportunities = req.trainingOpportunity.RequestedOpportunities - 1;

            _DbContext.TrainingOpportunities.Update(req.trainingOpportunity);


            _DbContext.SaveChanges();



            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult StudentCancelAfter(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }


            var req = _DbContext.StudentRequestsOnOpportunities.Where(item => item.StudentRequestOpportunityId == id).Include(item => item.trainingOpportunity).AsNoTracking().FirstOrDefault();

            if (req == null)
            {
                return Json(new { success = false });
            }

            if (req.DecisionStatus != "waitingStudentApprove")
            {
                TempData["error"] = "لايمكن الغاء الطلب";

                return Json(new { success = false });

            }

            req.DecisionDate = DateTime.Now.Date;
            req.DecisionStatus = "CancelAftereApprove";

            _DbContext.StudentRequestsOnOpportunities.Update(req);


            req.trainingOpportunity.RequestedOpportunities = req.trainingOpportunity.RequestedOpportunities - 1;

            req.trainingOpportunity.AvailableOpportunities = req.trainingOpportunity.AvailableOpportunities + 1;


            _DbContext.TrainingOpportunities.Update(req.trainingOpportunity);


            _DbContext.SaveChanges();



            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult Approval(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }


            var req = _DbContext.StudentRequestsOnOpportunities.Where(item => item.StudentRequestOpportunityId == id).Include(item => item.trainingOpportunity).AsNoTracking().FirstOrDefault();

            if (req == null)
            {
                return Json(new { success = false });
            }

            if (req.DecisionStatus != "waitingStudentApprove")
            {
                TempData["error"] = "لايمكن الغاء الطلب";

                return Json(new { success = false });

            }


            req.DecisionDate = DateTime.Now.Date;
            req.DecisionStatus = "approved";

            _DbContext.StudentRequestsOnOpportunities.Update(req);


            _DbContext.SaveChanges();




            ////----------------------------------------------------------------------

            //_DbContext.Database.ExecuteSqlRaw("UPDATE StudentRequestsOnOpportunities SET DecisionDate = GETDATE() , DecisionStatus = 'system disable' " +
            //    $"WHERE Trainee_TraineeId = {req.Trainee_TraineeId} AND StudentRequestOpportunityId != {req.StudentRequestOpportunityId} AND DecisionStatus = 'waitingStudentApprove' OR DecisionStatus = 'waiting'");

            ////----------------------------------------------------------------------

            req.trainingOpportunity.ApprovedOpportunities = req.trainingOpportunity.ApprovedOpportunities + 1;

            //req.trainingOpportunity.RequestedOpportunities = req.trainingOpportunity.RequestedOpportunities - 1;



            _DbContext.TrainingOpportunities.Update(req.trainingOpportunity);

            ///// لكي تعدل الفرص الاخرى التي نم ايقافها على الطالب وتعديل رقم المتقدمين عليها
            var RequestList = _DbContext.StudentRequestsOnOpportunities.Where(item => item.Trainee_TraineeId == req.Trainee_TraineeId).Include(item => item.trainingOpportunity).AsNoTracking().ToList();

            foreach (var i in RequestList)
            {

                if (i.DecisionStatus == "waiting" || i.DecisionStatus == "waitingStudentApprove")
                {

                    if (i.DecisionStatus == "waitingStudentApprove")

                    {
                        i.trainingOpportunity.AvailableOpportunities = i.trainingOpportunity.AvailableOpportunities + 1;
                    }


                    if (i.DecisionStatus == "waiting")

                    {
                        i.trainingOpportunity.RequestedOpportunities = i.trainingOpportunity.RequestedOpportunities - 1;
                    }


                    i.DecisionDate = DateTime.Now.Date;
                    i.DecisionStatus = "system disable";

                    _DbContext.StudentRequestsOnOpportunities.Update(i);
                    _DbContext.TrainingOpportunities.Update(i.trainingOpportunity);

                    _DbContext.SaveChanges();
                }





                //  _DbContext.Database.ExecuteSqlRaw("UPDATE TrainingOpportunities SET RequestedOpportunities =(SELECT COUNT(*) FROM UniversitiesTraineeStudents WHERE TraineeId IN " +
                //$"(SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities WHERE TrainingOpportunity_TrainingOpportunityId={i.TrainingOpportunity_TrainingOpportunityId} " +
                //$"AND DecisionStatus = 'waiting' OR DecisionStatus = 'waitingStudentApprove')) WHERE TrainingOpportunityId={i.TrainingOpportunity_TrainingOpportunityId}");


            }


            _DbContext.SaveChanges();



            return Json(new { success = true });
        }

        [HttpGet]
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


        #endregion







    }


}