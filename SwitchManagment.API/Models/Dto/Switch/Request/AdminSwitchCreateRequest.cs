using System.ComponentModel.DataAnnotations;

namespace SwitchManagment.API.Models.Dto.Switch.Request
{
    public class AdminSwitchCreateRequest
    {
        [Required]
        public string IpOrName { get; set; }

        public string Location { get; set; }

        public string Description { get; set; }

        public string Login { get; set; }
        
        public string Password { get; set; }

        public string SuperPassword { get; set; }
    }
}
