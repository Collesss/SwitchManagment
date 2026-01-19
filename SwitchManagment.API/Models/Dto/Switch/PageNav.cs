using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace SwitchManagment.API.Models.Dto.Switch
{
    public class PageNav
    {
        [Range(1, int.MaxValue)]
        public int PageNum { get; set; } = 1;

        [Range(1, 100)]
        public int PageSize { get; set; } = 20;

        [BindNever]
        public int CountElements { get; set; }

        public int PageCount => (CountElements / PageSize) + (CountElements % PageSize > 0 ? 1 : 0);
    }
}
