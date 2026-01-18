namespace SwitchManagment.API.Models.Dto.Switch.Response
{
    public class SwitchFullResponse : SwitchAnnotationResponse
    {
        public IEnumerable<PortResponse> Ports { get; set; }

        public IEnumerable<VlanResponse> VlansInfo { get; set; }
    }
}