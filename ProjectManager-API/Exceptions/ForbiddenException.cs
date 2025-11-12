namespace ProjectManager_API.Exceptions
{
    public class ForbiddenException : BaseException
    {
        public ForbiddenException(string message) : base(message) { }
    }
}
