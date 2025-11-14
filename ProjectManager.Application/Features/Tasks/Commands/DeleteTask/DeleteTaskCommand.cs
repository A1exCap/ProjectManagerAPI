using MediatR;

namespace ProjectManager.Application.Features.Tasks.Commands.DeleteTask
{
    public record DeleteTaskCommand(int ProjectId, int TaskId, string UserId) : IRequest<Unit>;
}
