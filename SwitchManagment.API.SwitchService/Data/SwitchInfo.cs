namespace SwitchManagment.API.SwitchService.Data
{
    public class SwitchInfo : SwitchSummary
    {
        public IEnumerable<SwitchPort> Ports { get; set; }

        public IEnumerable<SwitchVlan> Vlans { get; set; }
    }
}
