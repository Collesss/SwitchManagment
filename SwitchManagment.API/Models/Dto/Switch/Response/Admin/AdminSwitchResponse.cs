namespace SwitchManagment.API.Models.Dto.Switch.Response.Admin
{
    public class AdminSwitchResponse : SwitchResponse
    {
        public string Login { get; set; }

        public string HashPassword { get; set; }

        public string HashSuperPassword { get; set; }
    }
}
