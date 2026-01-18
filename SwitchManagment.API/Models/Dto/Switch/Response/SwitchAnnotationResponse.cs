namespace SwitchManagment.API.Models.Dto.Switch.Response
{
    public class SwitchAnnotationResponse
    {
        public int Id { get; set; }
        
        public string IpOrName { get; set; }

        public string Location { get; set; }

        public string Description { get; set; }
    }
}
