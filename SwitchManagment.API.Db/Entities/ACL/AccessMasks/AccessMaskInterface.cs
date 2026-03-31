namespace SwitchManagment.API.Db.Entities.ACL.AccessMasks
{
    [Flags]
    public enum AccessMaskInterface : byte
    {
        None = 0,
        Read = 1
    }
}
