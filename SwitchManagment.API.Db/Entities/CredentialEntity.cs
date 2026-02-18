namespace SwitchManagment.API.Db.Entities
{
    public class CredentialEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public string SuperPassword { get; set; }
    }
}
