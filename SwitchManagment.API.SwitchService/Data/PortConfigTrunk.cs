namespace SwitchManagment.API.SwitchService.Data
{
    public sealed class PortConfigTrunk : PortConfig
    {
        public IEnumerable<int> TrunkVlans { get; set; }
    }
}
