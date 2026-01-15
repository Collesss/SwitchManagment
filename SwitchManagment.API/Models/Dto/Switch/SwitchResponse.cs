namespace SwitchManagment.API.Models.Dto.Switch
{
    public class SwitchResponse
    {
        public int Id { get; set; }
        
        public string IpOrName { get; set; }

        public string Location { get; set; }

        public string Description { get; set; }
    }
}
