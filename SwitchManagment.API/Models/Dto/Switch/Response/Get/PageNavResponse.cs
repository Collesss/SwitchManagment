using System.Text.Json.Serialization;

namespace SwitchManagment.API.Models.Dto.Switch.Response.Get
{
    public class PageNavResponse
    {
        public int PageNum { get; set; }

        public int PageSize { get; set; }

        public int CountElements { get; set; }

        /*
        public int CountElements 
        {
            get; 
            set
            {
                field = value;
                PageNum = PageNum > PageCount ? PageCount : PageNum;
            }
        }
        */

        [JsonIgnore]
        public int PageCount => CountElements == 0 ? 1 : CountElements / PageSize + (CountElements % PageSize > 0 ? 1 : 0);
    }
}
