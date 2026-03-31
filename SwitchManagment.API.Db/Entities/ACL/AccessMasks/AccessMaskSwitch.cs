namespace SwitchManagment.API.Db.Entities.ACL.AccessMasks
{
    [Flags]
    public enum AccessMaskSwitch : byte
    {
        None = 0,
        View = 0b00000001,
        ViewCommon = 0b00000010
    }
}
