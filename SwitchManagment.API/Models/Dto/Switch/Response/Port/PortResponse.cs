namespace SwitchManagment.API.Models.Dto.Switch.Response.Port
{
    public class PortResponse
    {
        public string Name { get; set; }

        public PortStatusResponse Status { get; set; }
        
        public PortTypeResponse Type { get; set; }

        public IEnumerable<int> Vlans { get; set; }
    }
}
