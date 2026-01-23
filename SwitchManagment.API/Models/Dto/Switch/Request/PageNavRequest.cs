using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SwitchManagment.API.Models.Dto.Switch.Request
{
    public class PageNavRequest
    {
        [Range(1, int.MaxValue)]
        [DefaultValue(1)]
        public int PageNum { get; set; } = 1;

        [Range(1, 100)]
        [DefaultValue(20)]
        public int PageSize { get; set; } = 20;
    }
}
