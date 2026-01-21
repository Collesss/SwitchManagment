using SwitchManagment.API.ValidationAttributes;

namespace SwitchManagment.API.Models.Dto.Switch
{
    public class SwitchGet
    {
        public PageNav PageNav {  get; set; } = new PageNav();

        public Sort Sort { get; set; } = new Sort();

        [NotNullDictionaryValues]
        [OnlyDictionaryKeyValues(["Id", "IpOrName", "Location", "Description"])]
        public Dictionary<string, string> Filters { get; set; } = new Dictionary<string, string>();
    }
}
