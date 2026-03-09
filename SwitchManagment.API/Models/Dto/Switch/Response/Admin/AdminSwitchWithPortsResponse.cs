using SwitchManagment.API.Models.Dto.Switch.Response.Port;

namespace SwitchManagment.API.Models.Dto.Switch.Response.Admin
{
    public class AdminSwitchWithPortsResponse : AdminSwitchResponse
    {
        public IEnumerable<PortResponse> Ports { get; set; }

        public IEnumerable<VlanResponse> VlansInfo { get; set; }
    }
}
