using SwitchManagment.API.Models.Dto.ACL.AccessMask;

namespace SwitchManagment.API.Models.Dto.ACL.Switch
{
    public class ACESwitchResponse
    {
        public int Id { get; set; }

        public string GroupSID { get; set; }

        public AccessMaskInterfaceDto AccessMask { get; set; }

        public int SwitchId { get; set; }
    }
}
