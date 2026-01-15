namespace SwitchManagment.API.Repository.Entities
{
    public class SwitchEntity : BaseEntity
    {
        public string IpOrName { get; set; }

        public string Location { get; set; }

        public string Description { get; set; }
    }
}
