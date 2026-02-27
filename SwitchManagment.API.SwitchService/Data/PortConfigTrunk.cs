namespace SwitchManagment.API.SwitchService.Data
{
    public class PortConfigTrunk : PortConfig
    {
        public IEnumerable<int> TrunkVlans { get; set; }
    }
}
