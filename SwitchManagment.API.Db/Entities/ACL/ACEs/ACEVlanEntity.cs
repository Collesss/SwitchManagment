using SwitchManagment.API.Db.Entities.ACL.AccessMasks;

namespace SwitchManagment.API.Db.Entities.ACL.ACEs
{
    public class ACEVlanEntity : ACEBase<AccessMaskVlan>
    {
        public int SwitchId { get; set; }

        public SwitchEntity Switch { get; set; }

        public int Vlan {  get; set; }
    }
}
