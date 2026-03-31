namespace SwitchManagment.API.Db.Entities.ACL.ACEs
{
    public abstract class ACEBase<T> where T : Enum
    {
        public int Id { get; set; }

        public string GroupSID { get; set; }

        public T AccessMask { get; set; }
    }
}
