﻿using Microsoft.AspNetCore.Mvc;
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
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();
            var Branch = _DbContext.OrganizationBranches_TrainProv.Where(item => item.Responsible_UserId == RUserId).AsNoTracking().FirstOrDefault();

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Branch.Organization_OrganizationId).AsNoTracking().FirstOrDefault();


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
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();
            var Branch = _DbContext.OrganizationBranches_TrainProv.Where(item => item.Responsible_UserId == RUserId).AsNoTracking().FirstOrDefault();

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Branch.Organization_OrganizationId).AsNoTracking().FirstOrDefault();


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
            var Branch = _DbContext.OrganizationBranches_TrainProv.Where(item => item.Responsible_UserId == RUserId).AsNoTracking().FirstOrDefault();

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Branch.Organization_OrganizationId).AsNoTracking().FirstOrDefault();


            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Branch.BranchName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;
            var DEPOfR = _DbContext.Departments.Where(item => item.Branch_BranchId == Branch.BranchId && item.DepartmentName.Equals("قسم ادارة مشرفين التدريب")).AsNoTracking().FirstOrDefault();


            IEnumerable<UserAcount> Employees = Enumerable.Empty<UserAcount>();


            if (DEPOfR != null)
            {

                Employees = _DbContext.UserAcounts.FromSqlRaw($"Select * from UserAcounts WHERE UserId IN (Select UserAccount_UserId from Employees WHERE Department_DepartmentId = {DEPOfR.DepartmentId});")
                    .AsNoTracking().ToList();


            }



            return View(Employees);
        }


        [HttpGet]
        public IActionResult AddSupervisorUser()
        {
            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");

            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();
            var Branch = _DbContext.OrganizationBranches_TrainProv.Where(item => item.Responsible_UserId == RUserId).AsNoTracking().FirstOrDefault();

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Branch.Organization_OrganizationId).AsNoTracking().FirstOrDefault();


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

            var Branch = _DbContext.OrganizationBranches_TrainProv.Where(item => item.Responsible_UserId == RUserId).AsNoTracking().FirstOrDefault();
            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Branch.Organization_OrganizationId).AsNoTracking().FirstOrDefault();


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
                ActivationStatus = "Not_Active"

            };

            _DbContext.UserAcounts.Add(user);

            _DbContext.SaveChanges();

            var DEPOfR = _DbContext.Departments.Where(item => item.Branch_BranchId == Branch.BranchId && item.DepartmentName.Equals("قسم ادارة مشرفين التدريب")).AsNoTracking().FirstOrDefault();

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

            int DEPId = _DbContext.Departments.Where(item => item.Branch_BranchId == Branch.BranchId && item.DepartmentName.Equals("قسم ادارة مشرفين التدريب")).AsNoTracking().First().DepartmentId;

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
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();
            var Branch = _DbContext.OrganizationBranches_TrainProv.Where(item => item.Responsible_UserId == RUserId).AsNoTracking().FirstOrDefault();
            var Emplyee = _DbContext.Employees.Where(item => item.UserAccount_UserId == RUserId).FirstOrDefault();

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Branch.Organization_OrganizationId).AsNoTracking().FirstOrDefault();


            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Branch.BranchName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;

            var Opportunities = _DbContext.TrainingOpportunities.Where(item => item.CreatedByEmployeeId == Emplyee.EmployeeId).Include(item => item.DetailFiled).AsNoTracking().ToList();



            return View(Opportunities);

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
        public IActionResult AddOpportunities()
        {


            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");

            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();
            var Branch = _DbContext.OrganizationBranches_TrainProv.Where(item => item.Responsible_UserId == RUserId).AsNoTracking().FirstOrDefault();
            var Department = _DbContext.Departments.Where(item => item.Branch_BranchId == Branch.BranchId).AsNoTracking().FirstOrDefault();

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Branch.Organization_OrganizationId).AsNoTracking().FirstOrDefault();


            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Branch.BranchName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;
            var Opportunity = new TrainingOpportunityVM();

            var DEPOfR = _DbContext.Departments.Where(item => item.Branch_BranchId == Branch.BranchId && item.DepartmentName.Equals("قسم ادارة مشرفين التدريب")).AsNoTracking().FirstOrDefault();

            Opportunity.UserListItems = _DbContext.Employees.FromSqlRaw($"SELECT * FROM Employees WHERE Department_DepartmentId ={DEPOfR.DepartmentId}")
                .Include(item => item.userAcount).AsNoTracking().ToList().Select(u => new SelectListItem { Text = u.userAcount.FullName, Value = u.EmployeeId.ToString() });



            Opportunity.DepartmentListItems = _DbContext.Departments.FromSqlRaw($"SELECT * FROM Departments WHERE Responsible_UserId  = {Branch.Responsible_UserId} AND DepartmentName !='قسم ادارة مشرفين التدريب';")
           .AsNoTracking().ToList().Select(u => new SelectListItem { Text = u.DepartmentName, Value = u.DepartmentId.ToString() });

            Opportunity.DetailFieldsListItems = _DbContext.FieldOfSpecialtiesDetails.FromSqlRaw($"SELECT * FROM FieldOfSpecialtiesDetails WHERE DetailFieldId IN(SELECT TrainArea_DetailFiledId FROM DepartmentTrainingAreas WHERE Department_DepartmenId = {Department.DepartmentId})")
           .AsNoTracking().ToList().Select(u => new SelectListItem { Text = u.SpecializationName, Value = u.DetailFieldId.ToString() });

            Opportunity.TrainingTypeListItems = _DbContext.TrainingTypes.FromSqlRaw($"SELECT * FROM TrainingTypes")
           .AsNoTracking().ToList().Select(u => new SelectListItem { Text = u.TypeName, Value = u.TrainingTypeId.ToString() });


            return View(Opportunity);

        }

        [HttpPost]
        public IActionResult AddOpportunities(TrainingOpportunityVM TrainingOpportunityVM)
        {
            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();
            var Branch = _DbContext.OrganizationBranches_TrainProv.Where(item => item.Responsible_UserId == RUserId).AsNoTracking().FirstOrDefault();

            var Emplyee = _DbContext.Employees.Where(item => item.UserAccount_UserId == RUserId).AsNoTracking().FirstOrDefault();

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Branch.Organization_OrganizationId).AsNoTracking().FirstOrDefault();


            if (TrainingOpportunityVM == null)
            {

                return View();

            }

            //  var SyperEmplyee = _DbContext.Employees.Where(item => item.UserAccount_UserId == TrainingOpportunityVM.TrainingOpportunity.SupervisorEmployeeId).FirstOrDefault();

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

                SupervisorEmployeeId = TrainingOpportunityVM.TrainingOpportunity.SupervisorEmployeeId,
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
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();
            var Branch = _DbContext.OrganizationBranches_TrainProv.Where(item => item.Responsible_UserId == RUserId).AsNoTracking().FirstOrDefault();
            var Department = _DbContext.Departments.Where(item => item.Branch_BranchId == Branch.BranchId).FirstOrDefault();


            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Branch.Organization_OrganizationId).AsNoTracking().FirstOrDefault();

            var Opertitnty = _DbContext.TrainingOpportunities.Where(item => item.TrainingOpportunityId == id).AsNoTracking().FirstOrDefault();

            var Emplyee = _DbContext.Employees.Where(u => u.EmployeeId == Opertitnty.SupervisorEmployeeId).AsNoTracking().FirstOrDefault();

            var DEPOfR = _DbContext.Departments.Where(item => item.Branch_BranchId == Branch.BranchId && item.DepartmentName.Equals("قسم ادارة مشرفين التدريب")).AsNoTracking().FirstOrDefault();

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
                TrainingOpportunityVM.StudentRequestsOnOpportunities = _DbContext.StudentRequestsOnOpportunities.Where(u => u.TrainingOpportunity_TrainingOpportunityId == id).AsNoTracking().FirstOrDefault();

                TrainingOpportunityVM.TrainingOpportunity = _DbContext.TrainingOpportunities.Where(u => u.TrainingOpportunityId == id)
                    .Include(item => item.Department).Include(item => item.DetailFiled).AsNoTracking().FirstOrDefault();


                TrainingOpportunityVM.DepartmentListItems = _DbContext.Departments.FromSqlRaw($"SELECT * FROM Departments WHERE Responsible_UserId  = {Branch.Responsible_UserId} AND DepartmentName !='قسم ادارة مشرفين التدريب';")
                 .ToList().Select(u => new SelectListItem { Text = u.DepartmentName, Value = u.DepartmentId.ToString() });



                TrainingOpportunityVM.DetailFieldsListItems = _DbContext.FieldOfSpecialtiesDetails.Where(u => u.DetailFieldId == Opertitnty.DetailFiled_DetailFiledId)
                    .Select(u => new SelectListItem { Text = u.SpecializationName, Value = u.DetailFieldId.ToString() }).AsNoTracking().ToList();


                TrainingOpportunityVM.DetailFieldsListItems = _DbContext.DepartmentTrainingAreas.Where(u => u.Department_DepartmenId == Opertitnty.Department_DepartmentId).Include(u => u.fieldOfSpecialtyDetails)
                    .Select(u => new SelectListItem { Text = u.fieldOfSpecialtyDetails.SpecializationName, Value = u.fieldOfSpecialtyDetails.ToString() }).AsNoTracking().ToList();




                TrainingOpportunityVM.TrainingTypeListItems = _DbContext.TrainingTypes.FromSqlRaw($"SELECT * FROM TrainingTypes")
                    .AsNoTracking().ToList().Select(u => new SelectListItem { Text = u.TypeName, Value = u.TrainingTypeId.ToString() });


                TrainingOpportunityVM.UserListItems = _DbContext.Employees.FromSqlRaw($"SELECT * FROM Employees WHERE Department_DepartmentId ={DEPOfR.DepartmentId}")
                           .Include(item => item.userAcount).AsNoTracking().ToList().Select(u => new SelectListItem { Text = u.userAcount.FullName, Value = u.EmployeeId.ToString() });
                return View(TrainingOpportunityVM);
            }


            return NotFound();
        }

        [HttpPost]
        public IActionResult EditOpportunities(TrainingOpportunityVM TrainingOpportunityVM)
        {


            if (!ModelState.IsValid)
            {
                return View(TrainingOpportunityVM);
            }


            var Opportunity = _DbContext.TrainingOpportunities.Where(u => u.TrainingOpportunityId == TrainingOpportunityVM.TrainingOpportunity.TrainingOpportunityId).AsNoTracking().FirstOrDefault();



            if (TrainingOpportunityVM.TrainingOpportunity.AvailableOpportunities < 0)
            {
                TempData["error"] = "عذرا لا تستطيع تقليل عدد المقاعد المتاحة لهذا الحد  ";

                return RedirectToAction("ViewOpportunities");
            }


            if (Opportunity.AvailableOpportunities == 0 && TrainingOpportunityVM.TrainingOpportunity.AvailableOpportunities > 0)
            {
                Opportunity.OpportunityStatus = "Available";

            }

            if (Opportunity.AvailableOpportunities > 0 && TrainingOpportunityVM.TrainingOpportunity.AvailableOpportunities == 0)
            {
                Opportunity.OpportunityStatus = "Complete";
            }

            var totalseatupdate = Opportunity.TotalNumberOfSeats + TrainingOpportunityVM.TrainingOpportunity.AvailableOpportunities;


            Opportunity.OpportunityDescription = TrainingOpportunityVM.TrainingOpportunity.OpportunityDescription;


            Opportunity.TrainingType_TrainingTypeId = TrainingOpportunityVM.TrainingOpportunity.TrainingType_TrainingTypeId;


            Opportunity.SupervisorEmployeeId = TrainingOpportunityVM.TrainingOpportunity.SupervisorEmployeeId;

            Opportunity.TotalNumberOfSeats = totalseatupdate;

            Opportunity.AvailableOpportunities = TrainingOpportunityVM.TrainingOpportunity.AvailableOpportunities;

            Opportunity.MinimumHours = TrainingOpportunityVM.TrainingOpportunity.MinimumHours;

            Opportunity.MinimumWeeks = TrainingOpportunityVM.TrainingOpportunity.MinimumWeeks;

            Opportunity.StartDate = TrainingOpportunityVM.TrainingOpportunity.StartDate;

            Opportunity.EndDate = TrainingOpportunityVM.TrainingOpportunity.EndDate;

            Opportunity.StartRegisterDate = TrainingOpportunityVM.TrainingOpportunity.StartRegisterDate;

            Opportunity.EndRegisterDate = TrainingOpportunityVM.TrainingOpportunity.EndRegisterDate;




            _DbContext.TrainingOpportunities.Update(Opportunity);



            _DbContext.SaveChanges();
            TempData["success"] = "تم تعديل الفرصة بنجاح ";

            return RedirectToAction("ViewOpportunities");



        }

        [HttpGet]
        public IActionResult AddDepartmentFiledSpecialties()
        {
            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");

            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();
            var Branch = _DbContext.OrganizationBranches_TrainProv.Where(item => item.Responsible_UserId == RUserId).AsNoTracking().FirstOrDefault();

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Branch.Organization_OrganizationId).AsNoTracking().FirstOrDefault();


            ViewBag.OrganizationName = OrganizationOfR.OrganizationName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;

            SpecialitiesVM specialitiesVM = new()
            {

                MasterFieldsListItems = _DbContext.FieldOfSpecialtiesMaster.FromSqlRaw("SELECT * FROM FieldOfSpecialtiesMaster WHERE FieldId IN" +
                $"(SELECT FieldOfSpecialtiesDetails.Field_FieldId FROM FieldOfSpecialtiesDetails ,OrganizationsProvidTrainingInArea WHERE DetailFieldId = DetailField_DetailFieldId AND Organization_OrganizationId ={OrganizationOfR.OrganizationId});")
                .AsNoTracking().ToList().Select(u => new SelectListItem { Text = u.FieldName, Value = u.FieldId.ToString() }),

                DepartmentListItems = _DbContext.Departments.FromSqlRaw($"SELECT * FROM Departments WHERE Responsible_UserId  = {Branch.Responsible_UserId} AND DepartmentName !='قسم ادارة مشرفين التدريب';")
                .AsNoTracking().ToList().Select(u => new SelectListItem { Text = u.DepartmentName, Value = u.DepartmentId.ToString() }),



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
                //TempData["success"] = "تم إضافة التخصصات  بنجاح";


            }




            return Json(new { Exists = false });

        }



        [HttpGet]
        public IActionResult ViewDepartmentFiledSpecialties()
        {
            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");

            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();
            var Branch = _DbContext.OrganizationBranches_TrainProv.Where(item => item.Responsible_UserId == RUserId).AsNoTracking().FirstOrDefault();

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Branch.Organization_OrganizationId).AsNoTracking().FirstOrDefault();

            var Department = _DbContext.Departments.Where(item => item.Responsible_UserId == RUserId).AsNoTracking().ToList();


            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Branch.BranchName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;


            return View(Department);
        }


        public IActionResult OpportunitiesApplicants(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");

            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();
            var Branch = _DbContext.OrganizationBranches_TrainProv.Where(item => item.Responsible_UserId == RUserId).AsNoTracking().FirstOrDefault();

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Branch.Organization_OrganizationId).AsNoTracking().FirstOrDefault();

            var Department = _DbContext.Departments.Where(item => item.Responsible_UserId == RUserId).AsNoTracking().ToList();


            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Branch.BranchName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;

            ViewBag.OpportunitiyId = id;



            return View();
        }


        public IActionResult OpportunitiesApplicantsDetail(int? id, int? Oid)
        {


            if (id == null || id == 0)
            {
                return NotFound();
            }
            ViewBag.Name = _HttpContextAccessor.HttpContext.Session.GetString("Name");

            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();
            var Branch = _DbContext.OrganizationBranches_TrainProv.Where(item => item.Responsible_UserId == RUserId).AsNoTracking().FirstOrDefault();

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Branch.Organization_OrganizationId).AsNoTracking().FirstOrDefault();

            var Department = _DbContext.Departments.Where(item => item.Responsible_UserId == RUserId).AsNoTracking().ToList();


            ViewBag.OrganizationName = OrganizationOfR.OrganizationName + " - " + Branch.BranchName;
            ViewBag.OrganizationImage = OrganizationOfR.LogoPath;
            ViewBag.Username = user.FullName;



            var Student = _DbContext.StudentRequestsOnOpportunities.Where(item => item.Trainee_TraineeId == id && item.TrainingOpportunity_TrainingOpportunityId == Oid)
                   .AsNoTracking().Include(item => item.student.user)
                   .AsNoTracking().Include(item => item.student.department.universityCollege.organization)
                   .AsNoTracking().Include(item => item.student.department.universityCollege.city.Country).FirstOrDefault();

            return View(Student);


        }


        [HttpPost]
        public IActionResult AcceptReject(StudentRequestOpportunity StudentRequestOpportunity, string desison)
        {
            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;
            var user = _DbContext.UserAcounts.Where(item => item.UserId == RUserId).AsNoTracking().FirstOrDefault();
            var Branch = _DbContext.OrganizationBranches_TrainProv.Where(item => item.Responsible_UserId == RUserId).AsNoTracking().FirstOrDefault();

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Branch.Organization_OrganizationId).AsNoTracking().FirstOrDefault();

            var Department = _DbContext.Departments.Where(item => item.Responsible_UserId == RUserId).AsNoTracking().ToList();

            var employee = _DbContext.Employees.Where(item => item.UserAccount_UserId == RUserId).AsNoTracking().FirstOrDefault();

            var op = _DbContext.TrainingOpportunities.Where(item => item.TrainingOpportunityId == StudentRequestOpportunity.TrainingOpportunity_TrainingOpportunityId).AsNoTracking().FirstOrDefault();

            //var ReqStudent = _DbContext.StudentRequestsOnOpportunities.Where(item => item.Trainee_TraineeId == UniversityTraineeStudent.TraineeId).AsNoTracking().FirstOrDefault();

            var ReqStudentList = _DbContext.StudentRequestsOnOpportunities.Where(item => item.Trainee_TraineeId == StudentRequestOpportunity.Trainee_TraineeId).AsNoTracking().ToList();

            //var ReqCompany = _DbContext.StudentRequestsOnOpportunities.Where(item => item.TrainingOpportunity_TrainingOpportunityId == oper.TrainingOpportunityId).AsNoTracking().FirstOrDefault();

            var Requ = _DbContext.StudentRequestsOnOpportunities.Where(item => item.StudentRequestOpportunityId == StudentRequestOpportunity.StudentRequestOpportunityId).AsNoTracking().FirstOrDefault();

            var NotRequ = _DbContext.StudentRequestsOnOpportunities.Where(item => item.Trainee_TraineeId == StudentRequestOpportunity.Trainee_TraineeId
            && item.TrainingOpportunity_TrainingOpportunityId != StudentRequestOpportunity.TrainingOpportunity_TrainingOpportunityId).AsNoTracking().ToList();


            if (desison == "accept")
            {
                if (op.OpportunityStatus != "Available")
                {
                    TempData["error"] = "عدد المقاعد مكتملة ";
                    return RedirectToAction("index");
                }

                if (op.AbilityofSubmissionStatus != "Available")
                {
                    TempData["error"] = " عذرا الفرصة غير متاحة ";
                    return RedirectToAction("index");
                }

                if (Requ.DecisionStatus == "waiting")
                {


                    Requ.DecisionStatus = "approved";
                    Requ.DecisionDate = DateTime.Now;

                    _DbContext.StudentRequestsOnOpportunities.Update(Requ);



                    op.ApprovedOpportunities = op.ApprovedOpportunities + 1;
                    op.RequestedOpportunities = op.RequestedOpportunities - 1;
                    op.AvailableOpportunities = op.AvailableOpportunities - 1;
                    if (op.AvailableOpportunities < 1)
                    {
                        op.OpportunityStatus = "Complete";
                    }


                    _DbContext.TrainingOpportunities.Update(op);

                    _DbContext.SaveChanges();

                    foreach (var i in NotRequ)
                    {
                        i.DecisionStatus = "system disable";
                        _DbContext.StudentRequestsOnOpportunities.Update(i);
                        _DbContext.SaveChanges();
                    }


                    TempData["success"] = "تم قبول طلب المتدرب بنجاح ";
                    return RedirectToAction("ViewOpportunities");

                }

                TempData["error"] = "عذرا تم  قبول المتدرب بفرصة اخرى";
                return RedirectToAction("ViewOpportunities");

            }

            if (desison == "reject")
            {

                Requ.DecisionStatus = "rejected";

                _DbContext.StudentRequestsOnOpportunities.Update(Requ);



                op.RequestedOpportunities = op.RequestedOpportunities - 1;
                op.RejectedOpportunities = op.RejectedOpportunities + 1;


                _DbContext.TrainingOpportunities.Update(op);

                _DbContext.SaveChanges();

                TempData["success"] = "تم رفض طلب المتدرب بنجاح";
                return RedirectToAction("ViewOpportunities");
            }



            return View();
        }






        #region

        [HttpPost]
        public IActionResult Reject(int? Rid)
        {
            if (Rid == null || Rid == 0)
            {
                return Json(new { success = false });
            }

            var Request = _DbContext.StudentRequestsOnOpportunities.Where(item => item.StudentRequestOpportunityId == Rid).AsNoTracking().FirstOrDefault();

            if (Request == null)
            {
                return Json(new { success = false });
            }


            if (Request.DecisionStatus == "approved")
            {
                TempData["error"] = "تم قبول الطالب مسبقا";
                return Json(new { success = false });
            }

            var op = _DbContext.TrainingOpportunities.Where(item => item.TrainingOpportunityId == Request.TrainingOpportunity_TrainingOpportunityId).AsNoTracking().FirstOrDefault();

            if (op == null)
            {

                return Json(new { success = false });
            }



            Request.DecisionStatus = "rejected";
            Request.DecisionDate = DateTime.Now.Date;


            op.RequestedOpportunities = op.RequestedOpportunities - 1;
            op.RejectedOpportunities = op.RejectedOpportunities + 1;


            _DbContext.StudentRequestsOnOpportunities.Update(Request);
            _DbContext.TrainingOpportunities.Update(op);

            _DbContext.SaveChanges();

            TempData["success"] = "تم رفض طلب المتدرب بنجاح ";
            return Json(new { success = true });

        }



        [HttpPost]
        public IActionResult Accept(int? Rid)
        {
            if (Rid == null || Rid == 0)
            {
                return Json(new { success = false });
            }

            var Request = _DbContext.StudentRequestsOnOpportunities.Where(item => item.StudentRequestOpportunityId == Rid).AsNoTracking().FirstOrDefault();

            if (Request == null)
            {
                return Json(new { success = false });
            }

            if (Request.DecisionStatus == "approved")
            {
                TempData["error"] = "تم قبول الطالب مسبقا";
                return Json(new { success = false });
            }

            Request.DecisionStatus = "approved";
            Request.DecisionDate = DateTime.Now.Date;

            var op = _DbContext.TrainingOpportunities.Where(item => item.TrainingOpportunityId == Request.TrainingOpportunity_TrainingOpportunityId).AsNoTracking().FirstOrDefault();

            if (op == null)
            {

                return Json(new { success = false });
            }

            if (op.AvailableOpportunities == 0)
            {
                TempData["error"] = "الفرصه مكتملة المقاعد";
                return Json(new { success = false });
            }

            if (op.AbilityofSubmissionStatus.Equals("stop"))
            {
                TempData["error"] = "انتهى  تاريخ التسجيل في الفرصه";
                return Json(new { success = false });
            }

            if (op.OpportunityStatus.Equals("complete"))
            {
                TempData["error"] = "الفرصه مكتملة المقاعد";
                return Json(new { success = false });
            }

            _DbContext.Database.ExecuteSqlRaw("UPDATE StudentRequestsOnOpportunities SET DecisionDate = GETDATE() , DecisionStatus = 'system disable' " +
                $"WHERE Trainee_TraineeId = {Request.Trainee_TraineeId} AND StudentRequestOpportunityId != {Request.StudentRequestOpportunityId}");


            op.ApprovedOpportunities = op.ApprovedOpportunities + 1;
            op.RequestedOpportunities = op.RequestedOpportunities - 1;
            op.AvailableOpportunities = op.AvailableOpportunities - 1;
            if (op.AvailableOpportunities < 1)

            {
                op.OpportunityStatus = "Complete";
            }

            _DbContext.TrainingOpportunities.Update(op);

            _DbContext.StudentRequestsOnOpportunities.Update(Request);

            _DbContext.SaveChanges();


            var RequestList = _DbContext.StudentRequestsOnOpportunities.Where(item => item.Trainee_TraineeId == Request.Trainee_TraineeId).AsNoTracking().ToList();

            foreach (var i in RequestList)
            {
                _DbContext.Database.ExecuteSqlRaw("UPDATE TrainingOpportunities SET RequestedOpportunities =(SELECT COUNT(*) FROM UniversitiesTraineeStudents WHERE TraineeId IN " +
              $"(SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities WHERE TrainingOpportunity_TrainingOpportunityId={i.TrainingOpportunity_TrainingOpportunityId} " +
              $"AND DecisionStatus = 'waiting')) WHERE TrainingOpportunityId={i.TrainingOpportunity_TrainingOpportunityId}");

            }



            TempData["success"] = "تم قبول طلب المتدرب بنجاح ";
            return Json(new { success = true });

        }

        public IActionResult GetDetailFields(string? ids)
        {

            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;


            var Branch = _DbContext.OrganizationBranches_TrainProv.Where(item => item.Responsible_UserId == RUserId).AsNoTracking().FirstOrDefault();

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Branch.Organization_OrganizationId).AsNoTracking().FirstOrDefault();

            if (!string.IsNullOrEmpty(ids))
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


        public IActionResult AddDEP(string? name)
        {

            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;


            var Branch = _DbContext.OrganizationBranches_TrainProv.Where(item => item.Responsible_UserId == RUserId).AsNoTracking().FirstOrDefault();

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Branch.Organization_OrganizationId).AsNoTracking().FirstOrDefault();


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


            var Branch = _DbContext.OrganizationBranches_TrainProv.Where(item => item.Responsible_UserId == RUserId).AsNoTracking().FirstOrDefault();

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.OrganizationId == Branch.Organization_OrganizationId).AsNoTracking().FirstOrDefault();



            //   IEnumerable<Department> DEPList = Enumerable.Empty<Department>();

            var DEPList = _DbContext.Departments.Where(item => item.Organization_OrganizationId == OrganizationOfR.OrganizationId && item.Branch_BranchId == Branch.BranchId && !item.DepartmentName.Equals("قسم ادارة مشرفين التدريب")).AsNoTracking().ToList();



            return Json(new { DEPList });




        }



        public IActionResult EditeDEP(string? id, string? name)
        {

            if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(name))
            {

                int Id = Convert.ToInt32(id);


                var DEP = _DbContext.Departments.Where(item => item.DepartmentId == Id).AsNoTracking().FirstOrDefault();

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

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.ResponsibleUserId == RUserId).AsNoTracking().FirstOrDefault();

            var Branch = _DbContext.OrganizationBranches_TrainProv.Where(item => item.Responsible_UserId == RUserId).AsNoTracking().FirstOrDefault();

            var departmentofb = _DbContext.Departments.Where(item => item.Branch_BranchId == Branch.BranchId).AsNoTracking().FirstOrDefault();

            IEnumerable<FieldOfSpecialtyDetails> Specialities = Enumerable.Empty<FieldOfSpecialtyDetails>();
            var HasFileds = _DbContext.DepartmentTrainingAreas.Where(item => item.Department_DepartmenId == departmentofb.DepartmentId).AsNoTracking().ToList();

            if (HasFileds != null)
            {

                Specialities = _DbContext.FieldOfSpecialtiesDetails.FromSqlRaw("Select * from FieldOfSpecialtiesDetails WHERE DetailFieldId IN" +
                    $"(Select TrainArea_DetailFiledId From DepartmentTrainingAreas WHERE Department_DepartmenId={departmentofb.DepartmentId})").Include(item => item.FieldOfSpecialty).AsNoTracking().ToList();

            }

            //var Specialities = _DbContext.Departments.Where(item => item.Organization_OrganizationId == OrganizationOfR.OrganizationId && item.Branch_BranchId == Branch.BranchId && !item.DepartmentName.Equals("قسم ادارة مشرفين التدريب"));







            //return Json(new { DEPList });


            return Json(new { Specialities });


        }

        public IActionResult GetDepartmentTrainingArea()

        {
            int RUserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId").Value;

            var OrganizationOfR = _DbContext.Organizations.Where(item => item.ResponsibleUserId == RUserId).AsNoTracking().FirstOrDefault();

            var Branch = _DbContext.OrganizationBranches_TrainProv.Where(item => item.Responsible_UserId == RUserId).AsNoTracking().FirstOrDefault();

            var departmentofb = _DbContext.Departments.Where(item => item.Branch_BranchId == Branch.BranchId).AsNoTracking().FirstOrDefault();

            IEnumerable<DepartmentTrainingArea> Departments = Enumerable.Empty<DepartmentTrainingArea>();

            Departments = _DbContext.DepartmentTrainingAreas.FromSqlRaw("SELECT * FROM DepartmentTrainingAreas WHERE Department_DepartmenId IN" +
                $"(SELECT DepartmentId FROM Departments WHERE  DepartmentName!='قسم ادارة مشرفين التدريب' AND Branch_BranchId={Branch.BranchId})").Include(item => item.department)
                .Include(item => item.fieldOfSpecialtyDetails).OrderBy(item => item.Department_DepartmenId).AsNoTracking().ToList();





            return Json(new { Departments });



        }

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



        public IActionResult GetSpecialization(string? id)
        {

            if (!string.IsNullOrEmpty(id))
            {
                var Id = Convert.ToInt64(id);


                var Specialities = _DbContext.FieldOfSpecialtiesDetails.FromSqlRaw("Select * from FieldOfSpecialtiesDetails WHERE DetailFieldId IN" +
                       $"(Select TrainArea_DetailFiledId From DepartmentTrainingAreas WHERE Department_DepartmenId={Id})").AsNoTracking().ToList();





                return Json(new { Specialities });

            }


            return Json(new { Exists = false });
        }

        [HttpGet]
        public IActionResult GetStudentsList(int? id, string? gr, int? UPOrDw, int? ArOrWa)
        {


            IEnumerable<UniversityTraineeStudent> Students = Enumerable.Empty<UniversityTraineeStudent>();

            if (ArOrWa == 0)
            {
                if (gr == null && UPOrDw == 0)
                {

                    Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE TraineeId IN " +
                   $"(SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities WHERE TrainingOpportunity_TrainingOpportunityId ={id} AND DecisionStatus='waiting')")
                      .AsNoTracking().Include(item => item.user)
                      .AsNoTracking().Include(item => item.department.universityCollege.organization)
                      .AsNoTracking().Include(item => item.department.universityCollege.city.Country).ToList();

                    return Json(new { Students });
                }

                if (gr != null && UPOrDw == 0)
                {

                    Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE TraineeId IN " +
                   $"(SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities WHERE TrainingOpportunity_TrainingOpportunityId ={id} AND DecisionStatus='waiting') AND Gender ='{gr}' ")
                      .AsNoTracking().Include(item => item.user)
                      .AsNoTracking().Include(item => item.department.universityCollege.organization)
                      .AsNoTracking().Include(item => item.department.universityCollege.city.Country).ToList();

                    return Json(new { Students });
                }



                if (gr != null && UPOrDw == 1)
                {

                    Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE TraineeId IN " +
                   $"(SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities WHERE TrainingOpportunity_TrainingOpportunityId ={id} AND DecisionStatus='waiting' ) AND Gender ='{gr}' ")
                      .AsNoTracking().Include(item => item.user)
                      .AsNoTracking().Include(item => item.department.universityCollege.organization)
                      .AsNoTracking().Include(item => item.department.universityCollege.city.Country).OrderBy(item => item.GPA).ToList();

                    return Json(new { Students });
                }

                if (gr != null && UPOrDw == 2)
                {

                    Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE TraineeId IN " +
                   $"(SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities WHERE TrainingOpportunity_TrainingOpportunityId ={id} AND DecisionStatus='waiting' ) AND Gender ='{gr}' ")
                      .AsNoTracking().Include(item => item.user)
                      .AsNoTracking().Include(item => item.department.universityCollege.organization)
                      .AsNoTracking().Include(item => item.department.universityCollege.city.Country).OrderByDescending(item => item.GPA).ToList();

                    return Json(new { Students });
                }


                if (gr == null && UPOrDw == 1)
                {

                    Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE TraineeId IN " +
                   $"(SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities WHERE TrainingOpportunity_TrainingOpportunityId ={id} AND DecisionStatus='waiting') ")
                      .AsNoTracking().Include(item => item.user)
                      .AsNoTracking().Include(item => item.department.universityCollege.organization)
                      .AsNoTracking().Include(item => item.department.universityCollege.city.Country).OrderBy(item => item.GPA).ToList();

                    return Json(new { Students });
                }

                if (gr == null && UPOrDw == 2)
                {

                    Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE TraineeId IN " +
                   $"(SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities WHERE TrainingOpportunity_TrainingOpportunityId ={id} AND DecisionStatus='waiting' ) ")
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

                    Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE TraineeId IN " +
                   $"(SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities WHERE TrainingOpportunity_TrainingOpportunityId ={id} AND DecisionStatus='approved')")
                      .AsNoTracking().Include(item => item.user)
                      .AsNoTracking().Include(item => item.department.universityCollege.organization)
                      .AsNoTracking().Include(item => item.department.universityCollege.city.Country).ToList();

                    return Json(new { Students });
                }

                if (gr != null && UPOrDw == 0)
                {

                    Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE TraineeId IN " +
                   $"(SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities WHERE TrainingOpportunity_TrainingOpportunityId ={id} AND DecisionStatus='approved') AND Gender ='{gr}' ")
                      .AsNoTracking().Include(item => item.user)
                      .AsNoTracking().Include(item => item.department.universityCollege.organization)
                      .AsNoTracking().Include(item => item.department.universityCollege.city.Country).ToList();

                    return Json(new { Students });
                }



                if (gr != null && UPOrDw == 1)
                {

                    Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE TraineeId IN " +
                   $"(SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities WHERE TrainingOpportunity_TrainingOpportunityId ={id} AND DecisionStatus='approved' ) AND Gender ='{gr}' ")
                      .AsNoTracking().Include(item => item.user)
                      .AsNoTracking().Include(item => item.department.universityCollege.organization)
                      .AsNoTracking().Include(item => item.department.universityCollege.city.Country).OrderBy(item => item.GPA).ToList();

                    return Json(new { Students });
                }

                if (gr != null && UPOrDw == 2)
                {

                    Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE TraineeId IN " +
                   $"(SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities WHERE TrainingOpportunity_TrainingOpportunityId ={id} AND DecisionStatus='approved' ) AND Gender ='{gr}' ")
                      .AsNoTracking().Include(item => item.user)
                      .AsNoTracking().Include(item => item.department.universityCollege.organization)
                      .AsNoTracking().Include(item => item.department.universityCollege.city.Country).OrderByDescending(item => item.GPA).ToList();

                    return Json(new { Students });
                }


                if (gr == null && UPOrDw == 1)
                {

                    Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE TraineeId IN " +
                   $"(SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities WHERE TrainingOpportunity_TrainingOpportunityId ={id} AND DecisionStatus='approved') ")
                      .AsNoTracking().Include(item => item.user)
                      .AsNoTracking().Include(item => item.department.universityCollege.organization)
                      .AsNoTracking().Include(item => item.department.universityCollege.city.Country).OrderBy(item => item.GPA).ToList();

                    return Json(new { Students });
                }

                if (gr == null && UPOrDw == 2)
                {

                    Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE TraineeId IN " +
                   $"(SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities WHERE TrainingOpportunity_TrainingOpportunityId ={id} AND DecisionStatus='approved' ) ")
                      .AsNoTracking().Include(item => item.user)
                      .AsNoTracking().Include(item => item.department.universityCollege.organization)
                      .AsNoTracking().Include(item => item.department.universityCollege.city.Country).OrderByDescending(item => item.GPA).ToList();

                    return Json(new { Students });
                }


            }


            return Json(new { Students });
        }



        [HttpGet]
        public IActionResult GetStudentsListByFilter(int? id, int? ArOrWa, string? filter)
        {


            IEnumerable<UniversityTraineeStudent> Students = Enumerable.Empty<UniversityTraineeStudent>();


            if (ArOrWa == 0)
            {
                Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE TraineeId IN " +
                $"(SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities WHERE TrainingOpportunity_TrainingOpportunityId  ={id} AND DecisionStatus='waiting') " +
                $"AND UserAccount_UserId IN (SELECT UserId FROM UserAcounts WHERE FullName = '{filter}' )")
                .AsNoTracking().Include(item => item.user)
                .AsNoTracking().Include(item => item.department.universityCollege.organization)
                .AsNoTracking().Include(item => item.department.universityCollege.city.Country).ToList();


                if (Students.Count() != 0)
                {
                    return Json(new { Students });
                }



                Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw("SELECT * FROM UniversitiesTraineeStudents WHERE TraineeId IN " +
                    $"(SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities WHERE TrainingOpportunity_TrainingOpportunityId ={id} AND DecisionStatus='waiting') " +
                    $"AND Department_DepartmentId IN (SELECT Department_DepartmentId FROM Departments WHERE Organization_OrganizationId " +
                    $"IN( SELECT OrganizationId FROM Organizations WHERE OrganizationName = '{filter}'))")
                    .AsNoTracking().Include(item => item.user)
                    .AsNoTracking().Include(item => item.department.universityCollege.organization)
                    .AsNoTracking().Include(item => item.department.universityCollege.city.Country).ToList();

                return Json(new { Students });
            }



            if (ArOrWa == 1)
            {
                Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw($"SELECT * FROM UniversitiesTraineeStudents WHERE TraineeId IN " +
                $"(SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities WHERE TrainingOpportunity_TrainingOpportunityId  ={id} AND DecisionStatus='approved') " +
                $"AND UserAccount_UserId IN (SELECT UserId FROM UserAcounts WHERE FullName = '{filter}' )")
                .AsNoTracking().Include(item => item.user)
                .AsNoTracking().Include(item => item.department.universityCollege.organization)
                .AsNoTracking().Include(item => item.department.universityCollege.city.Country).ToList();


                if (Students.Count() != 0)
                {
                    return Json(new { Students });
                }



                Students = _DbContext.UniversitiesTraineeStudents.FromSqlRaw("SELECT * FROM UniversitiesTraineeStudents WHERE TraineeId IN " +
                    $"(SELECT Trainee_TraineeId FROM StudentRequestsOnOpportunities WHERE TrainingOpportunity_TrainingOpportunityId ={id} AND DecisionStatus='approved') " +
                    $"AND Department_DepartmentId IN (SELECT Department_DepartmentId FROM Departments WHERE Organization_OrganizationId " +
                    $"IN( SELECT OrganizationId FROM Organizations WHERE OrganizationName = '{filter}'))")
                    .AsNoTracking().Include(item => item.user)
                    .AsNoTracking().Include(item => item.department.universityCollege.organization)
                    .AsNoTracking().Include(item => item.department.universityCollege.city.Country).ToList();

                return Json(new { Students });

            }



            return Json(new { Students });
        }

        #endregion
    }
}
