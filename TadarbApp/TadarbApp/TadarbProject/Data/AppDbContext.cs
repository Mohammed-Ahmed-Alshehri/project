﻿using Microsoft.EntityFrameworkCore;
using TadarbProject.Models;

namespace TadarbProject.Data
{


    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Country> Countries { get; set; }

        public DbSet<City> Cities { get; set; }

        public DbSet<FieldOfSpecialtyMaster> FieldOfSpecialtiesMaster { get; set; }

        public DbSet<FieldOfSpecialtyDetails> FieldOfSpecialtiesDetails { get; set; }

        public DbSet<UserAcount> UserAcounts { get; set; }

        public DbSet<OrganizationType> OrganizationTypes { get; set; }

        public DbSet<Organization> Organizations { get; set; }

        public DbSet<DepartmentTrainingArea> DepartmentTrainingAreas { get; set; }

        public DbSet<Department> Departments { get; set; }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<Job> Jobs { get; set; }

        public DbSet<OrganizationBranch_TrainProv> OrganizationBranches_TrainProv { get; set; }

        public DbSet<OrganizationProvidTrainingInArea> OrganizationsProvidTrainingInArea { get; set; }

        public DbSet<UniversityCollege> UniversityColleges { get; set; }


        public DbSet<UniversityTraineeStudent> UniversitiesTraineeStudents { get; set; }


        public DbSet<TrainingType> TrainingTypes { get; set; }


        public DbSet<TrainingOpportunity> TrainingOpportunities { get; set; }


        public DbSet<StudentRequestOpportunity> StudentRequestsOnOpportunities { get; set; }



        public DbSet<AssessmentType> AssessmentTypes { get; set; }


        public DbSet<DepartmentAssessmentTypeMaster> DepartmentsAssessmentTypeMaster { get; set; }

        public DbSet<DepartmentAssessmentTypeDetail> DepartmentsAssessmentTypeDetail { get; set; }

        public DbSet<SemesterTrainingSettingMaster> SemestersTrainingSettingMaster { get; set; }

        public DbSet<SemesterStudentAndEvaluationDetail> SemestersStudentAndEvaluationDetails { get; set; }


        public DbSet<StudentSemesterEvaluationMark> StudentSemesterEvaluationMarks { get; set; }
    }
}
