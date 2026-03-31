namespace SwitchManagment.API.Db.Entities.ACL.AccessMasks
{
    [Flags]
    public enum AccessMaskVlan : byte
    {
        None = 0,
        Read = 1,
    }
}
