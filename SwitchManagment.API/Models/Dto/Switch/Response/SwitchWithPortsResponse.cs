using SwitchManagment.API.Models.Dto.Switch.Response.Port;

namespace SwitchManagment.API.Models.Dto.Switch.Response
{
    public class SwitchWithPortsResponse : SwitchResponse
    {
        public IEnumerable<PortResponse> Ports { get; set; }

        public IEnumerable<VlanResponse> VlansInfo { get; set; }
    }
}