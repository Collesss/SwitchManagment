using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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

        [BindNever]
        [JsonIgnore]
        public int PageCount => CountElements == 0 ? 1 : (CountElements / PageSize) + (CountElements % PageSize > 0 ? 1 : 0);
    }
}
