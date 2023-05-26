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
            var UnverTre = _DbContext.UniversitiesTraineeStudents.Where(item => item.UserAccount_UserId == RUserId).Include(item => item.user.City.Country).Include(item => item.department.User)
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
                $"(SELECT TrainArea_DetailFiledId FROM DepartmentTrainingAreas WHERE Department_DepartmenId ={Department.DepartmentId}) " +
                $" AND OpportunityStatus='Available' AND AbilityofSubmissionStatus='Available'").AsNoTracking().Include(item => item.DetailFiled)
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




        public IActionResult ViewAssignments()
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

            if (SSEMId == 0 || SSEMId == null)
            {
                return NotFound();
            }

            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");
            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();
            var UnverTre = _DbContext.UniversitiesTraineeStudents.Where(item => item.UserAccount_UserId == RUserId).AsNoTracking().FirstOrDefault();
            var Department = _DbContext.Departments.Where(item => item.DepartmentId == UnverTre.Department_DepartmentId).AsNoTracking().FirstOrDefault();
            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Department.Organization_OrganizationId).AsNoTracking().FirstOrDefault();


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
        public IActionResult SubmitAssignment(StudentSemesterEvaluationMark obj)
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



        #region
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
                    i.trainingOpportunity.AvailableOpportunities = i.trainingOpportunity.AvailableOpportunities + 1;

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


        #endregion







    }


}