namespace SwitchManagment.API.Models.Dto.ACL
{
    public class ACLBaseDtoResponse<T> : ACLBaseDto<T> where T : Enum
    {
        public int Id { get; set; }
    }
}
