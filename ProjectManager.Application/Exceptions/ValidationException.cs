namespace ProjectManager.Application.Exceptions
{
    public class ValidationException : BaseException
    {
        public ValidationException(string message) : base(message) { }
    }
}
