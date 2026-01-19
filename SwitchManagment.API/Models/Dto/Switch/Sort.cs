using SwitchManagment.API.ValidationAttributes;

namespace SwitchManagment.API.Models.Dto.Switch
{
    public class Sort
    {
        [OnlyValues(["Id", "IpOrName", "Location", "Description"])]
        public string Field { get; set; } = "Id";

        public bool IsAscending { get; set; } = true;
    }
}
