using MediatR;

namespace ProjectManager.Application.Features.Tasks.Commands.MarkTaskCompleted
{
    public record MarkTaskCompletedCommand(int ProjectId, int TaskId, string UserId) : IRequest<Unit>;
}
