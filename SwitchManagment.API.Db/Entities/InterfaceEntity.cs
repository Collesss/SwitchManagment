namespace SwitchManagment.API.Db.Entities
{
    public class InterfaceEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int SwitchId { get; set; }

        public int IdOnSwitch { get; set; }
    }
}
