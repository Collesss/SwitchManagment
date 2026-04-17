namespace SwitchManagment.API.Db.Entities.ACL.AccessMasks
{
    [Flags]
    public enum AccessMaskVlan : byte
    {
        None = 0,
        List = 1,
        Read = 2,
    }
}
