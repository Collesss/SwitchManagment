namespace SwitchManagment.API.Models.Dto.Switch.Response
{
    public class PortResponse
    {
        public string Name { get; set; }

        public PortStatusResponse Status { get; set; }
        
        public bool IsAccess { get; set; }

        public IEnumerable<int> Vlans { get; set; }
    }
}
