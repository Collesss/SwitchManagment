using System.ComponentModel.DataAnnotations;

namespace SwitchManagment.API.Models.Dto.Switch
{
    public class SwitchCreateRequest
    {
        [Required]
        public string IpOrName { get; set; }

        public string Location { get; set; }

        public string Description { get; set; }
    }
}
