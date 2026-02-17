namespace SwitchManagment.API.Db.Entities
{
    public class CredentialEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Login { get; set; }

        public string EncryptedPassword { get; set; }

        public string EncryptedSuperPassword { get; set; }
    }
}
