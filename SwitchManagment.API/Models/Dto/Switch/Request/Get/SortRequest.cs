using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SwitchManagment.API.Models.Dto.Switch.Request.Get
{
    public class SortRequest
    {
        //[OnlyValues(["Id", "IpOrName", "Location", "Description"])]
        [AllowedValues(["Id", "IpOrName", "Location", "Description"])]
        [DefaultValue("Id")]
        public string Field { get; set; } = "Id";

        [DefaultValue(true)]
        public bool IsAscending { get; set; } = true;
    }
}
