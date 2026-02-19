using System.ComponentModel.DataAnnotations;

namespace SwitchManagment.API.Models.Dto.Credential
{
    public class CredentialCreateRequest
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Login { get; set; }

        [Required]
        public string Password { get; set; }

        public string SuperPassword { get; set; }
    }
}
