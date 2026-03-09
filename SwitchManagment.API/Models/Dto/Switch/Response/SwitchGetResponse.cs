using SwitchManagment.API.Models.Dto.Switch.Response.Get;

namespace SwitchManagment.API.Models.Dto.Switch.Response
{
    public class SwitchGetResponse
    {
        public GetResponse SwitchGetInfo { get; set; }

        public IEnumerable<SwitchResponse> Switches { get; set; }
    }
}
