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


        public AcadmicSupervisor(AppDbContext DbContext, IEmailSender emailSender, IHttpContextAccessor HttpContextAccessor)
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

            var Employee = _DbContext.Employees.Where(item => item.UserAccount_UserId == RUserId).Include(item => item.userAcount).Include(item => item.department).AsNoTracking().FirstOrDefault();

            var Department = _DbContext.Departments.Where(item => item.DepartmentId == Employee.Department_DepartmentId).AsNoTracking().FirstOrDefault();


            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Department.Organization_OrganizationId).AsNoTracking().FirstOrDefault();
            if (user.ActivationStatus == "Not_Active")
            {


                TempData["error"] = "حالة الحساب غير نشط " +
                    "الرجاء تعديل كلمة المرور  ";
                return RedirectToAction("EditAccount");

            }


            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Department.DepartmentName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;

            return View(Employee);
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
        public IActionResult EditAccount(UserAcount UserAcount)
        {


            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");

            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();


            user.UserPassword = UserAcount.UserPassword;

            user.ActivationStatus = "Active";

            _DbContext.UserAcounts.Update(user);


            _DbContext.SaveChanges();

            return RedirectToAction("Index");

        }



        [HttpGet]
        public IActionResult AssignedStudents(int? Mid)
        {
            if (Mid == 0 || Mid == null)
            {
                return NotFound();
            }

            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");

            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;

            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();

            var Employee = _DbContext.Employees.Where(item => item.UserAccount_UserId == RUserId).Include(item => item.userAcount).Include(item => item.department).AsNoTracking().FirstOrDefault();

            var Department = _DbContext.Departments.Where(item => item.DepartmentId == Employee.Department_DepartmentId).AsNoTracking().FirstOrDefault();


            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Department.Organization_OrganizationId).AsNoTracking().FirstOrDefault();

            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Department.DepartmentName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;


            IEnumerable<StudentRequestOpportunity> students = Enumerable.Empty<StudentRequestOpportunity>();


            students = _DbContext.StudentRequestsOnOpportunities.FromSqlRaw("SELECT * FROM StudentRequestsOnOpportunities WHERE  StudentRequestOpportunityId IN " +
                "(SELECT StudentRequest_StudentRequestId FROM SemestersStudentAndEvaluationDetails WHERE AcademicSupervisor_EmployeeId = " +
                $"(SELECT EmployeeId FROM  Employees WHERE UserAccount_UserId ={RUserId}) AND SemesterMaster_SemesterMasterId ={Mid})")
                .Include(item => item.student.user).Include(item => item.trainingOpportunity.Branch.organization).AsNoTracking().ToList();


            return View(students);

        }


        [HttpGet]
        public IActionResult AssignSemester()
        {
            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");

            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;

            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();

            var Employee = _DbContext.Employees.Where(item => item.UserAccount_UserId == RUserId).Include(item => item.userAcount).Include(item => item.department).AsNoTracking().FirstOrDefault();

            var Department = _DbContext.Departments.Where(item => item.DepartmentId == Employee.Department_DepartmentId).AsNoTracking().FirstOrDefault();


            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Department.Organization_OrganizationId).AsNoTracking().FirstOrDefault();

            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Department.DepartmentName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;


            IEnumerable<SemesterTrainingSettingMaster> Semasters = Enumerable.Empty<SemesterTrainingSettingMaster>();


            Semasters = _DbContext.SemestersTrainingSettingMaster.FromSqlRaw("SELECT * FROM SemestersTrainingSettingMaster WHERE SemesterTrainingSettingMasterId IN " +
                "(SELECT SemesterMaster_SemesterMasterId FROM SemestersStudentAndEvaluationDetails WHERE AcademicSupervisor_EmployeeId = " +
                $"(SELECT EmployeeId FROM Employees WHERE UserAccount_UserId ={RUserId}))").AsNoTracking().ToList();

            return View(Semasters);

        }


        [HttpGet]
        public IActionResult AssignAssignments(int? Mid)
        {

            if (Mid == 0 || Mid == null)
            {
                return NotFound();
            }

            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");

            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;

            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();

            var Employee = _DbContext.Employees.Where(item => item.UserAccount_UserId == RUserId).Include(item => item.userAcount).Include(item => item.department).AsNoTracking().FirstOrDefault();

            var Department = _DbContext.Departments.Where(item => item.DepartmentId == Employee.Department_DepartmentId).AsNoTracking().FirstOrDefault();


            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Department.Organization_OrganizationId).AsNoTracking().FirstOrDefault();

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
        public IActionResult AssignedStudentAssignments(int? StuRqId)
        {

            if (StuRqId == 0 || StuRqId == null)
            {
                return NotFound();
            }

            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");

            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;

            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();

            var Employee = _DbContext.Employees.Where(item => item.UserAccount_UserId == RUserId).Include(item => item.userAcount).Include(item => item.department).AsNoTracking().FirstOrDefault();

            var Department = _DbContext.Departments.Where(item => item.DepartmentId == Employee.Department_DepartmentId).AsNoTracking().FirstOrDefault();


            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Department.Organization_OrganizationId).AsNoTracking().FirstOrDefault();

            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Department.DepartmentName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;


            IEnumerable<StudentSemesterEvaluationMark> Assignments = Enumerable.Empty<StudentSemesterEvaluationMark>();


            Assignments = _DbContext.StudentSemesterEvaluationMarks.FromSqlRaw("SELECT * FROM StudentSemesterEvaluationMarks WHERE SemesterStudentAndEvaluationDetail_DetailId IN" +
                $"(SELECT SemesterStudentAndEvaluationDetailId FROM SemestersStudentAndEvaluationDetails WHERE StudentRequest_StudentRequestId = {StuRqId})")
                .Include(item => item.assessmentTypeDetail.assessmentType).AsNoTracking().ToList();


            ViewBag.CurrentStuRqId = StuRqId;

            return View(Assignments);

        }


        [HttpGet]
        public IActionResult EvaluateStudentAssignment(int? SSEMId, int? StuRqId)
        {

            if (SSEMId == 0 || SSEMId == null)
            {
                return NotFound();
            }

            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");

            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;

            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();

            var Employee = _DbContext.Employees.Where(item => item.UserAccount_UserId == RUserId).Include(item => item.userAcount).Include(item => item.department).AsNoTracking().FirstOrDefault();

            var Department = _DbContext.Departments.Where(item => item.DepartmentId == Employee.Department_DepartmentId).AsNoTracking().FirstOrDefault();


            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Department.Organization_OrganizationId).AsNoTracking().FirstOrDefault();

            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Department.DepartmentName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;




            var Assignment = _DbContext.StudentSemesterEvaluationMarks.Where(item => item.StudentSemesterEvaluationMarkId == SSEMId)
                .Include(item => item.assessmentTypeDetail.assessmentType).AsNoTracking().FirstOrDefault();


            ViewBag.CurrentStuRqId = StuRqId;

            return View(Assignment);

        }


        [HttpPost]
        public IActionResult EvaluateStudentAssignment(StudentSemesterEvaluationMark Assignment, int? StuRqId)
        {


            if (Assignment == null)
            {
                return NotFound();
            }


            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");

            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;

            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();

            var Employee = _DbContext.Employees.Where(item => item.UserAccount_UserId == RUserId).Include(item => item.userAcount).Include(item => item.department).AsNoTracking().FirstOrDefault();

            var Department = _DbContext.Departments.Where(item => item.DepartmentId == Employee.Department_DepartmentId).AsNoTracking().FirstOrDefault();


            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Department.Organization_OrganizationId).AsNoTracking().FirstOrDefault();

            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Department.DepartmentName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;



            if (Assignment.StudentMark == null)
            {
                return RedirectToAction("AssignedStudentAssignments", new { StuRqId = StuRqId });
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




            return RedirectToAction("AssignedStudentAssignments", new { StuRqId = StuRqId });

        }


        #region


        [HttpPost]
        public IActionResult AddStudentSemesterEvaluationMark(int? Mid, int? DASid)
        {

            if (DASid == 0 || DASid == null || Mid == 0 || Mid == null)
            {
                return Json(new { success = false });
            }

            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;

            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();

            var Employee = _DbContext.Employees.Where(item => item.UserAccount_UserId == RUserId).Include(item => item.userAcount).Include(item => item.department).AsNoTracking().FirstOrDefault();

            var Department = _DbContext.Departments.Where(item => item.DepartmentId == Employee.Department_DepartmentId).AsNoTracking().FirstOrDefault();

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

            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;

            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();

            var Employee = _DbContext.Employees.Where(item => item.UserAccount_UserId == RUserId).Include(item => item.userAcount).Include(item => item.department).AsNoTracking().FirstOrDefault();

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
