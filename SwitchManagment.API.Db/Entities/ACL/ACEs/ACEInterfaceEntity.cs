using SwitchManagment.API.Db.Entities.ACL.AccessMasks;

namespace SwitchManagment.API.Db.Entities.ACL.ACEs
{
    public class ACEInterfaceEntity : ACEBase<AccessMaskInterface>
    {
        public int SwitchId { get; set; }

        public int IdOnSwitch {  get; set; }
    }
}
