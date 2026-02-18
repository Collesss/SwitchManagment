namespace SwitchManagment.API.Db.Entities
{
    public class SwitchEntity
    {
        public int Id { get; set; }

        public string IpOrName { get; set; }

        public string Location { get; set; }

        public string Description { get; set; }


        public int? CredentialId { get; set; }

        public CredentialEntity Credential { get; set; }
    }
}
