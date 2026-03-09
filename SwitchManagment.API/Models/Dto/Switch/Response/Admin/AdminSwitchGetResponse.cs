using SwitchManagment.API.Models.Dto.Switch.Response.Get;

namespace SwitchManagment.API.Models.Dto.Switch.Response.Admin
{
    public class AdminSwitchGetResponse
    {
        public GetResponse SwitchGetInfo { get; set; }

        public IEnumerable<AdminSwitchResponse> Switches { get; set; }
    }
}
