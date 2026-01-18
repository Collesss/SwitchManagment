using System.ComponentModel.DataAnnotations;

namespace SwitchManagment.API.Models.Dto.Switch
{
    public class PageNav
    {
        [Range(1, int.MaxValue)]
        public int PageNum { get; set; }

        [Range(1, 100)]
        public int PageSize { get; set; }

        public int PageCount { get; set; }
    }
}
