namespace SwitchManagment.API.SwitchService.Exceptions
{
    public class SwitchServiceException : Exception
    {
        public SwitchServiceException() { }

        public SwitchServiceException(string message) : base(message) { }

        public SwitchServiceException(string message, Exception innerException) : base(message, innerException) { }
    }
}
