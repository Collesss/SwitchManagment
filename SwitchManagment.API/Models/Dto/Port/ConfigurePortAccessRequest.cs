using System.ComponentModel.DataAnnotations;

namespace SwitchManagment.API.Models.Dto.Port
{
    public class ConfigurePortAccessRequest
    {
        [Required]
        public string InterfaceName { get; set; }

        [Range(1, 4094)]
        public int Vlan {  get; set; }
    }
}

