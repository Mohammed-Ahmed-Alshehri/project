using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TadarbProject.Data;
using TadarbProject.Models;
using TadarbProject.Models.ViewModels;
using TEST2.Services;

namespace TadarbProject.Controllers
{
    public class DepartmentUniversityController : Controller
    {
        private readonly AppDbContext _DbContext;
        private readonly IEmailSender _emailSender;
        private readonly IHttpContextAccessor _HttpContextAccessor;


        public DepartmentUniversityController(AppDbContext DbContext, IEmailSender emailSender, IHttpContextAccessor HttpContextAccessor)
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
            var Department = _DbContext.Departments.Where(item => item.Responsible_UserId == RUserId).AsNoTracking().FirstOrDefault();

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Department.Organization_OrganizationId).AsNoTracking().FirstOrDefault();



            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Department.DepartmentName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;

            return View();
        }



        [HttpGet]
        public IActionResult AddAcademicSupervisor()
        {

            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");
            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();

            var Department = _DbContext.Departments.Where(item => item.Responsible_UserId == RUserId).AsNoTracking().FirstOrDefault();

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Department.Organization_OrganizationId).AsNoTracking().FirstOrDefault();


            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Department.DepartmentName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;

            return View();
        }


        [HttpPost]
        public IActionResult AddAcademicSupervisor(EmployeeVM employeeVM)
        {

            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;

            var RUser = _DbContext.UserAcounts.Find(RUserId);

            var Department = _DbContext.Departments.Where(item => item.Responsible_UserId == RUserId).AsNoTracking().FirstOrDefault();

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Department.Organization_OrganizationId).AsNoTracking().FirstOrDefault();

            var College = _DbContext.UniversityColleges.Where(item => item.CollegeId == Department.College_CollegeId).AsNoTracking().FirstOrDefault();

            if (employeeVM == null)
            {

                return View();

            }

            var user = new UserAcount
            {
                UserEmail = employeeVM.userAcount.UserEmail,
                UserPassword = employeeVM.userAcount.UserPassword,
                FullName = employeeVM.userAcount.FullName,
                Phone = employeeVM.userAcount.Phone,
                City_CityId = College.City_CityId,
                UserType = "Academic_supervisor",
                ActivationStatus = "Not_Active"

            };

            _DbContext.UserAcounts.Add(user);

            _DbContext.SaveChanges();


            var EMP = new Employee
            {

                Department_DepartmentId = Department.DepartmentId,
                Job_JobId = 4,
                SSN = employeeVM.employee.SSN,
                UserAccount_UserId = user.UserId,
            };

            _DbContext.Employees.Add(EMP);

            _DbContext.SaveChanges();
            TempData["success"] = "تم إضافة حساب الموظف  بنجاح";
            return RedirectToAction("ViewAcademicSupervisors");

        }




        public IActionResult ViewAcademicSupervisors()
        {
            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");

            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();

            var Department = _DbContext.Departments.Where(item => item.Responsible_UserId == RUserId).AsNoTracking().FirstOrDefault();

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Department.Organization_OrganizationId).AsNoTracking().FirstOrDefault();


            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Department.DepartmentName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;

            IEnumerable<UserAcount> Employees = Enumerable.Empty<UserAcount>();


            Employees = _DbContext.UserAcounts.FromSqlRaw("Select * from UserAcounts WHERE UserId IN " +
            $"(Select UserAccount_UserId from Employees WHERE Department_DepartmentId = {Department.DepartmentId});").AsNoTracking().ToList();




            return View(Employees);
        }


        [HttpGet]
        public IActionResult ViewStudents()
        {

            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");

            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;

            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();

            var Department = _DbContext.Departments.Where(item => item.Responsible_UserId == RUserId).AsNoTracking().FirstOrDefault();

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Department.Organization_OrganizationId).AsNoTracking().FirstOrDefault();

            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Department.DepartmentName;

            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;




            return View();
        }

        [HttpGet]
        public IActionResult DetailStudent(int? id)
        {

            if (id == null)
            {

                return NotFound();
            }

            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");

            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;

            var user = _DbContext.UserAcounts.Where(item => item.UserId == id).AsNoTracking().FirstOrDefault();


            var Department = _DbContext.Departments.Where(item => item.Responsible_UserId == RUserId).AsNoTracking().FirstOrDefault();

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Department.Organization_OrganizationId).AsNoTracking().FirstOrDefault();

            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Department.DepartmentName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;


            var student = _DbContext.UniversitiesTraineeStudents.Where(item => item.TraineeId == id).Include(item => item.user).AsNoTracking().FirstOrDefault();

            ViewBag.StudentName = student.user.FullName;
            ViewBag.uninumber = student.UniversityStudentNumber;





            var Req = _DbContext.StudentRequestsOnOpportunities.Where(item => item.Trainee_TraineeId == id)
                .Include(item => item.trainingOpportunity.DetailFiled)
                .Include(item => item.trainingOpportunity.trainingType)
                .Include(item => item.trainingOpportunity.Branch.organization)
                .AsNoTracking().ToList();



            return View(Req);

        }

        [HttpGet]
        public IActionResult AddStudents()
        {
            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");

            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;

            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();


            var Department = _DbContext.Departments.Where(item => item.Responsible_UserId == RUserId).AsNoTracking().FirstOrDefault();

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Department.Organization_OrganizationId).AsNoTracking().FirstOrDefault();

            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Department.DepartmentName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;

            return View();

        }


        [HttpPost]
        public IActionResult AddStudents(StudentVM StudentVM)
        {
            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;


            var Department = _DbContext.Departments.Where(item => item.Responsible_UserId == RUserId).AsNoTracking().FirstOrDefault();

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Department.Organization_OrganizationId).AsNoTracking().FirstOrDefault();

            var college = _DbContext.UniversityColleges.Where(item => item.Organization_OrganizationId == OrganizationOfR.OrganizationId).AsNoTracking().FirstOrDefault();



            if (StudentVM == null)
            {

                return View();

            }

            var user = new UserAcount
            {
                UserEmail = StudentVM.userAcount.UserEmail,
                UserPassword = StudentVM.userAcount.UserPassword,
                FullName = StudentVM.userAcount.FullName,
                Phone = StudentVM.userAcount.Phone,
                City_CityId = college.City_CityId,
                UserType = "Student",
                ActivationStatus = "Not_Active"

            };

            _DbContext.UserAcounts.Add(user);

            _DbContext.SaveChanges();


            var Student = new UniversityTraineeStudent
            {

                Department_DepartmentId = Department.DepartmentId,
                UserAccount_UserId = user.UserId,
                UniversityStudentNumber = StudentVM.UniversityTraineeStudent.UniversityStudentNumber,
                CompletedHours = StudentVM.UniversityTraineeStudent.CompletedHours,
                GPA = StudentVM.UniversityTraineeStudent.GPA,
                Gender = StudentVM.UniversityTraineeStudent.Gender




            };


            _DbContext.UniversitiesTraineeStudents.Add(Student);

            _DbContext.SaveChanges();



            TempData["success"] = "تم إضافة حساب طالب بنجاح";
            return RedirectToAction("ViewStudents");

        }


        #region


        [HttpGet]
        public IActionResult GetStudentsList()
        {


            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;


            var Department = _DbContext.Departments.Where(item => item.Responsible_UserId == RUserId).AsNoTracking().FirstOrDefault();


            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Department.Organization_OrganizationId).AsNoTracking().FirstOrDefault();

            var college = _DbContext.UniversityColleges.Where(item => item.Organization_OrganizationId == OrganizationOfR.OrganizationId).AsNoTracking().FirstOrDefault();


            IEnumerable<UniversityTraineeStudent> Students = Enumerable.Empty<UniversityTraineeStudent>();


            Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId}")
                   .AsNoTracking().Include(item => item.user).AsNoTracking().ToList();


            return Json(new { Students });
        }

        [HttpGet]
        public IActionResult GetStudentsListDown()
        {


            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;


            var Department = _DbContext.Departments.Where(item => item.Responsible_UserId == RUserId).AsNoTracking().FirstOrDefault();


            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Department.Organization_OrganizationId).AsNoTracking().FirstOrDefault();

            var college = _DbContext.UniversityColleges.Where(item => item.Organization_OrganizationId == OrganizationOfR.OrganizationId).AsNoTracking().FirstOrDefault();


            IEnumerable<UniversityTraineeStudent> Students = Enumerable.Empty<UniversityTraineeStudent>();


            Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId}")
                   .AsNoTracking().Include(item => item.user).OrderBy(item => item.CompletedHours).AsNoTracking().ToList();


            return Json(new { Students });
        }

        [HttpGet]
        public IActionResult GetStudentsListUp()
        {


            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;


            var Department = _DbContext.Departments.Where(item => item.Responsible_UserId == RUserId).AsNoTracking().FirstOrDefault();


            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Department.Organization_OrganizationId).AsNoTracking().FirstOrDefault();

            var college = _DbContext.UniversityColleges.Where(item => item.Organization_OrganizationId == OrganizationOfR.OrganizationId).AsNoTracking().FirstOrDefault();


            IEnumerable<UniversityTraineeStudent> Students = Enumerable.Empty<UniversityTraineeStudent>();


            Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId}")
                   .AsNoTracking().Include(item => item.user).OrderByDescending(item => item.CompletedHours).AsNoTracking().ToList();


            return Json(new { Students });
        }


        [HttpGet]
        public IActionResult GetStudentsListDowngpa()
        {


            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;


            var Department = _DbContext.Departments.Where(item => item.Responsible_UserId == RUserId).AsNoTracking().FirstOrDefault();


            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Department.Organization_OrganizationId).AsNoTracking().FirstOrDefault();

            var college = _DbContext.UniversityColleges.Where(item => item.Organization_OrganizationId == OrganizationOfR.OrganizationId).AsNoTracking().FirstOrDefault();


            IEnumerable<UniversityTraineeStudent> Students = Enumerable.Empty<UniversityTraineeStudent>();


            Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId}")
                   .AsNoTracking().Include(item => item.user).OrderBy(item => item.GPA).AsNoTracking().ToList();


            return Json(new { Students });
        }

        [HttpGet]
        public IActionResult GetStudentsListUpgpa()
        {


            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;


            var Department = _DbContext.Departments.Where(item => item.Responsible_UserId == RUserId).AsNoTracking().FirstOrDefault();


            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Department.Organization_OrganizationId).AsNoTracking().FirstOrDefault();

            var college = _DbContext.UniversityColleges.Where(item => item.Organization_OrganizationId == OrganizationOfR.OrganizationId).AsNoTracking().FirstOrDefault();


            IEnumerable<UniversityTraineeStudent> Students = Enumerable.Empty<UniversityTraineeStudent>();


            Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId}")
                   .AsNoTracking().Include(item => item.user).OrderByDescending(item => item.GPA).AsNoTracking().ToList();


            return Json(new { Students });
        }


        public IActionResult GetByGender(string gender)
        {
            /* var list = _DbContext.Organizations.ToList();*/

            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;


            var Department = _DbContext.Departments.Where(item => item.Responsible_UserId == RUserId).AsNoTracking().FirstOrDefault();

            IEnumerable<UniversityTraineeStudent> list;




            if (gender == "ذكر")
            {

                list = _DbContext.UniversitiesTraineeStudents.Where(item => item.Gender.Equals(gender) && item.Department_DepartmentId == Department.DepartmentId).AsNoTracking().Include(item => item.user).AsNoTracking().ToList();

                return Json(new { list });
            }

            if (gender == "انثى")
            {

                list = _DbContext.UniversitiesTraineeStudents.Where(item => item.Gender.Equals(gender) && item.Department_DepartmentId == Department.DepartmentId).AsNoTracking().Include(item => item.user).AsNoTracking().ToList();

                return Json(new { list });
            }






            return Json(new { data = "No_data" });
        }





        [HttpGet]
        public IActionResult GetStudentsByNameOrNumber(string? filter)
        {
            IEnumerable<UniversityTraineeStudent> Students = Enumerable.Empty<UniversityTraineeStudent>();

            if (filter == null)
            {

                return Json(new { Students });

            }


            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;


            var Department = _DbContext.Departments.Where(item => item.Responsible_UserId == RUserId).AsNoTracking().FirstOrDefault();


            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Department.Organization_OrganizationId).AsNoTracking().FirstOrDefault();

            var college = _DbContext.UniversityColleges.Where(item => item.Organization_OrganizationId == OrganizationOfR.OrganizationId).AsNoTracking().FirstOrDefault();

            if (Char.IsDigit(filter, 0))
            {

                Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} AND " +
                    $"UniversityStudentNumber = '{filter}'")
                       .AsNoTracking().Include(item => item.user).AsNoTracking().ToList();
            }
            else
            {
                Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} AND " +
                    $"UserAccount_UserId IN (SELECT UserId FROM UserAcounts WHERE UserAcounts.FullName LIKE '{filter}%')")
                .AsNoTracking().Include(item => item.user).AsNoTracking().ToList();

            }






            return Json(new { Students });
        }


        #endregion




    }


}
