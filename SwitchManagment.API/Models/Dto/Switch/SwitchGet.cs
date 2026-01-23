using SwitchManagment.API.Models.Dto.Switch.Request;
using SwitchManagment.API.ValidationAttributes;
using System.ComponentModel;

namespace SwitchManagment.API.Models.Dto.Switch
{
    public class SwitchGet<T> where T : PageNavRequest, new()
    {
        public T PageNav {  get; set; } = new T();

        public Sort Sort { get; set; } = new Sort();

        [NotNullDictionaryValues]
        [OnlyDictionaryKeyValues(["Id", "IpOrName", "Location", "Description"])]
        public Dictionary<string, string> Filters { get; set; } = [];
    }
}
