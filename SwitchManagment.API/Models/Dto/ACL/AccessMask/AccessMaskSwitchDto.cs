namespace SwitchManagment.API.Models.Dto.ACL.AccessMask
{
    [Flags]
    public enum AccessMaskSwitchDto : byte
    {
        None = 0,
        View = 0b00000001,
        ViewCommon = 0b00000010
    }
}
