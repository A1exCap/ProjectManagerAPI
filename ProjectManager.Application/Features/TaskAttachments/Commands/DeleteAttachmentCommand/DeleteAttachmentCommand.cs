using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Features.TaskAttachments.Commands.DeleteAttachmentCommand
{
    public record DeleteAttachmentCommand(int ProjectId, int TaskId, string UserId, int AttachmentId) : IRequest<Unit>;
}
