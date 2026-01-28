namespace SwitchManagment.API.SwitchService.Data
{
    public class SwitchPort
    {
        public string Interface {  get; set; }

        public string Description { get; set; }

        public bool IsAccess { get; set; }

        public bool Up {  get; set; }

        public bool Enable { get; set; }

        public int[] Vlans { get; set; }
    }
}
