using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TadarbProject.Data;
using TadarbProject.Models;
using TadarbProject.Models.ViewModels;
using TEST2.Services;

namespace TadarbProject.Controllers
{
    public class BranchController : Controller
    {
        private readonly AppDbContext _DbContext;
        private readonly IEmailSender _emailSender;
        private readonly IHttpContextAccessor _HttpContextAccessor;


        public BranchController(AppDbContext DbContext, IEmailSender emailSender, IHttpContextAccessor HttpContextAccessor)
        {
            _DbContext = DbContext;

            _emailSender = emailSender;
            _HttpContextAccessor = HttpContextAccessor;

        }
        public IActionResult Index()
        {

            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");
            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).FirstOrDefault();
            var Branch = _DbContext.OrganizationBranches_TrainProv.Where(item => item.Responsible_UserId == RUserId).FirstOrDefault();

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Branch.Organization_OrganizationId).FirstOrDefault();


            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Branch.BranchName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;
            return View();

        }

        [HttpGet]
        public IActionResult ManageDepartment()
        {

            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");
            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).FirstOrDefault();
            var Branch = _DbContext.OrganizationBranches_TrainProv.Where(item => item.Responsible_UserId == RUserId).FirstOrDefault();

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Branch.Organization_OrganizationId).FirstOrDefault();


            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Branch.BranchName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;
            return View();
        }

        [HttpGet]
        public IActionResult ViewSupervisorsUser()
        {

            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");

            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).FirstOrDefault();
            var Branch = _DbContext.OrganizationBranches_TrainProv.Where(item => item.Responsible_UserId == RUserId).FirstOrDefault();

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Branch.Organization_OrganizationId).FirstOrDefault();


            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Branch.BranchName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;
            var DEPOfR = _DbContext.Departments.Where(item => item.Branch_BranchId == Branch.BranchId && item.DepartmentName.Equals("قسم ادارة مشرفين التدريب")).FirstOrDefault();


            IEnumerable<UserAcount> Employees = Enumerable.Empty<UserAcount>();


            if (DEPOfR != null)
            {

                Employees = _DbContext.UserAcounts.FromSqlRaw($"Select * from UserAcounts WHERE UserId IN (Select UserAccount_UserId from Employees WHERE Department_DepartmentId = {DEPOfR.DepartmentId});").ToList();


            }



            return View(Employees);
        }


        [HttpGet]
        public IActionResult AddSupervisorUser()
        {
            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");

            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).FirstOrDefault();
            var Branch = _DbContext.OrganizationBranches_TrainProv.Where(item => item.Responsible_UserId == RUserId).FirstOrDefault();

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Branch.Organization_OrganizationId).FirstOrDefault();


            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Branch.BranchName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;


            return View();

        }
        [HttpPost]
        public IActionResult AddSupervisorUser(EmployeeVM employeeVM)
        {
            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;

            var RUser = _DbContext.UserAcounts.Find(RUserId);

            var Branch = _DbContext.OrganizationBranches_TrainProv.Where(item => item.Responsible_UserId == RUserId).FirstOrDefault();
            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Branch.Organization_OrganizationId).FirstOrDefault();


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
                City_CityId = Branch.City_CityId,
                UserType = "Training_Supervisor",
                ActivationStatus = "Active"

            };

            _DbContext.UserAcounts.Add(user);

            _DbContext.SaveChanges();

            var DEPOfR = _DbContext.Departments.Where(item => item.Branch_BranchId == Branch.BranchId && item.DepartmentName.Equals("قسم ادارة مشرفين التدريب")).FirstOrDefault();

            if (DEPOfR == null)
            {
                var DEP = new Department
                {

                    DepartmentName = "قسم ادارة مشرفين التدريب",
                    Branch_BranchId = Branch.BranchId,
                    Responsible_UserId = Branch.Responsible_UserId,
                    Organization_OrganizationId = Branch.Organization_OrganizationId


                };

                _DbContext.Departments.Add(DEP);

                _DbContext.SaveChanges();
            }

            int DEPId = _DbContext.Departments.Where(item => item.Branch_BranchId == Branch.BranchId && item.DepartmentName.Equals("قسم ادارة مشرفين التدريب")).First().DepartmentId;

            var EMP = new Employee
            {

                Department_DepartmentId = DEPId,
                Job_JobId = 3,
                SSN = employeeVM.employee.SSN,
                UserAccount_UserId = user.UserId,



            };

            _DbContext.Employees.Add(EMP);

            _DbContext.SaveChanges();
            TempData["success"] = "تم إضافة حساب الموظف  بنجاح";
            return RedirectToAction("ViewSupervisorsUser");

        }

        public IActionResult ViewOpportunities()
        {

            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");

            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).FirstOrDefault();
            var Branch = _DbContext.OrganizationBranches_TrainProv.Where(item => item.Responsible_UserId == RUserId).FirstOrDefault();
            var Emplyee = _DbContext.Employees.Where(item => item.UserAccount_UserId == RUserId).FirstOrDefault();

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Branch.Organization_OrganizationId).FirstOrDefault();


            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Branch.BranchName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;

            var Opportunities = _DbContext.TrainingOpportunities.Where(item => item.CreatedByEmployeeId == Emplyee.EmployeeId).Include(item => item.DetailFiled).ToList();



            return View(Opportunities);

        }


        [HttpGet]
        public IActionResult EmailExists(string? Email)
        {
            if (Email == null)
            {
                return Json(new { Exists = false });
            }

            var item = _DbContext.UserAcounts.Where(item => item.UserEmail.Equals(Email)).FirstOrDefault();

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

            var item = _DbContext.UserAcounts.Where(item => item.Phone.Equals(Phone)).FirstOrDefault();

            if (item == null)
            {
                return Json(new { Exists = false });
            }



            return Json(new { Exists = true });
        }




        [HttpGet]
        public IActionResult AddOpportunities()
        {


            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");

            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).FirstOrDefault();
            var Branch = _DbContext.OrganizationBranches_TrainProv.Where(item => item.Responsible_UserId == RUserId).FirstOrDefault();
            var Department = _DbContext.Departments.Where(item => item.Branch_BranchId == Branch.BranchId).FirstOrDefault();

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Branch.Organization_OrganizationId).FirstOrDefault();


            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Branch.BranchName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;
            var Opportunity = new TrainingOpportunityVM();

            var DEPOfR = _DbContext.Departments.Where(item => item.Branch_BranchId == Branch.BranchId && item.DepartmentName.Equals("قسم ادارة مشرفين التدريب")).FirstOrDefault();

            Opportunity.UserListItems = _DbContext.UserAcounts.FromSqlRaw($"Select * from UserAcounts WHERE UserAcounts.UserId IN (Select Employees.UserAccount_UserId from Employees where Employees.Department_DepartmentId ={DEPOfR.DepartmentId})" +
            "AND UserAcounts.UserId NOT IN (select Responsible_UserId from dbo.OrganizationBranches_TrainProv);").ToList().Select(u => new SelectListItem { Text = u.FullName, Value = u.UserId.ToString() });



            Opportunity.DepartmentListItems = _DbContext.Departments.FromSqlRaw($"SELECT * FROM Departments WHERE Responsible_UserId  = {Branch.Responsible_UserId} AND DepartmentName !='قسم ادارة مشرفين التدريب';")
           .ToList().Select(u => new SelectListItem { Text = u.DepartmentName, Value = u.DepartmentId.ToString() });

            Opportunity.DetailFieldsListItems = _DbContext.FieldOfSpecialtiesDetails.FromSqlRaw($"SELECT * FROM FieldOfSpecialtiesDetails WHERE DetailFieldId IN(SELECT TrainArea_DetailFiledId FROM DepartmentTrainingAreas WHERE Department_DepartmenId = {Department.DepartmentId})")
           .ToList().Select(u => new SelectListItem { Text = u.SpecializationName, Value = u.DetailFieldId.ToString() });

            Opportunity.TrainingTypeListItems = _DbContext.TrainingTypes.FromSqlRaw($"SELECT * FROM TrainingTypes")
           .ToList().Select(u => new SelectListItem { Text = u.TypeName, Value = u.TrainingTypeId.ToString() });


            return View(Opportunity);

        }

        [HttpPost]
        public IActionResult AddOpportunities(TrainingOpportunityVM TrainingOpportunityVM)
        {
            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).FirstOrDefault();
            var Branch = _DbContext.OrganizationBranches_TrainProv.Where(item => item.Responsible_UserId == RUserId).FirstOrDefault();

            var Emplyee = _DbContext.Employees.Where(item => item.UserAccount_UserId == RUserId).FirstOrDefault();

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Branch.Organization_OrganizationId).FirstOrDefault();


            if (TrainingOpportunityVM == null)
            {

                return View();

            }

            var SyperEmplyee = _DbContext.Employees.Where(item => item.UserAccount_UserId == TrainingOpportunityVM.TrainingOpportunity.SupervisorEmployeeId).FirstOrDefault();

            var Opportunity = new TrainingOpportunity
            {
                Branch_BranchId = Branch.BranchId,
                CreatedByEmployeeId = Emplyee.EmployeeId,
                TotalNumberOfSeats = TrainingOpportunityVM.TrainingOpportunity.TotalNumberOfSeats,
                StartDate = TrainingOpportunityVM.TrainingOpportunity.StartDate,
                EndDate = TrainingOpportunityVM.TrainingOpportunity.EndDate,
                StartRegisterDate = TrainingOpportunityVM.TrainingOpportunity.StartRegisterDate,
                EndRegisterDate = TrainingOpportunityVM.TrainingOpportunity.EndRegisterDate,
                OpportunityStatus = "Available",
                AbilityofSubmissionStatus = "Available",

                SupervisorEmployeeId = SyperEmplyee.EmployeeId,
                Department_DepartmentId = TrainingOpportunityVM.TrainingOpportunity.Department_DepartmentId,
                DetailFiled_DetailFiledId = TrainingOpportunityVM.TrainingOpportunity.DetailFiled_DetailFiledId,

                OpportunityDescription = TrainingOpportunityVM.TrainingOpportunity.OpportunityDescription,
                RequestedOpportunities = 0,
                ApprovedOpportunities = 0,
                RejectedOpportunities = 0,
                AvailableOpportunities = TrainingOpportunityVM.TrainingOpportunity.TotalNumberOfSeats,
                MinimumHours = TrainingOpportunityVM.TrainingOpportunity.MinimumHours,
                MinimumWeeks = TrainingOpportunityVM.TrainingOpportunity.MinimumWeeks,
                TrainingType_TrainingTypeId = TrainingOpportunityVM.TrainingOpportunity.TrainingType_TrainingTypeId,





            };

            _DbContext.TrainingOpportunities.Add(Opportunity);

            _DbContext.SaveChanges();
            TempData["success"] = "تم إضافة فرصة تدريبية  بنجاح";
            return RedirectToAction("ViewOpportunities");

        }



        [HttpGet]
        public IActionResult EditOpportunities(int? id)
        {

            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).FirstOrDefault();
            var Branch = _DbContext.OrganizationBranches_TrainProv.Where(item => item.Responsible_UserId == RUserId).FirstOrDefault();
            var Department = _DbContext.Departments.Where(item => item.Branch_BranchId == Branch.BranchId).FirstOrDefault();


            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Branch.Organization_OrganizationId).FirstOrDefault();

            var Opertitnty = _DbContext.TrainingOpportunities.Where(item => item.TrainingOpportunityId == id).FirstOrDefault();

            var Emplyee = _DbContext.Employees.Where(u => u.EmployeeId == Opertitnty.SupervisorEmployeeId).FirstOrDefault();

            var DEPOfR = _DbContext.Departments.Where(item => item.Branch_BranchId == Branch.BranchId && item.DepartmentName.Equals("قسم ادارة مشرفين التدريب")).FirstOrDefault();

            // var SyperEmplyee = _DbContext.Employees.Where(item => item.UserAccount_UserId == TrainingOpportunityVM.TrainingOpportunity.SupervisorEmployeeId).FirstOrDefault();

            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Branch.BranchName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;




            TrainingOpportunityVM TrainingOpportunityVM = new()
            {
                TrainingOpportunity = new TrainingOpportunity()

            };

            if (id != null || id != 0)
            {

                TrainingOpportunityVM.TrainingOpportunity = _DbContext.TrainingOpportunities.Where(u => u.TrainingOpportunityId == id).FirstOrDefault();


                TrainingOpportunityVM.DepartmentListItems = _DbContext.Departments.Where(u => u.DepartmentId == Opertitnty.Department_DepartmentId).Select(u => new SelectListItem { Text = u.DepartmentName, Value = u.DepartmentId.ToString() }).ToList();

                TrainingOpportunityVM.DepartmentListItems = _DbContext.Departments.FromSqlRaw($"SELECT * FROM Departments WHERE Responsible_UserId  = {Branch.Responsible_UserId} AND DepartmentName !='قسم ادارة مشرفين التدريب';")
                 .ToList().Select(u => new SelectListItem { Text = u.DepartmentName, Value = u.DepartmentId.ToString() });



                TrainingOpportunityVM.DetailFieldsListItems = _DbContext.FieldOfSpecialtiesDetails.Where(u => u.DetailFieldId == Opertitnty.DetailFiled_DetailFiledId).Select(u => new SelectListItem { Text = u.SpecializationName, Value = u.DetailFieldId.ToString() }).ToList();

                //TrainingOpportunityVM.DetailFieldsListItems = _DbContext.FieldOfSpecialtiesDetails.FromSqlRaw($"SELECT * FROM FieldOfSpecialtiesDetails WHERE DetailFieldId IN(SELECT TrainArea_DetailFiledId FROM DepartmentTrainingAreas WHERE Department_DepartmenId = {Department.DepartmentId})")
                //  .ToList().Select(u => new SelectListItem { Text = u.SpecializationName, Value = u.DetailFieldId.ToString() });


                TrainingOpportunityVM.DetailFieldsListItems = _DbContext.DepartmentTrainingAreas.Where(u => u.Department_DepartmenId == Opertitnty.Department_DepartmentId).Include(u => u.fieldOfSpecialtyDetails).Select(u => new SelectListItem { Text = u.fieldOfSpecialtyDetails.SpecializationName, Value = u.fieldOfSpecialtyDetails.ToString() }).ToList();



                TrainingOpportunityVM.TrainingTypeListItems = _DbContext.TrainingTypes.Where(u => u.TrainingTypeId == Opertitnty.TrainingType_TrainingTypeId).Select(u => new SelectListItem { Text = u.TypeName, Value = u.TrainingTypeId.ToString() }).ToList();

                TrainingOpportunityVM.TrainingTypeListItems = _DbContext.TrainingTypes.FromSqlRaw($"SELECT * FROM TrainingTypes")
                    .ToList().Select(u => new SelectListItem { Text = u.TypeName, Value = u.TrainingTypeId.ToString() });




                TrainingOpportunityVM.UserListItems = _DbContext.UserAcounts.Where(u => u.UserId == Emplyee.UserAccount_UserId).Select(u => new SelectListItem { Text = u.FullName, Value = u.UserId.ToString() }).ToList();

                TrainingOpportunityVM.UserListItems = _DbContext.UserAcounts.FromSqlRaw($"Select * from UserAcounts WHERE UserAcounts.UserId IN (Select Employees.UserAccount_UserId from Employees where Employees.Department_DepartmentId ={DEPOfR.DepartmentId})" +
                "AND UserAcounts.UserId NOT IN (select Responsible_UserId from dbo.OrganizationBranches_TrainProv);").ToList().Select(u => new SelectListItem { Text = u.FullName, Value = u.UserId.ToString() });


                return View(TrainingOpportunityVM);
            }


            return NotFound();
        }

        [HttpPost]
        public IActionResult EditOpportunities(TrainingOpportunityVM TrainingOpportunityVM)
        {

            //if (!ModelState.IsValid)
            //{
            //    return View();
            //}

            //TrainingOpportunityVM.UserListItems = _DbContext.Employees.Where(u => u.UserId == Emplyee.UserAccount_UserId).Select(u => new SelectListItem { Text = u.FullName, Value = u.UserId.ToString() }).ToList();

            //var Oppertinty = _DbContext.TrainingOpportunities.Where(item => item.TrainingOpportunityId == TrainingOpportunityVM.TrainingOpportunity.TrainingOpportunityId).FirstOrDefault();


            var Emp = _DbContext.Employees.FirstOrDefault(item => item.EmployeeId == TrainingOpportunityVM.TrainingOpportunity.SupervisorEmployeeId);

            var type = _DbContext.TrainingTypes.FirstOrDefault(item => item.TrainingTypeId == TrainingOpportunityVM.TrainingOpportunity.TrainingType_TrainingTypeId);

            //var Detail = _DbContext.FieldOfSpecialtiesDetails.FirstOrDefault(item => item.DetailFieldId == TrainingOpportunityVM.TrainingOpportunity.DetailFiled_DetailFiledId);

            //var Depart = _DbContext.Departments.FirstOrDefault(item => item.DepartmentId == TrainingOpportunityVM.TrainingOpportunity.Department_DepartmentId);




            TrainingOpportunityVM.TrainingOpportunity.SupervisorEmployeeId = Emp.EmployeeId;

            TrainingOpportunityVM.TrainingOpportunity.TrainingType_TrainingTypeId = type.TrainingTypeId;

            //TrainingOpportunityVM.TrainingOpportunity.DetailFiled_DetailFiledId = Detail.DetailFieldId;

            //TrainingOpportunityVM.TrainingOpportunity.Department_DepartmentId = Depart.DepartmentId;



            _DbContext.TrainingOpportunities.Update(TrainingOpportunityVM.TrainingOpportunity);



            _DbContext.SaveChanges();

            return RedirectToAction("ViewOpportunities");



        }
        [HttpGet]
        public IActionResult AddDepartmentFiledSpecialties()
        {
            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");

            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).FirstOrDefault();
            var Branch = _DbContext.OrganizationBranches_TrainProv.Where(item => item.Responsible_UserId == RUserId).FirstOrDefault();

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Branch.Organization_OrganizationId).FirstOrDefault();


            ViewBag.OrganizationName = OrganizationOfR.OrganizationName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;

            SpecialitiesVM specialitiesVM = new()
            {

                MasterFieldsListItems = _DbContext.FieldOfSpecialtiesMaster.FromSqlRaw("SELECT * FROM FieldOfSpecialtiesMaster WHERE FieldId IN" +
                $"(SELECT FieldOfSpecialtiesDetails.Field_FieldId FROM FieldOfSpecialtiesDetails ,OrganizationsProvidTrainingInArea WHERE DetailFieldId = DetailField_DetailFieldId AND Organization_OrganizationId ={OrganizationOfR.OrganizationId});")
                .ToList().Select(u => new SelectListItem { Text = u.FieldName, Value = u.FieldId.ToString() }),

                DepartmentListItems = _DbContext.Departments.FromSqlRaw($"SELECT * FROM Departments WHERE Responsible_UserId  = {Branch.Responsible_UserId} AND DepartmentName !='قسم ادارة مشرفين التدريب';")
                .ToList().Select(u => new SelectListItem { Text = u.DepartmentName, Value = u.DepartmentId.ToString() }),



            };

            return View(specialitiesVM);


        }

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



                int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;

                var RUser = _DbContext.UserAcounts.Find(RUserId);


                int DEPId = Convert.ToInt32(Ids[0]);
                //   var OrganizationOfR = _DbContext.Organizations.Where(item => item.ResponsibleUserId == RUserId).FirstOrDefault();

                var Department = _DbContext.Departments.Where(item => item.DepartmentId == DEPId).FirstOrDefault();




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
                //TempData["success"] = "تم إضافة التخصصات  بنجاح";


            }




            return Json(new { Exists = false });

        }



        [HttpGet]
        public IActionResult ViewDepartmentFiledSpecialties()
        {
            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");

            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).FirstOrDefault();
            var Branch = _DbContext.OrganizationBranches_TrainProv.Where(item => item.Responsible_UserId == RUserId).FirstOrDefault();

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Branch.Organization_OrganizationId).FirstOrDefault();

            var Department = _DbContext.Departments.Where(item => item.Responsible_UserId == RUserId).ToList();


            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Branch.BranchName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;


            return View(Department);





        }

        #region
        public IActionResult GetDetailFields(string? ids)
        {

            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;


            var Branch = _DbContext.OrganizationBranches_TrainProv.Where(item => item.Responsible_UserId == RUserId).FirstOrDefault();

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Branch.Organization_OrganizationId).FirstOrDefault();





            if (!string.IsNullOrEmpty(ids))
            {

                string[] Ids = ids.Split(",");

                var FId = Convert.ToInt32(Ids[1]);
                var DId = Convert.ToInt32(Ids[0]);

                //Console.WriteLine(FId);

                var Detailfields = _DbContext.FieldOfSpecialtiesDetails.FromSqlRaw($"Select * From FieldOfSpecialtiesDetails WHERE Field_FieldId={FId} AND DetailFieldId IN" +
                $"(Select DetailField_DetailFieldId From OrganizationsProvidTrainingInArea WHERE Organization_OrganizationId={OrganizationOfR.OrganizationId})" +
                $"AND DetailFieldId NOT IN (SELECT TrainArea_DetailFiledId From DepartmentTrainingAreas WHERE Department_DepartmenId={DId})")
                .IgnoreQueryFilters()
                .Select(item => new

                {
                    DetailFieldId = item.DetailFieldId,

                    SpecializationName = item.SpecializationName

                }


                ).ToList();


                return Json(new { Detailfields });


            }


            return Json(new { Exists = false });
        }


        public IActionResult AddDEP(string? name)
        {

            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;


            var Branch = _DbContext.OrganizationBranches_TrainProv.Where(item => item.Responsible_UserId == RUserId).FirstOrDefault();

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Branch.Organization_OrganizationId).FirstOrDefault();


            if (!string.IsNullOrEmpty(name))
            {


                Department DEP = new Department
                {
                    DepartmentName = name,
                    Responsible_UserId = RUserId,
                    Organization_OrganizationId = OrganizationOfR.OrganizationId,
                    Branch_BranchId = Branch.BranchId,



                };

                _DbContext.Departments.Add(DEP);

                _DbContext.SaveChanges();

                //TempData["success"] = "تم إضافة قسم جديد بنجاح";

                return Json(new { success = true });

            }


            return Json(new { success = false });
        }




        public IActionResult GetAllDEPs()
        {

            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;


            var Branch = _DbContext.OrganizationBranches_TrainProv.Where(item => item.Responsible_UserId == RUserId).FirstOrDefault();

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Branch.Organization_OrganizationId).FirstOrDefault();



            //   IEnumerable<Department> DEPList = Enumerable.Empty<Department>();

            var DEPList = _DbContext.Departments.Where(item => item.Organization_OrganizationId == OrganizationOfR.OrganizationId && item.Branch_BranchId == Branch.BranchId && !item.DepartmentName.Equals("قسم ادارة مشرفين التدريب"));



            return Json(new { DEPList });




        }



        public IActionResult EditeDEP(string? id, string? name)
        {

            if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(name))
            {

                int Id = Convert.ToInt32(id);


                var DEP = _DbContext.Departments.Where(item => item.DepartmentId == Id).FirstOrDefault();

                DEP.DepartmentName = name;

                _DbContext.Departments.Update(DEP);

                _DbContext.SaveChanges();

                //TempData["success"] = "تم تعديل معلومات القسم  بنجاح";

                return Json(new { success = true });
            }

            return Json(new { success = false });





        }


        public IActionResult GetSpecialities()
        {
            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.ResponsibleUserId == RUserId).FirstOrDefault();

            var Branch = _DbContext.OrganizationBranches_TrainProv.Where(item => item.Responsible_UserId == RUserId).FirstOrDefault();

            var departmentofb = _DbContext.Departments.Where(item => item.Branch_BranchId == Branch.BranchId).FirstOrDefault();

            IEnumerable<FieldOfSpecialtyDetails> Specialities = Enumerable.Empty<FieldOfSpecialtyDetails>();
            var HasFileds = _DbContext.DepartmentTrainingAreas.Where(item => item.Department_DepartmenId == departmentofb.DepartmentId).ToList();

            if (HasFileds != null)
            {

                Specialities = _DbContext.FieldOfSpecialtiesDetails.FromSqlRaw("Select * from FieldOfSpecialtiesDetails WHERE DetailFieldId IN" +
                    $"(Select TrainArea_DetailFiledId From DepartmentTrainingAreas WHERE Department_DepartmenId={departmentofb.DepartmentId})").Include(item => item.FieldOfSpecialty).ToList();

            }

            //var Specialities = _DbContext.Departments.Where(item => item.Organization_OrganizationId == OrganizationOfR.OrganizationId && item.Branch_BranchId == Branch.BranchId && !item.DepartmentName.Equals("قسم ادارة مشرفين التدريب"));







            //return Json(new { DEPList });


            return Json(new { Specialities });


        }

        public IActionResult GetDepartmentTrainingArea()

        {
            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.ResponsibleUserId == RUserId).FirstOrDefault();

            var Branch = _DbContext.OrganizationBranches_TrainProv.Where(item => item.Responsible_UserId == RUserId).FirstOrDefault();

            var departmentofb = _DbContext.Departments.Where(item => item.Branch_BranchId == Branch.BranchId).FirstOrDefault();

            IEnumerable<DepartmentTrainingArea> Departments = Enumerable.Empty<DepartmentTrainingArea>();

            Departments = _DbContext.DepartmentTrainingAreas.FromSqlRaw("SELECT * FROM DepartmentTrainingAreas WHERE Department_DepartmenId IN" +
                $"(SELECT DepartmentId FROM Departments WHERE  DepartmentName!='قسم ادارة مشرفين التدريب' AND Branch_BranchId={Branch.BranchId})").Include(item => item.department)
                .Include(item => item.fieldOfSpecialtyDetails).OrderBy(item => item.Department_DepartmenId).ToList();





            return Json(new { Departments });



        }

        public IActionResult DeleteDepartmentTrainingArea(int? id)
        {



            if (id != null || id == 0)
            {



                var DEPTA = _DbContext.DepartmentTrainingAreas.Where(u => u.DepartmentTrainingAreaId == id).FirstOrDefault();

                _DbContext.DepartmentTrainingAreas.Remove(DEPTA);


                _DbContext.SaveChanges();


                return Json(new { success = true, message = "تم الحذف بنجاح" });
            }



            return Json(new { success = false, message = "حصل خطاء لم يتم الحذف" });

        }



        public IActionResult GetSpecialization(string? id)
        {

            if (!string.IsNullOrEmpty(id))
            {
                var Id = Convert.ToInt64(id);
                //   var Department = _DbContext.DepartmentTrainingAreas.Where(item => item.Department_DepartmenId == Id).Select(item => new

                //   {
                //       TrainArea_DetailFiledI = item.TrainArea_DetailFiledId,



                //   }


                //   ).FirstOrDefault();

                //   var speslaiztion = _DbContext.FieldOfSpecialtiesDetails.Where(item => item.DetailFieldId == Department.TrainArea_DetailFiledI).Select(item => new

                //   {


                //       DetailFieldId=item.DetailFieldId,
                //      SpecializationName=item.SpecializationName


                //   }


                //).ToList();



                //var HasFileds = _DbContext.DepartmentTrainingAreas.Where(item => item.Department_DepartmenId == Id).ToList();

                //if (HasFileds != null)
                //{

                var Specialities = _DbContext.FieldOfSpecialtiesDetails.FromSqlRaw("Select * from FieldOfSpecialtiesDetails WHERE DetailFieldId IN" +
                       $"(Select TrainArea_DetailFiledId From DepartmentTrainingAreas WHERE Department_DepartmenId={Id})").ToList();





                return Json(new { Specialities });

            }


            return Json(new { Exists = false });
        }
        #endregion
    }
}
