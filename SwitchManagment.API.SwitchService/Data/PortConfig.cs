namespace SwitchManagment.API.SwitchService.Data
{
    public abstract class PortConfig : ConnectConfig
    {
        public string InterfaceName { get; set; }
    }
}
