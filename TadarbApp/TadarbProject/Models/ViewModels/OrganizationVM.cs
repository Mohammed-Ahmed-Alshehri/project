using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TadarbProject.Models.ViewModels
{
    public class OrganizationVM
    {
        public Organization organization { get; set; }

        public UserAcount userAcount { get; set; }


        [ValidateNever]
        public IEnumerable<SelectListItem> CountryListItems { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> CityListItems { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> OrganizationTypeListItems { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> FieldListItems { get; set; }
    }
}
