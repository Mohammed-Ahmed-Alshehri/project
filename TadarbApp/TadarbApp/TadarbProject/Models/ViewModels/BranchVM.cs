using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TadarbProject.Models.ViewModels
{
    public class BranchVM
    {
        public OrganizationBranch_TrainProv Branch { get; set; }


        [ValidateNever]
        public Organization organization { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> CountryListItems { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> CityListItems { get; set; }
        [ValidateNever]

        public IEnumerable<SelectListItem> UserListItems { get; set; }




    }
}
