namespace SwitchManagment.API.Models.Dto.Switch.Response.Get
{
    public class GetResponse
    {
        public PageNavResponse PageNav { get; set; } = new PageNavResponse();

        public SortResponse Sort { get; set; } = new SortResponse();

        public Dictionary<string, string> Filters { get; set; } = [];
    }
}
