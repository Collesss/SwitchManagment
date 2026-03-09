using SwitchManagment.API.ValidationAttributes;

namespace SwitchManagment.API.Models.Dto.Switch.Request.Get
{
    public class GetRequest
    {
        public PageNavRequest PageNav { get; set; } = new PageNavRequest();

        public SortRequest Sort { get; set; } = new SortRequest();

        [NotNullDictionaryValues]
        [OnlyDictionaryKeyValues(["Id", "IpOrName", "Location", "Description"])]
        public Dictionary<string, string> Filters { get; set; } = [];
    }
}