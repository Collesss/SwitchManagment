namespace SwitchManagment.API.Models.Dto.Switch
{
    public class SwitchGet
    {
        public PageNav PageNav {  get; set; }

        public Sort Sort { get; set; }

        public IEnumerable<FilterField> Filters { get; set; }
    }
}
