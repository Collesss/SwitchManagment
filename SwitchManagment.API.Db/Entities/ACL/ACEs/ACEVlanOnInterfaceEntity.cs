using SwitchManagment.API.Db.Entities.ACL.AccessMasks;

namespace SwitchManagment.API.Db.Entities.ACL.ACEs
{
    public class ACEVlanOnInterfaceEntity : ACEBase<AccessMaskVlanOnInterface>
    {
        public int SwitchId { get; set; }

        public int IdOnSwitch {  get; set; }

        public InterfaceEntity Interface {  get; set; }

        public int Vlan {  get; set; }
    }
}