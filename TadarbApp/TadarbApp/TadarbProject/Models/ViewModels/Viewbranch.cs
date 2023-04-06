namespace TadarbProject.Models.ViewModels
{
    public class Viewbranch
    {
        //public City city { get; set; }
        //public OrganizationBranch_TrainProv Branch { get; set; }
        //public UserAcount User { get; set; }

        public IEnumerable<City> city { get; set; }


        public IEnumerable<OrganizationBranch_TrainProv> Branch { get; set; }
        public IEnumerable<UserAcount> User { get; set; }
    }
}
