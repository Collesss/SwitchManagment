namespace SwitchManagment.API.Models.Dto.ACL.AccessMask
{
    [Flags]
    public enum AccessMaskInterfaceDto : byte
    {
        None = 0,
        List = 1,
        Read = 2
    }
}