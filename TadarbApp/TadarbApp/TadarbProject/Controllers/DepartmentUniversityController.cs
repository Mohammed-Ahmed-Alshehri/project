using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        private readonly IWebHostEnvironment _WebHostEnvironment;
        private readonly IEmailSender _emailSender;
        private readonly IHttpContextAccessor _HttpContextAccessor;


        public DepartmentUniversityController(AppDbContext DbContext, IEmailSender emailSender, IHttpContextAccessor HttpContextAccessor, IWebHostEnvironment webHostEnvironment)
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
            var Department = _DbContext.Departments.Where(item => item.Responsible_UserId == RUserId).AsNoTracking().FirstOrDefault();

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Department.Organization_OrganizationId).AsNoTracking().FirstOrDefault();



            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Department.DepartmentName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;

            return View();
        }

        public IActionResult ViewAssessment()
        {
            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");
            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();
            var Department = _DbContext.Departments.Where(item => item.Responsible_UserId == RUserId).AsNoTracking().FirstOrDefault();

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Department.Organization_OrganizationId).AsNoTracking().FirstOrDefault();

            var MasterAssigmnt = _DbContext.DepartmentsAssessmentTypeMaster.Where(item => item.Department_DepartmentId == Department.DepartmentId)
                .AsNoTracking().FirstOrDefault();


            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Department.DepartmentName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;


            IEnumerable<DepartmentAssessmentTypeDetail> DetailAssigmnt = Enumerable.Empty<DepartmentAssessmentTypeDetail>();

            if (MasterAssigmnt != null)
            {
                DetailAssigmnt = _DbContext.DepartmentsAssessmentTypeDetail.Where(item => item.DepartmentAssessmentTypeMaster_MasterId ==
                MasterAssigmnt.DepartmentAssessmentTypeMasterId).Include(item => item.master).AsNoTracking().ToList();
            }


            return View(MasterAssigmnt);
        }

        public IActionResult EditAssessment()
        {
            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");
            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();
            var Department = _DbContext.Departments.Where(item => item.Responsible_UserId == RUserId).AsNoTracking().FirstOrDefault();
            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Department.Organization_OrganizationId).AsNoTracking().FirstOrDefault();

            var MasterAssigmnt = _DbContext.DepartmentsAssessmentTypeMaster.Where(item => item.Department_DepartmentId == Department.DepartmentId)
                .AsNoTracking().FirstOrDefault();


            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Department.DepartmentName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;



            AssessmentVM assessmentVM = new AssessmentVM
            {

                DepartmentAssessmentTypeMaster = MasterAssigmnt,

            };



            return View(assessmentVM);
        }

        public IActionResult ViewSemesters()
        {
            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");
            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();
            var Department = _DbContext.Departments.Where(item => item.Responsible_UserId == RUserId).AsNoTracking().FirstOrDefault();
            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Department.Organization_OrganizationId).AsNoTracking().FirstOrDefault();

            var Semesters = _DbContext.SemestersTrainingSettingMaster.Where(item => item.Department_DepartmenId ==
                 Department.DepartmentId).AsNoTracking().FirstOrDefault();


            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Department.DepartmentName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;



            IEnumerable<SemesterTrainingSettingMaster> Sem = Enumerable.Empty<SemesterTrainingSettingMaster>();

            if (Semesters != null)
            {
                Sem = _DbContext.SemestersTrainingSettingMaster.Where(item => item.Department_DepartmenId ==
                Department.DepartmentId).Include(item => item.trainingType).AsNoTracking().ToList();
            }


            return View(Sem);
        }

        public IActionResult AddSemesters()
        {
            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");
            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();
            var Department = _DbContext.Departments.Where(item => item.Responsible_UserId == RUserId).AsNoTracking().FirstOrDefault();
            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Department.Organization_OrganizationId).AsNoTracking().FirstOrDefault();




            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Department.DepartmentName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;




            SemesterMasterVM SemesterMasterVM = new SemesterMasterVM
            {
                TrainingTypeListItems = _DbContext.TrainingTypes.AsNoTracking().ToList().Select(u => new SelectListItem { Text = u.TypeName, Value = u.TrainingTypeId.ToString() }),

            };





            return View(SemesterMasterVM);
        }

        [HttpPost]
        public IActionResult AddSemesters(SemesterMasterVM SemesterMasterVM, IFormFile? CvFile)
        {
            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");
            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();
            var Department = _DbContext.Departments.Where(item => item.Responsible_UserId == RUserId).AsNoTracking().FirstOrDefault();
            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Department.Organization_OrganizationId).AsNoTracking().FirstOrDefault();
            var Emplyee = _DbContext.Employees.Where(item => item.UserAccount_UserId == RUserId).AsNoTracking().FirstOrDefault();
            //var SemesterMaste = _DbContext.SemestersTrainingSettingMaster.Where(item => item.Cre == RUserId).AsNoTracking().FirstOrDefault();




            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Department.DepartmentName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;

            //Startt UploadFile

            string fileName = Guid.NewGuid().ToString();

            string wwwRootPath = _WebHostEnvironment.WebRootPath;

            var uploadTo = Path.Combine(wwwRootPath, @"SupervisionFiles\TrainingEvaluation\");

            var extension = Path.GetExtension(CvFile.FileName);

            //here is to Create a file to has the user uploaded file
            using (var fileStreams = new FileStream(Path.Combine(uploadTo, fileName + extension), FileMode.Create))
            {
                CvFile.CopyTo(fileStreams);
            }

            //Here what will be in the database.
            var DbLogoPath = @"SupervisionFiles\TrainingEvaluation\" + fileName + extension;

            //___________________________end upload____________



            var Startdate = SemesterMasterVM.SemesterTrainingSettingMaster.StartDate;
            var active = "Not_Active";
            if (Startdate == DateTime.Now.Date)
            {

                active = "Active";
            }



            SemesterTrainingSettingMaster SemesterTrainingSettingMaster = new SemesterTrainingSettingMaster
            {
                Department_DepartmenId = Department.DepartmentId,
                AcademicYear = SemesterMasterVM.SemesterTrainingSettingMaster.AcademicYear,
                SemesterType = SemesterMasterVM.SemesterTrainingSettingMaster.SemesterType,
                StartDate = Startdate,
                EndDate = SemesterMasterVM.SemesterTrainingSettingMaster.EndDate,
                ActivationStatus = active,
                TrainingType_TrainingTypeId = SemesterMasterVM.SemesterTrainingSettingMaster.TrainingType_TrainingTypeId,
                RequiredWeeks = SemesterMasterVM.SemesterTrainingSettingMaster.RequiredWeeks,
                MinimumRequiredHours = SemesterMasterVM.SemesterTrainingSettingMaster.MinimumRequiredHours,
                CreateDate = DateTime.Now.Date,
                CreatedByEmployee_EmployeeId = Emplyee.EmployeeId,
                EvaluationFileToTrainingSupervisor = DbLogoPath,


            };


            _DbContext.SemestersTrainingSettingMaster.Add(SemesterTrainingSettingMaster);

            _DbContext.SaveChanges();



            TempData["success"] = "تم إضافة شعبة بنجاح";

            return RedirectToAction("ViewSemesters");


        }

        public IActionResult ManageAssessment()
        {
            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");
            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();
            var Department = _DbContext.Departments.Where(item => item.Responsible_UserId == RUserId).AsNoTracking().FirstOrDefault();

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Department.Organization_OrganizationId).AsNoTracking().FirstOrDefault();

            var MasterAssigmnt = _DbContext.DepartmentsAssessmentTypeMaster.Where(item => item.Department_DepartmentId == Department.DepartmentId)
               .AsNoTracking().FirstOrDefault();




            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Department.DepartmentName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;




            if (MasterAssigmnt != null)
            {

                TempData["success"] = "لديك قائمة تقييمات تم تحديده مسبقا";


                return RedirectToAction("EditAssessment");
            }

            AssessmentVM assessmentVM = new AssessmentVM
            {

                AssessmentTypeListItems = _DbContext.AssessmentTypes.AsNoTracking().ToList().Select(u => new SelectListItem { Text = u.AssessmentTypeName, Value = u.AssessmentTypeId.ToString() }),

            };

            return View(assessmentVM);
        }



        public IActionResult ViewAssessmentajax()
        {

            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();
            var Department = _DbContext.Departments.Where(item => item.Responsible_UserId == RUserId).AsNoTracking().FirstOrDefault();


            var MasterAssigmnt = _DbContext.DepartmentsAssessmentTypeMaster.Where(item => item.Department_DepartmentId == Department.DepartmentId)
                .AsNoTracking().FirstOrDefault();

            var DetailAssigmnt = _DbContext.DepartmentsAssessmentTypeDetail.Where(item => item.DepartmentAssessmentTypeMaster_MasterId ==
            MasterAssigmnt.DepartmentAssessmentTypeMasterId).Include(item => item.assessmentType).AsNoTracking().ToList();



            return Json(new { DetailAssigmnt });
        }



        public IActionResult ManageAssessmentajax()
        {

            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();
            var Department = _DbContext.Departments.Where(item => item.Responsible_UserId == RUserId).AsNoTracking().FirstOrDefault();




            var MasterAssigmnt = _DbContext.DepartmentsAssessmentTypeMaster.Where(item => item.Department_DepartmentId == Department.DepartmentId)
           .AsNoTracking().FirstOrDefault();

            IEnumerable<AssessmentType> Assiment = Enumerable.Empty<AssessmentType>();

            if(MasterAssigmnt != null)
            {

                Assiment = _DbContext.AssessmentTypes.FromSqlRaw($"SELECT * FROM AssessmentTypes WHERE AssessmentTypeId NOT IN " +
                    $"(SELECT AssessmentType_AssessmentTypeId FROM DepartmentsAssessmentTypeDetail WHERE DepartmentAssessmentTypeMaster_MasterId ={MasterAssigmnt.DepartmentAssessmentTypeMasterId})").AsNoTracking().ToList();

                return Json(new { Assiment });

            }

            Assiment = _DbContext.AssessmentTypes.AsNoTracking().ToList();



            return Json(new { Assiment });
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
        public IActionResult GetStudentsList(string? gender, int? UpdwGPA, int? UpdwHOUERS, int? StutReqStatus)
        {


            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;


            var Department = _DbContext.Departments.Where(item => item.Responsible_UserId == RUserId).AsNoTracking().FirstOrDefault();


            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Department.Organization_OrganizationId).AsNoTracking().FirstOrDefault();

            var college = _DbContext.UniversityColleges.Where(item => item.Organization_OrganizationId == OrganizationOfR.OrganizationId).AsNoTracking().FirstOrDefault();


            IEnumerable<UniversityTraineeStudent> Students = Enumerable.Empty<UniversityTraineeStudent>();

            if (StutReqStatus == 0)
            {
                if (gender == null && UpdwGPA == 0 && UpdwHOUERS == 0)
                {

                    Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId}")
                               .AsNoTracking().Include(item => item.user).AsNoTracking().ToList();

                    return Json(new { Students });
                }


                if (gender == null && UpdwGPA != 0 && UpdwHOUERS != 0)
                {



                    if (UpdwHOUERS == 1 && UpdwGPA == 1)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} ")
                            .AsNoTracking().Include(item => item.user).AsNoTracking().OrderBy(item => item.CompletedHours).ThenBy(item => item.GPA).ToList();

                        return Json(new { Students });
                    }

                    if (UpdwHOUERS == 2 && UpdwGPA == 1)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} ")
                                  .AsNoTracking().Include(item => item.user).AsNoTracking().OrderByDescending(item => item.CompletedHours).ThenBy(item => item.GPA).ToList();


                        return Json(new { Students });
                    }

                    if (UpdwHOUERS == 1 && UpdwGPA == 2)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} ")
                                  .AsNoTracking().Include(item => item.user).AsNoTracking().OrderBy(item => item.CompletedHours).ThenByDescending(item => item.GPA).ToList();


                        return Json(new { Students });
                    }


                    if (UpdwHOUERS == 2 && UpdwGPA == 2)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} ")
                                  .AsNoTracking().Include(item => item.user).AsNoTracking().OrderByDescending(item => item.CompletedHours).ThenByDescending(item => item.GPA).ToList();


                        return Json(new { Students });
                    }







                }


                if (gender == null && UpdwGPA == 0 && UpdwHOUERS != 0)
                {



                    if (UpdwHOUERS == 1)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId}")
                            .AsNoTracking().Include(item => item.user).AsNoTracking().OrderBy(item => item.CompletedHours).ToList();

                        return Json(new { Students });
                    }

                    if (UpdwHOUERS == 2)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId}")
                            .AsNoTracking().Include(item => item.user).AsNoTracking().OrderByDescending(item => item.CompletedHours).ToList();

                        return Json(new { Students });
                    }


                }


                if (gender == null && UpdwGPA != 0 && UpdwHOUERS == 0)
                {



                    if (UpdwGPA == 1)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId}")
                            .AsNoTracking().Include(item => item.user).AsNoTracking().OrderBy(item => item.GPA).ToList();

                        return Json(new { Students });
                    }

                    if (UpdwGPA == 2)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId}")
                            .AsNoTracking().Include(item => item.user).AsNoTracking().OrderByDescending(item => item.GPA).ToList();

                        return Json(new { Students });
                    }


                }


                if (gender != null && UpdwGPA == 0 && UpdwHOUERS == 0)
                {

                    Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} AND Gender ='{gender}'")
                               .AsNoTracking().Include(item => item.user).AsNoTracking().ToList();

                    return Json(new { Students });
                }

                if (gender != null && UpdwGPA != 0 && UpdwHOUERS == 0)
                {



                    if (UpdwGPA == 1)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} AND Gender ='{gender}'")
                            .AsNoTracking().Include(item => item.user).AsNoTracking().OrderBy(item => item.GPA).ToList();

                        return Json(new { Students });
                    }

                    if (UpdwGPA == 2)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} AND Gender ='{gender}'")
                            .AsNoTracking().Include(item => item.user).AsNoTracking().OrderByDescending(item => item.GPA).ToList();

                        return Json(new { Students });
                    }


                }

                if (gender != null && UpdwGPA == 0 && UpdwHOUERS != 0)
                {



                    if (UpdwHOUERS == 1)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} AND Gender ='{gender}'")
                            .AsNoTracking().Include(item => item.user).AsNoTracking().OrderBy(item => item.CompletedHours).ToList();

                        return Json(new { Students });
                    }

                    if (UpdwHOUERS == 2)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} AND Gender ='{gender}'")
                            .AsNoTracking().Include(item => item.user).AsNoTracking().OrderByDescending(item => item.CompletedHours).ToList();

                        return Json(new { Students });
                    }


                }

                if (gender != null && UpdwGPA != 0 && UpdwHOUERS != 0)
                {



                    if (UpdwHOUERS == 1 && UpdwGPA == 1)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} AND Gender ='{gender}'")
                            .AsNoTracking().Include(item => item.user).AsNoTracking().OrderBy(item => item.CompletedHours).ThenBy(item => item.GPA).ToList();

                        return Json(new { Students });
                    }

                    if (UpdwHOUERS == 2 && UpdwGPA == 1)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} AND Gender ='{gender}'")
                                  .AsNoTracking().Include(item => item.user).AsNoTracking().OrderByDescending(item => item.CompletedHours).ThenBy(item => item.GPA).ToList();


                        return Json(new { Students });
                    }

                    if (UpdwHOUERS == 1 && UpdwGPA == 2)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} AND Gender ='{gender}'")
                                  .AsNoTracking().Include(item => item.user).AsNoTracking().OrderBy(item => item.CompletedHours).ThenByDescending(item => item.GPA).ToList();


                        return Json(new { Students });
                    }


                    if (UpdwHOUERS == 2 && UpdwGPA == 2)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} AND Gender ='{gender}'")
                                  .AsNoTracking().Include(item => item.user).AsNoTracking().OrderByDescending(item => item.CompletedHours).ThenByDescending(item => item.GPA).ToList();


                        return Json(new { Students });
                    }



                }

            }


            if (StutReqStatus == 1)
            {
                if (gender == null && UpdwGPA == 0 && UpdwHOUERS == 0)
                {

                    Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId}" +
                        $"AND TraineeId IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'waitingStudentApprove' OR DecisionStatus = 'waiting')")
                               .AsNoTracking().Include(item => item.user).AsNoTracking().ToList();

                    return Json(new { Students });
                }


                if (gender == null && UpdwGPA != 0 && UpdwHOUERS != 0)
                {



                    if (UpdwHOUERS == 1 && UpdwGPA == 1)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId}" +
                            $"AND TraineeId IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'waitingStudentApprove' OR DecisionStatus = 'waiting')")
                            .AsNoTracking().Include(item => item.user).AsNoTracking().OrderBy(item => item.CompletedHours).ThenBy(item => item.GPA).ToList();

                        return Json(new { Students });
                    }

                    if (UpdwHOUERS == 2 && UpdwGPA == 1)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} " +
                            $"AND TraineeId IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'waitingStudentApprove' OR DecisionStatus = 'waiting')")
                                  .AsNoTracking().Include(item => item.user).AsNoTracking().OrderByDescending(item => item.CompletedHours).ThenBy(item => item.GPA).ToList();


                        return Json(new { Students });
                    }

                    if (UpdwHOUERS == 1 && UpdwGPA == 2)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} " +
                            $"AND TraineeId IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'waitingStudentApprove' OR DecisionStatus = 'waiting')")
                                  .AsNoTracking().Include(item => item.user).AsNoTracking().OrderBy(item => item.CompletedHours).ThenByDescending(item => item.GPA).ToList();


                        return Json(new { Students });
                    }


                    if (UpdwHOUERS == 2 && UpdwGPA == 2)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId}" +
                            $"AND TraineeId IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'waitingStudentApprove' OR DecisionStatus = 'waiting')")
                                  .AsNoTracking().Include(item => item.user).AsNoTracking().OrderByDescending(item => item.CompletedHours).ThenByDescending(item => item.GPA).ToList();


                        return Json(new { Students });
                    }







                }


                if (gender == null && UpdwGPA == 0 && UpdwHOUERS != 0)
                {



                    if (UpdwHOUERS == 1)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId}" +
                            $"AND TraineeId IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'waitingStudentApprove' OR DecisionStatus = 'waiting')")
                            .AsNoTracking().Include(item => item.user).AsNoTracking().OrderBy(item => item.CompletedHours).ToList();

                        return Json(new { Students });
                    }

                    if (UpdwHOUERS == 2)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId}" +
                            $"AND TraineeId IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'waitingStudentApprove' OR DecisionStatus = 'waiting')")
                            .AsNoTracking().Include(item => item.user).AsNoTracking().OrderByDescending(item => item.CompletedHours).ToList();

                        return Json(new { Students });
                    }


                }


                if (gender == null && UpdwGPA != 0 && UpdwHOUERS == 0)
                {



                    if (UpdwGPA == 1)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId}" +
                            $"AND TraineeId IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'waitingStudentApprove' OR DecisionStatus = 'waiting')")
                            .AsNoTracking().Include(item => item.user).AsNoTracking().OrderBy(item => item.GPA).ToList();

                        return Json(new { Students });
                    }

                    if (UpdwGPA == 2)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId}" +
                            $"AND TraineeId IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'waitingStudentApprove' OR DecisionStatus = 'waiting')")
                            .AsNoTracking().Include(item => item.user).AsNoTracking().OrderByDescending(item => item.GPA).ToList();

                        return Json(new { Students });
                    }


                }


                if (gender != null && UpdwGPA == 0 && UpdwHOUERS == 0)
                {

                    Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} AND Gender ='{gender}'" +
                        $"AND TraineeId IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'waitingStudentApprove' OR DecisionStatus = 'waiting')")
                               .AsNoTracking().Include(item => item.user).AsNoTracking().ToList();

                    return Json(new { Students });
                }

                if (gender != null && UpdwGPA != 0 && UpdwHOUERS == 0)
                {



                    if (UpdwGPA == 1)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} AND Gender ='{gender}'" +
                            $"AND TraineeId IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'waitingStudentApprove' OR DecisionStatus = 'waiting')")
                            .AsNoTracking().Include(item => item.user).AsNoTracking().OrderBy(item => item.GPA).ToList();

                        return Json(new { Students });
                    }

                    if (UpdwGPA == 2)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} AND Gender ='{gender}'" +
                            $"AND TraineeId IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'waitingStudentApprove' OR DecisionStatus = 'waiting')")
                            .AsNoTracking().Include(item => item.user).AsNoTracking().OrderByDescending(item => item.GPA).ToList();

                        return Json(new { Students });
                    }


                }

                if (gender != null && UpdwGPA == 0 && UpdwHOUERS != 0)
                {



                    if (UpdwHOUERS == 1)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} AND Gender ='{gender}'" +
                            $"AND TraineeId IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'waitingStudentApprove' OR DecisionStatus = 'waiting')")
                            .AsNoTracking().Include(item => item.user).AsNoTracking().OrderBy(item => item.CompletedHours).ToList();

                        return Json(new { Students });
                    }

                    if (UpdwHOUERS == 2)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} AND Gender ='{gender}'" +
                            $"AND TraineeId IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'waitingStudentApprove' OR DecisionStatus = 'waiting')")
                            .AsNoTracking().Include(item => item.user).AsNoTracking().OrderByDescending(item => item.CompletedHours).ToList();

                        return Json(new { Students });
                    }


                }

                if (gender != null && UpdwGPA != 0 && UpdwHOUERS != 0)
                {



                    if (UpdwHOUERS == 1 && UpdwGPA == 1)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} AND Gender ='{gender}'" +
                            $"AND TraineeId IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'waitingStudentApprove' OR DecisionStatus = 'waiting')")
                            .AsNoTracking().Include(item => item.user).AsNoTracking().OrderBy(item => item.CompletedHours).ThenBy(item => item.GPA).ToList();

                        return Json(new { Students });
                    }

                    if (UpdwHOUERS == 2 && UpdwGPA == 1)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} AND Gender ='{gender}'" +
                            $"AND TraineeId IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'waitingStudentApprove' OR DecisionStatus = 'waiting')")
                                  .AsNoTracking().Include(item => item.user).AsNoTracking().OrderByDescending(item => item.CompletedHours).ThenBy(item => item.GPA).ToList();


                        return Json(new { Students });
                    }

                    if (UpdwHOUERS == 1 && UpdwGPA == 2)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} AND Gender ='{gender}'" +
                            $"AND TraineeId IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'waitingStudentApprove' OR DecisionStatus = 'waiting')")
                                  .AsNoTracking().Include(item => item.user).AsNoTracking().OrderBy(item => item.CompletedHours).ThenByDescending(item => item.GPA).ToList();


                        return Json(new { Students });
                    }


                    if (UpdwHOUERS == 2 && UpdwGPA == 2)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} AND Gender ='{gender}'" +
                            $"AND TraineeId IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'waitingStudentApprove' OR DecisionStatus = 'waiting')")
                                  .AsNoTracking().Include(item => item.user).AsNoTracking().OrderByDescending(item => item.CompletedHours).ThenByDescending(item => item.GPA).ToList();


                        return Json(new { Students });
                    }



                }

            }


            if (StutReqStatus == 2)
            {
                if (gender == null && UpdwGPA == 0 && UpdwHOUERS == 0)
                {

                    Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId}" +
                        $"AND TraineeId NOT IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities)")
                               .AsNoTracking().Include(item => item.user).AsNoTracking().ToList();

                    return Json(new { Students });
                }


                if (gender == null && UpdwGPA != 0 && UpdwHOUERS != 0)
                {



                    if (UpdwHOUERS == 1 && UpdwGPA == 1)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} " +
                            $"AND TraineeId NOT IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities)")
                            .AsNoTracking().Include(item => item.user).AsNoTracking().OrderBy(item => item.CompletedHours).ThenBy(item => item.GPA).ToList();

                        return Json(new { Students });
                    }

                    if (UpdwHOUERS == 2 && UpdwGPA == 1)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} " +
                            $"AND TraineeId NOT IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities)")
                                  .AsNoTracking().Include(item => item.user).AsNoTracking().OrderByDescending(item => item.CompletedHours).ThenBy(item => item.GPA).ToList();


                        return Json(new { Students });
                    }

                    if (UpdwHOUERS == 1 && UpdwGPA == 2)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} " +
                            $"AND TraineeId NOT IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities)")
                                  .AsNoTracking().Include(item => item.user).AsNoTracking().OrderBy(item => item.CompletedHours).ThenByDescending(item => item.GPA).ToList();


                        return Json(new { Students });
                    }


                    if (UpdwHOUERS == 2 && UpdwGPA == 2)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} " +
                            $"AND TraineeId NOT IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities)")
                                  .AsNoTracking().Include(item => item.user).AsNoTracking().OrderByDescending(item => item.CompletedHours).ThenByDescending(item => item.GPA).ToList();


                        return Json(new { Students });
                    }


                }


                if (gender == null && UpdwGPA == 0 && UpdwHOUERS != 0)
                {



                    if (UpdwHOUERS == 1)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId}" +
                            $"AND TraineeId NOT IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities)")
                            .AsNoTracking().Include(item => item.user).AsNoTracking().OrderBy(item => item.CompletedHours).ToList();

                        return Json(new { Students });
                    }

                    if (UpdwHOUERS == 2)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId}" +
                            $"AND TraineeId NOT IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities)")
                            .AsNoTracking().Include(item => item.user).AsNoTracking().OrderByDescending(item => item.CompletedHours).ToList();

                        return Json(new { Students });
                    }


                }


                if (gender == null && UpdwGPA != 0 && UpdwHOUERS == 0)
                {



                    if (UpdwGPA == 1)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId}" +
                            $"AND TraineeId NOT IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities)")
                            .AsNoTracking().Include(item => item.user).AsNoTracking().OrderBy(item => item.GPA).ToList();

                        return Json(new { Students });
                    }

                    if (UpdwGPA == 2)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId}" +
                            $"AND TraineeId NOT IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities)")
                            .AsNoTracking().Include(item => item.user).AsNoTracking().OrderByDescending(item => item.GPA).ToList();

                        return Json(new { Students });
                    }


                }


                if (gender != null && UpdwGPA == 0 && UpdwHOUERS == 0)
                {

                    Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} AND Gender ='{gender}'" +
                        $"AND TraineeId NOT IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities)")
                               .AsNoTracking().Include(item => item.user).AsNoTracking().ToList();

                    return Json(new { Students });
                }

                if (gender != null && UpdwGPA != 0 && UpdwHOUERS == 0)
                {



                    if (UpdwGPA == 1)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} AND Gender ='{gender}'" +
                            $"AND TraineeId NOT IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities)")
                            .AsNoTracking().Include(item => item.user).AsNoTracking().OrderBy(item => item.GPA).ToList();

                        return Json(new { Students });
                    }

                    if (UpdwGPA == 2)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} AND Gender ='{gender}'" +
                            $"AND TraineeId NOT IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities)")
                            .AsNoTracking().Include(item => item.user).AsNoTracking().OrderByDescending(item => item.GPA).ToList();

                        return Json(new { Students });
                    }


                }

                if (gender != null && UpdwGPA == 0 && UpdwHOUERS != 0)
                {



                    if (UpdwHOUERS == 1)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} AND Gender ='{gender}'" +
                            $"AND TraineeId NOT IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities)")
                            .AsNoTracking().Include(item => item.user).AsNoTracking().OrderBy(item => item.CompletedHours).ToList();

                        return Json(new { Students });
                    }

                    if (UpdwHOUERS == 2)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} AND Gender ='{gender}'" +
                            $"AND TraineeId NOT IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities)")
                            .AsNoTracking().Include(item => item.user).AsNoTracking().OrderByDescending(item => item.CompletedHours).ToList();

                        return Json(new { Students });
                    }


                }

                if (gender != null && UpdwGPA != 0 && UpdwHOUERS != 0)
                {



                    if (UpdwHOUERS == 1 && UpdwGPA == 1)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} AND Gender ='{gender}'" +
                            $"AND TraineeId NOT IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities)")
                            .AsNoTracking().Include(item => item.user).AsNoTracking().OrderBy(item => item.CompletedHours).ThenBy(item => item.GPA).ToList();

                        return Json(new { Students });
                    }

                    if (UpdwHOUERS == 2 && UpdwGPA == 1)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} AND Gender ='{gender}'" +
                            $"AND TraineeId NOT IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities)")
                                  .AsNoTracking().Include(item => item.user).AsNoTracking().OrderByDescending(item => item.CompletedHours).ThenBy(item => item.GPA).ToList();


                        return Json(new { Students });
                    }

                    if (UpdwHOUERS == 1 && UpdwGPA == 2)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} AND Gender ='{gender}'" +
                            $"AND TraineeId NOT IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities)")
                                  .AsNoTracking().Include(item => item.user).AsNoTracking().OrderBy(item => item.CompletedHours).ThenByDescending(item => item.GPA).ToList();


                        return Json(new { Students });
                    }


                    if (UpdwHOUERS == 2 && UpdwGPA == 2)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} AND Gender ='{gender}'" +
                            $"AND TraineeId NOT IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities)")
                                  .AsNoTracking().Include(item => item.user).AsNoTracking().OrderByDescending(item => item.CompletedHours).ThenByDescending(item => item.GPA).ToList();


                        return Json(new { Students });
                    }



                }

            }



            if (StutReqStatus == 3)

            {
                if (gender == null && UpdwGPA == 0 && UpdwHOUERS == 0)
                {

                    Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} " +
                        $"AND TraineeId IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'rejected' OR DecisionStatus = 'system disable' OR  DecisionStatus = 'CancelBeforeApprove' OR  DecisionStatus = 'CancelAftereApprove')" +
                        $"AND TraineeId NOT IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'approved')")
                               .AsNoTracking().Include(item => item.user).AsNoTracking().ToList();

                    return Json(new { Students });
                }


                if (gender == null && UpdwGPA != 0 && UpdwHOUERS != 0)
                {



                    if (UpdwHOUERS == 1 && UpdwGPA == 1)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} " +
                            $"AND TraineeId IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'rejected' OR DecisionStatus = 'system disable' OR  DecisionStatus = 'CancelBeforeApprove' OR  DecisionStatus = 'CancelAftereApprove')" +
                            $"AND TraineeId NOT IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'approved')")
                            .AsNoTracking().Include(item => item.user).AsNoTracking().OrderBy(item => item.CompletedHours).ThenBy(item => item.GPA).ToList();

                        return Json(new { Students });
                    }

                    if (UpdwHOUERS == 2 && UpdwGPA == 1)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} " +
                            $"AND TraineeId IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'rejected' OR DecisionStatus = 'system disable' OR  DecisionStatus = 'CancelBeforeApprove' OR  DecisionStatus = 'CancelAftereApprove')" +
                            $"AND TraineeId NOT IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'approved')")
                                  .AsNoTracking().Include(item => item.user).AsNoTracking().OrderByDescending(item => item.CompletedHours).ThenBy(item => item.GPA).ToList();


                        return Json(new { Students });
                    }

                    if (UpdwHOUERS == 1 && UpdwGPA == 2)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} " +
                            $"AND TraineeId IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'rejected' OR DecisionStatus = 'system disable' OR  DecisionStatus = 'CancelBeforeApprove' OR  DecisionStatus = 'CancelAftereApprove')" +
                            $"AND TraineeId NOT IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'approved')")
                                  .AsNoTracking().Include(item => item.user).AsNoTracking().OrderBy(item => item.CompletedHours).ThenByDescending(item => item.GPA).ToList();


                        return Json(new { Students });
                    }


                    if (UpdwHOUERS == 2 && UpdwGPA == 2)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} " +
                            $"AND TraineeId IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'rejected' OR DecisionStatus = 'system disable' OR  DecisionStatus = 'CancelBeforeApprove' OR  DecisionStatus = 'CancelAftereApprove')" +
                            $"AND TraineeId NOT IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'approved')")
                                  .AsNoTracking().Include(item => item.user).AsNoTracking().OrderByDescending(item => item.CompletedHours).ThenByDescending(item => item.GPA).ToList();


                        return Json(new { Students });
                    }







                }


                if (gender == null && UpdwGPA == 0 && UpdwHOUERS != 0)
                {



                    if (UpdwHOUERS == 1)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId}" +
                         $" AND TraineeId IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'rejected' OR DecisionStatus = 'system disable' OR  DecisionStatus = 'CancelBeforeApprove' OR  DecisionStatus = 'CancelAftereApprove')" +
                         $"AND TraineeId NOT IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'approved')")
                         .AsNoTracking().Include(item => item.user).AsNoTracking().OrderBy(item => item.CompletedHours).ToList();
                        return Json(new { Students });
                    }

                    if (UpdwHOUERS == 2)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId}" +
                            $" AND TraineeId IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'rejected' OR DecisionStatus = 'system disable' OR  DecisionStatus = 'CancelBeforeApprove' OR  DecisionStatus = 'CancelAftereApprove')" +
                            $"AND TraineeId NOT IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'approved')")
                            .AsNoTracking().Include(item => item.user).AsNoTracking().OrderByDescending(item => item.CompletedHours).ToList();

                        return Json(new { Students });
                    }


                }


                if (gender == null && UpdwGPA != 0 && UpdwHOUERS == 0)
                {



                    if (UpdwGPA == 1)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId}" +
                            $"AND TraineeId IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'rejected' OR DecisionStatus = 'system disable' OR  DecisionStatus = 'CancelBeforeApprove' OR  DecisionStatus = 'CancelAftereApprove')" +
                            $"AND TraineeId NOT IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'approved')")
                            .AsNoTracking().Include(item => item.user).AsNoTracking().OrderBy(item => item.GPA).ToList();

                        return Json(new { Students });
                    }

                    if (UpdwGPA == 2)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId}" +
                            $"AND TraineeId IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'rejected' OR DecisionStatus = 'system disable' OR  DecisionStatus = 'CancelBeforeApprove' OR  DecisionStatus = 'CancelAftereApprove')" +
                            $"AND TraineeId NOT IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'approved')")
                            .AsNoTracking().Include(item => item.user).AsNoTracking().OrderByDescending(item => item.GPA).ToList();

                        return Json(new { Students });
                    }


                }


                if (gender != null && UpdwGPA == 0 && UpdwHOUERS == 0)
                {

                    Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} AND Gender ='{gender}'" +
                        $"AND TraineeId IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'rejected' OR DecisionStatus = 'system disable' OR  DecisionStatus = 'CancelBeforeApprove' OR  DecisionStatus = 'CancelAftereApprove')" +
                        $"AND TraineeId NOT IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'approved')")
                               .AsNoTracking().Include(item => item.user).AsNoTracking().ToList();

                    return Json(new { Students });
                }

                if (gender != null && UpdwGPA != 0 && UpdwHOUERS == 0)
                {



                    if (UpdwGPA == 1)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} AND Gender ='{gender}'" +
                            $"AND TraineeId IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'rejected' OR DecisionStatus = 'system disable' OR  DecisionStatus = 'CancelBeforeApprove' OR  DecisionStatus = 'CancelAftereApprove')" +
                            $"AND TraineeId NOT IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'approved')")
                            .AsNoTracking().Include(item => item.user).AsNoTracking().OrderBy(item => item.GPA).ToList();

                        return Json(new { Students });
                    }

                    if (UpdwGPA == 2)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} AND Gender ='{gender}'" +
                            $"AND TraineeId IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'rejected' OR DecisionStatus = 'system disable' OR  DecisionStatus = 'CancelBeforeApprove' OR  DecisionStatus = 'CancelAftereApprove')" +
                            $"AND TraineeId NOT IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'approved')")
                            .AsNoTracking().Include(item => item.user).AsNoTracking().OrderByDescending(item => item.GPA).ToList();

                        return Json(new { Students });
                    }


                }

                if (gender != null && UpdwGPA == 0 && UpdwHOUERS != 0)
                {



                    if (UpdwHOUERS == 1)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} AND Gender ='{gender}'" +
                            $"AND TraineeId IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'rejected' OR DecisionStatus = 'system disable' OR  DecisionStatus = 'CancelBeforeApprove' OR  DecisionStatus = 'CancelAftereApprove')" +
                            $"AND TraineeId NOT IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'approved')")
                            .AsNoTracking().Include(item => item.user).AsNoTracking().OrderBy(item => item.CompletedHours).ToList();

                        return Json(new { Students });
                    }

                    if (UpdwHOUERS == 2)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} AND Gender ='{gender}'" +
                            $"AND TraineeId IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'rejected' OR DecisionStatus = 'system disable' OR  DecisionStatus = 'CancelBeforeApprove' OR  DecisionStatus = 'CancelAftereApprove')" +
                            $"AND TraineeId NOT IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'approved')")
                            .AsNoTracking().Include(item => item.user).AsNoTracking().OrderByDescending(item => item.CompletedHours).ToList();

                        return Json(new { Students });
                    }


                }

                if (gender != null && UpdwGPA != 0 && UpdwHOUERS != 0)
                {



                    if (UpdwHOUERS == 1 && UpdwGPA == 1)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} AND Gender ='{gender}'" +
                            $"AND TraineeId IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'rejected' OR DecisionStatus = 'system disable' OR  DecisionStatus = 'CancelBeforeApprove' OR  DecisionStatus = 'CancelAftereApprove')" +
                            $"AND TraineeId NOT IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'approved')")
                            .AsNoTracking().Include(item => item.user).AsNoTracking().OrderBy(item => item.CompletedHours).ThenBy(item => item.GPA).ToList();

                        return Json(new { Students });
                    }

                    if (UpdwHOUERS == 2 && UpdwGPA == 1)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} AND Gender ='{gender}'" +
                            $"AND TraineeId IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'rejected' OR DecisionStatus = 'system disable' OR  DecisionStatus = 'CancelBeforeApprove' OR  DecisionStatus = 'CancelAftereApprove')" +
                            $"AND TraineeId NOT IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'approved')")
                                  .AsNoTracking().Include(item => item.user).AsNoTracking().OrderByDescending(item => item.CompletedHours).ThenBy(item => item.GPA).ToList();


                        return Json(new { Students });
                    }

                    if (UpdwHOUERS == 1 && UpdwGPA == 2)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} AND Gender ='{gender}'" +
                            $"AND TraineeId IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'rejected' OR DecisionStatus = 'system disable' OR  DecisionStatus = 'CancelBeforeApprove' OR  DecisionStatus = 'CancelAftereApprove')" +
                            $"AND TraineeId NOT IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'approved')")
                                  .AsNoTracking().Include(item => item.user).AsNoTracking().OrderBy(item => item.CompletedHours).ThenByDescending(item => item.GPA).ToList();


                        return Json(new { Students });
                    }


                    if (UpdwHOUERS == 2 && UpdwGPA == 2)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} AND Gender ='{gender}'" +
                            $"AND TraineeId IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'rejected' OR DecisionStatus = 'system disable' OR  DecisionStatus = 'CancelBeforeApprove' OR  DecisionStatus = 'CancelAftereApprove')" +
                            $"AND TraineeId NOT IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'approved')")
                                  .AsNoTracking().Include(item => item.user).AsNoTracking().OrderByDescending(item => item.CompletedHours).ThenByDescending(item => item.GPA).ToList();


                        return Json(new { Students });
                    }



                }

            }


            if (StutReqStatus == 4)
            {
                if (gender == null && UpdwGPA == 0 && UpdwHOUERS == 0)
                {

                    Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId}" +
                        $"AND TraineeId IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'approved')")
                               .AsNoTracking().Include(item => item.user).AsNoTracking().ToList();

                    return Json(new { Students });
                }


                if (gender == null && UpdwGPA != 0 && UpdwHOUERS != 0)
                {



                    if (UpdwHOUERS == 1 && UpdwGPA == 1)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId}" +
                            $"AND TraineeId IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'approved')")
                            .AsNoTracking().Include(item => item.user).AsNoTracking().OrderBy(item => item.CompletedHours).ThenBy(item => item.GPA).ToList();

                        return Json(new { Students });
                    }

                    if (UpdwHOUERS == 2 && UpdwGPA == 1)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} " +
                            $"AND TraineeId IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'approved')")
                                  .AsNoTracking().Include(item => item.user).AsNoTracking().OrderByDescending(item => item.CompletedHours).ThenBy(item => item.GPA).ToList();


                        return Json(new { Students });
                    }

                    if (UpdwHOUERS == 1 && UpdwGPA == 2)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} " +
                            $"AND TraineeId IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'approved')")
                                  .AsNoTracking().Include(item => item.user).AsNoTracking().OrderBy(item => item.CompletedHours).ThenByDescending(item => item.GPA).ToList();


                        return Json(new { Students });
                    }


                    if (UpdwHOUERS == 2 && UpdwGPA == 2)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId}" +
                            $"AND TraineeId IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'approved')")
                                  .AsNoTracking().Include(item => item.user).AsNoTracking().OrderByDescending(item => item.CompletedHours).ThenByDescending(item => item.GPA).ToList();


                        return Json(new { Students });
                    }







                }


                if (gender == null && UpdwGPA == 0 && UpdwHOUERS != 0)
                {



                    if (UpdwHOUERS == 1)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId}" +
                            $"AND TraineeId IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'approved')")
                            .AsNoTracking().Include(item => item.user).AsNoTracking().OrderBy(item => item.CompletedHours).ToList();

                        return Json(new { Students });
                    }

                    if (UpdwHOUERS == 2)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId}" +
                            $"AND TraineeId IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'approved')")
                            .AsNoTracking().Include(item => item.user).AsNoTracking().OrderByDescending(item => item.CompletedHours).ToList();

                        return Json(new { Students });
                    }


                }


                if (gender == null && UpdwGPA != 0 && UpdwHOUERS == 0)
                {



                    if (UpdwGPA == 1)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId}" +
                            $"AND TraineeId IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'approved')")
                            .AsNoTracking().Include(item => item.user).AsNoTracking().OrderBy(item => item.GPA).ToList();

                        return Json(new { Students });
                    }

                    if (UpdwGPA == 2)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId}" +
                            $"AND TraineeId IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'approved')")
                            .AsNoTracking().Include(item => item.user).AsNoTracking().OrderByDescending(item => item.GPA).ToList();

                        return Json(new { Students });
                    }


                }


                if (gender != null && UpdwGPA == 0 && UpdwHOUERS == 0)
                {

                    Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} AND Gender ='{gender}'" +
                        $"AND TraineeId IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'approved')")
                               .AsNoTracking().Include(item => item.user).AsNoTracking().ToList();

                    return Json(new { Students });
                }

                if (gender != null && UpdwGPA != 0 && UpdwHOUERS == 0)
                {



                    if (UpdwGPA == 1)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} AND Gender ='{gender}'" +
                            $"AND TraineeId IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'approved')")
                            .AsNoTracking().Include(item => item.user).AsNoTracking().OrderBy(item => item.GPA).ToList();

                        return Json(new { Students });
                    }

                    if (UpdwGPA == 2)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} AND Gender ='{gender}'" +
                            $"AND TraineeId IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'approved')")
                            .AsNoTracking().Include(item => item.user).AsNoTracking().OrderByDescending(item => item.GPA).ToList();

                        return Json(new { Students });
                    }


                }

                if (gender != null && UpdwGPA == 0 && UpdwHOUERS != 0)
                {



                    if (UpdwHOUERS == 1)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} AND Gender ='{gender}'" +
                            $"AND TraineeId IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'approved')")
                            .AsNoTracking().Include(item => item.user).AsNoTracking().OrderBy(item => item.CompletedHours).ToList();

                        return Json(new { Students });
                    }

                    if (UpdwHOUERS == 2)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} AND Gender ='{gender}'" +
                            $"AND TraineeId IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'approved')")
                            .AsNoTracking().Include(item => item.user).AsNoTracking().OrderByDescending(item => item.CompletedHours).ToList();

                        return Json(new { Students });
                    }


                }

                if (gender != null && UpdwGPA != 0 && UpdwHOUERS != 0)
                {



                    if (UpdwHOUERS == 1 && UpdwGPA == 1)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} AND Gender ='{gender}'" +
                            $"AND TraineeId IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'approved')")
                            .AsNoTracking().Include(item => item.user).AsNoTracking().OrderBy(item => item.CompletedHours).ThenBy(item => item.GPA).ToList();

                        return Json(new { Students });
                    }

                    if (UpdwHOUERS == 2 && UpdwGPA == 1)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} AND Gender ='{gender}'" +
                            $"AND TraineeId IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'approved')")
                                  .AsNoTracking().Include(item => item.user).AsNoTracking().OrderByDescending(item => item.CompletedHours).ThenBy(item => item.GPA).ToList();


                        return Json(new { Students });
                    }

                    if (UpdwHOUERS == 1 && UpdwGPA == 2)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} AND Gender ='{gender}'" +
                            $"AND TraineeId IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'approved')")
                                  .AsNoTracking().Include(item => item.user).AsNoTracking().OrderBy(item => item.CompletedHours).ThenByDescending(item => item.GPA).ToList();


                        return Json(new { Students });
                    }


                    if (UpdwHOUERS == 2 && UpdwGPA == 2)
                    {
                        Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE Department_DepartmentId ={Department.DepartmentId} AND Gender ='{gender}'" +
                            $"AND TraineeId IN (SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities  WHERE DecisionStatus = 'approved')")
                                  .AsNoTracking().Include(item => item.user).AsNoTracking().OrderByDescending(item => item.CompletedHours).ThenByDescending(item => item.GPA).ToList();


                        return Json(new { Students });
                    }



                }

            }




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


        [HttpPost]
        public IActionResult UpdateAssessmentTypeMasterAndItsDetail(string? StartDate, string? RequireHours, string? AcademicMarks, string? TrainingMarks
            , string? AssessmentTypeIds, string? RequiredMarks, string? RequiredMarksToUpdate)
        {


            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;


            var Department = _DbContext.Departments.Where(item => item.Responsible_UserId == RUserId).AsNoTracking().FirstOrDefault();


            var Mastr = _DbContext.DepartmentsAssessmentTypeMaster.Where(item => item.Department_DepartmentId == Department.DepartmentId).AsNoTracking().FirstOrDefault();

            if (Mastr == null)
            {

                return Json(new { success = false });

            }


            if (!String.IsNullOrEmpty(StartDate))
            {
                Mastr.StartActivationDate = DateTime.Parse(StartDate);
            }


            if (!String.IsNullOrEmpty(RequireHours))
            {
                Mastr.RequireCompletionHours = int.Parse(RequireHours);
            }


            if (!String.IsNullOrEmpty(AcademicMarks))
            {

                Mastr.AcademicSupervisorMarks = int.Parse(AcademicMarks);
            }

            if (!String.IsNullOrEmpty(TrainingMarks))
            {

                Mastr.TrainingSupervisorMarks = int.Parse(TrainingMarks);
            }


            _DbContext.DepartmentsAssessmentTypeMaster.Update(Mastr);

            _DbContext.SaveChanges();


            var DetailList = _DbContext.DepartmentsAssessmentTypeDetail.Where(item => item.DepartmentAssessmentTypeMaster_MasterId == Mastr.DepartmentAssessmentTypeMasterId).AsNoTracking().ToList();

            var RequiredMarksToUpdateList = RequiredMarksToUpdate.Split(",");

            if (!String.IsNullOrEmpty(RequiredMarksToUpdate))

            {
                for (int i = 0; i < DetailList.Count; i++)
                {
                    if (DetailList[i].RequiredMark != int.Parse(RequiredMarksToUpdateList[i]))

                    {

                        var DetailL = _DbContext.DepartmentsAssessmentTypeDetail.Find(DetailList[i].DepartmentAssessmentTypeDetailId);

                        DetailL.RequiredMark = int.Parse(RequiredMarksToUpdateList[i]);

                        _DbContext.DepartmentsAssessmentTypeDetail.Update(DetailL);

                    }


                }

                _DbContext.SaveChanges();
            }


            if (!String.IsNullOrEmpty(AssessmentTypeIds) && !String.IsNullOrEmpty(RequiredMarks))
            {

                var AssessmentIds = AssessmentTypeIds.Split(",");
                var RequiredMarksList = RequiredMarks.Split(",");

                for (int i = 0; i < AssessmentIds.Length; i++)
                {


                    var DTypeDetail = new DepartmentAssessmentTypeDetail()
                    {

                        DepartmentAssessmentTypeMaster_MasterId = Mastr.DepartmentAssessmentTypeMasterId,
                        AssessmentType_AssessmentTypeId = int.Parse(AssessmentIds[i]),
                        RequiredMark = int.Parse(RequiredMarksList[i]),
                    };

                    _DbContext.DepartmentsAssessmentTypeDetail.Add(DTypeDetail);
                }

                _DbContext.SaveChanges();


            }




            TempData["success"] = "تم تعديل  تقسم الدرجات بنجاح";

            return Json(new { success = true });


        }


        [HttpPost]
        public IActionResult AddAssessmentTypeMasterAndItsDetail(string? StartDate, string? RequireHours, string? AcademicMarks, string? TrainingMarks
            , string? AssessmentTypeIds, string? RequiredMarks)
        {
            if (String.IsNullOrEmpty(StartDate) || String.IsNullOrEmpty(RequireHours) || String.IsNullOrEmpty(AcademicMarks) || String.IsNullOrEmpty(TrainingMarks)
                || String.IsNullOrEmpty(AssessmentTypeIds) || String.IsNullOrEmpty(RequiredMarks))
            {
                return Json(new { success = false });
            }

            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;


            var Department = _DbContext.Departments.Where(item => item.Responsible_UserId == RUserId).AsNoTracking().FirstOrDefault();

            var Employee = _DbContext.Employees.Where(item => item.UserAccount_UserId == RUserId).AsNoTracking().FirstOrDefault();



            DepartmentAssessmentTypeMaster DTypeMaster = new DepartmentAssessmentTypeMaster()
            {
                Department_DepartmentId = Department.DepartmentId,
                Employee_EmployeeId = Employee.EmployeeId,
                StartActivationDate = DateTime.Parse(StartDate),
                RequireCompletionHours = int.Parse(RequireHours),
                AcademicSupervisorMarks = int.Parse(AcademicMarks),
                TrainingSupervisorMarks = int.Parse(TrainingMarks),

            };


            _DbContext.DepartmentsAssessmentTypeMaster.Add(DTypeMaster);

            _DbContext.SaveChanges();

            var DTypeMasterFromDb = _DbContext.DepartmentsAssessmentTypeMaster.Where(item => item.Department_DepartmentId == DTypeMaster.Department_DepartmentId).AsNoTracking()
                .FirstOrDefault();

            var AssessmentIds = AssessmentTypeIds.Split(",");
            var RequiredMarksList = RequiredMarks.Split(",");

            for (int i = 0; i < AssessmentIds.Length; i++)
            {

                //Console.WriteLine(AssessmentIds[i] + " AND " + RequiredMarksList[i]);

                var DTypeDetail = new DepartmentAssessmentTypeDetail()
                {

                    DepartmentAssessmentTypeMaster_MasterId = DTypeMasterFromDb.DepartmentAssessmentTypeMasterId,
                    AssessmentType_AssessmentTypeId = int.Parse(AssessmentIds[i]),
                    RequiredMark = int.Parse(RequiredMarksList[i]),
                };

                _DbContext.DepartmentsAssessmentTypeDetail.Add(DTypeDetail);
            }

            _DbContext.SaveChanges();


            TempData["success"] = "تم إضافة  تقسم الدرجات بنجاح";

            return Json(new { success = true });


        }


        [HttpGet]
        public IActionResult getAssessmentTypeDetail(int? id)
        {

            IEnumerable<DepartmentAssessmentTypeDetail> AssessmentTypeDetail = Enumerable.Empty<DepartmentAssessmentTypeDetail>();

            if (id == null || id == 0)
            {

                return Json(new { AssessmentTypeDetail });

            }


            AssessmentTypeDetail = _DbContext.DepartmentsAssessmentTypeDetail.Where(item => item.DepartmentAssessmentTypeMaster_MasterId == id).Include(item=> item.assessmentType).AsNoTracking().ToList();





            return Json(new { AssessmentTypeDetail });

        }


        #endregion




    }


}
