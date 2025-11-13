namespace ProjectManager.Application.Exceptions
{
    public class NotFoundException : BaseException
    {
        public NotFoundException(string message) : base(message) { }
    }
}
