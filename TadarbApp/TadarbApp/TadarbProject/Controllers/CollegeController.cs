using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TadarbProject.Data;
using TadarbProject.Models;
using TadarbProject.Models.ViewModels;
using TEST2.Services;

namespace TadarbProject.Controllers
{
    public class CollegeController : Controller
    {
        private readonly AppDbContext _DbContext;
        private readonly IEmailSender _emailSender;
        private readonly IHttpContextAccessor _HttpContextAccessor;
        private static string Name;
        private static int UserId;
        private static UserAcount User;
        private static UniversityCollege college;
        private static Organization organizationOfR;

        public CollegeController(AppDbContext DbContext, IEmailSender emailSender, IHttpContextAccessor HttpContextAccessor)
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

            int RUserId = UserId;

            User = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();

            var user = User;

            college = _DbContext.UniversityColleges.Where(item => item.Responsible_UserId == RUserId).AsNoTracking().FirstOrDefault();

            var College = college;

            organizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == College.Organization_OrganizationId).AsNoTracking().FirstOrDefault();

            var OrganizationOfR = organizationOfR;

            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + College.CollegeName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;







            IEnumerable<SemesterStudentAndEvaluationDetail> Students = Enumerable.Empty<SemesterStudentAndEvaluationDetail>(); ;

            Students = _DbContext.SemestersStudentAndEvaluationDetails.FromSqlRaw($"Select * from SemestersStudentAndEvaluationDetails where AcademicSupervisor_EmployeeId IN " +
                $" ( Select EmployeeId from Employees where Department_DepartmentId IN " +
                $"( Select DepartmentId from Departments where College_CollegeId = {College.CollegeId}))").AsNoTracking().ToList();

            int? UnderTraining = 0;

            UnderTraining = Students.Where(item => item.GeneralTrainingStatus != "stop training").Count();

            ViewBag.Student = UnderTraining;

            int? HaveBeenTrained = 0;

            HaveBeenTrained = Students.Where(item => item.GeneralTrainingStatus == "stop training").Count();

            ViewBag.HaveBeenTrained = HaveBeenTrained;

            var Student = _DbContext.UniversitiesTraineeStudents.Where(item => item.department.College_CollegeId == College.CollegeId && item.ActivationStatus.Equals("Active")).AsNoTracking().ToList();

            ViewBag.AllStudent = Student.Count();

            int? NoSupervisor = 0;


            NoSupervisor = _DbContext.UniversitiesTraineeStudents.FromSqlRaw("SELECT * FROM UniversitiesTraineeStudents WHERE TraineeId NOT IN " +
                "(SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities WHERE DecisionStatus !='stop training' AND StudentRequestOpportunityId  IN " +
                "(SELECT StudentRequest_StudentRequestId FROM SemestersStudentAndEvaluationDetails)) AND TraineeId  IN " +
                "(SELECT Trainee_TraineeId  FROM StudentRequestsOnOpportunities WHERE DecisionStatus !='stop training') AND Department_DepartmentId IN " +
                $"(SELECT DepartmentId FROM Departments WHERE College_CollegeId = {College.CollegeId})").AsNoTracking().Count();

            ViewBag.NotAssign = NoSupervisor;




            return View();

        }




        [HttpGet]
        public IActionResult ViewDepartmentUser()
        {
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(UserId.ToString()))
            {

                return RedirectToAction("Login", "Home");

            }

            ViewBag.Name = Name;

            int RUserId = UserId;
            var user = User;
            var College = college;

            var OrganizationOfR = organizationOfR;

            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + College.CollegeName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;

            var DEPOfR = _DbContext.Departments.Where(item => item.Organization_OrganizationId == OrganizationOfR.OrganizationId
            && item.College_CollegeId == College.CollegeId && item.DepartmentName.Equals("قسم ادارة مسؤولين اقسام الجامعة")).AsNoTracking().FirstOrDefault();

            IEnumerable<UserAcount> OrgEMP = Enumerable.Empty<UserAcount>(); ;

            if (DEPOfR != null)
            {
                OrgEMP = _DbContext.UserAcounts.FromSqlRaw($"select * from UserAcounts WHERE UserAcounts.UserId in (select Employees.UserAccount_UserId from Employees where Employees.Department_DepartmentId = {DEPOfR.DepartmentId});").AsNoTracking().ToList();

            }



            return View(OrgEMP);





        }

        [HttpGet]
        public IActionResult AddDepartmentUser()
        {
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(UserId.ToString()))
            {

                return RedirectToAction("Login", "Home");

            }

            ViewBag.Name = Name;

            int RUserId = UserId;
            var user = User;
            var College = college;

            var OrganizationOfR = organizationOfR;

            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + College.CollegeName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;



            return View();


        }

        [HttpPost]
        public IActionResult AddDepartmentUser(EmployeeVM employeeVM)
        {

            int RUserId = UserId;

            var RUser = User;

            var College = college;

            var OrganizationOfR = organizationOfR;


            if (employeeVM == null)
            {

                return View();

            }

            var Euser = new UserAcount
            {
                UserEmail = employeeVM.userAcount.UserEmail,
                UserPassword = employeeVM.userAcount.UserPassword,
                FullName = employeeVM.userAcount.FullName,
                Phone = employeeVM.userAcount.Phone,
                City_CityId = College.City_CityId,
                UserType = "DepUni_Admin",
                ActivationStatus = "Active"




            };

            _DbContext.UserAcounts.Add(Euser);

            _DbContext.SaveChanges();



            var DEPOfR = _DbContext.Departments.Where(item => item.Organization_OrganizationId == OrganizationOfR.OrganizationId && item.College_CollegeId == College.CollegeId
            && item.DepartmentName.Equals("قسم ادارة مسؤولين اقسام الجامعة")).AsNoTracking().FirstOrDefault();



            if (DEPOfR == null)
            {
                var DEP = new Department
                {

                    DepartmentName = "قسم ادارة مسؤولين اقسام الجامعة",
                    Organization_OrganizationId = OrganizationOfR.OrganizationId,
                    Responsible_UserId = RUser.UserId,
                    College_CollegeId = College.CollegeId,



                };

                _DbContext.Departments.Add(DEP);

                _DbContext.SaveChanges();
            }



            int DEPId = _DbContext.Departments.Where(item => item.Organization_OrganizationId == OrganizationOfR.OrganizationId &&
            item.College_CollegeId == College.CollegeId && item.DepartmentName.Equals("قسم ادارة مسؤولين اقسام الجامعة")).First().DepartmentId;

            var EMP = new Employee
            {

                Department_DepartmentId = DEPId,
                Job_JobId = 5,
                SSN = employeeVM.employee.SSN,
                UserAccount_UserId = Euser.UserId,

            };

            _DbContext.Employees.Add(EMP);

            _DbContext.SaveChanges();
            TempData["success"] = "تم إضافة حساب مسؤول القسم  بنجاح";
            return RedirectToAction("ViewDepartmentUser");

        }


        [HttpGet]
        public IActionResult AddViewDepartmentUni()
        {
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(UserId.ToString()))
            {

                return RedirectToAction("Login", "Home");

            }

            int RUserId = UserId;
            var user = User;
            var College = college;

            var OrganizationOfR = organizationOfR;

            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + College.CollegeName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;


            IEnumerable<UserAcount> CollegeEMP = Enumerable.Empty<UserAcount>(); ;


            var DEPOfC = _DbContext.Departments.Where(item => item.Organization_OrganizationId == OrganizationOfR.OrganizationId && item.College_CollegeId == College.CollegeId).AsNoTracking().FirstOrDefault();


            if (DEPOfC != null)
            {

                CollegeEMP = _DbContext.UserAcounts.FromSqlRaw($"Select * from UserAcounts WHERE UserAcounts.UserId IN (Select Employees.UserAccount_UserId from Employees where Employees.Department_DepartmentId = {DEPOfC.DepartmentId})" +
                "AND UserAcounts.UserId NOT IN(select Responsible_UserId from dbo.Departments);").AsNoTracking().ToList();


            }




            DepartmentVM departmentVM = new()
            {
                department = new(),

                UserListItems = CollegeEMP.ToList().Select(u => new SelectListItem { Text = u.FullName, Value = u.UserId.ToString() }),


            };



            return View(departmentVM);




        }




        [HttpPost]
        public IActionResult AddViewDepartmentUni(DepartmentVM departmentVM)
        {

            int RUserId = UserId;
            var user = User;

            var College = college;
            var OrganizationOfR = organizationOfR;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;

            if (ModelState.IsValid)
            {

                var DEP = new Department
                {
                    DepartmentName = departmentVM.department.DepartmentName,

                    Responsible_UserId = departmentVM.department.Responsible_UserId,


                    Organization_OrganizationId = OrganizationOfR.OrganizationId,

                    College_CollegeId = College.CollegeId,

                };

                _DbContext.Departments.Add(DEP);




                _DbContext.SaveChanges();


                TempData["success"] = "تم إضافة الفسم  بنجاح";



            }

            return View(departmentVM);


        }


        [HttpGet]
        public IActionResult EditBDepartment(int? id)
        {
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(UserId.ToString()))
            {

                return RedirectToAction("Login", "Home");

            }

            int RUserId = UserId;
            var user = User;

            var College = college;
            var OrganizationOfR = organizationOfR;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;
            //BranchVM branchVM = new()
            //{

            //    Branch = new OrganizationBranch_TrainProv()

            //};

            DepartmentVM DepartmentVM = new()
            {
                department = new Department()

            };

            if (id != null || id != 0)
            {

                DepartmentVM.department = _DbContext.Departments.Where(u => u.DepartmentId == id).AsNoTracking().FirstOrDefault();


                var DEPOfR = _DbContext.Departments.Where(item => item.Organization_OrganizationId == OrganizationOfR.OrganizationId && item.DepartmentName.Equals("قسم ادارة مسؤولين اقسام الجامعة"))
                    .AsNoTracking().FirstOrDefault();

                DepartmentVM.UserListItems = _DbContext.UserAcounts.FromSqlRaw($"Select * from UserAcounts WHERE UserAcounts.UserId IN (Select Employees.UserAccount_UserId FROM Employees WHERE Employees.Department_DepartmentId ={DEPOfR.DepartmentId})" +
                   $"AND UserAcounts.UserId NOT IN (Select Responsible_UserId FROM Departments WHERE Responsible_UserId!={DepartmentVM.department.Responsible_UserId})")
                   .AsNoTracking().Select(u => new SelectListItem { Text = u.FullName, Value = u.UserId.ToString() });

                DepartmentVM.department.College_CollegeId = College.CollegeId;

                //var DEPOfR = _DbContext.Departments.Where(item => item.Organization_OrganizationId == OrganizationOfR.OrganizationId && item.DepartmentName.Equals("قسم ادارة الفروع")).FirstOrDefault();


                //branchVM.UserListItems = _DbContext.UserAcounts.FromSqlRaw($"Select * from UserAcounts WHERE UserAcounts.UserId IN (Select Employees.UserAccount_UserId FROM Employees WHERE Employees.Department_DepartmentId ={DEPOfR.DepartmentId})" +
                //    $"AND UserAcounts.UserId NOT IN (Select Responsible_UserId FROM OrganizationBranches_TrainProv WHERE Responsible_UserId!={branchVM.Branch.Responsible_UserId})")
                //    .Select(u => new SelectListItem { Text = u.FullName, Value = u.UserId.ToString() });

                return View(DepartmentVM);
            }


            return NotFound();
        }

        [HttpPost]
        public IActionResult EditBDepartment(DepartmentVM DepartmentVM)
        {
            int RUserId = UserId;
            var College = college;


            if (!ModelState.IsValid)
            {
                return View(DepartmentVM);
            }

            DepartmentVM.department.College_CollegeId = College.CollegeId;


            _DbContext.Departments.Update(DepartmentVM.department);

            var User = _DbContext.UserAcounts.AsNoTracking().FirstOrDefault(item => item.UserId == DepartmentVM.department.Responsible_UserId);

            _DbContext.UserAcounts.Update(User);
            _DbContext.SaveChanges();

            TempData["success"] = "تم تعديل معلومات القسم  بنجاح";

            return RedirectToAction("AddViewDepartmentUni");
        }

        [HttpGet]
        public IActionResult GetAllDEPs()
        {
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(UserId.ToString()))
            {

                return RedirectToAction("Login", "Home");

            }

            int RUserId = UserId;

            var College = college;

            var OrganizationOfR = organizationOfR;

            var depList = _DbContext.Departments.Where(item => item.Organization_OrganizationId == OrganizationOfR.OrganizationId && item.College_CollegeId == College.CollegeId && !item.DepartmentName.Equals("قسم ادارة مسؤولين اقسام الجامعة"))
                .Include(item => item.User)
                .AsNoTracking().ToList();

            if (depList == null)
            {
                return Json(new { Exists = false });
            }



            return Json(new { Exists = true, depList });
        }


        [HttpGet]
        public IActionResult GetStudentByUniAjax()
        {
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(UserId.ToString()))
            {

                return RedirectToAction("Login", "Home");

            }

            ViewBag.Name = Name;

            int RUserId = UserId;

            var College = college;

            var OrganizationOfR = organizationOfR;




            IEnumerable<SemesterStudentAndEvaluationDetail> Students = Enumerable.Empty<SemesterStudentAndEvaluationDetail>();

            Students = _DbContext.SemestersStudentAndEvaluationDetails.FromSqlRaw("SELECT * FROM SemestersStudentAndEvaluationDetails WHERE AcademicSupervisor_EmployeeId IN " +
                "(SELECT EmployeeId FROM Employees WHERE Department_DepartmentId IN " +
                $"(SELECT DepartmentId FROM Departments WHERE College_CollegeId ={College.CollegeId})) AND GeneralTrainingStatus !='stop training'")
                .Include(item => item.EmployeeTrainingSupervisor.department.organization).AsNoTracking().ToList();


            var OrganizationStudentsCount = 0;

            List<OrganizationStudents> OrganizationStudentList = new List<OrganizationStudents>();

            IEnumerable<Organization> universities = Enumerable.Empty<Organization>();


            universities = _DbContext.Organizations.FromSqlRaw("SELECT * FROM Organizations WHERE OrganizationId IN " +
                "(SELECT Organization_OrganizationId FROM Departments WHERE DepartmentId IN " +
                "(SELECT Department_DepartmentId FROM Employees WHERE EmployeeId IN " +
                "(SELECT TrainingSupervisor_EmployeeId FROM SemestersStudentAndEvaluationDetails WHERE AcademicSupervisor_EmployeeId IN " +
                "(SELECT EmployeeId FROM Employees WHERE Department_DepartmentId IN " +
                $"(SELECT DepartmentId FROM Departments WHERE College_CollegeId = {College.CollegeId})))))").AsNoTracking().ToList();


            if (universities.Any())
            {

                foreach (var univer in universities)
                {

                    OrganizationStudentsCount = Students.Where(item => item.EmployeeTrainingSupervisor.department.organization.OrganizationId == univer.OrganizationId).Count();

                    OrganizationStudents organizationStudents = new OrganizationStudents() { OrganizationName = univer.OrganizationName, StudentsCount = OrganizationStudentsCount };

                    OrganizationStudentList.Add(organizationStudents);
                }

            }


            return Json(new { OrganizationStudentList });


        }



        [HttpGet]
        public IActionResult ViewDepartmentFiledSpecialties()
        {
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(UserId.ToString()))
            {

                return RedirectToAction("Login", "Home");

            }

            ViewBag.Name = Name;

            int RUserId = UserId;
            var user = User;
            var College = college;

            var OrganizationOfR = organizationOfR;

            var Department = _DbContext.Departments.Where(item => item.Responsible_UserId == RUserId).AsNoTracking().ToList();

            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + College.CollegeName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;


            return View(Department);

        }

        [HttpGet]
        public IActionResult AddDepartmentFiledSpecialties()
        {
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(UserId.ToString()))
            {

                return RedirectToAction("Login", "Home");

            }


            ViewBag.Name = Name;
            int RUserId = UserId;
            var user = User;
            var College = college;

            var OrganizationOfR = organizationOfR;

            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + College.CollegeName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;

            SpecialitiesVM specialitiesVM = new()
            {

                MasterFieldsListItems = _DbContext.FieldOfSpecialtiesMaster.FromSqlRaw("SELECT * FROM FieldOfSpecialtiesMaster WHERE FieldId IN" +
                $"(SELECT FieldOfSpecialtiesDetails.Field_FieldId FROM FieldOfSpecialtiesDetails ,OrganizationsProvidTrainingInArea WHERE DetailFieldId = DetailField_DetailFieldId AND Organization_OrganizationId ={OrganizationOfR.OrganizationId});")
                .AsNoTracking().ToList().Select(u => new SelectListItem { Text = u.FieldName, Value = u.FieldId.ToString() }),

                DepartmentListItems = _DbContext.Departments.FromSqlRaw($"SELECT * FROM Departments  WHERE College_CollegeId   = {College.CollegeId} AND DepartmentName !='قسم ادارة مسؤولين اقسام الجامعة';")
                .AsNoTracking().ToList().Select(u => new SelectListItem { Text = u.DepartmentName, Value = u.DepartmentId.ToString() }),



            };

    


            return View(specialitiesVM);


        }



        #region
        [HttpPost]
        public IActionResult AddDetailFields(string dFieldIds)
        {

            if (dFieldIds != "[]")
            {
                var charsToRemove = new string[] { "[", "]" };

                foreach (var c in charsToRemove)
                {
                    dFieldIds = dFieldIds.Replace(c, string.Empty);
                }

                string[] Ids = dFieldIds.Split(",");

                // Console.WriteLine(Ids[0]);


                int RUserId = UserId;

                var RUser = User;


                int DEPId = Convert.ToInt32(Ids[0]);
                //   var OrganizationOfR = _DbContext.Organizations.Where(item => item.ResponsibleUserId == RUserId).FirstOrDefault();

                var Department = _DbContext.Departments.Where(item => item.DepartmentId == DEPId).AsNoTracking().FirstOrDefault();




                foreach (var i in Ids.Skip(1))
                {
                    int id = Convert.ToInt32(i);

                    var DTA = new DepartmentTrainingArea
                    {
                        Department_DepartmenId = Department.DepartmentId,
                        TrainArea_DetailFiledId = id,


                    };


                    _DbContext.DepartmentTrainingAreas.Add(DTA);
                }

                _DbContext.SaveChanges();
                //TempData["success"] = "تم إضافة التخصصات للقسم بنجاح";


            }




            return Json(new { Exists = false });

        }


        [HttpGet]
        public IActionResult EmailExists(string? Email)
        {
            if (Email == null)
            {
                return Json(new { Exists = false });
            }

            var item = _DbContext.UserAcounts.Where(item => item.UserEmail.Equals(Email)).AsNoTracking().FirstOrDefault();

            if (item == null)
            {
                return Json(new { Exists = false });
            }



            return Json(new { Exists = true });
        }


        [HttpGet]
        public IActionResult PhoneExists(string? Phone)
        {
            if (Phone == null)
            {
                return Json(new { Exists = false });
            }

            var item = _DbContext.UserAcounts.Where(item => item.Phone.Equals(Phone)).AsNoTracking().FirstOrDefault();

            if (item == null)
            {
                return Json(new { Exists = false });
            }



            return Json(new { Exists = true });
        }


        [HttpGet]
        public IActionResult GetDetailFields(string? ids)
        {

            int RUserId = UserId;


            var College = college;

            var OrganizationOfR = organizationOfR;





            if (!string.IsNullOrEmpty(ids) && ids.Length > 2)
            {

                string[] Ids = ids.Split(",");

                var FId = Convert.ToInt32(Ids[1]);
                var DId = Convert.ToInt32(Ids[0]);

                //Console.WriteLine(FId);

                var Detailfields = _DbContext.FieldOfSpecialtiesDetails.FromSqlRaw($"Select * From FieldOfSpecialtiesDetails WHERE Field_FieldId={FId} AND DetailFieldId IN" +
                $"(Select DetailField_DetailFieldId From OrganizationsProvidTrainingInArea WHERE Organization_OrganizationId={OrganizationOfR.OrganizationId})" +
                $"AND DetailFieldId NOT IN (SELECT TrainArea_DetailFiledId From DepartmentTrainingAreas WHERE Department_DepartmenId={DId})")

                .Select(item => new

                {
                    DetailFieldId = item.DetailFieldId,

                    SpecializationName = item.SpecializationName

                }


                ).AsNoTracking().ToList();


                return Json(new { Detailfields });


            }


            return Json(new { Exists = false });
        }

        [HttpGet]
        public IActionResult GetDepartmentTrainingArea()

        {
            int RUserId = UserId;

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.ResponsibleUserId == RUserId).AsNoTracking().FirstOrDefault();

            var College = _DbContext.UniversityColleges.Where(item => item.Responsible_UserId == RUserId).AsNoTracking().FirstOrDefault();

            var departmentofb = _DbContext.Departments.Where(item => item.Branch_BranchId == College.CollegeId).AsNoTracking().FirstOrDefault();

            IEnumerable<DepartmentTrainingArea> Departments = Enumerable.Empty<DepartmentTrainingArea>();

            Departments = _DbContext.DepartmentTrainingAreas.FromSqlRaw("SELECT * FROM DepartmentTrainingAreas WHERE Department_DepartmenId IN" +
                $"(SELECT DepartmentId FROM Departments WHERE  DepartmentName!='قسم ادارة مسؤولين اقسام الجامعة' AND College_CollegeId={College.CollegeId})").Include(item => item.department)
                .Include(item => item.fieldOfSpecialtyDetails).OrderBy(item => item.Department_DepartmenId).AsNoTracking().ToList();

            return Json(new { Departments });



        }

        [HttpPost]
        public IActionResult DeleteDepartmentTrainingArea(int? id)
        {



            if (id != null || id == 0)
            {



                var DEPTA = _DbContext.DepartmentTrainingAreas.Where(u => u.DepartmentTrainingAreaId == id).AsNoTracking().FirstOrDefault();

                _DbContext.DepartmentTrainingAreas.Remove(DEPTA);


                _DbContext.SaveChanges();


                return Json(new { success = true, message = "تم الحذف بنجاح" });
            }



            return Json(new { success = false, message = "حصل خطاء لم يتم الحذف" });

        }


        #endregion


    }
}
