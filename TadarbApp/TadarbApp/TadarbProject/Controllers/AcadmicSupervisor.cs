using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TadarbProject.Data;
using TadarbProject.Models;
using TEST2.Services;

namespace TadarbProject.Controllers
{
    public class AcadmicSupervisor : Controller
    {
        private readonly AppDbContext _DbContext;
        private readonly IEmailSender _emailSender;
        private readonly IHttpContextAccessor _HttpContextAccessor;
        private static string Name;
        private static int UserId;
        private static UserAcount User;
        private static Employee employee;
        private static Department department;
        private static Organization OrganizationOfR;


        public AcadmicSupervisor(AppDbContext DbContext, IEmailSender emailSender, IHttpContextAccessor HttpContextAccessor)
        {
            _DbContext = DbContext;

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

            User = _DbContext.UserAcounts.Where(item => item.UserId == UserId).AsNoTracking().FirstOrDefault();

            employee = _DbContext.Employees.Where(item => item.UserAccount_UserId == UserId).Include(item => item.userAcount).Include(item => item.department).AsNoTracking().FirstOrDefault();

            department = _DbContext.Departments.Where(item => item.DepartmentId == employee.Department_DepartmentId).AsNoTracking().FirstOrDefault();


            OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == department.Organization_OrganizationId).AsNoTracking().FirstOrDefault();
            if (User.ActivationStatus == "Not_Active")
            {


                TempData["error"] = "حالة الحساب غير نشط " +
                    "الرجاء تعديل كلمة المرور  ";
                return RedirectToAction("EditAccount");

            }


            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + department.DepartmentName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = User.FullName;

            return View(employee);
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
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();

            ViewBag.Username = user.FullName;


            return View();
        }
        [HttpPost]
        public IActionResult EditAccount(UserAcount UserAcount)
        {
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(UserId.ToString()))
            {

                return RedirectToAction("Login", "Home");

            }

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
        public IActionResult AssignedStudents(int? Mid)
        {
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(UserId.ToString()))
            {

                return RedirectToAction("Login", "Home");

            }

            if (Mid == 0 || Mid == null)
            {
                return NotFound();
            }

            ViewBag.Name = Name;

            int RUserId = UserId;

            var user = User;

            var Employee = employee;

            var Department = department;


            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Department.DepartmentName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;


            IEnumerable<StudentRequestOpportunity> students = Enumerable.Empty<StudentRequestOpportunity>();


            students = _DbContext.StudentRequestsOnOpportunities.FromSqlRaw("SELECT * FROM StudentRequestsOnOpportunities WHERE  StudentRequestOpportunityId IN " +
                "(SELECT StudentRequest_StudentRequestId FROM SemestersStudentAndEvaluationDetails WHERE AcademicSupervisor_EmployeeId = " +
                $"(SELECT EmployeeId FROM  Employees WHERE UserAccount_UserId ={RUserId}) AND SemesterMaster_SemesterMasterId ={Mid} AND GeneralTrainingStatus ='Company Approved')")
                .Include(item => item.student.user).Include(item => item.trainingOpportunity.Branch.organization).AsNoTracking().ToList();


            ViewBag.SemesterMasterId = Mid;

            return View(students);

        }


        [HttpGet]
        public IActionResult AssignSemester()
        {
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(UserId.ToString()))
            {

                return RedirectToAction("Login", "Home");

            }



            ViewBag.Name = Name;

            int RUserId = UserId;

            var user = User;

            var Employee = employee;

            var Department = department;

            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Department.DepartmentName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;

            var semester = _DbContext.SemestersStudentAndEvaluationDetails.Where(item => item.AcademicSupervisor_EmployeeId == Employee.EmployeeId).AsNoTracking().FirstOrDefault();


            if (semester == null)
            {
                TempData["error"] = "لا توجد لديك شعبة مسجلة الى الان ..";

                return RedirectToAction("Index", "AcadmicSupervisor");

            }


            IEnumerable<SemesterTrainingSettingMaster> Semasters = Enumerable.Empty<SemesterTrainingSettingMaster>();


            Semasters = _DbContext.SemestersTrainingSettingMaster.FromSqlRaw("SELECT * FROM SemestersTrainingSettingMaster WHERE SemesterTrainingSettingMasterId IN " +
                "(SELECT SemesterMaster_SemesterMasterId FROM SemestersStudentAndEvaluationDetails WHERE AcademicSupervisor_EmployeeId = " +
                $"(SELECT EmployeeId FROM Employees WHERE UserAccount_UserId ={RUserId}))").AsNoTracking().ToList();

            return View(Semasters);

        }


        [HttpGet]
        public IActionResult AssignAssignments(int? Mid)
        {
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(UserId.ToString()))
            {

                return RedirectToAction("Login", "Home");

            }

            if (Mid == 0 || Mid == null)
            {
                return NotFound();
            }

            ViewBag.Name = Name;

            int RUserId = UserId;

            var user = User;

            var Employee = employee;

            var Department = department;

            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Department.DepartmentName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;

            IEnumerable<DepartmentAssessmentTypeDetail> DepartmentsAssessmentTypes = Enumerable.Empty<DepartmentAssessmentTypeDetail>();


            DepartmentsAssessmentTypes = _DbContext.DepartmentsAssessmentTypeDetail.FromSqlRaw("SELECT * FROM DepartmentsAssessmentTypeDetail WHERE DepartmentAssessmentTypeMaster_MasterId = " +
                 $"(SELECT DepartmentAssessmentTypeMasterId FROM DepartmentsAssessmentTypeMaster WHERE Department_DepartmentId = {Department.DepartmentId}) AND DepartmentAssessmentTypeDetailId  NOT IN " +
                 $"(SELECT DepartmentAssessmentTypeDetail_DetailId FROM StudentSemesterEvaluationMarks WHERE SemesterStudentAndEvaluationDetail_DetailId IN " +
                 $"(SELECT SemesterStudentAndEvaluationDetailId FROM SemestersStudentAndEvaluationDetails WHERE AcademicSupervisor_EmployeeId = {Employee.EmployeeId} ))")
                .Include(item => item.assessmentType).AsNoTracking().ToList();

            ViewBag.DepartmentsAssessmentTypesList = DepartmentsAssessmentTypes.Select(item => new SelectListItem
            {
                Value = item.DepartmentAssessmentTypeDetailId.ToString(),
                Text = item.assessmentType.AssessmentTypeName
            });

            ViewBag.DepartmentId = Department.DepartmentId;


            ViewBag.SemesterMasterId = Mid;

            return View();

        }



        [HttpGet]
        public IActionResult AssignedStudentAssignments(int? StuRqId, int? SSEMId)
        {
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(UserId.ToString()))
            {

                return RedirectToAction("Login", "Home");

            }

            if (StuRqId == 0 || StuRqId == null)
            {
                return NotFound();
            }

            ViewBag.Name = Name;

            int RUserId = UserId;

            var user = User;

            var Employee = employee;

            var Department = department;

            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Department.DepartmentName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;
            IEnumerable<StudentSemesterEvaluationMark> Assignments = Enumerable.Empty<StudentSemesterEvaluationMark>();


            Assignments = _DbContext.StudentSemesterEvaluationMarks.FromSqlRaw("SELECT * FROM StudentSemesterEvaluationMarks WHERE SemesterStudentAndEvaluationDetail_DetailId IN" +
                $"(SELECT SemesterStudentAndEvaluationDetailId FROM SemestersStudentAndEvaluationDetails WHERE StudentRequest_StudentRequestId = {StuRqId})")
                .Include(item => item.assessmentTypeDetail.assessmentType).AsNoTracking().ToList();


            ViewBag.CurrentStuRqId = StuRqId;
            ViewBag.SemesterMasterId = SSEMId;

            return View(Assignments);

        }


        [HttpGet]
        public IActionResult EvaluateStudentAssignment(int? SSEMId, int? StuRqId)
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

            var Employee = employee;

            var Department = department;

            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Department.DepartmentName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;



            var Assignment = _DbContext.StudentSemesterEvaluationMarks.Where(item => item.StudentSemesterEvaluationMarkId == SSEMId)
                .Include(item => item.assessmentTypeDetail.assessmentType).AsNoTracking().FirstOrDefault();


            ViewBag.CurrentStuRqId = StuRqId;
            ViewBag.SemesterMasterId = SSEMId;

            return View(Assignment);

        }


        [HttpPost]
        public IActionResult EvaluateStudentAssignment(StudentSemesterEvaluationMark Assignment, int? StuRqId ,int? SSEMId)
        {
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(UserId.ToString()))
            {

                return RedirectToAction("Login", "Home");

            }

            if (Assignment == null)
            {
                return NotFound();
            }


            ViewBag.Name = Name;

            int RUserId = UserId;

            var user = User;

            var Employee = employee;

            var Department = department;

            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Department.DepartmentName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;


            if (Assignment.StudentMark == null)
            {
                return RedirectToAction("AssignedStudentAssignments", new { StuRqId = StuRqId, SSEMId = SSEMId });
            }

            var StudentAndEvaluationDetails = _DbContext.SemestersStudentAndEvaluationDetails.Where(item => item.SemesterStudentAndEvaluationDetailId == Assignment.SemesterStudentAndEvaluationDetail_DetailId)
                .AsNoTracking().FirstOrDefault();


            if (StudentAndEvaluationDetails.AcademicSupervisorEvaluationMark == null)
            {
                StudentAndEvaluationDetails.AcademicSupervisorEvaluationMark = 0;
            }

            var oldAssignment = _DbContext.StudentSemesterEvaluationMarks.Where(item => item.StudentSemesterEvaluationMarkId == Assignment.StudentSemesterEvaluationMarkId).AsNoTracking().FirstOrDefault();

            if (oldAssignment.StudentMark != null)
            {
                StudentAndEvaluationDetails.AcademicSupervisorEvaluationMark -= oldAssignment.StudentMark;
            }

            StudentAndEvaluationDetails.AcademicSupervisorEvaluationMark += Assignment.StudentMark;

            _DbContext.StudentSemesterEvaluationMarks.Update(Assignment);
            _DbContext.SemestersStudentAndEvaluationDetails.Update(StudentAndEvaluationDetails);
            _DbContext.SaveChanges();


            TempData["success"] = "تم تقيم التقرير بنجاح.";

            return RedirectToAction("AssignedStudentAssignments", new { StuRqId = StuRqId, SSEMId = SSEMId });

        }



        [HttpGet]
        public IActionResult FollowUpDetails(int? SSEMId, int? StuRqId)
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

            var Employee = employee;

            var Department = department;

            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Department.DepartmentName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;


            var SemestersStudentAndEvaluationDetails = _DbContext.SemestersStudentAndEvaluationDetails.Where(item => item.StudentRequest_StudentRequestId == StuRqId && item.SemesterMaster_SemesterMasterId == SSEMId
            && item.AcademicSupervisor_EmployeeId == Employee.EmployeeId).Include(item => item.EmployeeTrainingSupervisor.userAcount).AsNoTracking().FirstOrDefault();


            ViewBag.SSEMId = SSEMId;

            ViewBag.StuRqId = StuRqId;

            return View(SemestersStudentAndEvaluationDetails);

        }






        #region

        [HttpGet]
        public IActionResult EndStudentTraining(int? SSEMId, int? StuRqId)
        {

            if (SSEMId == null || StuRqId == null || SSEMId == 0 || StuRqId == 0)
            {
                NotFound();
            }

            var StudentAndEvaluationDetail = _DbContext.SemestersStudentAndEvaluationDetails.Where(item => item.StudentRequest_StudentRequestId == StuRqId
            && item.SemesterMaster_SemesterMasterId == SSEMId).Include(item => item.studentRequest.student).Include(item => item.studentRequest.trainingOpportunity).AsNoTracking().FirstOrDefault();

            if (StudentAndEvaluationDetail == null)
            {
                return Json(new { success = false });
            }

            if (StudentAndEvaluationDetail.AcademicSupervisorEvaluationMark == null || StudentAndEvaluationDetail.TrainingSupervisorEvaluationMark == null)
            {
                return Json(new { success = false });

            }

            StudentAndEvaluationDetail.GeneralTrainingStatus = "stop training";
            StudentAndEvaluationDetail.studentRequest.DecisionStatus = "stop training";
            StudentAndEvaluationDetail.studentRequest.DecisionDate = DateTime.Now.Date;
            StudentAndEvaluationDetail.studentRequest.student.ActivationStatus = "Not_Active";

            StudentAndEvaluationDetail.studentRequest.trainingOpportunity.ApprovedOpportunities = StudentAndEvaluationDetail.studentRequest.trainingOpportunity.ApprovedOpportunities - 1;

            _DbContext.SemestersStudentAndEvaluationDetails.Update(StudentAndEvaluationDetail);

            _DbContext.StudentRequestsOnOpportunities.Update(StudentAndEvaluationDetail.studentRequest);

            _DbContext.UniversitiesTraineeStudents.Update(StudentAndEvaluationDetail.studentRequest.student);

            _DbContext.TrainingOpportunities.Update(StudentAndEvaluationDetail.studentRequest.trainingOpportunity);

            _DbContext.SaveChanges();

            TempData["success"] = "تم تأكيد انهاء تدريب الطالب بنجاح.";

            return Json(new { success = true });
        }


        [HttpPost]
        public IActionResult AddStudentSemesterEvaluationMark(int? Mid, int? DASid)
        {

            if (DASid == 0 || DASid == null || Mid == 0 || Mid == null)
            {
                return Json(new { success = false });
            }



            int RUserId = UserId;

            var user = User;

            var Employee = employee;

            var Department = department;


            //IEnumerable<DepartmentAssessmentTypeDetail> DepartmentsAssessmentTypes = Enumerable.Empty<DepartmentAssessmentTypeDetail>();

            var SemestersStudentAndEvaluationDetailsList = _DbContext.SemestersStudentAndEvaluationDetails
                .Where(item => item.SemesterMaster_SemesterMasterId == Mid && item.AcademicSupervisor_EmployeeId == Employee.EmployeeId).AsNoTracking().ToList();

            if (SemestersStudentAndEvaluationDetailsList == null)
            {
                return Json(new { success = false });

            }


            foreach (var i in SemestersStudentAndEvaluationDetailsList)

            {
                var NewStudentSemesterEvaluationMark = new StudentSemesterEvaluationMark()
                {

                    SemesterStudentAndEvaluationDetail_DetailId = i.SemesterStudentAndEvaluationDetailId,
                    DepartmentAssessmentTypeDetail_DetailId = (int)DASid

                };


                _DbContext.StudentSemesterEvaluationMarks.Add(NewStudentSemesterEvaluationMark);

            }

            _DbContext.SaveChanges();

            TempData["success"] = "تم تعين التقارير بنجاح.";

            return Json(new { success = true });

        }

        [HttpGet]
        public IActionResult GetOldAssignAssignments(int? Did)
        {

            if (Did == 0 || Did == null)
            {
                return NotFound();
            }



            IEnumerable<DepartmentAssessmentTypeDetail> DepartmentsAssessmentTypes = Enumerable.Empty<DepartmentAssessmentTypeDetail>();

            int RUserId = UserId;

            var user = User;

            var Employee = employee;

            var Department = department;

            DepartmentsAssessmentTypes = _DbContext.DepartmentsAssessmentTypeDetail.FromSqlRaw("SELECT * FROM DepartmentsAssessmentTypeDetail WHERE DepartmentAssessmentTypeMaster_MasterId = " +
                 $"(SELECT DepartmentAssessmentTypeMasterId FROM DepartmentsAssessmentTypeMaster WHERE Department_DepartmentId = {Did}) " +
                 $"AND DepartmentAssessmentTypeDetailId IN (SELECT  DepartmentAssessmentTypeDetail_DetailId FROM StudentSemesterEvaluationMarks WHERE SemesterStudentAndEvaluationDetail_DetailId IN " +
                 $"(SELECT SemesterStudentAndEvaluationDetailId FROM SemestersStudentAndEvaluationDetails WHERE AcademicSupervisor_EmployeeId = {Employee.EmployeeId} ))")
                .Include(item => item.assessmentType).AsNoTracking().ToList();



            return Json(new { DepartmentsAssessmentTypes });

        }

        #endregion

    }
}
