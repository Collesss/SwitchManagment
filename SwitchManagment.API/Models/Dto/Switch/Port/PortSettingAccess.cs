using System.ComponentModel.DataAnnotations;

namespace SwitchManagment.API.Models.Dto.Switch.Port
{
    public class PortSettingAccess
    {
        [Range(1, 4094)]
        public int Vlan {  get; set; }
    }
}
