namespace SwitchManagment.API.SwitchService.Data
{
    public class SwitchInfo
    {
        public string IpOrName { get; set; }

        public IEnumerable<SwitchPort> Ports { get; set; }

        public IEnumerable<SwitchVlan> Vlans { get; set; }
    }
}
