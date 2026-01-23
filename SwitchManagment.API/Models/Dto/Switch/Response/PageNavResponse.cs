using SwitchManagment.API.Models.Dto.Switch.Request;
using System.Text.Json.Serialization;

namespace SwitchManagment.API.Models.Dto.Switch.Response
{
    public class PageNavResponse : PageNavRequest
    {
        public int CountElements { get; set; }

        [JsonIgnore]
        public int PageCount => CountElements == 0 ? 1 : ((CountElements / PageSize) + (CountElements % PageSize > 0 ? 1 : 0));
    }
}
