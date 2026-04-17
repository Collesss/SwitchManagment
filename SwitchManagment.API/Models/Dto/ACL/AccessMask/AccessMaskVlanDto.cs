namespace SwitchManagment.API.Models.Dto.ACL.AccessMask
{
    [Flags]
    public enum AccessMaskVlanDto : byte
    {
        None = 0,
        List = 1,
        Read = 2,
    }
}
