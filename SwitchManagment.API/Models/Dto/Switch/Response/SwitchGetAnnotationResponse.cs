namespace SwitchManagment.API.Models.Dto.Switch.Response
{
    public class SwitchGetAnnotationResponse
    {
        public SwitchGet SwitchGetInfo { get; set; }

        public IEnumerable<SwitchAnnotationResponse> Switches { get; set; }
    }
}
