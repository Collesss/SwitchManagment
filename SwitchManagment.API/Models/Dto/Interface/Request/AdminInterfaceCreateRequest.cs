namespace SwitchManagment.API.Models.Dto.Interface.Request
{
    public class AdminInterfaceCreateRequest
    {
        public int SwitchId { get; set; }

        public int IdOnSwitch { get; set; }

        public string Name { get; set; }
    }
}
