namespace SwitchManagment.API.Db.Entities.ACL.AccessMasks
{
    [Flags]
    public enum AccessMaskInterface : byte
    {
        None = 0,
        List = 1,
        Read = 2
    }
}