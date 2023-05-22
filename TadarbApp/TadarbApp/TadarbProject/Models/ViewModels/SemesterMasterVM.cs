using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TadarbProject.Models.ViewModels
{
    public class SemesterMasterVM
    {
        public SemesterTrainingSettingMaster SemesterTrainingSettingMaster { get; set; }

        public Department Department { get; set; }


        public Employee Employee { get; set; }


        [ValidateNever]
        public IEnumerable<SelectListItem> TrainingTypeListItems { get; set; }

    }
}
