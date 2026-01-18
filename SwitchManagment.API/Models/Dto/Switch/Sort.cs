using SwitchManagment.API.ValidationAttributes;

namespace SwitchManagment.API.Models.Dto.Switch
{
    public class Sort
    {
        [OnlyValues([null, "", "IpOrName", "Location", "Description"])]
        public string Field { get; set; }

        public bool IsAscending { get; set; }
    }
}
