namespace SwitchManagment.API.SwitchService.Data
{
    public class PortConfig : ConnectConfig
    {
        public string InterfaceName { get; set; }

        public bool IsTrunk { get; set; }

        public IEnumerable<int> Vlans { get; set; }
    }
}
