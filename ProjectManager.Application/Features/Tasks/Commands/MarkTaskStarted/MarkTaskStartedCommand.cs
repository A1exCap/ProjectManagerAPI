using MediatR;

namespace ProjectManager.Application.Features.Tasks.Commands.MarkTaskStarted
{
    public record MarkTaskStartedCommand(int ProjectId, int TaskId, string UserId) : IRequest<Unit>;
}
