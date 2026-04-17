namespace SwitchManagment.API.Models.Dto.ACL
{
    public class ACLBaseDto<T> where T : Enum
    {
        public string GroupSID { get; set; }

        public T AccessMask { get; set; }
    }
}
