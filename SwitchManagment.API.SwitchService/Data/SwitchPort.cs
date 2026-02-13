namespace SwitchManagment.API.SwitchService.Data
{
    public class SwitchPort
    {
        public string Interface {  get; set; }

        public string Description { get; set; }

        public SwitchPortType Type { get; set; }

        public SwitchPortStatus Status { get; set; }

        public int[] Vlans { get; set; }
    }
}
