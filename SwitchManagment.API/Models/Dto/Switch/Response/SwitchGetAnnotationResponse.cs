namespace SwitchManagment.API.Models.Dto.Switch.Response
{
    public class SwitchGetAnnotationResponse
    {
        public SwitchGetResponse SwitchGetInfo { get; set; }

        public IEnumerable<SwitchAnnotationResponse> Switches { get; set; }
    }
}
