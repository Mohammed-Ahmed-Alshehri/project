using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TadarbProject.Models.ViewModels
{
    public class AssessmentVM
    {
        public DepartmentAssessmentTypeMaster DepartmentAssessmentTypeMaster { get; set; }

        public DepartmentAssessmentTypeDetail DepartmentAssessmentTypeDetail { get; set; }


        [ValidateNever]
        public IEnumerable<SelectListItem> AssessmentTypeListItems { get; set; }




    }
}
