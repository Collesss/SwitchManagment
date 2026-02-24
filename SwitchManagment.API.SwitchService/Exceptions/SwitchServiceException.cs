namespace SwitchManagment.API.SwitchService.Exceptions
{
    public class SwitchServiceException : Exception
    {
        private static readonly Dictionary<SwitchServiceErrorType, string> ErrorMessage = new() 
        {
            [SwitchServiceErrorType.Unknown] = "Unknown",
            [SwitchServiceErrorType.HostNotExistOrUnreac] = "HostNotExistOrUnreac",
            [SwitchServiceErrorType.WrongLoginOrPass] = "WrongLoginOrPass",
            [SwitchServiceErrorType.WrongSuperPass] = "WrongSuperPass",
            [SwitchServiceErrorType.WrongInterface] = "WrongInterface"
        };

        public SwitchServiceErrorType ErrorType { get; private set; }


        public SwitchServiceException() { }

        public SwitchServiceException(string message) : base(message) { }

        public SwitchServiceException(string message, Exception innerException) : base(message, innerException) { }

        public SwitchServiceException(SwitchServiceErrorType errorType) : base(ErrorMessage[errorType])
        {
            ErrorType = errorType;
        }

        public SwitchServiceException(SwitchServiceErrorType errorType, Exception innerException) : base(ErrorMessage[errorType], innerException)
        {
            ErrorType = errorType;
        }
    }
}
