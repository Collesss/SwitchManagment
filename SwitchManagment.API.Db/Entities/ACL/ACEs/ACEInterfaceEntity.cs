using SwitchManagment.API.Db.Entities.ACL.AccessMasks;

namespace SwitchManagment.API.Db.Entities.ACL.ACEs
{
    public class ACEInterfaceEntity : ACEBase<AccessMaskInterface>
    {
        public int SwitchId { get; set; }

        public SwitchEntity Switch { get; set; }

        public string InterfaceName {  get; set; }

        //public InterfaceEntity Interface {  get; set; }
    }
}
