namespace ProjectManager.Application.Exceptions
{ 
    public class UnauthorizedException : BaseException
    {
        public UnauthorizedException(string message) : base(message) { }
    }
}
