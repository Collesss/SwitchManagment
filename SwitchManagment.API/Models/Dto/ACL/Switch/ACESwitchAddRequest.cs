using SwitchManagment.API.Models.Dto.ACL.AccessMask;
using System.ComponentModel.DataAnnotations;

namespace SwitchManagment.API.Models.Dto.ACL.Switch
{
    public class ACESwitchAddRequest
    {
        [Required]
        public string GroupSID { get; set; }

        public AccessMaskInterfaceDto AccessMask { get; set; }

        [Range(1, int.MaxValue)]
        public int SwitchId { get; set; }
    }
}
