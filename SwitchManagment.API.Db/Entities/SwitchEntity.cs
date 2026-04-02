using SwitchManagment.API.Db.Entities.ACL.ACEs;

namespace SwitchManagment.API.Db.Entities
{
    public class SwitchEntity
    {
        public int Id { get; set; }

        public string IpOrName { get; set; }

        public string Location { get; set; }

        public string Description { get; set; }


        public string Login { get; set; }

        public string EncryptedPassword { get; set; }

        public string EncryptedSuperPassword { get; set; }
        

        public IEnumerable<InterfaceEntity> Interfaces { get; set; }
        
        public IEnumerable<ACESwitchEntity> ACLSwitch { get; set; }
        
        public IEnumerable<ACEVlanEntity> ACLVlans { get; set; }
    }
}
