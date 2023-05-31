using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TadarbProject.Data;
using TadarbProject.Models;
using TEST2.Services;

namespace TadarbProject.Controllers
{
    public class TrainingSupervisorController : Controller
    {
        private readonly AppDbContext _DbContext;
        private readonly IEmailSender _emailSender;
        private readonly IHttpContextAccessor _HttpContextAccessor;
        private readonly IWebHostEnvironment _WebHostEnvironment;
        private static string Name;
        private static int UserId;
        private static UserAcount User;
        private static Employee employee;
        private static Department department;
        private static Organization OrganizationOfR;


        public TrainingSupervisorController(AppDbContext DbContext, IEmailSender emailSender, IHttpContextAccessor HttpContextAccessor, IWebHostEnvironment webHostEnvironment)
        {
            _DbContext = DbContext;

            _emailSender = emailSender;
            _HttpContextAccessor = HttpContextAccessor;
            _WebHostEnvironment = webHostEnvironment;

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

            User = _DbContext.UserAcounts.Where(item => item.UserId == UserId).AsNoTracking().FirstOrDefault();

            if (User.ActivationStatus == "Not_Active")
            {


                TempData["error"] = "حالة الحساب غير نشط " +
                    "الرجاء تعديل كلمة المرور  ";
                return RedirectToAction("EditAccount");

            }

            employee = _DbContext.Employees.Where(item => item.UserAccount_UserId == UserId).Include(item => item.userAcount).
                 Include(item => item.department).AsNoTracking().FirstOrDefault();

            department = _DbContext.Departments.Where(item => item.DepartmentId == employee.Department_DepartmentId).Include(item => item.OrganizationBranch).AsNoTracking().FirstOrDefault();


            OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == department.Organization_OrganizationId).AsNoTracking().FirstOrDefault();


            ViewBag.OrganizationName = OrganizationOfR.OrganizationName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = User.FullName;

            ViewBag.Department = department.OrganizationBranch.BranchName;

            return View(employee);

        }

        [HttpGet]
        public IActionResult EditAccount()
        {

            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(UserId.ToString()))
            {

                return RedirectToAction("Login", "Home");

            }

            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");
            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();


            ViewBag.Username = user.FullName;


            return View();
        }


        [HttpPost]
        public IActionResult EditAccount(UserAcount UserAcount)
        {


            ViewBag.Name = Name;

            int RUserId = UserId;
            var user = User;


            user.UserPassword = UserAcount.UserPassword;

            user.ActivationStatus = "Active";

            _DbContext.UserAcounts.Update(user);


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

            var Emplyee = employee;

            var Department = department;


            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Department.OrganizationBranch.BranchName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;

            var Opportunities = _DbContext.TrainingOpportunities.Where(item => item.SupervisorEmployeeId == Emplyee.EmployeeId).Include(item => item.DetailFiled).AsNoTracking().ToList();

            List<int> StudentNumber = new List<int>();

            foreach (var item in Opportunities)
            {

                int count = _DbContext.SemestersStudentAndEvaluationDetails.FromSqlRaw($"SELECT * FROM SemestersStudentAndEvaluationDetails WHERE TrainingSupervisor_EmployeeId = {Emplyee.EmployeeId} AND StudentRequest_StudentRequestId IN " +
                   $"(SELECT StudentRequestOpportunityId FROM StudentRequestsOnOpportunities WHERE TrainingOpportunity_TrainingOpportunityId = {item.TrainingOpportunityId} AND DecisionStatus ='approved')").AsNoTracking().ToList().Count();

                StudentNumber.Add(count);
            }



            ViewBag.StudentNumber = StudentNumber;

            return View(Opportunities);

        }
        [HttpGet]
        public IActionResult ViewOpportunitiesByUni(int? id)
        {
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(UserId.ToString()))
            {

                return RedirectToAction("Login", "Home");

            }

            ViewBag.Name = Name;

            int RUserId = UserId;
            var user = User;

            var Emplyee = employee;

            var Department = department;


            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Department.OrganizationBranch.BranchName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;




            //var SemesterMaster = _DbContext.SemestersTrainingSettingMaster.FromSqlRaw($"Select * from SemestersTrainingSettingMaster where SemesterTrainingSettingMasterId IN " +
            //    $"(Select SemesterMaster_SemesterMasterId  From SemestersStudentAndEvaluationDetails where TrainingSupervisor_EmployeeId = {Emplyee.EmployeeId} ) AND ActivationStatus = 'Active' ").Include(item => item.departmet.universityCollege.organization)
            //    .AsNoTracking().ToList();




            var SemesterMaster = _DbContext.SemestersTrainingSettingMaster.FromSqlRaw($"SELECT * FROM SemestersTrainingSettingMaster WHERE SemesterTrainingSettingMasterId IN " +
                $"(SELECT SemesterMaster_SemesterMasterId  FROM SemestersStudentAndEvaluationDetails WHERE TrainingSupervisor_EmployeeId = {employee.EmployeeId} AND StudentRequest_StudentRequestId IN " +
                $"(SELECT StudentRequestOpportunityId FROM StudentRequestsOnOpportunities WHERE TrainingOpportunity_TrainingOpportunityId = {id} AND DecisionStatus ='approved')) " +
                $"AND ActivationStatus = 'Active'").Include(item => item.departmet.universityCollege.organization)
                .AsNoTracking().ToList();


            ViewBag.OpportunityId = id;

            return View(SemesterMaster);

        }

        [HttpGet]
        public IActionResult OpportunitiesApplicants(int? id, int? Oppid, string EvaluationFile)
        {
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(UserId.ToString()))
            {

                return RedirectToAction("Login", "Home");

            }

            if (id == null || id == 0)
            {
                return NotFound();
            }

            ViewBag.Name = Name;

            int RUserId = UserId;
            var user = User;

            var Emplyee = employee;

            var Department = department;


            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Department.OrganizationBranch.BranchName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;

            ViewBag.MasterId = id;

            ViewBag.OpportunityId = Oppid;

            ViewBag.EvaluationFile = EvaluationFile;

            return View();
        }

        [HttpGet]
        public IActionResult TraineeEvaluation(int? id)
        {

            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(UserId.ToString()))
            {

                return RedirectToAction("Login", "Home");

            }

            if (id == null || id == 0)
            {
                return NotFound();
            }

            ViewBag.Name = Name;

            int RUserId = UserId;
            var user = User;

            var Emplyee = employee;

            var Department = department;


            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Department.OrganizationBranch.BranchName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;


            var TraineeRqu = _DbContext.StudentRequestsOnOpportunities.Where(item => item.Trainee_TraineeId == id && item.DecisionStatus == "approved").Include(item => item.student).AsNoTracking().FirstOrDefault();

            var StudentAndEvaluationDetails = _DbContext.SemestersStudentAndEvaluationDetails.Where(item => item.StudentRequest_StudentRequestId == TraineeRqu.StudentRequestOpportunityId)
                .AsNoTracking().FirstOrDefault();

            var SetMaxMark = _DbContext.DepartmentsAssessmentTypeMaster.Where(item => item.Department_DepartmentId == TraineeRqu.student.Department_DepartmentId)
                .AsNoTracking().FirstOrDefault().TrainingSupervisorMarks;

            ViewBag.MaxMark = SetMaxMark;



            return View(StudentAndEvaluationDetails);

        }


        [HttpPost]
        public IActionResult TraineeEvaluation(SemesterStudentAndEvaluationDetail opj, IFormFile? EvaluationFile)
        {

            if (opj == null)
            {
                return NotFound();
            }

            if (EvaluationFile != null)
            {
                //Startt UploadFile

                string fileName = Guid.NewGuid().ToString();

                string wwwRootPath = _WebHostEnvironment.WebRootPath;

                var uploadTo = Path.Combine(wwwRootPath, @"SupervisionFiles\TrainingSupervisorEvaluationFiles\");

                var extension = Path.GetExtension(EvaluationFile.FileName);



                if (opj.TrainingSupervisorEvaluationFilePath != null)
                {
                    var OldCVPath = Path.Combine(wwwRootPath, opj.TrainingSupervisorEvaluationFilePath.TrimStart('\\'));

                    if (System.IO.File.Exists(OldCVPath))
                    {
                        System.IO.File.Delete(OldCVPath);
                    }

                }

                //here is to Create a file to has the user uploaded file
                using (var fileStreams = new FileStream(Path.Combine(uploadTo, fileName + extension), FileMode.Create))
                {
                    EvaluationFile.CopyTo(fileStreams);
                }

                //Here what will be in the database.
                opj.TrainingSupervisorEvaluationFilePath = @"SupervisionFiles\TrainingSupervisorEvaluationFiles\" + fileName + extension;

                //___________________________end upload____________



            }




            _DbContext.SemestersStudentAndEvaluationDetails.Update(opj);

            _DbContext.SaveChanges();



            return RedirectToAction("ViewOpportunities");

        }



        #region




        [HttpGet]
        public IActionResult GetStudentsList(int? id, int? OppId, string? gr, int? UPOrDw, int? ArOrWa)
        {


            IEnumerable<UniversityTraineeStudent> Students = Enumerable.Empty<UniversityTraineeStudent>();

            if (ArOrWa == 0)
            {
                if (gr == null && UPOrDw == 0)
                {

                    Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"Select * from UniversitiesTraineeStudents Where TraineeId IN " +
                        $" (Select Trainee_TraineeId from StudentRequestsOnOpportunities where StudentRequestOpportunityId IN " +
                        $" (Select StudentRequest_StudentRequestId from SemestersStudentAndEvaluationDetails where SemesterMaster_SemesterMasterId = {id}) AND TrainingOpportunity_TrainingOpportunityId = {OppId})")
                      .AsNoTracking().Include(item => item.user)
                      .AsNoTracking().Include(item => item.department.universityCollege.organization)
                      .AsNoTracking().Include(item => item.department.universityCollege.city.Country).ToList();

                    return Json(new { Students });
                }

                if (gr != null && UPOrDw == 0)
                {

                    Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"Select * from UniversitiesTraineeStudents Where TraineeId IN " +
                       $" (Select Trainee_TraineeId from StudentRequestsOnOpportunities where StudentRequestOpportunityId IN " +
                       $" (Select StudentRequest_StudentRequestId from SemestersStudentAndEvaluationDetails where SemesterMaster_SemesterMasterId = {id}) AND TrainingOpportunity_TrainingOpportunityId = {OppId}) AND Gender = '{gr}' ")
                     .AsNoTracking().Include(item => item.user)
                      .AsNoTracking().Include(item => item.department.universityCollege.organization)
                      .AsNoTracking().Include(item => item.department.universityCollege.city.Country).ToList();

                    return Json(new { Students });
                }



                if (gr != null && UPOrDw == 1)
                {

                    Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"Select * from UniversitiesTraineeStudents Where TraineeId IN " +
                       $" (Select Trainee_TraineeId from StudentRequestsOnOpportunities where StudentRequestOpportunityId IN " +
                       $" (Select StudentRequest_StudentRequestId from SemestersStudentAndEvaluationDetails where SemesterMaster_SemesterMasterId = {id}) AND TrainingOpportunity_TrainingOpportunityId = {OppId}) AND Gender = '{gr}' ")
                      .AsNoTracking().Include(item => item.user)
                      .AsNoTracking().Include(item => item.department.universityCollege.organization)
                      .AsNoTracking().Include(item => item.department.universityCollege.city.Country).OrderBy(item => item.GPA).ToList();

                    return Json(new { Students });
                }

                if (gr != null && UPOrDw == 2)
                {

                    Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"Select * from UniversitiesTraineeStudents Where TraineeId IN " +
                       $" (Select Trainee_TraineeId from StudentRequestsOnOpportunities where StudentRequestOpportunityId IN " +
                       $" (Select StudentRequest_StudentRequestId from SemestersStudentAndEvaluationDetails where SemesterMaster_SemesterMasterId = {id}) AND TrainingOpportunity_TrainingOpportunityId =  {OppId} ) AND Gender = '{gr}' ")
                      .AsNoTracking().Include(item => item.user)
                      .AsNoTracking().Include(item => item.department.universityCollege.organization)
                      .AsNoTracking().Include(item => item.department.universityCollege.city.Country).OrderByDescending(item => item.GPA).ToList();

                    return Json(new { Students });
                }


                if (gr == null && UPOrDw == 1)
                {

                    Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"Select * from UniversitiesTraineeStudents Where TraineeId IN " +
                        $" (Select Trainee_TraineeId from StudentRequestsOnOpportunities where StudentRequestOpportunityId IN " +
                        $" (Select StudentRequest_StudentRequestId from SemestersStudentAndEvaluationDetails where SemesterMaster_SemesterMasterId = {id}) AND TrainingOpportunity_TrainingOpportunityId = {OppId})")
                      .AsNoTracking().Include(item => item.user)
                      .AsNoTracking().Include(item => item.department.universityCollege.organization)
                      .AsNoTracking().Include(item => item.department.universityCollege.city.Country).OrderBy(item => item.GPA).ToList();

                    return Json(new { Students });
                }

                if (gr == null && UPOrDw == 2)
                {

                    Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"Select * from UniversitiesTraineeStudents Where TraineeId IN " +
                        $" (Select Trainee_TraineeId from StudentRequestsOnOpportunities where StudentRequestOpportunityId IN " +
                        $" (Select StudentRequest_StudentRequestId from SemestersStudentAndEvaluationDetails where SemesterMaster_SemesterMasterId = {id}) AND TrainingOpportunity_TrainingOpportunityId = {OppId})")
                      .AsNoTracking().Include(item => item.user)
                      .AsNoTracking().Include(item => item.department.universityCollege.organization)
                      .AsNoTracking().Include(item => item.department.universityCollege.city.Country).OrderByDescending(item => item.GPA).ToList();

                    return Json(new { Students });
                }

            }



            if (ArOrWa == 1)
            {
                if (gr == null && UPOrDw == 0)
                {

                    Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"Select * from UniversitiesTraineeStudents Where TraineeId IN " +
                       $" (Select Trainee_TraineeId from StudentRequestsOnOpportunities where StudentRequestOpportunityId IN " +
                       $" (Select StudentRequest_StudentRequestId from SemestersStudentAndEvaluationDetails where SemesterMaster_SemesterMasterId = {id}) AND TrainingOpportunity_TrainingOpportunityId = {OppId})")
                     .AsNoTracking().Include(item => item.user)
                      .AsNoTracking().Include(item => item.department.universityCollege.organization)
                      .AsNoTracking().Include(item => item.department.universityCollege.city.Country).ToList();

                    return Json(new { Students });
                }

                if (gr != null && UPOrDw == 0)
                {

                    Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"Select * from UniversitiesTraineeStudents Where TraineeId IN " +
                        $" (Select Trainee_TraineeId from StudentRequestsOnOpportunities where StudentRequestOpportunityId IN " +
                        $" (Select StudentRequest_StudentRequestId from SemestersStudentAndEvaluationDetails where SemesterMaster_SemesterMasterId = {id}) AND TrainingOpportunity_TrainingOpportunityId = {OppId}) AND Gender = '{gr}'")
                       .AsNoTracking().Include(item => item.user)
                      .AsNoTracking().Include(item => item.department.universityCollege.organization)
                      .AsNoTracking().Include(item => item.department.universityCollege.city.Country).ToList();

                    return Json(new { Students });
                }



                if (gr != null && UPOrDw == 1)
                {

                    Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"Select * from UniversitiesTraineeStudents Where TraineeId IN " +
                       $" (Select Trainee_TraineeId from StudentRequestsOnOpportunities where StudentRequestOpportunityId IN " +
                       $" (Select StudentRequest_StudentRequestId from SemestersStudentAndEvaluationDetails where SemesterMaster_SemesterMasterId = {id}) AND TrainingOpportunity_TrainingOpportunityId = {OppId}) AND Gender = '{gr}'")
                      .AsNoTracking().Include(item => item.user)
                      .AsNoTracking().Include(item => item.department.universityCollege.organization)
                      .AsNoTracking().Include(item => item.department.universityCollege.city.Country).OrderBy(item => item.GPA).ToList();

                    return Json(new { Students });
                }

                if (gr != null && UPOrDw == 2)
                {

                    Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"Select * from UniversitiesTraineeStudents Where TraineeId IN " +
                       $" (Select Trainee_TraineeId from StudentRequestsOnOpportunities where StudentRequestOpportunityId IN " +
                       $" (Select StudentRequest_StudentRequestId from SemestersStudentAndEvaluationDetails where SemesterMaster_SemesterMasterId = {id}) AND TrainingOpportunity_TrainingOpportunityId = {OppId}) AND Gender = '{gr}' ")
                      .AsNoTracking().Include(item => item.user)
                      .AsNoTracking().Include(item => item.department.universityCollege.organization)
                      .AsNoTracking().Include(item => item.department.universityCollege.city.Country).OrderByDescending(item => item.GPA).ToList();

                    return Json(new { Students });
                }


                if (gr == null && UPOrDw == 1)
                {

                    Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"Select * from UniversitiesTraineeStudents Where TraineeId IN " +
                        $" (Select Trainee_TraineeId from StudentRequestsOnOpportunities where StudentRequestOpportunityId IN " +
                        $" (Select StudentRequest_StudentRequestId from SemestersStudentAndEvaluationDetails where SemesterMaster_SemesterMasterId = {id}) AND TrainingOpportunity_TrainingOpportunityId = {OppId})")
                       .AsNoTracking().Include(item => item.user)
                      .AsNoTracking().Include(item => item.department.universityCollege.organization)
                      .AsNoTracking().Include(item => item.department.universityCollege.city.Country).OrderBy(item => item.GPA).ToList();

                    return Json(new { Students });
                }

                if (gr == null && UPOrDw == 2)
                {

                    Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"Select * from UniversitiesTraineeStudents Where TraineeId IN " +
                      $" (Select Trainee_TraineeId from StudentRequestsOnOpportunities where StudentRequestOpportunityId IN " +
                      $" (Select StudentRequest_StudentRequestId from SemestersStudentAndEvaluationDetails where SemesterMaster_SemesterMasterId = {id}) AND TrainingOpportunity_TrainingOpportunityId = {OppId})")
                     .AsNoTracking().Include(item => item.user)
                      .AsNoTracking().Include(item => item.department.universityCollege.organization)
                      .AsNoTracking().Include(item => item.department.universityCollege.city.Country).OrderByDescending(item => item.GPA).ToList();

                    return Json(new { Students });
                }


            }


            return Json(new { Students });
        }

        [HttpGet]
        public IActionResult GetStudentsListByFilter(int? id, int? OppId, string? filter)
        {


            IEnumerable<UniversityTraineeStudent> Students = Enumerable.Empty<UniversityTraineeStudent>();



            Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"Select * from UniversitiesTraineeStudents Where TraineeId IN " +
                  $" (Select Trainee_TraineeId from StudentRequestsOnOpportunities where StudentRequestOpportunityId IN " +
                  $" (Select StudentRequest_StudentRequestId from SemestersStudentAndEvaluationDetails where SemesterMaster_SemesterMasterId = {id}) AND TrainingOpportunity_TrainingOpportunityId={OppId})" +
                  $" AND UserAccount_UserId IN (SELECT UserId FROM UserAcounts WHERE FullName = '{filter}')")
            .AsNoTracking().Include(item => item.user)
            .AsNoTracking().Include(item => item.department.universityCollege.organization)
            .AsNoTracking().Include(item => item.department.universityCollege.city.Country).ToList();


            if (Students.Count() != 0)
            {
                return Json(new { Students });
            }


            Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"Select * from UniversitiesTraineeStudents Where TraineeId IN " +
                  $" (Select Trainee_TraineeId from StudentRequestsOnOpportunities where StudentRequestOpportunityId IN " +
                  $" (Select StudentRequest_StudentRequestId from SemestersStudentAndEvaluationDetails where SemesterMaster_SemesterMasterId = {id}) AND TrainingOpportunity_TrainingOpportunityId={OppId} ) " +
                  $" AND Department_DepartmentId IN (SELECT Department_DepartmentId FROM Departments WHERE Organization_OrganizationId IN " +
                  $"( SELECT OrganizationId FROM Organizations WHERE OrganizationName = '{filter}'))")
                     .AsNoTracking().Include(item => item.user)
                     .AsNoTracking().Include(item => item.department.universityCollege.organization)
                     .AsNoTracking().Include(item => item.department.universityCollege.city.Country).ToList();

            return Json(new { Students });


        }

        #endregion

    }
}
