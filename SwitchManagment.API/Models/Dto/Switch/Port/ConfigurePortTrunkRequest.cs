using SwitchManagment.API.ValidationAttributes;

namespace SwitchManagment.API.Models.Dto.Switch.Port
{
    public class ConfigurePortTrunkRequest
    {
        [VlanList]
        public IEnumerable<int> Vlans = new List<int>();
    }
}
