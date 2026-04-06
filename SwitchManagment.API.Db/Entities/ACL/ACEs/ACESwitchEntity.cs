using SwitchManagment.API.Db.Entities.ACL.AccessMasks;

namespace SwitchManagment.API.Db.Entities.ACL.ACEs
{
    public class ACESwitchEntity : ACEBase<AccessMaskSwitch>
    {
        public int SwitchId { get; set; }

        public SwitchEntity Switch {  get; set; }
    }
}
