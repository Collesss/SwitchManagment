namespace SwitchManagment.API.Repository.Exceptions
{
    /*
    enum RepositoryCodeException
    {
        Unknown,
        IndexUsed,
        KeyUsed,
        KeyNotFound,
        FieldEmpty
    }
    */

    internal class RepositoryException : Exception
    {
        public RepositoryException() { }

        public RepositoryException(string message) : base(message) { }

        public RepositoryException(string message, Exception innerException) : base(message, innerException) { }
    }
}
