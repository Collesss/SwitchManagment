using SwitchManagment.API.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace SwitchManagment.API.Models.Dto.Port
{
    public class ConfigurePortTrunkRequest
    {
        [Required]
        public string InterfaceName { get; set; }

        [VlanList]
        public IEnumerable<int> Vlans = new List<int>();
    }
}
